# Map benchmark (MEM-E0-PLAT-05)

The real app, signed in, on the map tab, with 10 000 synthetic points. Lives outside `lib/`, so it is never
shipped. Decisions and measured results: `docs/decisions.md`, D-190 to D-193.

- `map/basemap.pmtiles`: zoom 0-3 of a world build (OpenStreetMap data, ODbL), made by
  `tools/map-benchmark/make-fixture.mjs`.
- `map/fonts/`: Noto Sans glyphs (OFL), from the assets of the Protomaps base maps.

## Web

```bash
cd app
flutter build web --release --no-web-resources-cdn -t benchmark/map_benchmark_main.dart
cd ..
node tools/map-benchmark/run.mjs --runs=20 --gpu   # QS3 on this machine's GPU
node tools/map-benchmark/run.mjs --runs=5 --check  # what CI runs: software rendering, regression guard
```

`--no-web-resources-cdn` is required for every web build: without it the browser fetches the rendering
engine from a third party (D-032, D-193). Screenshot and numbers land in `app/build/map-benchmark/`.

## Android and iOS

Not run yet: there was no SDK or device when the spike was written. On a device the times are printed to the
log; there is no globe (D-191) and frame rate must be read from the Flutter performance overlay or DevTools.

```bash
node tools/map-benchmark/run.mjs --serve --port=8791   # base map for devices, on all network interfaces
cd app
flutter run -t benchmark/map_benchmark_main.dart --dart-define=MAP_BASE_URL=http://<computer-ip>:8791/map
```

Plain HTTP may be blocked on the device; if tiles do not load, serve the folder over HTTPS. Debug builds are slow,
so take the times from a `--profile` run.
