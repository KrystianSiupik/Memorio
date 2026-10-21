import 'package:flutter/widgets.dart';
import 'package:go_router/go_router.dart';

/// Typed routes of the app: navigate with `const AlbumsRoute().go(context)`, never with raw paths.
/// Routes take no parameters yet; a route with parameters gets fields and builds its [location].
sealed class AppRoute {
  const AppRoute();

  String get location;

  void go(BuildContext context) => context.go(location);
}

/// S01: registration and sign-in. The only screen a signed-out user can reach.
final class SignInRoute extends AppRoute {
  const SignInRoute();

  static const path = '/sign-in';

  @override
  String get location => path;
}

/// The four tabs of S02, in the order of the navigation bar.
sealed class TabRoute extends AppRoute {
  const TabRoute();

  static const all = [MapRoute(), AlbumsRoute(), SharedWithMeRoute(), AccountRoute()];
}

/// S03: map and globe; the start screen after sign-in.
final class MapRoute extends TabRoute {
  const MapRoute();

  static const path = '/map';

  @override
  String get location => path;
}

/// S09: albums.
final class AlbumsRoute extends TabRoute {
  const AlbumsRoute();

  static const path = '/albums';

  @override
  String get location => path;
}

/// S16: shared with me.
final class SharedWithMeRoute extends TabRoute {
  const SharedWithMeRoute();

  static const path = '/shared';

  @override
  String get location => path;
}

/// S24: account and privacy.
final class AccountRoute extends TabRoute {
  const AccountRoute();

  static const path = '/account';

  @override
  String get location => path;
}
