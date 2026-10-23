#!/usr/bin/env bash
# Runs inside a one-off container of the postgres service with the live database volume
# (restore-server.sh): fetches the latest base backup and prepares recovery; the database replays the
# archived WAL when it next starts, up to the target time when one is given, then promotes.
set -euo pipefail

target="${1:-}"

if [ -s "$PGDATA/PG_VERSION" ]; then
    echo "The database volume holds a database: nothing restored. Remove the volume first if it is lost." >&2
    exit 1
fi

mkdir -p "$PGDATA"
chown postgres:postgres "$PGDATA"
chmod 0700 "$PGDATA"
gosu postgres wal-g backup-fetch "$PGDATA" LATEST
gosu postgres touch "$PGDATA/recovery.signal"
{
    echo "restore_command = 'wal-g wal-fetch \"%f\" \"%p\"'"
    if [ -n "$target" ]; then
        echo "recovery_target_time = '$target'"
    fi
    echo "recovery_target_action = 'promote'"
} >> "$PGDATA/postgresql.auto.conf"
echo "Base backup fetched; the database replays the archived WAL${target:+ up to $target} on start."
