#!/usr/bin/env bash
# First start of a deployed database: every Memorio role gets its password from the environment
# variable PG_PASSWORD_<ROLE IN UPPER CASE> (e.g. PG_PASSWORD_MEMORIO_ACCOUNT), set from this
# environment's secrets (MEM-E0-PLAT-18). A role without its variable stops the initialization, so no
# role is left without a password or with a default one (D-129). The owner's password is POSTGRES_PASSWORD.
# Also run later by deploy/secrets/rotate.sh with the values of postgres.env (rotation, after a restore).
set -euo pipefail

roles=$(psql -v ON_ERROR_STOP=1 --username "$POSTGRES_USER" --dbname "$POSTGRES_DB" -tAc \
    "SELECT rolname FROM pg_roles WHERE rolname LIKE 'memorio\_%' AND rolname <> current_user ORDER BY rolname")

set_password() {
    psql -v ON_ERROR_STOP=1 -q --username "$POSTGRES_USER" --dbname "$POSTGRES_DB" \
        -v role="$1" -v password="$2" <<'SQL'
ALTER ROLE :"role" PASSWORD :'password';
SQL
}

for role in $roles; do
    variable="PG_PASSWORD_${role^^}"
    password="${!variable:-}"
    if [ -z "$password" ]; then
        echo "Missing $variable: role $role would have no password." >&2
        exit 1
    fi
    set_password "$role" "$password"
done
if [ -n "${POSTGRES_PASSWORD:-}" ]; then
    set_password "$POSTGRES_USER" "$POSTGRES_PASSWORD"
    roles="$roles $POSTGRES_USER"
fi
echo "Passwords set for: $(echo "$roles" | tr '\n' ' ')"
