# Stage 1 application server (ADR 0013, MEM-E0-PLAT-09)

One VPS in the EU runs, with Docker Compose (`compose.yaml`):

| Service | What | Reachable from |
|---|---|---|
| `nginx` | TLS (Let's Encrypt), HTTP → HTTPS, rate limit on `/v1/auth/`, request bodies ≤ 1 MB | internet: 80, 443 |
| `api`, `worker` | the two hosts, images `memorio-api` and `memorio-worker` of the release | nginx only |
| `postgres` | PostgreSQL + PostGIS with WAL-G (image `memorio-postgres`, `deploy/postgres/Dockerfile`) | the hosts only |
| `otel-collector`, `node-exporter` | metrics and logs for the monitoring server (D-160) | private network: 8889, 9100 |

The monitoring server (Prometheus, Loki, Grafana, Alertmanager, external probes) is a separate VPS
(D-038, `monitoring/`). Backups go to an S3-compatible bucket in the EU other than the media bucket.

## Files on the server

```
/opt/memorio/                 the repository at the released tag (read-only use)
/opt/memorio/deploy/.env      compose variables          (from env.example)
/opt/memorio/deploy/postgres.env  database passwords     (from postgres.env.example)
/opt/memorio/deploy/app.env   API and worker settings    (from app.env.example)
/var/lib/memorio/metrics/     backup, WAL and restore test metrics (node exporter textfile collector)
/var/lib/memorio/restore-reports/  one Markdown report per restore test
```

The three env files are secrets (MEM-E0-PLAT-18): mode 0600, never in the repository.

## First installation

1. Ubuntu 24.04 VPS in the EU with a private network to the monitoring server; DNS of the API name
   points at it.
2. Repository at the release tag in `/opt/memorio`; the three env files filled in.
3. `CERTBOT_EMAIL=… /opt/memorio/deploy/provision.sh` (as root). It installs Docker, closes every port
   but SSH, HTTP and HTTPS, turns on automatic security updates, gets the first certificate, starts the
   stack, installs the timers (`systemd/`) and takes the first base backup.
4. On the monitoring server: the target files (`collector.yml`, `node.yml` with the private address,
   `health.yml`, `ready.yml` with the public `/health` and `/ready`, `/internal/worker/ready`,
   `bucket.yml` with the media bucket's anonymous addresses).

## Releases and deployment (D-041, MEM-E0-PLAT-13)

1. Release: `git tag v0.2.0 && git push origin v0.2.0`. CI runs every check and only then publishes
   `memorio-api`, `memorio-worker` and `memorio-postgres` `:0.2.0` (with SBOM and provenance).
2. Deploy, one command: `DEPLOY_HOST=deploy@server tools/deploy/deploy.sh 0.2.0`, or the `deploy` workflow
   (version, environment; production can require an approval).

On the server `release.sh` checks the repository out at the tag, sets a named restore point
(`before-0.2.0`, its time in `/var/lib/memorio/deployments.log`), pulls the images and restarts: about half
a minute without the API (D-041), migrations run when the API starts (D-152). Then it checks both hosts
and the API through nginx; if a check fails it starts the previous version again (about 4 minutes in
all) and exits with an error. The database stays migrated: migrations only expand (SD18), so the
previous version runs on it. Data damaged by a release: `backup/restore-server.sh` with the restore
point's time.

The server pulls private images: once, `docker login ghcr.io` with a read-only token (MEM-E0-PLAT-18).

## Backups and restore (QS2: RPO ≤ 15 min, RTO ≤ 8 h; SD13)

| What | How | Timer |
|---|---|---|
| WAL archiving | `archive_command = wal-g wal-push`, a segment at least every 5 min | continuous |
| Base backup | `backup/backup.sh`; 14 kept (`BACKUP_RETAIN_FULL`), so any moment of the last 14 days | daily 02:30 UTC |
| Archive status | `backup/archive-status.sh` → alerts `WalArchiveStale`, `WalArchiveFailing` | every 5 min |
| Restore test | `backup/restore-test.sh`: point-in-time restore into a throwaway container, report + metrics | 1st of the month |
| Certificates | `certbot renew` + nginx reload | twice a day |

Backups are encrypted on the server (`BACKUP_ENCRYPTION_KEY`, libsodium) before they leave it: keep the
key with the other secrets, a backup without it cannot be read.

**Restore test (SD13).** Writes a marker, takes the moment right after it as the target, writes a
second marker, waits until the WAL with both is archived, restores latest base backup + WAL up to the
target and checks: first marker there, second not, every Memorio schema present. The report gives the
measured RPO (marker → archived) and RTO (fetch + replay + checks). A failed test exits non-zero
(systemd shows the unit failed) and `RestoreTestOverdue` fires after 35 days without a passed one.

**Disaster recovery** (database volume lost, or a new server):

```bash
docker compose down                                  # if anything still runs
docker volume rm memorio_postgres-data               # only when the volume is lost or damaged
./backup/restore-server.sh                           # everything archived
./backup/restore-server.sh '2026-11-02 09:41:00+00'  # or up to a moment (e.g. before a bad change)
```

It refuses to write into a volume that holds a database. After a point-in-time restore the database
continues on a new timeline; take a base backup right after (`./backup/backup.sh`).

## Local check

`tools/deploy/check-stage1.sh` runs this file with `compose.local.yaml` (images built from the checkout,
a local S3 server, a self-signed certificate; values in `local/`) and checks nginx, a backup, the restore
test and disaster recovery end to end; CI job `stage1` runs it on every push.
