// The module the app imports instead of maplibre-gl.mjs (MapLibreJsSource.urls): MapLibre GL JS plus
// the pmtiles:// protocol, loaded only when the first map opens. Self-hosted because the app must not
// tell a third party that a map was opened (D-032).
import { addProtocol } from './maplibre-gl.mjs';

export * from './maplibre-gl.mjs';

// pmtiles.js is a classic script that defines a global, so it cannot be imported as a module.
await new Promise((resolve, reject) => {
  const script = document.createElement('script');
  script.src = new URL('./pmtiles.js', import.meta.url).href;
  script.onload = resolve;
  script.onerror = reject;
  document.head.append(script);
});
addProtocol('pmtiles', new globalThis.pmtiles.Protocol().tile);
