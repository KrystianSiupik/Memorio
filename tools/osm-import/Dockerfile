# OpenStreetMap import tool (MEM-E0-PLAT-03): osm2pgsql and psql. Used through docker compose
# (service osm-import), never deployed with the API.
FROM debian:trixie-slim
RUN apt-get update \
    && apt-get install --yes --no-install-recommends osm2pgsql postgresql-client \
    && rm -rf /var/lib/apt/lists/*
COPY places.lua import.sh /import/
RUN chmod +x /import/import.sh
ENTRYPOINT ["/import/import.sh"]
