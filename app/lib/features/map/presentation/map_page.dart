import 'package:flutter/material.dart';
import 'package:flutter_riverpod/flutter_riverpod.dart';

import 'map_providers.dart';

/// S03 Map and globe. Only the map for now; the screen itself is MEM-E2-PLAT-01.
class MapPage extends ConsumerWidget {
  const MapPage({super.key});

  @override
  Widget build(BuildContext context, WidgetRef ref) {
    return Scaffold(
      appBar: AppBar(title: const Text('Map')),
      body: ref.watch(mapViewProvider),
    );
  }
}
