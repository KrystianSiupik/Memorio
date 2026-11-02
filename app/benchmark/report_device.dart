import 'package:flutter/foundation.dart';

final _clock = Stopwatch()..start();

/// Time since the app started, in milliseconds.
double now() => _clock.elapsedMicroseconds / 1000;

/// On a device the numbers go to the log (`flutter run` shows them).
void report(String key, num value) => debugPrint('map-benchmark $key=$value');
