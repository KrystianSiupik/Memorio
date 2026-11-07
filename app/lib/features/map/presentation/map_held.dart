import 'package:flutter/widgets.dart';
import 'package:flutter_riverpod/flutter_riverpod.dart';

import 'map_providers.dart';

/// MEM-E13-PLAT-09: holds the map still while [child] is on the screen (a window over it).
class MapHeld extends ConsumerStatefulWidget {
  const MapHeld({super.key, required this.child});

  final Widget child;

  @override
  ConsumerState<MapHeld> createState() => _MapHeldState();
}

class _MapHeldState extends ConsumerState<MapHeld> {
  late final MapHold _hold = ref.read(mapHoldProvider.notifier);

  @override
  void initState() {
    super.initState();
    // Not while the tree is being built.
    Future.microtask(_hold.hold);
  }

  @override
  void dispose() {
    Future.microtask(_hold.release);
    super.dispose();
  }

  @override
  Widget build(BuildContext context) => widget.child;
}
