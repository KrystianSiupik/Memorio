#!/usr/bin/env bash
# Prepares the media bucket (MEM-E0-PLAT-04, ADR 0005) on any S3-compatible service: creates it when
# missing (private: no public policy or ACL is ever set) and expires the upload zone (uploads/) after a
# day. Idempotent. Runs in the amazon/aws-cli image (docker compose run --rm storage-bootstrap) with
# S3_ENDPOINT, S3_BUCKET, AWS_ACCESS_KEY_ID, AWS_SECRET_ACCESS_KEY, AWS_DEFAULT_REGION.
set -euo pipefail

: "${S3_ENDPOINT:?}" "${S3_BUCKET:?}"
s3() { aws --endpoint-url "$S3_ENDPOINT" s3api "$@"; }
here="$(dirname "$0")"

# A fresh local server creates its default bucket a moment after it starts.
for attempt in $(seq 1 30); do
    if s3 head-bucket --bucket "$S3_BUCKET" 2>/dev/null; then
        break
    fi
    if [ "$attempt" -eq 30 ]; then
        s3 create-bucket --bucket "$S3_BUCKET"
    fi
    sleep 1
done

s3 put-bucket-lifecycle-configuration --bucket "$S3_BUCKET" --lifecycle-configuration "file://$here/lifecycle.json"
bash "$here/check-private.sh"
