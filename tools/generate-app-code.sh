#!/usr/bin/env bash
# Regenerates the app's generated code: the API client in app/lib/core/api/generated (from
# api/openapi.v1.json) and every build_runner output (*.g.dart: JSON, retrofit, Drift).
# Idempotent; run after a contract or schema change and commit the result. CI fails on a diff.
set -euo pipefail
cd "$(dirname "$0")/../app"
dart run swagger_parser
dart run build_runner build --delete-conflicting-outputs
dart format lib
