import 'package:flutter/material.dart';
import 'package:flutter_riverpod/flutter_riverpod.dart';

import 'router.dart';

/// Root widget. `app/` is the composition root: the only place that knows every feature
/// and wires implementations (provider overrides) to the interfaces features depend on.
class MemorioApp extends ConsumerWidget {
  const MemorioApp({super.key});

  @override
  Widget build(BuildContext context, WidgetRef ref) {
    return MaterialApp.router(
      title: 'Memorio',
      theme: ThemeData(colorSchemeSeed: Colors.teal, useMaterial3: true),
      routerConfig: ref.watch(routerProvider),
    );
  }
}
