#!/usr/bin/env bash
# The manual source of the entitlement ledger in Plans (MEM-E0-PLAT-15, D-078, D-188), on the stage 1 server as
# root in /opt/memorio/deploy: a paid plan for testers, a goodwill extension, a complaint settled outside the
# stores. The plan cycle reads only the ledger, so this is the same as a payment for it. The SQL is in the *.sql
# files next to this script. Usage:
#   entitlements.sh list <account|album> <id>                                   the ledger lines of one
#   entitlements.sh grant <account|album> <id> --until <YYYY-MM-DD> --reason "…"  paid until that day (UTC)
#   entitlements.sh end <account|album> <id> --reason "…"                       ends the manual entitlement now
set -euo pipefail
# shellcheck source-path=SCRIPTDIR/../backup
source "$(dirname "$0")/../backup/common.sh"

sql_dir="$(cd "$(dirname "$0")" && pwd)"
operations_log="${OPERATIONS_LOG:-/var/lib/memorio/operations.log}"

usage() { sed -n '6,8p' "$0" >&2; exit 2; }

# A SQL file of this directory through psql as the database owner; the file switches to memorio_plans.
run_sql() {
    local file="$1"
    shift
    compose exec -T postgres psql -v ON_ERROR_STOP=1 -X -q "$@" -f - < "$sql_dir/$file"
}

# The resource kind as the ledger stores it (EntitledResourceKind); anything else stops the script.
kind_of() {
    case "$1" in
        account) echo Account ;;
        album) echo Album ;;
        *) usage ;;
    esac
}

valid_id() { [[ "$1" =~ ^[0-9a-f]{8}-[0-9a-f]{4}-[0-9a-f]{4}-[0-9a-f]{4}-[0-9a-f]{12}$ ]] || usage; }

grant() {
    local kind="$1" id="$2" until="$3" reason="$4" row
    [ -n "$reason" ] || usage
    row="$(run_sql grant.sql -tA -v kind="$kind" -v id="$id" -v until="$until" -v note="$reason")"
    mkdir -p "$(dirname "$operations_log")"
    echo "$(date -u +%Y-%m-%dT%H:%M:%SZ) manual entitlement by ${SUDO_USER:-$(id -un)}: $row" >> "$operations_log"
    echo "ok: $kind $id paid until $until (manual; written to $operations_log)"
}

command="${1:-}"
[ $# -ge 3 ] || usage
[[ "$2" =~ ^(account|album)$ ]] || usage   # before kind_of: usage in its subshell would not stop the script
kind="$(kind_of "$2")"
valid_id "$3"
case "$command" in
    list) run_sql list.sql -v kind="$kind" -v id="$3" ;;
    grant)
        [ "${4:-}" = --until ] && [[ "${5:-}" =~ ^[0-9]{4}-[0-9]{2}-[0-9]{2}$ ]] && [ "${6:-}" = --reason ] || usage
        grant "$kind" "$3" "${5}T23:59:59Z" "${7:-}" ;;
    end)
        [ "${4:-}" = --reason ] || usage
        grant "$kind" "$3" "$(date -u +%Y-%m-%dT%H:%M:%SZ)" "${5:-}" ;;
    *) usage ;;
esac
