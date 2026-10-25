# Runbook: a secret leaked (MEM-E0-PLAT-18)

When: a secret was seen by someone who should not have it (pasted into a chat or a ticket, committed,
in a screenshot, on a lost laptop, in a provider's breach notice), or you are not sure it was not.
Rotate first, investigate after: rotation is cheap, a working leaked key is not.

## Steps for every leak

1. **Which secret.** Find it in the tables below. Not sure which one: rotate every secret the leaked
   file or screen held.
2. **Rotate it** with the row's procedure, on every environment that uses the same value.
3. **Confirm the old value is dead.** Each procedure ends with a check that the old value is refused;
   the rotation is not done before that check passed.
4. **Refresh the encrypted copy** of the environment (below) and commit it, so a restore never brings
   the leaked value back.
5. **Look for use.** Access logs of the provider (storage, backups), `pg_stat_activity` and the
   PostgreSQL log (database), Loki (sign-ins, unusual 401/403). Personal data reached by someone else
   is a breach: the 72-hour notification procedure of epic E11 starts.
6. **Write down** what leaked, how, when it was rotated, and what the logs showed.

All commands run on the application server as root in `/opt/memorio/deploy`.

## Application server: compose variables (`.env`, from `deploy/env.example`)

| Variable | Secret | If it leaked | Rotation | Effect on users |
|---|---|---|---|---|
| `BACKUP_S3_ACCESS_KEY`, `BACKUP_S3_SECRET_KEY` | yes | read (encrypted) backups, delete them | new key at the provider next to the old one; both values in `.env`; `docker compose up -d postgres` (a few seconds without the database, the hosts reconnect); `./backup/archive-status.sh` and `./backup/backup.sh` succeed; delete the old key; a request with it gets 403 | none |
| `BACKUP_ENCRYPTION_KEY` | yes | with the backup key too: read every backup | new key (`openssl rand -hex 32`) in `.env`; `docker compose up -d postgres`; `./backup/backup.sh` right away (a new base backup under the new key); keep the old key, encrypted, until the last backup made with it expires (`BACKUP_RETAIN_FULL`, 14 days), then delete it. If the backup key leaked too, also delete the old backups once the new base backup passed `./backup/restore-test.sh` | none |
| `MEMORIO_REGISTRY`, `MEMORIO_VERSION`, `MEMORIO_ENVIRONMENT`, `MEMORIO_DOMAIN`, `MONITORING_IP`, `PRIVATE_IP`, `LOKI_OTLP_ENDPOINT`, `BACKUP_S3_PREFIX`, `BACKUP_S3_ENDPOINT`, `BACKUP_S3_REGION`, `POSTGRES_MAX_CONNECTIONS`, `POSTGRES_SHARED_BUFFERS`, `POSTGRES_EFFECTIVE_CACHE_SIZE`, `POSTGRES_WORK_MEM`, `POSTGRES_MAINTENANCE_WORK_MEM` | no | nothing to rotate | — | — |

## Application server: database passwords (`postgres.env`, from `deploy/postgres.env.example`)

| Variable | Role | Rotation |
|---|---|---|
| `POSTGRES_PASSWORD` | `memorio_admin`, the owner | `./secrets/rotate.sh database memorio_admin` |
| `PG_PASSWORD_MEMORIO_MESSAGING` | outbox and inbox of both hosts | `./secrets/rotate.sh database memorio_messaging` |
| `PG_PASSWORD_MEMORIO_SCHEDULER` | Quartz in the worker | `./secrets/rotate.sh database memorio_scheduler` |
| `PG_PASSWORD_MEMORIO_ACCESS`, `PG_PASSWORD_MEMORIO_ACCOUNT`, `PG_PASSWORD_MEMORIO_AI_NARRATION`, `PG_PASSWORD_MEMORIO_ALBUM`, `PG_PASSWORD_MEMORIO_ARCHIVE`, `PG_PASSWORD_MEMORIO_LIMITS`, `PG_PASSWORD_MEMORIO_MODERATION`, `PG_PASSWORD_MEMORIO_NOTIFICATIONS`, `PG_PASSWORD_MEMORIO_PLACE_PROFILES`, `PG_PASSWORD_MEMORIO_PLACES`, `PG_PASSWORD_MEMORIO_PLANS`, `PG_PASSWORD_MEMORIO_PROVENANCE`, `PG_PASSWORD_MEMORIO_PUBLISHING`, `PG_PASSWORD_MEMORIO_SUCCESSION` | the module's role (its schema only, ADR 0004) | `./secrets/rotate.sh database memorio_<module>` |

`rotate.sh database` sets a new random password in `postgres.env` and in the role's connection string in
`app.env`, changes it in the database, recreates the API and the worker (a few seconds each, as in a
deployment; open connections stay signed in until then), waits until both are ready, and checks that the
role signs in with the new password and is refused with the old one. When the hosts are not ready, the old
password is put back.

If they leaked: anyone who reaches the database port could act as that role. The port is not published
(only the hosts on the server's Docker network reach it), so a leak matters together with access to the
server; rotate anyway. Effect on users: none.

## Application server: API and worker settings (`app.env`, from `deploy/app.env.example`)

| Variable | Secret | If it leaked | Rotation | Effect on users |
|---|---|---|---|---|
| `Storage__AccessKey`, `Storage__SecretKey` | yes | read, change and delete every photo; **sign links to any file** | below | none: links handed out before keep working until the old key is deleted; the app asks for new ones (5-minute links) |
| `Messaging__ConnectionString`, `Account__ConnectionString`, `Scheduling__ConnectionString` | the password in it | as the role's password | `./secrets/rotate.sh database <role>` (the role is the `Username` in it) | none |
| `AppVersion__Minimum`, `Storage__Endpoint`, `Storage__Region`, `Storage__Bucket`, `Account__Google__ClientIds__0`, `Account__Apple__ClientIds__0` | no (the client ids are public, they are in the apps) | nothing to rotate | — | — |

**Media bucket key.** The secret signs every link to a photo (PLAT-04); a single link cannot be revoked,
so this is also the emergency switch that kills every link at once.

1. At the provider: a new key with access to the media bucket only. Keep the old one for now.
2. `NEW_STORAGE_ACCESS_KEY=… NEW_STORAGE_SECRET_KEY=… ./secrets/rotate.sh storage`: checks the new key on
   the bucket, switches the API and the worker, waits until both are ready (readiness reads the bucket).
3. At the provider: delete the old key.
4. `./secrets/rotate.sh storage-revoked`: the bucket must refuse the old key (403). Every link signed with
   it is dead from now on.

## Not in the env files

| Secret | Where | If it leaked | Rotation | Effect on users |
|---|---|---|---|---|
| Data Protection keys | table `account."DataProtectionKeys"` | forge access and refresh tokens of any account | `./secrets/rotate.sh data-protection`: deletes the keys, the hosts create new ones when recreated | **every user is signed out** and signs in again |
| age private key of a person in `deploy/secrets/<environment>/recipients.txt` | that person's password manager | decrypt every secret of the environment from the repository | treat every secret of the environment as leaked: rotate them all, then a new age key for the person, `recipients.txt` without the old one, `secrets.sh encrypt` and commit | as the secrets rotated |
| registry read token | `docker login ghcr.io` on the server (`/root/.docker/config.json`) | pull the release images (code, no data) | new read-only token, `docker login ghcr.io` again, revoke the old one in GitHub | none |
| `DEPLOY_SSH_KEY` (with `DEPLOY_HOST`, `DEPLOY_KNOWN_HOSTS`) | GitHub environment secrets of the `deploy` workflow | sign in to the server as the deploy user and run a release | new key pair, public key in the deploy user's `authorized_keys`, old one removed, secret updated; `ssh` with the old key is refused | none |
| Grafana admin password, alert webhook URL | monitoring server | read metrics and logs (no content, PLAT-06); send fake alerts | new value on the monitoring server, restart Grafana / Alertmanager | none |

## The encrypted copy (sops + age)

The repository holds each environment's three env files encrypted, in `deploy/secrets/<environment>/`
(`deploy/README.md`, "Secrets"). After any rotation, on the server:

```bash
SECRETS_DIR=/root/secrets-out ./secrets/secrets.sh encrypt production   # public keys only, no private key needed
```

then copy `/root/secrets-out/production/*.sops.env` (encrypted, safe to copy) into your checkout,
commit, and delete `/root/secrets-out`.

## Restoring to a moment before a rotation

The database then holds the old passwords; `backup/restore-server.sh` sets them from `postgres.env` again
(`rotate.sh apply-passwords`) before the hosts start. Data Protection keys come back with the restored
moment: rotate them again if they were rotated after it.
