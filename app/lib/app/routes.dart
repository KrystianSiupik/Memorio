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

/// S06: the trash, on the way through the map tab.
final class TrashRoute extends AppRoute {
  const TrashRoute();

  static const path = 'trash';

  @override
  String get location => '${MapRoute.path}/$path';
}

/// S05: one memory, on the way through the map tab.
final class MemoryRoute extends AppRoute {
  const MemoryRoute(this.id);

  static const path = 'memories/:id';

  final String id;

  @override
  String get location => '${MapRoute.path}/memories/$id';
}

/// S04: a new memory, on the way through the map tab.
final class NewMemoryRoute extends AppRoute {
  const NewMemoryRoute();

  static const path = 'new';

  @override
  String get location => '${MapRoute.path}/$path';
}

/// S09: albums.
final class AlbumsRoute extends TabRoute {
  const AlbumsRoute();

  static const path = '/albums';

  @override
  String get location => path;
}

/// S16: shared with me.
final class AlbumRoute extends AppRoute {
  const AlbumRoute(this.id);

  static const path = 'album/:id';

  final String id;

  @override
  String get location => '${AlbumsRoute.path}/album/$id';
}

/// S15: what a link to an album opens.
final class AlbumLinkRoute extends AppRoute {
  const AlbumLinkRoute(this.token);

  static const path = 'link/:token';

  final String token;

  @override
  String get location => '${AlbumsRoute.path}/link/$token';
}

/// S11: only its owner gets a way in.
final class AlbumManageRoute extends AppRoute {
  const AlbumManageRoute(this.id);

  static const path = 'manage';

  final String id;

  @override
  String get location => '${const AlbumRoute('').location}$id/$path';
}

/// S18 of an album, for its owner.
final class AlbumPlanRoute extends AppRoute {
  const AlbumPlanRoute(this.id);

  static const path = 'plan';

  final String id;

  @override
  String get location => '${const AlbumRoute('').location}$id/$path';
}

/// S19 of an album.
final class AlbumPlanMaterialRoute extends AppRoute {
  const AlbumPlanMaterialRoute(this.id);

  static const path = 'material';

  final String id;

  @override
  String get location => '${AlbumPlanRoute(id).location}/$path';
}

/// S12 from inside an album: its curator too.
final class AlbumConsentsRoute extends AppRoute {
  const AlbumConsentsRoute(this.id);

  static const path = 'consents';

  final String id;

  @override
  String get location => '${const AlbumRoute('').location}$id/$path';
}

/// S12 from the list: what stayed in albums the person left.
final class MyConsentsRoute extends AppRoute {
  const MyConsentsRoute();

  static const path = 'my-consents';

  @override
  String get location => '${AlbumsRoute.path}/$path';
}

/// S13.
final class EntrustedRoute extends AppRoute {
  const EntrustedRoute();

  static const path = 'entrusted';

  @override
  String get location => '${AlbumsRoute.path}/$path';
}

/// S14.
final class InvitationRoute extends AppRoute {
  const InvitationRoute(this.id);

  static const path = 'invitation/:id';

  final String id;

  @override
  String get location => '${AlbumsRoute.path}/invitation/$id';
}

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

/// S18 of the person.
final class PlanRoute extends AppRoute {
  const PlanRoute();

  static const path = 'plan';

  @override
  String get location => '${AccountRoute.path}/$path';
}

/// S19 of the person.
final class PlanMaterialRoute extends AppRoute {
  const PlanMaterialRoute();

  static const path = 'material';

  @override
  String get location => '${const PlanRoute().location}/$path';
}
