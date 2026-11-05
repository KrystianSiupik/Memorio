#!/usr/bin/env bash
# The service's side of a takeover case (MEM-E9-SUKCESJA-03, D-295, D-055), on the stage 1 server as root in
# /opt/memorio/deploy. A decision is a row the worker carries out within a minute; there is no public way in.
# Reading a certificate and every decision go to the operations log. Usage:
#   cases.sh list                                  reports waiting for a decision
#   cases.sh certificate <case id> <file>          downloads the certificate
#   cases.sh confirm <case id> --reason "…"        the death is confirmed
#   cases.sh reject <case id> --reason "…"         the report is not confirmed
#   cases.sh result <case id>                      what became of the decision
set -euo pipefail
# shellcheck source-path=SCRIPTDIR/../backup
source "$(dirname "$0")/../backup/common.sh"

operations_log="${OPERATIONS_LOG:-/var/lib/memorio/operations.log}"
operator="${SUDO_USER:-$(id -un)}"

usage() { sed -n '5,9p' "$0" >&2; exit 2; }

valid_id() { [[ "$1" =~ ^[0-9a-f]{8}-[0-9a-f]{4}-[0-9a-f]{4}-[0-9a-f]{4}-[0-9a-f]{12}$ ]] || usage; }

sql() { compose exec -T postgres psql -v ON_ERROR_STOP=1 -X -q -c "SET ROLE memorio_succession" "$@"; }

note() {
    mkdir -p "$(dirname "$operations_log")"
    echo "$(date -u +%Y-%m-%dT%H:%M:%SZ) succession by $operator: $1" >> "$operations_log"
}

decide() {
    local id="$1" confirmed="$2" reason="$3"
    [ -n "$reason" ] || usage
    sql -c "INSERT INTO succession.operator_decisions (id, case_id, confirmed, decided_by, decided_at) VALUES (gen_random_uuid(), '$id', $confirmed, '$operator', now())"
    note "case $id confirmed=$confirmed, reason: $reason"
    echo "ok: the worker applies it within a minute; check with: cases.sh result $id"
}

certificate() {
    local id="$1" file="$2" key
    key="$(sql -tA -c "SELECT certificate_key FROM succession.takeover_cases WHERE id = '$id'" | tail -1)"
    [ -n "$key" ] || { echo "no certificate for this case" >&2; exit 1; }
    note "case $id certificate read"
    # Bucket credentials from the application's own environment.
    # shellcheck disable=SC2016
    compose exec -T api sh -c 'AWS_ACCESS_KEY_ID="$Storage__AccessKey" AWS_SECRET_ACCESS_KEY="$Storage__SecretKey" aws --endpoint-url "$Storage__Endpoint" s3 cp "s3://$Storage__Bucket/'"$key"'" -' > "$file"
    echo "ok: written to $file; delete it after checking"
}

case "${1:-}" in
    list) sql -c "SELECT id, album_id, owner_account_id, reported_at FROM succession.takeover_cases WHERE state = 'Reported' ORDER BY reported_at" ;;
    result) valid_id "${2:-}"; sql -c "SELECT c.state, d.decided_by, d.decided_at, d.applied_at, d.outcome FROM succession.takeover_cases c LEFT JOIN succession.operator_decisions d ON d.case_id = c.id WHERE c.id = '$2' ORDER BY d.decided_at" ;;
    certificate) valid_id "${2:-}"; [ -n "${3:-}" ] || usage; certificate "$2" "$3" ;;
    confirm) valid_id "${2:-}"; [ "${3:-}" = --reason ] || usage; decide "$2" true "${4:-}" ;;
    reject) valid_id "${2:-}"; [ "${3:-}" = --reason ] || usage; decide "$2" false "${4:-}" ;;
    *) usage ;;
esac
