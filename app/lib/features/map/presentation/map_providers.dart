import 'package:flutter/widgets.dart';
import 'package:flutter_riverpod/flutter_riverpod.dart';

import '../domain/map_point.dart';

/// The map itself. A platform view does not render in widget tests, so screens depend on this
/// provider and `app/` supplies the MapLibre view.
final mapViewProvider = Provider<Widget>((ref) => throw UnimplementedError('Overridden in app/ (composition root)'));

/// Points on the map; empty until the map query exists (MEM-E2-ARCHIWUM-08).
final mapPointsProvider = Provider<List<MapPoint>>((ref) => const []);
