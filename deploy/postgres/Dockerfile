# PostgreSQL + PostGIS of stage 1 (ADR 0013, MEM-E0-PLAT-09) with WAL-G for continuous archiving and
# point-in-time recovery (QS2: RPO ≤ 15 min). The same image runs the database and the monthly restore
# test, so a backup is always restored by the version that made it.
FROM postgis/postgis:18-3.6

ARG WALG_VERSION=v3.0.9
ARG WALG_SHA256=4f03ee4679db7f660bfd1b2b8291ac5df247cd2ec48074b60c112f4897299f24

ADD https://github.com/wal-g/wal-g/releases/download/${WALG_VERSION}/wal-g-pg-22.04-amd64.tar.gz /tmp/wal-g.tar.gz
RUN echo "${WALG_SHA256}  /tmp/wal-g.tar.gz" | sha256sum -c - \
    && tar -xzf /tmp/wal-g.tar.gz -C /tmp \
    && install -m 0755 /tmp/wal-g-pg-22.04-amd64 /usr/local/bin/wal-g \
    && rm /tmp/wal-g.tar.gz /tmp/wal-g-pg-22.04-amd64 \
    && wal-g --version

# First start of an empty data directory: the bootstrap (schemas and roles, as in every environment),
# then the role passwords from this environment's secrets.
COPY db/bootstrap.sql /docker-entrypoint-initdb.d/20-bootstrap.sql
COPY deploy/postgres/30-role-passwords.sh /docker-entrypoint-initdb.d/30-role-passwords.sh
COPY deploy/postgres/40-operations.sql /docker-entrypoint-initdb.d/40-operations.sql
