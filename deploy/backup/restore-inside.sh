#!/usr/bin/env bash
# Runs inside the throwaway restore-test container (deploy/compose.yaml): fetches the latest base
# backup, replays the archived WAL up to the target time (point-in-time recovery), promotes, then checks
# the copy. Arguments: target time (timestamptz), the marker id written just before it and the one
# written just after it. Prints key=value results; exits non-zero when the copy is not what the live
# database had at the target.
set -euo pipefail

target="$1"
marker="$2"
after="$3"

mkdir -p "$PGDATA"
chown postgres:postgres "$PGDATA"
chmod 0700 "$PGDATA"
gosu postgres wal-g backup-fetch "$PGDATA" LATEST

gosu postgres touch "$PGDATA/recovery.signal"
cat >> "$PGDATA/postgresql.auto.conf" <<EOF
restore_command = 'wal-g wal-fetch "%f" "%p"'
recovery_target_time = '$target'
recovery_target_action = 'promote'
archive_mode = off
listen_addresses = ''
EOF

# RTO ≤ 8 h (QS2): the replay may take that long before the test gives up.
gosu postgres pg_ctl -D "$PGDATA" -w -t 28800 -l /tmp/restore.log start || { cat /tmp/restore.log; exit 1; }

query() { gosu postgres psql -v ON_ERROR_STOP=1 -tAX -d memorio -U memorio_admin -c "$1"; }

for _ in $(seq 1 600); do
    [ "$(query 'SELECT pg_is_in_recovery()')" = "f" ] && break
    sleep 1
done
if [ "$(query 'SELECT pg_is_in_recovery()')" != "f" ]; then
    echo "error=still in recovery"
    cat /tmp/restore.log
    exit 1
fi

marker_found=$(query "SELECT count(*) FROM operations.restore_markers WHERE id = '$marker'")
after_target_found=$(query "SELECT count(*) FROM operations.restore_markers WHERE id = '$after'")
schemas=$(query "SELECT count(*) FROM pg_namespace WHERE pg_get_userbyid(nspowner) LIKE 'memorio\_%'")
tables=$(query "SELECT count(*) FROM pg_tables WHERE tableowner LIKE 'memorio\_%'")
replayed_until=$(query "SELECT max(created_at) FROM operations.restore_markers")

echo "marker_found=$marker_found"
echo "after_target_found=$after_target_found"
echo "memorio_schemas=$schemas"
echo "memorio_tables=$tables"
echo "last_marker_at=$replayed_until"

gosu postgres pg_ctl -D "$PGDATA" -w stop >/dev/null
# The bootstrap creates 17 schemas owned by Memorio roles: 14 modules, 2 messaging, the scheduler.
[ "$marker_found" = "1" ] && [ "$after_target_found" = "0" ] && [ "$schemas" -ge 17 ]
