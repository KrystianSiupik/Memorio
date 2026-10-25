#!/usr/bin/env bash
# Local check of the one-command deployment (MEM-E0-PLAT-13; CI job stage1): on the local stage 1 server
# (compose.local.yaml) deploy/release.sh moves from release "check" to "next", then a release whose API
# cannot start is refused and "next" runs again. Images are local tags; no checkout, no pull.
# Leaves the stack running; `KEEP=0` removes it with its volumes at the end. Needs Docker.
set -euo pipefail

root="$(cd "$(dirname "$0")/../.." && pwd)"
run="$root/deploy/local/run"
mkdir -p "$run"
cp "$root/deploy/local/local.env" "$run/release.env"
export COMPOSE_PATH_SEPARATOR=":"
export COMPOSE_FILE="compose.yaml:compose.local.yaml"
export COMPOSE_ENV_FILES="local/run/release.env"
export METRICS_DIR="$run/metrics"
mkdir -p "$METRICS_DIR"   # by this user: Docker would create a missing mounted directory as root
export DEPLOYMENTS_LOG="$run/deployments.log"
export RELEASE_CHECKOUT=0 RELEASE_PULL=0
compose() { (cd "$root/deploy" && docker compose "$@"); }
fail() { echo "FAIL: $1"; compose logs --tail 40 api; exit 1; }
running() { sed -n 's/^MEMORIO_VERSION=//p' "$run/release.env"; }
api_image() { compose ps --format '{{.Image}}' api; }

compose up -d --build --wait --wait-timeout 300

for image in api worker postgres; do
    docker tag "memorio-local/memorio-$image:check" "memorio-local/memorio-$image:next"
    docker tag "memorio-local/memorio-$image:next" "memorio-local/memorio-$image:broken"
done
printf 'FROM memorio-local/memorio-api:next\nENTRYPOINT ["false"]\n' | docker build -q -t memorio-local/memorio-api:broken - >/dev/null

bash "$root/deploy/release.sh" next || fail "release next"
[ "$(running)" = next ] && [ "$(api_image)" = "memorio-local/memorio-api:next" ] || fail "next is not running"
grep -q 'memorio_deployment_info{version="next"} 1' "$METRICS_DIR/deployment.prom" || fail "deployment metric"
echo "ok: release next deployed"

if bash "$root/deploy/release.sh" broken; then fail "a release whose API cannot start was accepted"; fi
[ "$(running)" = next ] && [ "$(api_image)" = "memorio-local/memorio-api:next" ] || fail "next is not running again"
[ "$(curl -ks -o /dev/null -w '%{http_code}' https://localhost:8443/ready)" = 200 ] || fail "API not ready after the rollback"
grep -q ' broken rolled back to next ' "$DEPLOYMENTS_LOG" || fail "deployment log"
echo "ok: a broken release is rolled back to the previous one"

if [ "${KEEP:-1}" = 0 ]; then
    compose --profile operations down --volumes
fi
echo "One-command deployment works."
