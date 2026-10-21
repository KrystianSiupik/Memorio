#!/usr/bin/env bash
# Regenerates the Dart API client in app/lib/core/api/generated from api/openapi.v1.json.
# Idempotent; run after every contract change and commit the result. CI runs it and fails on a diff.
set -euo pipefail
cd "$(dirname "$0")/../app"
dart run swagger_parser
dart run build_runner build --delete-conflicting-outputs
dart format lib/core/api/generated
