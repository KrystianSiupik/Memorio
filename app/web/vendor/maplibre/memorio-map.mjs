// The module the app imports instead of maplibre-gl.mjs (MapLibreJsSource.urls): MapLibre GL JS plus
// the pmtiles:// protocol, loaded only when the first map opens. Self-hosted because the app must not
// tell a third party that a map was opened (D-032).
import { addProtocol, Map as MapLibreMap } from './maplibre-gl.mjs';

export * from './maplibre-gl.mjs';

// The right click on the map (MEM-E12-PLAT-02): instead of the browser's menu, the app hears where it was,
// as an event on window. The plugin builds every map from this module's Map, so each map has it.
export class Map extends MapLibreMap {
  constructor(options) {
    super(options);
    this.on('contextmenu', (event) => {
      event.originalEvent?.preventDefault();
      window.dispatchEvent(new CustomEvent('memorio-map-contextmenu', {
        detail: { latitude: event.lngLat.lat, longitude: event.lngLat.lng, x: event.point.x, y: event.point.y },
      }));
    });
  }
}

// pmtiles.js is a classic script that defines a global, so it cannot be imported as a module.
await new Promise((resolve, reject) => {
  const script = document.createElement('script');
  script.src = new URL('./pmtiles.js', import.meta.url).href;
  script.onload = resolve;
  script.onerror = reject;
  document.head.append(script);
});
addProtocol('pmtiles', new globalThis.pmtiles.Protocol().tile);
