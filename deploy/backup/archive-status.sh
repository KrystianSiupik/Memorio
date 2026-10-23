#!/usr/bin/env bash
# WAL archiving status for the monitoring server (QS2: RPO ≤ 15 min), run every 5 minutes by
# memorio-archive-status.timer: when the last segment reached the backup bucket and how many failed.
# Alerts WalArchiveStale and WalArchiveFailing (monitoring/prometheus/rules).
set -euo pipefail
# shellcheck source-path=SCRIPTDIR
source "$(dirname "$0")/common.sh"

IFS='|' read -r archived failed last_failed < <(psql_live \
    "SELECT coalesce(extract(epoch FROM last_archived_time)::bigint, 0), failed_count,
            coalesce(extract(epoch FROM last_failed_time)::bigint, 0)
     FROM pg_stat_archiver")

write_metrics wal-archive <<EOF
# HELP memorio_wal_archived_last_timestamp_seconds When the last WAL segment was archived (0: never).
# TYPE memorio_wal_archived_last_timestamp_seconds gauge
memorio_wal_archived_last_timestamp_seconds $archived
# HELP memorio_wal_archive_failed_total WAL archiving attempts that failed since the database started.
# TYPE memorio_wal_archive_failed_total counter
memorio_wal_archive_failed_total $failed
# HELP memorio_wal_archive_last_failed_timestamp_seconds When WAL archiving last failed (0: never).
# TYPE memorio_wal_archive_last_failed_timestamp_seconds gauge
memorio_wal_archive_last_failed_timestamp_seconds $last_failed
EOF
