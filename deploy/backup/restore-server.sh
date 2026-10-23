#!/usr/bin/env bash
# Disaster recovery (QS2: RTO ≤ 8 h, MEM-E0-PLAT-09; deploy/README.md): restores this server's database
# from the backup bucket into an EMPTY database volume — on a new server or after the volume was lost —
# and starts the stack on it. Optional argument: the moment to restore to (timestamptz); without it,
# everything archived. Refuses to touch a volume that holds a database.
set -euo pipefail
# shellcheck source-path=SCRIPTDIR
source "$(dirname "$0")/common.sh"

target="${1:-}"
started=$(date +%s)

compose stop api worker postgres
# MSYS_NO_PATHCONV: Git Bash on Windows would rewrite the container path /operations; no effect elsewhere.
MSYS_NO_PATHCONV=1 compose run --rm --no-deps -T -v "$deploy_dir/backup:/operations:ro" --entrypoint bash \
    postgres /operations/restore-server-inside.sh "$target"
compose up -d --wait postgres
compose up -d --wait

echo "Database restored${target:+ to $target} and the stack started in $(($(date +%s) - started)) s."
