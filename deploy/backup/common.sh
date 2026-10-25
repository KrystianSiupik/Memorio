# shellcheck shell=bash
# Shared by the operation scripts of deploy/backup (sourced, not run). They run on the application
# server from systemd timers (deploy/systemd), in /opt/memorio/deploy; COMPOSE_FILE and
# COMPOSE_ENV_FILES select other Compose files (the local check uses compose.local.yaml).

deploy_dir="$(cd "$(dirname "${BASH_SOURCE[0]}")/.." && pwd)"
metrics_dir="${METRICS_DIR:-/var/lib/memorio/metrics}"

compose() {
    (cd "$deploy_dir" && docker compose "$@")
}

# psql as the database owner inside the live database container.
psql_live() {
    compose exec -T postgres psql -v ON_ERROR_STOP=1 -qtAX -c "$1"
}

# Both hosts answer /ready inside the network (database, messaging, object storage), within a minute.
hosts_ready() {
    for _ in $(seq 1 60); do
        if compose exec -T nginx wget -qO /dev/null http://api:8080/ready 2>/dev/null \
            && compose exec -T nginx wget -qO /dev/null http://worker:8080/ready 2>/dev/null; then
            return 0
        fi
        sleep 1
    done
    return 1
}

# Replaces <name>.prom in the node exporter's textfile directory atomically with the standard input.
write_metrics() {
    mkdir -p "$metrics_dir"
    local file
    file="$(mktemp "$metrics_dir/.$1.XXXXXX")"
    cat > "$file"
    chmod 0644 "$file"
    mv "$file" "$metrics_dir/$1.prom"
}
