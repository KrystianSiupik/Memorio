#!/usr/bin/env bash
# The one command (D-041, MEM-E0-PLAT-13): deploys a released version to an environment's server over
# SSH; the server does the rest (deploy/release.sh: restore point, images, restart, checks, rollback).
# The release must exist: tag v<version> pushed and its images published by CI (job release).
# Usage: DEPLOY_HOST=deploy@app.example.org tools/deploy/deploy.sh 0.2.0
set -euo pipefail

version="${1:?usage: deploy.sh <version>}"
host="${DEPLOY_HOST:?DEPLOY_HOST: user@server of the environment}"

# The version reaches a shell on the server: only major.minor.patch passes.
if [[ ! "$version" =~ ^[0-9]+\.[0-9]+\.[0-9]+$ ]]; then
    echo "A version is major.minor.patch, not \"$version\"." >&2
    exit 1
fi

if ! git ls-remote --exit-code --tags origin "refs/tags/v$version" >/dev/null; then
    echo "No tag v$version on origin: release it first (git tag v$version && git push origin v$version)." >&2
    exit 1
fi

# shellcheck disable=SC2029 # expanded here on purpose: the checked version
ssh "$host" sudo /opt/memorio/deploy/release.sh "$version"
