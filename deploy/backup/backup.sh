#!/usr/bin/env bash
# Daily base backup with WAL-G (QS2, MEM-E0-PLAT-09), run by memorio-backup.timer. With the WAL archived
# continuously (archive_timeout 5 min) it allows a restore to any moment of the last BACKUP_RETAIN_FULL
# days; older backups and the WAL only they need are deleted. Writes backup.prom for the monitoring
# server (alert BackupOverdue).
set -euo pipefail
# shellcheck source-path=SCRIPTDIR
source "$(dirname "$0")/common.sh"

retain="${BACKUP_RETAIN_FULL:-14}"
started=$(date +%s)

# PGDATA is the container's, so it expands inside it.
# shellcheck disable=SC2016
compose exec -T -u postgres postgres bash -c 'wal-g backup-push "$PGDATA"'
compose exec -T -u postgres postgres wal-g delete retain FULL "$retain" --confirm

finished=$(date +%s)
write_metrics backup <<EOF
# HELP memorio_backup_last_success_timestamp_seconds When the last base backup finished.
# TYPE memorio_backup_last_success_timestamp_seconds gauge
memorio_backup_last_success_timestamp_seconds $finished
# HELP memorio_backup_duration_seconds How long the last base backup took.
# TYPE memorio_backup_duration_seconds gauge
memorio_backup_duration_seconds $((finished - started))
EOF
echo "Base backup done in $((finished - started)) s; $retain full backups kept."
