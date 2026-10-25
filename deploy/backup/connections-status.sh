#!/usr/bin/env bash
# Connections of the database server for the monitoring server (MEM-E0-PLAT-21), run every minute by
# memorio-connections-status.timer: client connections per role against max_connections. Alert
# DatabaseConnectionsHigh (monitoring/prometheus/rules) above 80%. Runs as the owner over the socket,
# which keeps a reserved slot (superuser_reserved_connections), so it measures even when the server is full.
set -euo pipefail
# shellcheck source-path=SCRIPTDIR
source "$(dirname "$0")/common.sh"

max="$(psql_live 'SHOW max_connections')"
per_role="$(psql_live "SELECT coalesce(usename, ''), count(*) FROM pg_stat_activity
                       WHERE backend_type = 'client backend' GROUP BY 1 ORDER BY 1")"

{
    echo "# HELP memorio_postgres_max_connections The server's max_connections."
    echo "# TYPE memorio_postgres_max_connections gauge"
    echo "memorio_postgres_max_connections $max"
    echo "# HELP memorio_postgres_connections Client connections to the server, per role."
    echo "# TYPE memorio_postgres_connections gauge"
    while IFS='|' read -r role count; do
        if [ -n "$count" ]; then echo "memorio_postgres_connections{role=\"$role\"} $count"; fi
    done <<<"$per_role"
} | write_metrics connections
