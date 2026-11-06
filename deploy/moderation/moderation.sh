#!/usr/bin/env bash
# The service's side of reports (MEM-E8-MODERACJA-02, D-309), on the stage 1 server as root in /opt/memorio/deploy.
# A decision is a row the worker carries out within a minute; there is no public way in for the service.
# Every look at content and every decision go to the operations log (S2:C8). Usage:
#   moderation.sh list                                        reports and appeals waiting, the most urgent first (R9)
#   moderation.sh show <report id> [directory]                the report and the content; photos into the directory
#   moderation.sh decide <report id> removed|kept|unfounded --reason "…"   the decision, with reasons for both sides
#                                                             (unfounded: kept, and a warning; 3 in 90 days suspend reporting)
#   moderation.sh appeal <report id> reversed|upheld --reason "…"  the decision on the author's appeal
#   moderation.sh result <report id>                          what became of the decisions
# Orders of authorities (Z6, D-312), entered only once their basis is checked:
#   moderation.sh order remove memory|album <id> --authority "…" --reference "…" [--confidential]
#   moderation.sh order preserve|produce <account id> --authority "…" --reference "…" [--confidential]
#   moderation.sh orders                                      orders and their state
#   moderation.sh disclose <order id> <directory> --what "…"  hands the account's data over, logged
#   moderation.sh close <order id>                            the authority no longer needs the data: the hold ends
#   moderation.sh log [<report or order id>]                  the log of looks and disclosures
set -euo pipefail
# shellcheck source-path=SCRIPTDIR/../backup
source "$(dirname "$0")/../backup/common.sh"

operations_log="${OPERATIONS_LOG:-/var/lib/memorio/operations.log}"
operator="${SUDO_USER:-$(id -un)}"

usage() { sed -n '5,18p' "$0" >&2; exit 2; }

valid_id() { [[ "$1" =~ ^[0-9a-f]{8}-[0-9a-f]{4}-[0-9a-f]{4}-[0-9a-f]{4}-[0-9a-f]{12}$ ]] || usage; }

# Each module's data only through its own role (ADR 0004).
sql() { local role="$1"; shift; compose exec -T postgres psql -v ON_ERROR_STOP=1 -X -q -c "SET ROLE memorio_$role" "$@"; }

note() {
    mkdir -p "$(dirname "$operations_log")"
    echo "$(date -u +%Y-%m-%dT%H:%M:%SZ) moderation by $operator: $1" >> "$operations_log"
}

act() {
    local id="$1" kind="$2" outcome="$3" reason="$4"
    [ -n "$reason" ] || usage
    # The reasons go to both sides as written: psql quotes them (a -c command would not take the variable).
    sql moderation -v statement="$reason" -f - <<SQL
INSERT INTO moderation.operator_actions (id, kind, report_id, outcome, statement, decided_by, decided_at)
VALUES (gen_random_uuid(), '$kind', '$id', '$outcome', :'statement', '$operator', now());
SQL
    note "report $id $kind $outcome, reasons: $reason"
    echo "ok: the worker applies it within a minute; check with: moderation.sh result $id"
}

# The log in the database (S2:C8), next to the operations log: who, when, what, on which basis.
logged() {
    local kind="$1" subject="$2" concerns="$3" what="$4" basis="$5"
    sql moderation -v what="$what" -v basis="$basis" -f - <<SQL
INSERT INTO moderation.access_log (id, at, by, kind, subject_id, concerns_id, what, basis)
VALUES (gen_random_uuid(), now(), '$operator', '$kind', '$subject', NULLIF('$concerns', '')::uuid, :'what', :'basis');
SQL
}

order() {
    local kind="$1" resource_kind="" resource="" account="" authority="" reference="" confidential=false
    case "$kind" in
        remove) kind=RemoveContent; case "${2:-}" in memory) resource_kind=Memory ;; album) resource_kind=Album ;; *) usage ;; esac; valid_id "${3:-}"; resource="$3"; shift 3 ;;
        preserve|produce) [ "$kind" = preserve ] && kind=PreserveData || kind=ProduceData; valid_id "${2:-}"; account="$2"; shift 2 ;;
        *) usage ;;
    esac
    while [ $# -gt 0 ]; do
        case "$1" in
            --authority) authority="${2:-}"; shift 2 ;;
            --reference) reference="${2:-}"; shift 2 ;;
            --confidential) confidential=true; shift ;;
            *) usage ;;
        esac
    done
    [ -n "$authority" ] && [ -n "$reference" ] || usage
    local id
    id="$(sql moderation -tA -c "SELECT gen_random_uuid()" | tail -1)"
    sql moderation -v authority="$authority" -v reference="$reference" -f - <<SQL
INSERT INTO moderation.incoming_orders (id, kind, authority, reference, resource_kind, resource_id, account_id, confidential, received_by, received_at)
VALUES ('$id', '$kind', :'authority', :'reference', NULLIF('$resource_kind', ''), NULLIF('$resource', '')::uuid, NULLIF('$account', '')::uuid, $confidential, '$operator', now());
SQL
    note "order $id $kind entered: $authority, $reference"
    echo "ok: order $id; the worker acts on it within a minute; check with: moderation.sh orders"
}

step() {
    local id="$1" kind="$2" details="$3"
    sql moderation -v details="$details" -f - <<SQL
INSERT INTO moderation.order_actions (id, order_id, kind, details, decided_by, decided_at)
VALUES (gen_random_uuid(), '$id', '$kind', :'details', '$operator', now());
SQL
}

# The disclosure is logged by the worker first; only then the data leaves, into the directory.
disclose() {
    local id="$1" directory="$2" what="$3" account result=""
    [ -n "$what" ] || usage
    account="$(sql moderation -tA -c "SELECT account_id FROM moderation.orders WHERE id = '$id' AND kind = 'ProduceData'" | tail -1)"
    [ -n "$account" ] || { echo "no order to produce data with this id" >&2; exit 1; }
    step "$id" Disclose "$what"
    for _ in $(seq 1 90); do
        result="$(sql moderation -tA -c "SELECT result FROM moderation.order_actions WHERE order_id = '$id' AND kind = 'Disclose' ORDER BY decided_at DESC LIMIT 1" | tail -1)"
        [ -n "$result" ] && break
        sleep 2
    done
    [ "$result" = Applied ] || { echo "not disclosed: ${result:-the worker did not answer}" >&2; exit 1; }
    mkdir -p "$directory"
    sql archive -tA -c "SELECT coalesce(json_agg(m), '[]') FROM (SELECT id, place_name, latitude, longitude, date_precision, date_from, date_to, description, created_at, state FROM archive.memories WHERE author_account_id = '$account') m" > "$directory/memories.json"
    sql archive -tA -c "SELECT p.id || ' ' || p.storage_key FROM archive.photos p JOIN archive.memories m ON m.id = p.memory_id WHERE m.author_account_id = '$account'" | while read -r photo_id key; do
        [ -n "$key" ] && photo "$key" "$directory/$photo_id"
    done
    note "order $id: data of account $account disclosed into $directory: $what"
    echo "ok: in $directory; hand it over and delete it"
}

storage() { compose exec -T api printenv "Storage__$1" | tr -d '\r'; }

photo() {
    local key="$1" file="$2"
    # The application's own bucket credentials, in a throwaway container on the application's network.
    docker run --rm --network "container:$(compose ps -q api)" \
        -e AWS_ACCESS_KEY_ID="$(storage AccessKey)" -e AWS_SECRET_ACCESS_KEY="$(storage SecretKey)" -e AWS_DEFAULT_REGION="$(storage Region)" \
        amazon/aws-cli:2.22.35 --endpoint-url "$(storage Endpoint)" s3 cp "s3://$(storage Bucket)/$key" - > "$file"
}

show() {
    local id="$1" directory="${2:-}" target target_id
    sql moderation -x -c "SELECT id, state, category, reason, explanation, target, target_id, location, reported_at, due_by, verdict, statement, appeal_text FROM moderation.reports WHERE id = '$id'"
    target="$(sql moderation -tA -c "SELECT target FROM moderation.reports WHERE id = '$id'" | tail -1)"
    target_id="$(sql moderation -tA -c "SELECT target_id FROM moderation.reports WHERE id = '$id'" | tail -1)"
    note "report $id: looked at $target $target_id"
    logged View "$id" "$target_id" "$target" "report $id"
    case "$target" in
        Memory)
            sql archive -x -c "SELECT place_name, date_from, date_to, description, state FROM archive.memories WHERE id = '$target_id'"
            if [ -n "$directory" ]; then
                mkdir -p "$directory"
                sql archive -tA -c "SELECT id || ' ' || storage_key FROM archive.photos WHERE memory_id = '$target_id' ORDER BY id" | while read -r photo_id key; do
                    [ -n "$key" ] && photo "$key" "$directory/$photo_id"
                done
                note "report $id: photos of memory $target_id downloaded to $directory"
                echo "ok: photos in $directory; delete them after deciding"
            fi
            ;;
        Album)
            sql album -x -c "SELECT name, created_at FROM album.albums WHERE id = '$target_id'"
            sql album -c "SELECT memory_id FROM album.album_memories WHERE album_id = '$target_id'"
            ;;
        *) echo "reported without an account: find it from the location above" ;;
    esac
}

case "${1:-}" in
    list) sql moderation -c "SELECT id, state, category, reason, target, target_id, left(location, 60) AS location, due_by FROM moderation.reports WHERE state IN ('Open', 'Appealed') ORDER BY due_by" ;;
    result) valid_id "${2:-}"; sql moderation -c "SELECT r.state, r.hidden, a.kind, a.outcome, a.decided_by, a.decided_at, a.applied_at, a.result FROM moderation.reports r LEFT JOIN moderation.operator_actions a ON a.report_id = r.id WHERE r.id = '$2' ORDER BY a.decided_at" ;;
    show) valid_id "${2:-}"; show "$2" "${3:-}" ;;
    decide)
        valid_id "${2:-}"; [ "${4:-}" = --reason ] || usage
        case "${3:-}" in removed) act "$2" Decide Removed "${5:-}" ;; kept) act "$2" Decide Kept "${5:-}" ;; unfounded) act "$2" Decide Unfounded "${5:-}" ;; *) usage ;; esac
        ;;
    appeal)
        valid_id "${2:-}"; [ "${4:-}" = --reason ] || usage
        case "${3:-}" in reversed) act "$2" DecideAppeal Reversed "${5:-}" ;; upheld) act "$2" DecideAppeal Upheld "${5:-}" ;; *) usage ;; esac
        ;;
    order) shift; order "$@" ;;
    orders) sql moderation -c "SELECT id, kind, authority, reference, resource_kind, resource_id, account_id, confidential, received_at, hidden_at, secured_at, disclosed_at, closed_at FROM moderation.orders ORDER BY received_at DESC" -c "SELECT id, kind, received_at, applied_at, result FROM moderation.incoming_orders WHERE result IS DISTINCT FROM 'Applied' ORDER BY received_at DESC" ;;
    disclose) valid_id "${2:-}"; [ -n "${3:-}" ] && [ "${4:-}" = --what ] || usage; disclose "$2" "$3" "${5:-}" ;;
    close) valid_id "${2:-}"; step "$2" Close "closed by $operator"; note "order $2 closed"; echo "ok: the hold ends within a minute" ;;
    log)
        if [ -n "${2:-}" ]; then valid_id "$2"; sql moderation -c "SELECT at, by, kind, subject_id, concerns_id, what, basis FROM moderation.access_log WHERE subject_id = '$2' ORDER BY at"
        else sql moderation -c "SELECT at, by, kind, subject_id, concerns_id, what, basis FROM moderation.access_log ORDER BY at DESC LIMIT 100"; fi
        ;;
    *) usage ;;
esac
