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
      MapPoint(id: pin.id, latitude: pin.latitude, longitude: pin.longitude, photoUrl: pin.photoUrl),
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

/// MEM-E12-PLAT-01: the memory the mouse is over, and where on the map; the map view sets it, S03 shows the card.
final hoveredPointProvider = NotifierProvider<HoveredPointNotifier, ({String id, Offset at})?>(
  HoveredPointNotifier.new,
);

class HoveredPointNotifier extends Notifier<({String id, Offset at})?> {
  @override
  ({String id, Offset at})? build() => null;

  void hover(String memoryId, Offset at) => state = (id: memoryId, at: at);

  void leave(String memoryId) {
    if (ref.mounted && state?.id == memoryId) {
      state = null;
    }
  }
}

/// MEM-E12-PLAT-02: where the menu of the map was asked for (right click, long press); S03 shows it.
final mapMenuProvider = NotifierProvider<MapMenuNotifier, ({double latitude, double longitude, Offset at})?>(
  MapMenuNotifier.new,
);

class MapMenuNotifier extends Notifier<({double latitude, double longitude, Offset at})?> {
  @override
  ({double latitude, double longitude, Offset at})? build() => null;

  void open(double latitude, double longitude, Offset at) => state = (latitude: latitude, longitude: longitude, at: at);

  void close() {
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

/// MEM-E13-PLAT-09: how many windows are open over the map; while any is, the map does not move under the mouse.
final mapHoldProvider = NotifierProvider<MapHold, int>(MapHold.new);

class MapHold extends Notifier<int> {
  @override
  int build() => 0;

  void hold() => state++;

  void release() => state = state > 0 ? state - 1 : 0;
}
