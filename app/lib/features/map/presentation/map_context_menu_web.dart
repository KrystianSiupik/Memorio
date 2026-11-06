import 'dart:js_interop';
import 'dart:js_interop_unsafe';

/// MEM-E12-PLAT-02: the right click on the map, as `web/vendor/maplibre/memorio-map.mjs` tells it on window.
typedef MapContextMenuHandler = void Function(double latitude, double longitude, double x, double y);

const _event = 'memorio-map-contextmenu';

/// Starts listening; the returned function stops it.
void Function() listenToMapContextMenu(MapContextMenuHandler handler) {
  final listener = ((JSObject event) {
    final value = event['detail'];
    if (value == null || !value.isA<JSObject>()) {
      return;
    }
    final detail = value as JSObject;
    double read(String name) => (detail[name] as JSNumber?)?.toDartDouble ?? double.nan;
    handler(read('latitude'), read('longitude'), read('x'), read('y'));
  }).toJS;
  globalContext.callMethod('addEventListener'.toJS, _event.toJS, listener);
  return () => globalContext.callMethod('removeEventListener'.toJS, _event.toJS, listener);
}
