#!/usr/bin/env bash
# Monthly point-in-time restore test (SD13, QS2: RPO ≤ 15 min, RTO ≤ 8 h, MEM-E0-PLAT-09), run by
# memorio-restore-test.timer. It writes a marker into the live database, takes the moment right after
# it as the recovery target, writes a second marker, waits until the WAL holding both is archived,
# restores backup + WAL up to the target into a throwaway container and checks that the first marker
# and every Memorio schema are there and the second marker is not (the copy stops at the target).
# Measures RPO (marker to archived WAL) and RTO (whole restore), writes a report to REPORTS_DIR and
# restore-test.prom for the monitoring server (alert RestoreTestOverdue); exits non-zero on failure.
set -euo pipefail
# shellcheck source-path=SCRIPTDIR
source "$(dirname "$0")/common.sh"

reports_dir="${REPORTS_DIR:-/var/lib/memorio/restore-reports}"
rpo_limit=900
rto_limit=28800
mkdir -p "$reports_dir"
report="$reports_dir/restore-$(date -u +%Y-%m-%dT%H%M%SZ).md"

IFS='|' read -r marker marker_at < <(psql_live \
    "INSERT INTO operations.restore_markers (id) VALUES (gen_random_uuid()) RETURNING id, created_at")
target=$(psql_live "SELECT clock_timestamp()")
# A commit after the target: recovery stops on the first one it sees, and this one must not be restored.
after=$(psql_live "INSERT INTO operations.restore_markers (id) VALUES (gen_random_uuid()) RETURNING id")
segment=$(psql_live "SELECT pg_walfile_name(pg_switch_wal())")

# The segment with the marker must reach the bucket: that delay is the data a crash would lose (RPO).
archived=""
for _ in $(seq 1 "$rpo_limit"); do
    archived=$(psql_live "SELECT last_archived_wal >= '$segment' FROM pg_stat_archiver")
    [ "$archived" = "t" ] && break
    sleep 1
done
rpo=$(psql_live "SELECT round(extract(epoch FROM (last_archived_time - '$marker_at'::timestamptz)))::int FROM pg_stat_archiver")

started=$(date +%s)
set +e
output=$(compose run --rm -T restore-test "$target" "$marker" "$after" 2>&1)
status=$?
set -e
rto=$(($(date +%s) - started))

result="passed"
problems=()
[ "$archived" = "t" ] || problems+=("WAL segment $segment not archived within ${rpo_limit} s")
[ "$status" -eq 0 ] || problems+=("restored copy failed its checks (exit $status)")
[ "${rpo:-$rpo_limit}" -le "$rpo_limit" ] || problems+=("RPO ${rpo} s above ${rpo_limit} s")
[ "$rto" -le "$rto_limit" ] || problems+=("RTO ${rto} s above ${rto_limit} s")
[ "${#problems[@]}" -eq 0 ] || result="FAILED"

{
    echo "# Restore test $(date -u +%Y-%m-%d) — $result"
    echo
    echo "| | |"
    echo "|---|---|"
    echo "| Recovery target | $target |"
    echo "| Marker before the target (must be restored) | $marker (written $marker_at) |"
    echo "| Marker after the target (must not be restored) | $after |"
    echo "| RPO (marker → WAL archived) | ${rpo:-?} s (limit $rpo_limit s) |"
    echo "| RTO (fetch, replay, checks) | $rto s (limit $rto_limit s) |"
    echo
    for problem in "${problems[@]}"; do echo "- $problem"; done
    echo
    echo '```'
    echo "$output" | grep -E '^[a-z_]+=' || echo "$output" | tail -40
    echo '```'
} > "$report"

now=$(date +%s)
if [ "$result" = "passed" ]; then
    write_metrics restore-test <<EOF
# HELP memorio_restore_test_last_success_timestamp_seconds When the last restore test passed (SD13).
# TYPE memorio_restore_test_last_success_timestamp_seconds gauge
memorio_restore_test_last_success_timestamp_seconds $now
# HELP memorio_restore_test_rpo_seconds Data a crash would have lost in the last test (QS2: ≤ 900).
# TYPE memorio_restore_test_rpo_seconds gauge
memorio_restore_test_rpo_seconds $rpo
# HELP memorio_restore_test_rto_seconds How long the last restore took (QS2: ≤ 28800).
# TYPE memorio_restore_test_rto_seconds gauge
memorio_restore_test_rto_seconds $rto
EOF
fi

cat "$report"
[ "$result" = "passed" ]
