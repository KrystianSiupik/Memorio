#!/usr/bin/env bash
# Fitness function SD5 (ADR 0005): the media bucket allows no anonymous access. Run after every
# bootstrap and daily against each environment (scheduling: MEM-E0-PLAT-10). Needs S3_ENDPOINT and
# S3_BUCKET; an optional argument names an existing object to try as well.
set -euo pipefail

: "${S3_ENDPOINT:?}" "${S3_BUCKET:?}"
failed=0
for path in "$S3_BUCKET/" "$S3_BUCKET?list-type=2" ${1:+"$S3_BUCKET/$1"}; do
    status=$(curl -s -o /dev/null -w '%{http_code}' "${S3_ENDPOINT%/}/$path")
    if [ "$status" != "401" ] && [ "$status" != "403" ]; then
        echo "Anonymous GET /$path answered $status, expected 401 or 403: the bucket is not private."
        failed=1
    fi
done
[ "$failed" -eq 0 ] && echo "Bucket $S3_BUCKET refuses anonymous access."
exit "$failed"
