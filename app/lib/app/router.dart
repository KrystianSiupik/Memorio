import 'package:flutter/material.dart';
import 'package:flutter_riverpod/flutter_riverpod.dart';
import 'package:go_router/go_router.dart';

/// Routes of the app. Tabs and the redirect of signed-out users come with MEM-E0-PLAT-19.
final routerProvider = Provider<GoRouter>((ref) {
  final router = GoRouter(
    routes: [GoRoute(path: '/', builder: (context, state) => const _StartPage())],
  );
  ref.onDispose(router.dispose);
  return router;
});

class _StartPage extends StatelessWidget {
  const _StartPage();

  @override
  Widget build(BuildContext context) {
    return const Scaffold(body: Center(child: Text('Memorio')));
  }
}
