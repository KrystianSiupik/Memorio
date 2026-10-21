#!/usr/bin/env bash
# Fails when api/openapi.v1.json breaks backward compatibility against the contract at <base-ref>
# (ADR 0017: /v1 changes must stay backward compatible). Uses oasdiff in Docker.
# Usage: tools/check-api-breaking.sh <base-ref>     e.g. origin/version/0.1
set -euo pipefail
base_ref="${1:?usage: $0 <base-ref>}"
cd "$(dirname "$0")/.."
tmp="$(mktemp -d)"
trap 'rm -rf "$tmp"' EXIT
if ! git show "$base_ref:api/openapi.v1.json" > "$tmp/base.json" 2>/dev/null; then
  echo "No api/openapi.v1.json at $base_ref: nothing to compare against."
  exit 0
fi
cp api/openapi.v1.json "$tmp/revision.json"
# Git Bash on Windows: Docker needs a Windows path and no MSYS path conversion.
mount="$tmp"
if command -v cygpath > /dev/null; then
  mount="$(cygpath -w "$tmp")"
  export MSYS_NO_PATHCONV=1
fi
docker run --rm -v "$mount:/specs" tufin/oasdiff:v1.34.1 breaking /specs/base.json /specs/revision.json --fail-on ERR
