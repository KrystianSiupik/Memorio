import 'package:flutter/widgets.dart';
import 'package:flutter_riverpod/flutter_riverpod.dart';

import '../../memories/memories.dart';
import '../domain/map_point.dart';

/// The map itself. A platform view does not render in widget tests, so screens depend on this
/// provider and `app/` supplies the MapLibre view.
final mapViewProvider = Provider<Widget>((ref) => throw UnimplementedError('Overridden in app/ (composition root)'));

/// Points on the map: the last known ones, also while reloading or after a failed reload.
final mapPointsProvider = Provider<List<MapPoint>>((ref) {
  final replay = ref.watch(replayPointsProvider);
  if (replay != null) {
    return replay;
  }
  final pins = ref.watch(memoryPinsProvider.select((loaded) => loaded.value));
  return [
    for (final pin in pins ?? const <MemoryPin>[])
      MapPoint(id: pin.id, latitude: pin.latitude, longitude: pin.longitude),
  ];
});

/// S08 replays a period: while it is open the map shows only those points (null: all, as S03 asks).
final replayPointsProvider = NotifierProvider<ReplayPointsNotifier, List<MapPoint>?>(ReplayPointsNotifier.new);

class ReplayPointsNotifier extends Notifier<List<MapPoint>?> {
  @override
  List<MapPoint>? build() => null;

  void show(List<MapPoint> points) => state = points;

  // Also from a screen going away, perhaps with the whole app.
  void clear() {
    if (ref.mounted) {
      state = null;
    }
  }
}

/// The point the map goes to: the memory S08 shows now.
final mapFocusProvider = NotifierProvider<MapFocusNotifier, MapPoint?>(MapFocusNotifier.new);

class MapFocusNotifier extends Notifier<MapPoint?> {
  @override
  MapPoint? build() => null;

  void focus(MapPoint point) => state = point;

  // Also from a screen going away, perhaps with the whole app.
  void clear() {
    if (ref.mounted) {
      state = null;
    }
  }
}

/// The memory whose point was just tapped; the map view lives behind [mapViewProvider], so S03 hears it here.
final tappedPointProvider = NotifierProvider<TappedPointNotifier, String?>(TappedPointNotifier.new);

class TappedPointNotifier extends Notifier<String?> {
  @override
  String? build() => null;

  void tap(String memoryId) => state = memoryId;

  // Also from a screen going away, perhaps with the whole app.
  void clear() {
    if (ref.mounted) {
      state = null;
    }
  }
}
