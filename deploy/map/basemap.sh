#!/usr/bin/env bash
# The base map of the app (MEM-E12-PLAT-03, D-032, D-320), on the stage 1 server as root in /opt/memorio/deploy.
# Takes an area of a Protomaps world build (OpenStreetMap data, ODbL) into the directory nginx serves at /map/,
# with the fonts of the style next to it. The app reads it in pieces, so nothing of the map goes elsewhere. Usage:
#   basemap.sh install <yyyymmdd> [minLon,minLat,maxLon,maxLat] [maxZoom]
#                         a build of that day; by default Europe up to zoom 15 (tens of GB)
#   basemap.sh status     what is served now
# The new file replaces the old one only once it is whole: a map that is open keeps working.
set -euo pipefail
# shellcheck source-path=SCRIPTDIR/../backup
source "$(dirname "$0")/../backup/common.sh"

map_dir="${MAP_DIR:-/var/lib/memorio/map}"
operations_log="${OPERATIONS_LOG:-/var/lib/memorio/operations.log}"
operator="${SUDO_USER:-$(id -un)}"
europe="-25,34,45,72"

usage() { sed -n '4,7p' "$0" >&2; exit 2; }

note() {
    mkdir -p "$(dirname "$operations_log")"
    echo "$(date -u +%Y-%m-%dT%H:%M:%SZ) map by $operator: $1" >> "$operations_log"
}

install() {
    local day="$1" bbox="${2:-$europe}" zoom="${3:-15}" free needed
    [[ "$day" =~ ^[0-9]{8}$ ]] || usage
    [[ "$bbox" =~ ^-?[0-9.]+,-?[0-9.]+,-?[0-9.]+,-?[0-9.]+$ ]] || usage
    [[ "$zoom" =~ ^[0-9]+$ ]] && [ "$zoom" -le 15 ] || usage
    mkdir -p "$map_dir"
    # The whole world up to zoom 15 is about 120 GB; an area takes a share of it. Leave room for the old file.
    free="$(df --output=avail -B1G "$map_dir" | tail -1 | tr -d ' ')"
    needed=$([ "$bbox" = "$europe" ] && echo 60 || echo 10)
    [ "$free" -ge "$needed" ] || { echo "only ${free} GB free in $map_dir, about ${needed} GB needed" >&2; exit 1; }

    docker run --rm -v "$map_dir:/data" protomaps/go-pmtiles:v1.31.2 \
        extract "https://build.protomaps.com/$day.pmtiles" /data/basemap.next.pmtiles --bbox="$bbox" --maxzoom="$zoom"
    docker run --rm -v "$map_dir:/data" protomaps/go-pmtiles:v1.31.2 verify /data/basemap.next.pmtiles
    rm -rf "$map_dir/fonts.next"
    cp -r "$(dirname "$0")/../../app/benchmark/map/fonts" "$map_dir/fonts.next"
    mv -f "$map_dir/basemap.next.pmtiles" "$map_dir/basemap.pmtiles"
    rm -rf "$map_dir/fonts" && mv "$map_dir/fonts.next" "$map_dir/fonts"
    note "base map $day, area $bbox, zoom $zoom installed"
    echo "ok: $(du -h "$map_dir/basemap.pmtiles" | cut -f1) served at /map/basemap.pmtiles"
}

case "${1:-}" in
    install) [ -n "${2:-}" ] || usage; install "$2" "${3:-}" "${4:-}" ;;
    status) ls -la "$map_dir" 2>/dev/null || echo "no base map yet" ;;
    *) usage ;;
esac
