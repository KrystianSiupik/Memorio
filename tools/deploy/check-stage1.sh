#!/usr/bin/env bash
# Local check of the stage 1 server (MEM-E0-PLAT-09; CI job stage1): starts deploy/compose.yaml with
# compose.local.yaml (images from this checkout, local S3 server, self-signed certificate), then checks
# nginx (TLS, redirect, sign-in required, worker readiness closed to the internet), WAL archiving, a
# base backup, the point-in-time restore test with its report and metrics (SD13, QS2), and disaster
# recovery of a lost database volume from the backup bucket.
# Leaves the stack running; `KEEP=0` removes it with its volumes at the end. Needs Docker.
set -euo pipefail

root="$(cd "$(dirname "$0")/../.." && pwd)"
export COMPOSE_PATH_SEPARATOR=":"
export COMPOSE_FILE="compose.yaml:compose.local.yaml"
export COMPOSE_ENV_FILES="local/local.env"
export METRICS_DIR="$root/deploy/local/run/metrics"
export REPORTS_DIR="$root/deploy/local/run/restore-reports"
https="https://localhost:8443"
compose() { (cd "$root/deploy" && docker compose "$@"); }

fail() { echo "FAIL: $1"; compose logs --tail 60; exit 1; }
status_of() { curl -ks -o /dev/null -w '%{http_code}' "$@"; }

rm -rf "$root/deploy/local/run"
compose up -d --build --wait --wait-timeout 300

# The API needs a few seconds after its container starts (migrations, Wolverine). Only the API answers
# 401 for /v1 (the worker answers 404), so this also waits until nginx reaches the API itself.
wait_for_api() {
    for _ in $(seq 1 120); do
        [ "$(status_of "$https/ready")" = 200 ] && [ "$(status_of "$https/v1/account/me")" = 401 ] && return
        sleep 1
    done
}
wait_for_api

[ "$(status_of "$https/health")" = 200 ] || fail "API /health through nginx and TLS"
[ "$(status_of "$https/ready")" = 200 ] || fail "API /ready through nginx"
[ "$(status_of "$https/v1/account/me")" = 401 ] || fail "/v1 requires a signed-in account"
[ "$(status_of "http://localhost:8180/health")" = 301 ] || fail "HTTP redirects to HTTPS"
[ "$(status_of "$https/internal/worker/ready")" = 403 ] || fail "worker readiness closed outside the monitoring server"
echo "ok: nginx (TLS, redirect, sign-in required, worker readiness closed)"

# A deployment replaces the API container (new address): nginx must follow it, not the old address.
compose up -d --force-recreate --no-deps api
wait_for_api
[ "$(status_of "$https/v1/account/me")" = 401 ] || fail "nginx reaches the API after it is recreated"
echo "ok: nginx follows a recreated API container"

registered=$(curl -ks -o /dev/null -w '%{http_code}' -X POST "$https/v1/auth/register" -H 'Content-Type: application/json' \
    -H 'X-App-Version: 99.0.0' -d '{"email":"stage1@example.com","password":"long-enough-password","declaresMinimumAge":true}')
[ "$registered" = 200 ] || [ "$registered" = 409 ] || fail "registering an account ($registered)"
echo "ok: an account is registered through nginx"

bash "$root/deploy/backup/backup.sh"
grep -q '^memorio_backup_last_success_timestamp_seconds [1-9]' "$METRICS_DIR/backup.prom" || fail "backup metrics"
echo "ok: base backup and its metrics"

bash "$root/deploy/backup/restore-test.sh" || fail "restore test"
grep -q '^memorio_restore_test_last_success_timestamp_seconds [1-9]' "$METRICS_DIR/restore-test.prom" || fail "restore test metrics"
ls "$REPORTS_DIR"/restore-*.md >/dev/null || fail "restore test report"
echo "ok: point-in-time restore test, report and metrics"

bash "$root/deploy/backup/archive-status.sh"
grep -q '^memorio_wal_archived_last_timestamp_seconds [1-9]' "$METRICS_DIR/wal-archive.prom" || fail "WAL archive metrics"
echo "ok: WAL archive status"

# Disaster recovery: the database volume is lost; the server is restored from the bucket and the account
# registered before the backup signs in again.
compose stop api worker postgres
compose rm -f postgres
docker volume rm "memorio-stage1_postgres-data" >/dev/null
bash "$root/deploy/backup/restore-server.sh"
wait_for_api
signed_in=$(curl -ks -o /dev/null -w '%{http_code}' -X POST "$https/v1/auth/login" -H 'Content-Type: application/json' \
    -H 'X-App-Version: 99.0.0' -d '{"email":"stage1@example.com","password":"long-enough-password"}')
[ "$signed_in" = 200 ] || fail "the account signs in after the database was restored ($signed_in)"
echo "ok: database restored from the backup bucket after losing its volume"

if [ "${KEEP:-1}" = 0 ]; then
    compose --profile operations down --volumes
fi
echo "Stage 1 server works."
