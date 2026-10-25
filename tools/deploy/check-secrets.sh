#!/usr/bin/env bash
# Local check of the secret rotation (MEM-E0-PLAT-18; CI job stage1) on the stage 1 stack that
# tools/deploy/check-stage1.sh leaves running, with copies of the env files in deploy/local/run/:
# Data Protection keys (a token from before is refused), a database role's password (hosts ready, the old
# password refused), the media bucket key (a link signed with the deleted key gets 403, new links work);
# then back to the checked-in values, passwords set again from postgres.env (apply-passwords).
set -euo pipefail

root="$(cd "$(dirname "$0")/../.." && pwd)"
export COMPOSE_PATH_SEPARATOR=":"
export COMPOSE_FILE="compose.yaml:compose.local.yaml"
export COMPOSE_ENV_FILES="local/local.env"
export APP_ENV_FILE="local/run/app.env"
export POSTGRES_ENV_FILE="local/run/postgres.env"
# The same metrics directory as check-stage1.sh, so the node exporter is not recreated with another one.
export METRICS_DIR="$root/deploy/local/run/metrics"
https="https://localhost:8443"
compose() { (cd "$root/deploy" && docker compose "$@"); }
rotate() { bash "$root/deploy/secrets/rotate.sh" "$@"; }
# MSYS_NO_PATHCONV: Git Bash on Windows would rewrite the container path /garage; no effect elsewhere.
garage() { MSYS_NO_PATHCONV=1 compose exec -T garage /garage "$@" >/dev/null; }

fail() { echo "FAIL: $1"; compose logs --tail 60 api worker; exit 1; }
status_of() { curl -ks -o /dev/null -w '%{http_code}' "$@"; }

wait_for_api() {
    for _ in $(seq 1 120); do
        [ "$(status_of "$https/ready")" = 200 ] && [ "$(status_of "$https/v1/account/me")" = 401 ] && return
        sleep 1
    done
    fail "API ready through nginx"
}

account='{"email":"secrets@example.com","password":"long-enough-password"}'
sign_in() {
    curl -ks -X POST "$https/v1/auth/login" -H 'Content-Type: application/json' -H 'X-App-Version: 99.0.0' -d "$account" \
        | sed -n 's/.*"accessToken":"\([^"]*\)".*/\1/p'
}
me_with() { status_of -H "Authorization: Bearer $1" "$https/v1/account/me"; }

# The media bucket from inside the network (the hosts' view): S3 client and the status of a link.
s3_with() {
    AWS_ACCESS_KEY_ID="$1" AWS_SECRET_ACCESS_KEY="$2" compose run --rm --no-deps -T \
        -e AWS_ACCESS_KEY_ID -e AWS_SECRET_ACCESS_KEY storage-cli \
        --endpoint-url http://garage:3900 --region garage "${@:3}"
}
link_status() {
    compose exec -T nginx wget -S -q -O /dev/null "$1" 2>&1 | grep -o 'HTTP/1.1 [0-9]*' | tail -n 1 | cut -d' ' -f2
}

mkdir -p "$METRICS_DIR"
install -m 0600 "$root/deploy/local/app.env" "$root/deploy/local/run/app.env"
install -m 0600 "$root/deploy/local/postgres.env" "$root/deploy/local/run/postgres.env"
rotate apply-passwords >/dev/null   # a run that stopped half way may have left other passwords
compose up -d --wait
wait_for_api

# Data Protection: the keys protect the access and refresh tokens.
curl -ks -o /dev/null -X POST "$https/v1/auth/register" -H 'Content-Type: application/json' -H 'X-App-Version: 99.0.0' \
    -d '{"email":"secrets@example.com","password":"long-enough-password","declaresMinimumAge":true}'
before="$(sign_in)"
[ "$(me_with "$before")" = 200 ] || fail "a signed-in account reads /v1/account/me"
rotate data-protection
wait_for_api
[ "$(me_with "$before")" = 401 ] || fail "a token from before the new Data Protection keys is refused"
[ "$(me_with "$(sign_in)")" = 200 ] || fail "signing in again works after the new keys"
echo "ok: Data Protection keys rotated, tokens from before refused"

# A database role used by the API: new password, hosts ready, the old one refused (rotate.sh checks both).
rotate database memorio_account
wait_for_api
grep -q 'Username=memorio_account;Password=stage1_local' "$root/deploy/local/run/app.env" \
    && fail "the connection string still has the old password"
[ "$(me_with "$(sign_in)")" = 200 ] || fail "signing in works with the new password of memorio_account"
echo "ok: database role password rotated without the hosts losing readiness"

# Media bucket key. The local default key stays (the local backups use it too): the hosts move to key A,
# then from A to B, and A is the old key that gets deleted.
key_a=GK0000000000000000000000a1
secret_a=a1a1a1a1a1a1a1a1a1a1a1a1a1a1a1a1a1a1a1a1a1a1a1a1a1a1a1a1a1a1a1a1
key_b=GK0000000000000000000000b2
secret_b=b2b2b2b2b2b2b2b2b2b2b2b2b2b2b2b2b2b2b2b2b2b2b2b2b2b2b2b2b2b2b2b2
for key in "$key_a" "$key_b"; do
    garage key delete --yes "$key" 2>/dev/null || true
done
garage key import --yes -n rotation-a "$key_a" "$secret_a"
garage bucket allow --read --write memorio-media --key "$key_a"
garage key import --yes -n rotation-b "$key_b" "$secret_b"
garage bucket allow --read --write memorio-media --key "$key_b"

NEW_STORAGE_ACCESS_KEY="$key_a" NEW_STORAGE_SECRET_KEY="$secret_a" rotate storage
s3_with "$key_a" "$secret_a" s3api put-object --bucket memorio-media --key rotation/probe.txt >/dev/null
old_link="$(s3_with "$key_a" "$secret_a" s3 presign s3://memorio-media/rotation/probe.txt --expires-in 300 | tr -d '\r')"
[ "$(link_status "$old_link")" = 200 ] || fail "a link signed with key A opens"

NEW_STORAGE_ACCESS_KEY="$key_b" NEW_STORAGE_SECRET_KEY="$secret_b" rotate storage
[ "$(link_status "$old_link")" = 200 ] || fail "a link signed with key A keeps working until A is deleted"
if rotate storage-revoked 2>/dev/null; then
    fail "storage-revoked passes while key A still exists"
fi
garage key delete --yes "$key_a"
rotate storage-revoked
[ "$(link_status "$old_link")" = 403 ] || fail "a link signed with the deleted key gets 403"
new_link="$(s3_with "$key_b" "$secret_b" s3 presign s3://memorio-media/rotation/probe.txt --expires-in 300 | tr -d '\r')"
[ "$(link_status "$new_link")" = 200 ] || fail "a link signed with key B opens"
[ "$(status_of "$https/ready")" = 200 ] || fail "API ready on key B (readiness reads the bucket)"
echo "ok: media bucket key rotated, links signed with the deleted key get 403"

# Back to the checked-in local values for the checks that follow: the passwords from local/postgres.env.
unset APP_ENV_FILE POSTGRES_ENV_FILE
rotate apply-passwords >/dev/null
compose up -d --wait
wait_for_api
garage key delete --yes "$key_b"
echo "ok: passwords set again from postgres.env (apply-passwords), stack back on the local values"
