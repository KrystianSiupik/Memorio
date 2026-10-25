#!/usr/bin/env bash
# One-command deployment of a release on the stage 1 server (D-041, MEM-E0-PLAT-13), run as root in
# /opt/memorio/deploy, usually through tools/deploy/deploy.sh. Steps: the repository to the release tag
# (configuration of that release), a named restore point in the database, the release's images, a short
# restart, then checks; when a check fails, the previous version is started again. The database is never
# rolled back: migrations only expand (SD18), so the previous version runs on the migrated schema.
# Usage: ./release.sh <version>      e.g. ./release.sh 0.2.0 (tag v0.2.0, images :0.2.0)
# RELEASE_CHECKOUT=0 / RELEASE_PULL=0 skip the checkout / the pull (local check, tools/deploy/check-release.sh).
set -euo pipefail

version="${1:?usage: release.sh <version>}"
if [[ ! "$version" =~ ^[0-9A-Za-z][0-9A-Za-z.-]*$ ]]; then
    echo "Not a version: \"$version\"." >&2
    exit 1
fi
deploy_dir="$(cd "$(dirname "$0")" && pwd)"

# The checkout below may replace this very file; run from a copy so bash keeps reading what it started.
if [ -z "${RELEASE_RUNNING_COPY:-}" ]; then
    copy="$(mktemp)"
    cp "$0" "$copy"
    RELEASE_RUNNING_COPY=1 RELEASE_DEPLOY_DIR="$deploy_dir" exec bash "$copy" "$@"
fi
deploy_dir="$RELEASE_DEPLOY_DIR"

if [ "${RELEASE_CHECKOUT:-1}" = 1 ]; then
    echo "== Repository to v$version"
    git -C "$deploy_dir/.." fetch --tags --quiet
    git -C "$deploy_dir/.." checkout --quiet --detach "v$version"
fi

# shellcheck source-path=SCRIPTDIR
source "$deploy_dir/backup/common.sh"

env_file="${COMPOSE_ENV_FILES:-$deploy_dir/.env}"
[[ "$env_file" = /* ]] || env_file="$deploy_dir/$env_file"
previous="$(sed -n 's/^MEMORIO_VERSION=\([^ #]*\).*/\1/p' "$env_file")"
domain="$(sed -n 's/^MEMORIO_DOMAIN=\([^ #]*\).*/\1/p' "$env_file")"
https_port="$(sed -n 's/^HTTPS_PORT=\([^ #]*\).*/\1/p' "$env_file")"
https_port="${https_port:-443}"
deployments_log="${DEPLOYMENTS_LOG:-/var/lib/memorio/deployments.log}"
# A host that does not start restarts in a loop; the hosts start in seconds, so 2 minutes tells.
wait_timeout="${RELEASE_WAIT_TIMEOUT:-120}"

set_version() { sed -i "s/^MEMORIO_VERSION=[^ #]*/MEMORIO_VERSION=$1/" "$env_file"; }

# Ready means: both hosts answer /ready inside the network, and the API answers through nginx and TLS.
ready() {
    hosts_ready || return 1
    for _ in $(seq 1 60); do
        curl -fsk -o /dev/null --resolve "$domain:$https_port:127.0.0.1" "https://$domain:$https_port/v1/app/version-policy" && return 0
        sleep 1
    done
    return 1
}

start() {
    set_version "$1" && compose config --quiet && compose up -d --wait --wait-timeout "$wait_timeout" && ready
}

echo "== Release $version (running: ${previous:-none})"
if [ "${RELEASE_PULL:-1}" = 1 ]; then
    MEMORIO_VERSION="$version" compose pull --quiet postgres api worker
fi

# A named point to restore to if the release damages data (restore-server.sh with its time).
restore_point="$(psql_live "SELECT pg_create_restore_point('before-$version'), clock_timestamp()" | cut -d'|' -f2)"
echo "Restore point before-$version at $restore_point"

started=$(date +%s)
if start "$version"; then
    outcome="deployed"
else
    echo "== Release $version failed its checks: starting $previous again" >&2
    compose logs --tail 40 api worker >&2 || true
    if [ -n "$previous" ] && start "$previous"; then
        outcome="rolled back to $previous"
    else
        outcome="FAILED, rollback to ${previous:-none} failed too"
    fi
fi

mkdir -p "$(dirname "$deployments_log")"
echo "$(date -u +%Y-%m-%dT%H:%M:%SZ) $version $outcome (restore point $restore_point, $(($(date +%s) - started)) s)" >> "$deployments_log"
running="$(sed -n 's/^MEMORIO_VERSION=\([^ #]*\).*/\1/p' "$env_file")"
write_metrics deployment <<EOF
# HELP memorio_deployment_info The release running on this server.
# TYPE memorio_deployment_info gauge
memorio_deployment_info{version="$running"} 1
# HELP memorio_deployment_last_timestamp_seconds When a release was last attempted.
# TYPE memorio_deployment_last_timestamp_seconds gauge
memorio_deployment_last_timestamp_seconds $(date +%s)
EOF

echo "== $version: $outcome"
[ "$outcome" = "deployed" ]
