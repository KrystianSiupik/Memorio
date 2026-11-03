// The real app, signed in, on the real map tab with a synthetic profile of 10 000 points; driven by
// tools/map-benchmark/run.mjs in a browser (MEM-E0-PLAT-05). Never shipped: it lives outside lib/.
import 'dart:math';

import 'package:flutter/foundation.dart';
import 'package:flutter/widgets.dart';
import 'package:flutter_riverpod/flutter_riverpod.dart';
import 'package:maplibre_gl/maplibre_gl.dart';
import 'package:memorio/app/composition.dart';
import 'package:memorio/app/memorio_app.dart';
import 'package:memorio/core/api/api.dart';
import 'package:memorio/features/account/account.dart';
import 'package:memorio/features/map/map.dart';
import 'package:memorio/features/memories/memories.dart';

import 'report_device.dart' if (dart.library.js_interop) 'report_web.dart';

const _pointCount = int.fromEnvironment('MAP_POINTS', defaultValue: 10000);

class _SignedIn implements SessionRepository {
  @override
  Future<Session?> restore() async => const Session(accountId: 'benchmark');
}

/// Deterministic points gathered around a few dozen places, like a real user's memories.
List<MemoryPin> syntheticPoints(int count) {
  final random = Random(42);
  final places = [for (var i = 0; i < 40; i++) (random.nextDouble() * 110 - 50, random.nextDouble() * 340 - 170)];
  double spread() => (random.nextDouble() - 0.5) * 3;
  return [
    for (var i = 0; i < count; i++)
      () {
        final (latitude, longitude) = places[random.nextInt(places.length)];
        return MemoryPin(id: 'p$i', latitude: latitude + spread(), longitude: longitude + spread());
      }(),
  ];
}

/// Marks when the map first shows the base map and the points together (QS3) and when it has finished
/// drawing, then moves the camera so the harness can count frames.
class _Script {
  static const _everything = Rect.fromLTWH(0, 0, 10000, 10000);

  MapLibreMapController? _controller;
  bool _idleReported = false;

  Future<void> pointsAdded(MapLibreMapController controller) async {
    report('pointsAddedAt', now());
    _controller = controller;
    for (;;) {
      final base = await controller.queryRenderedFeaturesInRect(_everything, ['earth'], null);
      final points = await controller.queryRenderedFeaturesInRect(_everything, mapPointLayers, null);
      if (base.isNotEmpty && points.isNotEmpty) {
        report('pointsVisibleAt', now());
        report('renderedFeatures', points.length);
        return;
      }
      await Future<void>.delayed(const Duration(milliseconds: 16));
    }
  }

  Future<void> idle() async {
    final controller = _controller;
    if (controller == null || _idleReported) {
      return;
    }
    final points = await controller.queryRenderedFeaturesInRect(_everything, mapPointLayers, null);
    if (points.isEmpty || _idleReported) {
      return;
    }
    _idleReported = true;
    report('mapIdleAt', now());
    await _move(controller);
  }

  Future<void> _move(MapLibreMapController controller) async {
    await Future<void>.delayed(const Duration(milliseconds: 300));
    report('moveStartedAt', now());
    const duration = Duration(milliseconds: 4000);
    await controller.animateCamera(
      CameraUpdate.newCameraPosition(const CameraPosition(target: LatLng(45, 90), zoom: 3)),
      duration: duration,
    );
    await Future<void>.delayed(duration);
    report('moveEndedAt', now());
  }
}

Future<void> main() async {
  WidgetsFlutterBinding.ensureInitialized();
  useSelfHostedMapLibre();
  final script = _Script();
  final points = syntheticPoints(_pointCount);
  report('startedAt', now());

  runApp(
    ProviderScope(
      overrides: [
        ...appOverrides(appVersion: '1.0.0'),
        if (kIsWeb) apiBaseUrlProvider.overrideWithValue(Uri.base.origin),
        sessionRepositoryProvider.overrideWithValue(_SignedIn()),
        memoryPinsProvider.overrideWith((ref) async => points),
        mapViewProvider.overrideWith((ref) {
          report('mapOpenedAt', now());
          return MapLibreMapView(onPointsAdded: script.pointsAdded, onIdle: script.idle);
        }),
      ],
      child: const MemorioApp(),
    ),
  );
}
