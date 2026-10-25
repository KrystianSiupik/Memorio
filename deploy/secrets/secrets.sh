#!/usr/bin/env bash
# Encrypted copy of an environment's secrets in the repository (RD10, D-061, MEM-E0-PLAT-18): the three
# env files of a stage 1 server, encrypted with sops + age for the environment's recipients
# (deploy/secrets/<environment>/recipients.txt: the age public keys of the people who may restore it).
# Encrypting needs only those public keys; decrypting needs one private key (SOPS_AGE_KEY_FILE).
# Usage:
#   secrets.sh encrypt <environment> [dir]   dir/.env, postgres.env, app.env → deploy/secrets/<environment>/
#   secrets.sh decrypt <environment> [dir]   the reverse, mode 0600; dir defaults to deploy/ (the server's place)
#   secrets.sh check <environment>           every variable of the *.example files is in the copy, every value encrypted
# SECRETS_DIR writes and reads the encrypted copy elsewhere (on the server: never into the checkout).
set -euo pipefail

deploy_dir="$(cd "$(dirname "$0")/.." && pwd)"
secrets_dir="${SECRETS_DIR:-$deploy_dir/secrets}"
# Encrypted file : plain file on the server : the example that lists its variables.
files=(compose.sops.env:.env:env.example postgres.sops.env:postgres.env:postgres.env.example app.sops.env:app.env:app.env.example)

usage() { sed -n '6,10p' "$0" >&2; exit 2; }

command="${1:-}"
environment="${2:-}"
[[ "$environment" =~ ^[a-z][a-z0-9-]*$ ]] || usage
copy_dir="$secrets_dir/$environment"
plain_dir="${3:-$deploy_dir}"

variables() { sed -n 's/^\([A-Za-z_][A-Za-z0-9_]*\)=.*/\1/p' "$1" | grep -v '^sops_' | sort; }

check() {
    local entry encrypted example missing extra plain failed=0
    for entry in "${files[@]}"; do
        IFS=: read -r encrypted _ example <<<"$entry"
        if [ ! -f "$copy_dir/$encrypted" ]; then
            echo "Missing $copy_dir/$encrypted." >&2
            failed=1
            continue
        fi
        missing="$(comm -23 <(variables "$deploy_dir/$example") <(variables "$copy_dir/$encrypted"))"
        extra="$(comm -13 <(variables "$deploy_dir/$example") <(variables "$copy_dir/$encrypted"))"
        plain="$(grep -E '^[A-Za-z_][A-Za-z0-9_]*=.' "$copy_dir/$encrypted" | grep -vE '^(sops_[a-z_0-9]*|[A-Za-z_][A-Za-z0-9_]*=ENC\[)' | cut -d= -f1 || true)"
        [ -z "$missing" ] || { echo "$encrypted lacks (in $example): $(echo "$missing" | tr '\n' ' ')" >&2; failed=1; }
        [ -z "$extra" ] || { echo "$encrypted has variables $example does not list: $(echo "$extra" | tr '\n' ' ')" >&2; failed=1; }
        [ -z "$plain" ] || { echo "$encrypted has values in plain text: $(echo "$plain" | tr '\n' ' ')" >&2; failed=1; }
    done
    [ "$failed" = 0 ] && echo "ok: the encrypted copy of $environment is complete"
}

encrypt() {
    local recipients entry encrypted plain
    recipients="$(grep -vE '^[[:space:]]*(#|$)' "$copy_dir/recipients.txt" 2>/dev/null | paste -sd, - || true)"
    [ -n "$recipients" ] || { echo "No age public keys in $copy_dir/recipients.txt." >&2; exit 1; }
    for entry in "${files[@]}"; do
        IFS=: read -r encrypted plain _ <<<"$entry"
        [ -f "$plain_dir/$plain" ] || { echo "Missing $plain_dir/$plain." >&2; exit 1; }
    done
    for entry in "${files[@]}"; do
        IFS=: read -r encrypted plain _ <<<"$entry"
        sops encrypt --age "$recipients" --input-type dotenv --output-type dotenv "$plain_dir/$plain" > "$copy_dir/$encrypted.new"
        mv "$copy_dir/$encrypted.new" "$copy_dir/$encrypted"
    done
    check
}

decrypt() {
    local entry encrypted plain
    for entry in "${files[@]}"; do
        IFS=: read -r encrypted plain _ <<<"$entry"
        if [ -e "$plain_dir/$plain" ] && [ "${FORCE:-0}" != 1 ]; then
            echo "$plain_dir/$plain exists: nothing written (FORCE=1 replaces it)." >&2
            exit 1
        fi
    done
    for entry in "${files[@]}"; do
        IFS=: read -r encrypted plain _ <<<"$entry"
        (umask 077 && sops decrypt --input-type dotenv --output-type dotenv "$copy_dir/$encrypted" > "$plain_dir/$plain.new")
        mv "$plain_dir/$plain.new" "$plain_dir/$plain"
    done
    echo "Decrypted into $plain_dir: .env, postgres.env, app.env (mode 0600)."
}

case "$command" in
    encrypt) encrypt ;;
    decrypt) decrypt ;;
    check) check ;;
    *) usage ;;
esac
