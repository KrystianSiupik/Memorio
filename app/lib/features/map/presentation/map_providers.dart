import 'package:flutter/widgets.dart';
import 'package:flutter_riverpod/flutter_riverpod.dart';

import '../../memories/memories.dart';
import '../domain/map_point.dart';

/// The map itself. A platform view does not render in widget tests, so screens depend on this
/// provider and `app/` supplies the MapLibre view.
final mapViewProvider = Provider<Widget>((ref) => throw UnimplementedError('Overridden in app/ (composition root)'));

/// Points on the map: the last known ones, also while reloading or after a failed reload.
final mapPointsProvider = Provider<List<MapPoint>>((ref) {
  final pins = ref.watch(memoryPinsProvider.select((loaded) => loaded.value));
  return [
    for (final pin in pins ?? const <MemoryPin>[])
      MapPoint(id: pin.id, latitude: pin.latitude, longitude: pin.longitude),
  ];
});

/// The memory whose point was just tapped; the map view lives behind [mapViewProvider], so S03 hears it here.
final tappedPointProvider = NotifierProvider<TappedPointNotifier, String?>(TappedPointNotifier.new);

class TappedPointNotifier extends Notifier<String?> {
  @override
  String? build() => null;

  void tap(String memoryId) => state = memoryId;

  void clear() => state = null;
}
