#!/usr/bin/env bash
# Dead letters of the integration events (MEM-E0-PLAT-20, docs/runbooks/dead-letters.md), on the stage 1
# server as root in /opt/memorio/deploy: events whose module handler failed every retry (1 s, 10 s, 1 min;
# D-137), counted by the alert MessageDeadLettered. The SQL is in the *.sql files next to this script.
# Usage:
#   dead-letters.sh list                          by event, handler and exception; then the messages with their ids
#   dead-letters.sh show <id>                     the exception and the event of one
#   dead-letters.sh replay <id>|--all             back to the handler; waits to see it handled or failing again
#   dead-letters.sh discard <id> --reason "…"     deletes one on purpose, written to the operations log
set -euo pipefail
# shellcheck source-path=SCRIPTDIR/../backup
source "$(dirname "$0")/../backup/common.sh"

sql_dir="$(cd "$(dirname "$0")" && pwd)"
operations_log="${OPERATIONS_LOG:-/var/lib/memorio/operations.log}"
# A handler's attempts take 1 s + 10 s + 1 min: a replayed event that fails again is back by then.
retry_window="${REPLAY_RETRY_WINDOW:-90}"

usage() { sed -n '6,9p' "$0" >&2; exit 2; }

# A SQL file of this directory through psql as the database owner; the file switches to memorio_messaging.
run_sql() {
    local file="$1"
    shift
    compose exec -T postgres psql -v ON_ERROR_STOP=1 -X -q "$@" -f - < "$sql_dir/$file"
}

# A dead letter id (the envelope id, a uuid); anything else stops the script before it reaches psql.
valid_id() { [[ "$1" =~ ^[0-9a-f]{8}-[0-9a-f]{4}-[0-9a-f]{4}-[0-9a-f]{4}-[0-9a-f]{12}$ ]] || usage; }

replay() {
    local ids count quoted back
    ids="$(run_sql replay.sql -tA -v id="$1")"
    [ -n "$ids" ] || { echo "No dead letter to replay." >&2; exit 1; }
    count="$(wc -l <<<"$ids")"
    echo "== $count marked for replay"

    # The worker's durability agent moves marked rows to its inbox every few seconds.
    for _ in $(seq 1 60); do
        [ "$(psql_live 'SELECT count(*) FROM wolverine.wolverine_dead_letters WHERE replayable')" = 0 ] && break
        sleep 1
    done
    if [ "$(psql_live 'SELECT count(*) FROM wolverine.wolverine_dead_letters WHERE replayable')" != 0 ]; then
        echo "The worker has not taken them back within a minute: is it running (/internal/worker/ready)?" >&2
        exit 1
    fi

    echo "== Back with their handlers; $retry_window s to see whether a handler fails again"
    sleep "$retry_window"
    quoted="'${ids//$'\n'/"','"}'"   # 'id1','id2' (uuids only)
    back="$(psql_live "SELECT count(*) FROM wolverine.wolverine_dead_letters WHERE id::text IN ($quoted)")"
    if [ "$back" != 0 ]; then
        echo "$back of $count failed again and are back in the dead letters: $0 list" >&2
        exit 1
    fi
    echo "ok: $count handled, none back in the dead letters"
}

discard() {
    local id="$1" reason="$2" row
    [ -n "$reason" ] || usage
    row="$(run_sql discard.sql -tA -v id="$id")"
    [ -n "$row" ] || { echo "No dead letter $id." >&2; exit 1; }
    mkdir -p "$(dirname "$operations_log")"
    echo "$(date -u +%Y-%m-%dT%H:%M:%SZ) dead letter discarded by ${SUDO_USER:-$(id -un)}: $row; reason: $reason" >> "$operations_log"
    echo "ok: discarded $id (written to $operations_log)"
}

case "${1:-}" in
    list) run_sql list.sql ;;
    show)
        valid_id "${2:-}"
        run_sql show.sql -x -v id="$2" ;;
    replay)
        if [ "${2:-}" = --all ]; then
            replay all
        else
            valid_id "${2:-}"
            replay "$2"
        fi ;;
    discard)
        valid_id "${2:-}"
        [ "${3:-}" = --reason ] || usage
        discard "$2" "${4:-}" ;;
    *) usage ;;
esac
