#!/usr/bin/env bash
# Imports an OpenStreetMap extract into places.osm_places (MEM-E0-PLAT-03), replacing the previous
# import. Runs inside the osm-import image; connection from PGHOST, PGPORT, PGUSER, PGPASSWORD,
# PGDATABASE (the database superuser: it creates and replaces tables in the places schema, which
# belongs to memorio_places). Usage: import.sh <file>
# with an .osm or .osm.pbf file, e.g. a regional extract from download.geofabrik.de.
set -euo pipefail

file="${1:?usage: import.sh <extract.osm.pbf>}"

# --slim --drop keeps node locations in temporary tables instead of memory and removes them afterwards;
# --schema and --middle-schema keep osm2pgsql's own tables in the module schema too, out of public.
osm2pgsql --create --slim --drop --schema=places --middle-schema=places \
    --output=flex --style=/import/places.lua "$file"

# The tables belong to the Places module (SD4): only its role reads them.
psql -v ON_ERROR_STOP=1 \
    -c "ALTER TABLE places.osm_places OWNER TO memorio_places" \
    -c "ALTER TABLE places.osm2pgsql_properties OWNER TO memorio_places" \
    -c "ANALYZE places.osm_places"
