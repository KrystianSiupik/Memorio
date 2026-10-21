import 'package:flutter/material.dart';
import 'package:flutter_riverpod/flutter_riverpod.dart';
import 'package:go_router/go_router.dart';

import '../features/account/account.dart';
import '../features/albums/albums.dart';
import '../features/map/map.dart';
import '../features/shared_with_me/shared_with_me.dart';
import 'home_shell.dart';
import 'routes.dart';

/// Path of the screen shown while the session is restored; `?from=` keeps the requested location.
const startPath = '/';

final routerProvider = Provider<GoRouter>((ref) {
  // Re-runs the redirect whenever the session changes (restored, signed in, signed out).
  final sessionChanged = ValueNotifier(0);
  ref.listen(sessionProvider, (_, _) => sessionChanged.value++);

  final router = GoRouter(
    initialLocation: startPath,
    refreshListenable: sessionChanged,
    redirect: (context, state) => redirectFor(session: ref.read(sessionProvider), uri: state.uri),
    routes: [
      GoRoute(path: startPath, builder: (context, state) => const _StartPage()),
      GoRoute(path: SignInRoute.path, builder: (context, state) => const SignInPage()),
      StatefulShellRoute.indexedStack(
        builder: (context, state, navigationShell) => HomeShell(navigationShell: navigationShell),
        branches: [
          _tab(MapRoute.path, const MapPage()),
          _tab(AlbumsRoute.path, const AlbumsPage()),
          _tab(SharedWithMeRoute.path, const SharedWithMePage()),
          _tab(AccountRoute.path, const AccountPage()),
        ],
      ),
    ],
  );
  ref.onDispose(() {
    router.dispose();
    sessionChanged.dispose();
  });
  return router;
});

StatefulShellBranch _tab(String path, Widget page) => StatefulShellBranch(
  routes: [GoRoute(path: path, builder: (context, state) => page)],
);

/// Where to send the user, or null to stay (S02 "forbidden": signed out → S01).
/// - while the session is restored: the start screen, remembering the requested location;
/// - signed out: sign-in (a failed restore counts as signed out);
/// - signed in: the requested location, or the map instead of the start or sign-in screen.
String? redirectFor({required AsyncValue<Session?> session, required Uri uri}) {
  final location = uri.path;
  final requested = uri.queryParameters['from'];

  if (session.isLoading && !session.hasValue) {
    if (location == startPath) {
      return null;
    }
    return Uri(path: startPath, queryParameters: {'from': uri.toString()}).toString();
  }

  final signedIn = session.value != null;
  if (!signedIn) {
    return location == SignInRoute.path ? null : SignInRoute.path;
  }
  if (location == startPath || location == SignInRoute.path) {
    return requested != null && requested != startPath && requested != SignInRoute.path ? requested : MapRoute.path;
  }
  return null;
}

class _StartPage extends StatelessWidget {
  const _StartPage();

  @override
  Widget build(BuildContext context) {
    return const Scaffold(body: Center(child: CircularProgressIndicator()));
  }
}
