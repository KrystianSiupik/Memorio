#!/usr/bin/env bash
# Secrets of the stage 1 servers (MEM-E0-PLAT-18; CI job stage1): the plain env files are never in the
# repository, the leak runbook covers every variable of them, the encrypted copy round-trips through
# sops + age with every value encrypted, and each environment's copy in deploy/secrets/ is complete.
# Needs sops and age on PATH.
set -euo pipefail

root="$(cd "$(dirname "$0")/../.." && pwd)"
cd "$root"
secrets="deploy/secrets/secrets.sh"
fail() { echo "FAIL: $1" >&2; exit 1; }

for file in deploy/.env deploy/postgres.env deploy/app.env; do
    git check-ignore -q "$file" || fail "$file is not ignored by git"
done
# deploy/local/ holds the local check's public test values, nothing else may be a plain env file.
tracked="$(git ls-files | grep -E '(^|/)(\.env|postgres\.env|app\.env)$' | grep -v '^deploy/local/' || true)"
[ -z "$tracked" ] || fail "plain env files in the repository: $tracked"
echo "ok: the server's env files stay out of the repository"

missing=""
for example in deploy/env.example deploy/postgres.env.example deploy/app.env.example; do
    while IFS= read -r name; do
        grep -qF "\`$name\`" docs/runbooks/secret-leak.md || missing="$missing $name"
    done < <(sed -n 's/^\([A-Za-z_][A-Za-z0-9_]*\)=.*/\1/p' "$example")
done
[ -z "$missing" ] || fail "docs/runbooks/secret-leak.md does not cover:$missing"
echo "ok: the leak runbook covers every variable of the env files"

work="$(mktemp -d)"
trap 'rm -rf "$work"' EXIT
mkdir -p "$work/copy/check" "$work/plain" "$work/back"
age-keygen -o "$work/key.txt" 2>/dev/null
age-keygen -y "$work/key.txt" > "$work/copy/check/recipients.txt"
# The examples with a different random value in every variable.
for pair in env.example:.env postgres.env.example:postgres.env app.env.example:app.env; do
    awk -v seed="$RANDOM" 'BEGIN { srand(seed) }
        /^[A-Za-z_][A-Za-z0-9_]*=/ { split($0, part, "="); printf "%s=%04x%04x%04x%04x\n", part[1], int(rand() * 65536), int(rand() * 65536), int(rand() * 65536), int(rand() * 65536); next }
        { print }' "deploy/${pair%%:*}" > "$work/plain/${pair##*:}"
done

SECRETS_DIR="$work/copy" bash "$secrets" encrypt check "$work/plain" >/dev/null
while IFS= read -r value; do
    if grep -rqF "$value" "$work/copy/check"; then
        fail "a value is readable in the encrypted copy"
    fi
done < <(sed -n 's/^[A-Za-z_][A-Za-z0-9_]*=//p' "$work"/plain/.env "$work"/plain/*.env)
SOPS_AGE_KEY_FILE="$work/key.txt" SECRETS_DIR="$work/copy" bash "$secrets" decrypt check "$work/back" >/dev/null
diff -r "$work/plain" "$work/back" >/dev/null || fail "decrypting does not give the files back"
for file in "$work"/back/.env "$work"/back/*.env; do
    [ "$(stat -c %a "$file")" = 600 ] || fail "$(basename "$file") decrypted with mode $(stat -c %a "$file"), not 600"
done
if SOPS_AGE_KEY_FILE="$work/key.txt" SECRETS_DIR="$work/copy" bash "$secrets" decrypt check "$work/back" 2>/dev/null; then
    fail "decrypting over existing files must need FORCE=1"
fi
age-keygen -o "$work/other.txt" 2>/dev/null
if SOPS_AGE_KEY_FILE="$work/other.txt" FORCE=1 SECRETS_DIR="$work/copy" bash "$secrets" decrypt check "$work/back" 2>/dev/null; then
    fail "a key that is not a recipient decrypts the copy"
fi
echo "ok: encrypted copy round-trips (every value encrypted, mode 0600, only recipients decrypt)"

for dir in deploy/secrets/*/; do
    [ -f "$dir/recipients.txt" ] || continue
    bash "$secrets" check "$(basename "$dir")"
done
