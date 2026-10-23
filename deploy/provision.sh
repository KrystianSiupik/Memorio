#!/usr/bin/env bash
# Prepares a stage 1 application server (MEM-E0-PLAT-09; deploy/README.md): Ubuntu 24.04, run as root
# from /opt/memorio/deploy once the repository is there at the released version and .env, postgres.env
# and app.env are in place (MEM-E0-PLAT-18). Idempotent: running it again changes nothing that is
# already right. Installs Docker, closes everything but SSH, HTTP and HTTPS, turns on automatic security
# updates, gets the first certificate, starts the stack and the operation timers.
# Usage: CERTBOT_EMAIL=ops@example.org ./provision.sh
set -euo pipefail

deploy_dir="$(cd "$(dirname "$0")" && pwd)"
cd "$deploy_dir"
: "${CERTBOT_EMAIL:?e-mail address for the certificate notices}"
for file in .env postgres.env app.env; do
    [ -f "$file" ] || { echo "Missing $deploy_dir/$file (see $file.example and MEM-E0-PLAT-18)." >&2; exit 1; }
done
chmod 0600 .env postgres.env app.env
# shellcheck disable=SC1091
domain="$(set -a && source ./.env && echo "$MEMORIO_DOMAIN")"

echo "== Packages and automatic security updates"
apt-get update -q
apt-get install -yq ca-certificates curl ufw unattended-upgrades
dpkg-reconfigure -f noninteractive unattended-upgrades

if ! command -v docker >/dev/null; then
    echo "== Docker Engine from Docker's repository"
    install -m 0755 -d /etc/apt/keyrings
    curl -fsSL https://download.docker.com/linux/ubuntu/gpg -o /etc/apt/keyrings/docker.asc
    chmod a+r /etc/apt/keyrings/docker.asc
    # shellcheck disable=SC1091
    echo "deb [arch=$(dpkg --print-architecture) signed-by=/etc/apt/keyrings/docker.asc] https://download.docker.com/linux/ubuntu $(. /etc/os-release && echo "$VERSION_CODENAME") stable" \
        > /etc/apt/sources.list.d/docker.list
    apt-get update -q
    apt-get install -yq docker-ce docker-ce-cli containerd.io docker-compose-plugin
fi

echo "== Firewall: SSH, HTTP, HTTPS only"
# Ports for the monitoring server (8889, 9100) are published on the private network address only
# (PRIVATE_IP in .env); Docker's published ports bypass ufw, so the binding is what keeps them private.
ufw default deny incoming
ufw default allow outgoing
ufw allow OpenSSH
ufw allow 80/tcp
ufw allow 443/tcp
ufw --force enable

echo "== Directories of the operation scripts"
install -d -m 0755 /var/lib/memorio/metrics
install -d -m 0750 /var/lib/memorio/restore-reports

echo "== Images"
docker compose pull

if ! docker compose run --rm --entrypoint test certbot -f "/etc/letsencrypt/live/$domain/fullchain.pem"; then
    echo "== First certificate for $domain (port 80 must reach this server)"
    docker compose run --rm -p 80:80 certbot certonly --standalone --non-interactive --agree-tos \
        --email "$CERTBOT_EMAIL" -d "$domain"
fi

echo "== Stack"
docker compose up -d --wait

echo "== Timers: backup, WAL archive status, restore test, certificate renewal"
install -m 0644 systemd/*.service systemd/*.timer /etc/systemd/system/
systemctl daemon-reload
systemctl enable --now memorio-backup.timer memorio-archive-status.timer memorio-restore-test.timer memorio-certificates.timer

echo "== First base backup, so the WAL archive has something to start from"
./backup/backup.sh
./backup/archive-status.sh
echo "Stage 1 server ready: https://$domain"
