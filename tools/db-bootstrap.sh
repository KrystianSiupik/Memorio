#!/usr/bin/env bash
# Applies db/bootstrap.sql to the local Compose database again (e.g. after a module was added).
# Idempotent. Usage: tools/db-bootstrap.sh
set -euo pipefail
cd "$(dirname "$0")/.."

docker compose exec -T postgres psql -v ON_ERROR_STOP=1 -U memorio_admin -d memorio < db/bootstrap.sql
