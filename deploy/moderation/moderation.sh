#!/usr/bin/env bash
# The service's side of reports (MEM-E8-MODERACJA-02, D-309), on the stage 1 server as root in /opt/memorio/deploy.
# A decision is a row the worker carries out within a minute; there is no public way in for the service.
# Every look at content and every decision go to the operations log (S2:C8). Usage:
#   moderation.sh list                                        reports and appeals waiting, the most urgent first (R9)
#   moderation.sh show <report id> [directory]                the report and the content; photos into the directory
#   moderation.sh decide <report id> removed|kept --reason "…"   the decision, with reasons for both sides
#   moderation.sh appeal <report id> reversed|upheld --reason "…"  the decision on the author's appeal
#   moderation.sh result <report id>                          what became of the decisions
set -euo pipefail
# shellcheck source-path=SCRIPTDIR/../backup
source "$(dirname "$0")/../backup/common.sh"

operations_log="${OPERATIONS_LOG:-/var/lib/memorio/operations.log}"
operator="${SUDO_USER:-$(id -un)}"

usage() { sed -n '5,9p' "$0" >&2; exit 2; }

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
        case "${3:-}" in removed) act "$2" Decide Removed "${5:-}" ;; kept) act "$2" Decide Kept "${5:-}" ;; *) usage ;; esac
        ;;
    appeal)
        valid_id "${2:-}"; [ "${4:-}" = --reason ] || usage
        case "${3:-}" in reversed) act "$2" DecideAppeal Reversed "${5:-}" ;; upheld) act "$2" DecideAppeal Upheld "${5:-}" ;; *) usage ;; esac
        ;;
    *) usage ;;
esac
