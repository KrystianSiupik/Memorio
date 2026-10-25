#!/usr/bin/env bash
# Connection budget (MEM-E0-PLAT-21, D-182; CI job stage1): every connection string of the hosts has an
# explicit Maximum Pool Size and Timeout, and the ceilings of all pools of all processes stay under
# PostgreSQL's max_connections minus a reserve, so the server never refuses a connection (53300) while a
# pool still has room. Each role has one pool per process (D-181).
set -euo pipefail

root="$(cd "$(dirname "$0")/../.." && pwd)"
cd "$root"
# superuser_reserved_connections (3, the operation scripts as the owner), base backups, an administrator, headroom.
reserve=20
failed=0

# A setting of a connection string ("Maximum Pool Size" also spelled "Max Pool Size"), empty when not set.
setting() {
    tr ';' '\n' <<<"$1" | sed -n -E "s/^[[:space:]]*($2)[[:space:]]*=[[:space:]]*([0-9]+)[[:space:]]*$/\2/Ip" | head -n 1
}

# Checks one connection string; its pool ceiling goes to $size (no subshell, so a failure is kept).
ceiling() {
    local where="$1" value="$2" timeout
    size="$(setting "$value" 'Maximum Pool Size|Max Pool Size')"
    timeout="$(setting "$value" 'Timeout')"
    if [ -z "$size" ] || [ -z "$timeout" ]; then
        echo "FAIL: $where has no explicit Maximum Pool Size and Timeout" >&2
        failed=1
        size=100   # Npgsql's default, what the pool would really use
    fi
}

budget() {
    local label="$1" sum="$2" max="$3" left=$(($3 - reserve))
    if [ "$sum" -gt "$left" ]; then
        echo "FAIL: $label: pools $sum > max_connections $max − reserve $reserve = $left" >&2
        failed=1
    else
        echo "ok: $label: pools $sum ≤ max_connections $max − reserve $reserve = $left"
    fi
}

# Stage 1 server: API and worker read the same app.env, so each of its pools counts once per host.
# shellcheck disable=SC2016 # a literal ${...} of compose.yaml, not an expansion
default_max="$(sed -n 's/.*max_connections=\${POSTGRES_MAX_CONNECTIONS:-\([0-9]*\)}.*/\1/p' deploy/compose.yaml)"
for pair in deploy/app.env.example:deploy/env.example deploy/local/app.env:deploy/local/local.env; do
    app_env="${pair%%:*}"
    compose_env="${pair##*:}"
    max="$(sed -n 's/^POSTGRES_MAX_CONNECTIONS=\([0-9]*\).*/\1/p' "$compose_env")"
    sum=0
    while IFS= read -r line; do
        ceiling "$app_env: ${line%%=*}" "${line#*=}"
        sum=$((sum + 2 * size))
    done < <(grep -E '^[A-Za-z_]+__ConnectionString=' "$app_env")
    budget "$app_env (API + worker)" "$sum" "${max:-$default_max}"
done

# Local development (compose.yaml): each service lists its own connection strings; YAML anchors reuse one.
declare -A anchors=()
sum=0
while IFS= read -r line; do
    value="${line#*__ConnectionString: }"
    name="${line%%: *}"
    name="${name##* }"
    if [[ "$value" =~ ^\*([A-Za-z_]+)$ ]]; then
        size="${anchors[${BASH_REMATCH[1]}]:?unknown anchor in compose.yaml: $value}"
    else
        anchor=""
        if [[ "$value" =~ ^\&([A-Za-z_]+)\ (.*)$ ]]; then
            anchor="${BASH_REMATCH[1]}"
            value="${BASH_REMATCH[2]}"
        fi
        ceiling "compose.yaml: $name" "$value"
        [ -z "$anchor" ] || anchors[$anchor]="$size"
    fi
    sum=$((sum + size))
done < <(grep -E '__ConnectionString: ' compose.yaml)
budget "compose.yaml (local development)" "$sum" 100

exit "$failed"
