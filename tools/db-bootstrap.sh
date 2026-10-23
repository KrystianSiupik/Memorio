#!/usr/bin/env bash
# Applies db/bootstrap.sql to the local Compose database again (e.g. after a module was added), then
# the local passwords of the roles. Idempotent. Usage: tools/db-bootstrap.sh
set -euo pipefail
cd "$(dirname "$0")/.."

for script in db/bootstrap.sql db/local-passwords.sql; do
    docker compose exec -T postgres psql -q -v ON_ERROR_STOP=1 -U memorio_admin -d memorio < "$script"
done
