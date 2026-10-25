#!/usr/bin/env bash
# Secret rotation on the stage 1 server (MEM-E0-PLAT-18, docs/runbooks/secret-leak.md), run as root in
# /opt/memorio/deploy. Each command changes the secret in the env file, recreates the API and the worker
# (a few seconds each, as in a deployment, D-041), waits until both are ready and puts the previous value
# back when they are not.
# Usage:
#   rotate.sh storage            media bucket key: NEW_STORAGE_ACCESS_KEY, NEW_STORAGE_SECRET_KEY, created at the
#                                provider next to the old one; checked on the bucket before the hosts switch
#   rotate.sh storage-revoked    after the old key was deleted at the provider: the bucket must refuse it
#   rotate.sh database <role>    new random password for a memorio_* role (the owner memorio_admin too)
#   rotate.sh data-protection    new Data Protection keys: every access and refresh token stops working
#   rotate.sh apply-passwords    every role's password from postgres.env (e.g. after a restore to an earlier moment)
set -euo pipefail
# shellcheck source-path=SCRIPTDIR/../backup
source "$(dirname "$0")/../backup/common.sh"

usage() { sed -n '6,12p' "$0" >&2; exit 2; }

# A path compose reads from a variable (shell first, then the compose env file), as compose.yaml does.
compose_path() {
    local name="$1" value="${!1:-}" file
    if [ -z "$value" ]; then
        file="${COMPOSE_ENV_FILES:-.env}"
        [[ "$file" = /* ]] || file="$deploy_dir/$file"
        value="$(sed -n "s/^$name=\([^ #]*\).*/\1/p" "$file" 2>/dev/null || true)"
    fi
    value="${value:-$2}"
    [[ "$value" = /* ]] || value="$deploy_dir/$value"
    echo "$value"
}
app_env="$(compose_path APP_ENV_FILE app.env)"
postgres_env="$(compose_path POSTGRES_ENV_FILE postgres.env)"
# The storage key the hosts used before the last `storage`, until `storage-revoked` confirms it is dead.
previous_storage_key="$(dirname "$app_env")/.previous-storage-key"

kept="$(mktemp -d)"
trap 'rm -rf "$kept"' EXIT
keep() { cp -p "$1" "$kept/$(basename "$1")"; }
put_back() { cp -p "$kept/$(basename "$1")" "$1"; }

# The value of a variable in an env file: the rest of the line (secrets are written without comments).
value_of() { sed -n "s/^$1=//p" "$2" | tail -n 1; }

# NAME=value in an env file, in place and still mode 0600; the value goes through the environment, so
# no character in it needs escaping.
set_value() {
    (umask 077 && NAME="$2" VALUE="$3" awk -F= '
        $1 == ENVIRON["NAME"] { print ENVIRON["NAME"] "=" ENVIRON["VALUE"]; found = 1; next }
        { print }
        END { if (!found) print ENVIRON["NAME"] "=" ENVIRON["VALUE"] }' "$1" > "$1.new")
    mv "$1.new" "$1"
}

# The password in every connection string of a role in the app env file.
set_connection_password() {
    (umask 077 && ROLE="$1" VALUE="$2" awk '
        index($0, "Username=" ENVIRON["ROLE"] ";") && match($0, /Password=[^;]*/) {
            $0 = substr($0, 1, RSTART - 1) "Password=" ENVIRON["VALUE"] substr($0, RSTART + RLENGTH)
        }
        { print }' "$app_env" > "$app_env.new")
    mv "$app_env.new" "$app_env"
}

restart_hosts() {
    compose up -d --no-deps --force-recreate api worker && hosts_ready
}

# Head of the media bucket with a key, from the S3 client container (the hosts' view of the endpoint).
bucket_with() {
    AWS_ACCESS_KEY_ID="$1" AWS_SECRET_ACCESS_KEY="$2" compose run --rm --no-deps -T \
        -e AWS_ACCESS_KEY_ID -e AWS_SECRET_ACCESS_KEY storage-cli s3api head-bucket \
        --bucket "$(value_of Storage__Bucket "$app_env")" \
        --endpoint-url "$(value_of Storage__Endpoint "$app_env")" \
        --region "$(value_of Storage__Region "$app_env")"
}

rotate_storage() {
    local new_access="${NEW_STORAGE_ACCESS_KEY:?the new key, created at the provider for the media bucket}"
    local new_secret="${NEW_STORAGE_SECRET_KEY:?the secret of the new key}"
    local old_access
    old_access="$(value_of Storage__AccessKey "$app_env")"
    [ "$new_access" != "$old_access" ] || { echo "The hosts already use $new_access." >&2; exit 1; }

    echo "== New key $new_access on the bucket"
    bucket_with "$new_access" "$new_secret" || { echo "The new key cannot reach the bucket: nothing changed." >&2; exit 1; }

    echo "== API and worker on the new key"
    keep "$app_env"
    set_value "$app_env" Storage__AccessKey "$new_access"
    set_value "$app_env" Storage__SecretKey "$new_secret"
    if ! restart_hosts; then
        echo "Not ready on the new key: back on $old_access." >&2
        put_back "$app_env"
        restart_hosts || true
        exit 1
    fi
    (umask 077 && printf 'ACCESS_KEY=%s\nSECRET_KEY=%s\n' "$old_access" \
        "$(value_of Storage__SecretKey "$kept/$(basename "$app_env")")" > "$previous_storage_key")
    echo "Done. Links signed with $old_access work until it is deleted at the provider; then run: $0 storage-revoked"
}

storage_revoked() {
    [ -f "$previous_storage_key" ] || { echo "No previous key waiting: run $0 storage first." >&2; exit 1; }
    local old_access answer
    old_access="$(value_of ACCESS_KEY "$previous_storage_key")"
    if answer="$(bucket_with "$old_access" "$(value_of SECRET_KEY "$previous_storage_key")" 2>&1)"; then
        echo "The bucket still accepts $old_access: delete it at the provider first." >&2
        exit 1
    fi
    if ! grep -q '(403)' <<<"$answer"; then
        echo "Could not tell whether $old_access is refused: $answer" >&2
        exit 1
    fi
    rm -f "$previous_storage_key"
    echo "ok: the bucket refuses $old_access (403); every link signed with it stopped working"
}

# Every role's password from postgres.env, by deploy/postgres/30-role-passwords.sh inside the database
# container; the values reach it through the environment, never the command line.
apply_passwords() {
    (
        names=()
        while IFS= read -r line; do
            if [[ "$line" =~ ^([A-Z_][A-Z0-9_]*)=(.*)$ ]]; then
                export "${BASH_REMATCH[1]}=${BASH_REMATCH[2]}"
                names+=(-e "${BASH_REMATCH[1]}")
            fi
        done < "$postgres_env"
        compose exec -T "${names[@]}" postgres bash -s < "$deploy_dir/postgres/30-role-passwords.sh"
    )
}

# Over the network as the hosts sign in: the image trusts the socket and 127.0.0.1 without a password.
signs_in() {
    PGPASSWORD="$2" compose exec -T -e PGPASSWORD postgres \
        psql -h postgres -U "$1" -d memorio -qtAX -c 'SELECT 1' >/dev/null 2>&1
}

rotate_database() {
    local role="${1:-}" variable old new
    [[ "$role" =~ ^memorio_[a-z_]+$ ]] || usage
    [ "$(psql_live "SELECT count(*) FROM pg_roles WHERE rolname = '$role'")" = 1 ] || { echo "No role $role." >&2; exit 1; }
    variable="PG_PASSWORD_${role^^}"
    [ "$role" != "$(psql_live 'SELECT current_user')" ] || variable=POSTGRES_PASSWORD
    old="$(value_of "$variable" "$postgres_env")"
    new="$(openssl rand -hex 24)"

    echo "== New password of $role"
    keep "$postgres_env"
    keep "$app_env"
    set_value "$postgres_env" "$variable" "$new"
    set_connection_password "$role" "$new"
    apply_passwords >/dev/null
    # Open connections stay signed in; the hosts reconnect with the new password once recreated.
    if ! cmp -s "$app_env" "$kept/$(basename "$app_env")"; then
        echo "== API and worker reconnect with it"
        if ! restart_hosts; then
            echo "Not ready with the new password: the old one is back." >&2
            put_back "$postgres_env"
            put_back "$app_env"
            apply_passwords >/dev/null
            restart_hosts || true
            exit 1
        fi
    fi
    signs_in "$role" "$new" || { echo "$role does not sign in with the new password." >&2; exit 1; }
    if [ -n "$old" ] && signs_in "$role" "$old"; then
        echo "$role still signs in with the old password." >&2
        exit 1
    fi
    echo "ok: $role has a new password and the old one is refused"
}

rotate_data_protection() {
    echo "== New Data Protection keys: every session ends, users sign in again"
    psql_live 'DELETE FROM account."DataProtectionKeys"' >/dev/null
    restart_hosts || { echo "API or worker not ready after the new keys." >&2; exit 1; }
    echo "ok: tokens protected with the old keys are refused"
}

case "${1:-}" in
    storage) rotate_storage ;;
    storage-revoked) storage_revoked ;;
    database) rotate_database "${2:-}" ;;
    data-protection) rotate_data_protection ;;
    apply-passwords) apply_passwords ;;
    *) usage ;;
esac
case "${1:-}" in
    storage | database | data-protection)
        echo "Refresh the encrypted copy of this environment (deploy/secrets/secrets.sh encrypt) and commit it." ;;
esac
