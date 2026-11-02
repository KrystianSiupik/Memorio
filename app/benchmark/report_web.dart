import 'dart:js_interop';
import 'dart:js_interop_unsafe';

/// Time since the page opened, in milliseconds.
double now() => globalContext.getProperty<JSObject>('performance'.toJS).callMethod<JSNumber>('now'.toJS).toDartDouble;

/// Leaves a number on the page for tools/map-benchmark/run.mjs.
void report(String key, num value) {
  final results = globalContext.getProperty<JSObject?>('mapBenchmark'.toJS) ?? JSObject();
  results.setProperty(key.toJS, value.toJS);
  globalContext.setProperty('mapBenchmark'.toJS, results);
}
