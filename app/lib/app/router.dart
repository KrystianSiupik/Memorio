import 'package:flutter/material.dart';
import 'package:flutter_riverpod/flutter_riverpod.dart';
import 'package:go_router/go_router.dart';

import '../features/account/account.dart';
import '../features/albums/albums.dart';
import '../features/map/map.dart';
import '../features/memories/memories.dart';
import '../features/plans/plans.dart';
import '../features/replay/replay.dart';
import '../features/shared_with_me/shared_with_me.dart';
import '../features/moderation/moderation.dart';
import '../features/settings/settings.dart';
import '../features/succession/succession.dart';
import 'home_shell.dart';
import 'modal_page.dart';
import 'routes.dart';

/// Path of the screen shown while the session is restored; `?from=` keeps the requested location.
const startPath = '/';

/// The navigator above the tabs: windows on it cover the bar too (MEM-E13-PLAT-07).
final _rootNavigator = GlobalKey<NavigatorState>(debugLabel: 'root');

final routerProvider = Provider<GoRouter>((ref) {
  // Re-runs the redirect whenever the session changes (restored, signed in, signed out).
  final sessionChanged = ValueNotifier(0);
  ref.listen(sessionProvider, (_, _) => sessionChanged.value++);

  final router = GoRouter(
    navigatorKey: _rootNavigator,
    initialLocation: startPath,
    refreshListenable: sessionChanged,
    redirect: (context, state) => redirectFor(session: ref.read(sessionProvider), uri: state.uri),
    routes: [
      GoRoute(path: startPath, builder: (context, state) => const _StartPage()),
      GoRoute(path: SignInRoute.path, builder: (context, state) => const SignInPage()),
      StatefulShellRoute.indexedStack(
        builder: (context, state, navigationShell) => HomeShell(navigationShell: navigationShell),
        branches: [
          _tab(
            MapRoute.path,
            Builder(
              builder: (context) => MapPage(
                onAddMemory: () => const NewMemoryRoute().go(context),
                onOpenTrash: () => const TrashRoute().go(context),
                onOpenMemory: (id) => MemoryRoute(id).go(context),
                onReplay: () => const ReplayRoute().go(context),
                onAddMemoryAt: (latitude, longitude) =>
                    NewMemoryRoute(latitude: latitude, longitude: longitude).go(context),
              ),
            ),
            routes: [
              GoRoute(
                path: ReplayRoute.path,
                builder: (context, state) => ReplayPage(
                  onOpenMemory: (id) => MemoryRoute(id).go(context),
                  onOpenAlbum: (id) => AlbumRoute(id).go(context),
                ),
              ),
              GoRoute(path: TrashRoute.path, builder: (context, state) => const TrashPage()),
              GoRoute(
                path: NewMemoryRoute.path,
                parentNavigatorKey: _rootNavigator,
                // MEM-E13-PLAT-07: a window over the map, closed only by its X or Cancel.
                pageBuilder: (context, state) {
                  final point = NewMemoryRoute.pointOf(state.uri.queryParameters);
                  return ModalPage(
                    key: state.pageKey,
                    child: NewMemoryPage(
                      onSaved: () => const MapRoute().go(context),
                      onClose: () => const MapRoute().go(context),
                      initialLatitude: point?.latitude,
                      initialLongitude: point?.longitude,
                    ),
                  );
                },
              ),
              GoRoute(
                path: MemoryRoute.path,
                builder: (context, state) {
                  final id = state.pathParameters['id']!;
                  return MemoryDetailsPage(
                    memoryId: id,
                    onLeave: () => const MapRoute().go(context),
                    onReport: () => MemoryReportRoute(id).go(context),
                  );
                },
                routes: [
                  GoRoute(
                    path: MemoryReportRoute.path,
                    builder: (context, state) {
                      final id = state.pathParameters['id']!;
                      return ReportContentPage(
                        target: ReportTarget.memory,
                        targetId: id,
                        onDone: () => MemoryRoute(id).go(context),
                      );
                    },
                  ),
                ],
              ),
            ],
          ),
          _tab(
            AlbumsRoute.path,
            Builder(
              builder: (context) => AlbumsPage(
                onOpenAlbum: (id) => AlbumRoute(id).go(context),
                onOpenInvitation: (id) => InvitationRoute(id).go(context),
                onOpenConsents: () => const MyConsentsRoute().go(context),
                onOpenEntrusted: () => const EntrustedRoute().go(context),
                header: TakeoverOffersList(onOpen: (id) => TakeoverOfferRoute(id).go(context)),
              ),
            ),
            routes: [
              GoRoute(
                path: AlbumLinkRoute.path,
                builder: (context, state) => AlbumLinkPage(
                  token: state.pathParameters['token']!,
                  onEntered: (albumId) => AlbumRoute(albumId).go(context),
                ),
              ),
              GoRoute(
                path: TakeoverOfferRoute.path,
                builder: (context, state) => TakeoverOfferPage(
                  caseId: state.pathParameters['id']!,
                  onDone: () => const AlbumsRoute().go(context),
                ),
              ),
              GoRoute(path: MyConsentsRoute.path, builder: (context, state) => const AlbumConsentsPage()),
              GoRoute(path: EntrustedRoute.path, builder: (context, state) => const AlbumEntrustedPage()),
              GoRoute(
                path: InvitationRoute.path,
                builder: (context, state) => AlbumInvitationPage(
                  invitationId: state.pathParameters['id']!,
                  onAccepted: (albumId) => AlbumRoute(albumId).go(context),
                  onDone: () => const AlbumsRoute().go(context),
                ),
              ),
              GoRoute(
                path: AlbumRoute.path,
                builder: (context, state) {
                  final id = state.pathParameters['id']!;
                  return AlbumPage(
                    albumId: id,
                    onOpenMemory: (memoryId) => MemoryRoute(memoryId).go(context),
                    onManage: () => AlbumManageRoute(id).go(context),
                    onConsents: () => AlbumConsentsRoute(id).go(context),
                    onPlan: () => AlbumPlanRoute(id).go(context),
                    onReportDeath: () => ReportDeathRoute(id).go(context),
                    onReport: () => AlbumReportRoute(id).go(context),
                    ownerlessBlock: AlbumTakeoverBlock(albumId: id),
                    onLeft: () => const AlbumsRoute().go(context),
                  );
                },
                routes: [
                  GoRoute(
                    path: AlbumManageRoute.path,
                    builder: (context, state) => AlbumManagePage(albumId: state.pathParameters['id']!),
                  ),
                  GoRoute(
                    path: AlbumReportRoute.path,
                    builder: (context, state) {
                      final id = state.pathParameters['id']!;
                      return ReportContentPage(
                        target: ReportTarget.album,
                        targetId: id,
                        onDone: () => AlbumRoute(id).go(context),
                      );
                    },
                  ),
                  GoRoute(
                    path: ReportDeathRoute.path,
                    builder: (context, state) {
                      final id = state.pathParameters['id']!;
                      return ReportDeathPage(albumId: id, onDone: () => AlbumRoute(id).go(context));
                    },
                  ),
                  GoRoute(
                    path: AlbumPlanRoute.path,
                    builder: (context, state) {
                      final id = state.pathParameters['id']!;
                      return PlanPage(albumId: id, onChooseMaterial: () => AlbumPlanMaterialRoute(id).go(context));
                    },
                    routes: [
                      GoRoute(
                        path: AlbumPlanMaterialRoute.path,
                        builder: (context, state) => PlanMaterialPage(albumId: state.pathParameters['id']),
                      ),
                    ],
                  ),
                  GoRoute(
                    path: AlbumConsentsRoute.path,
                    builder: (context, state) => AlbumConsentsPage(albumId: state.pathParameters['id']),
                  ),
                ],
              ),
            ],
          ),
          _tab(
            SharedWithMeRoute.path,
            Builder(
              builder: (context) => SharedWithMePage(
                onOpenAlbum: (id) => AlbumRoute(id).go(context),
                header: LegacyArchivesList(onOpen: (owner) => LegacyArchiveRoute(owner).go(context)),
              ),
            ),
            routes: [
              GoRoute(
                path: LegacyArchiveRoute.path,
                builder: (context, state) => LegacyArchivePage(
                  ownerAccountId: state.pathParameters['owner']!,
                  onOpenMemory: (id) => MemoryRoute(id).go(context),
                ),
              ),
            ],
          ),
          _tab(
            AccountRoute.path,
            Builder(
              builder: (context) => AccountPage(
                onOpenPlan: () => const PlanRoute().go(context),
                onOpenLegacyContact: () => const LegacyContactRoute().go(context),
                onOpenModeration: () => const MyModerationRoute().go(context),
                onOpenSettings: () => const SettingsRoute().go(context),
              ),
            ),
            routes: [
              GoRoute(path: LegacyContactRoute.path, builder: (context, state) => const LegacyContactPage()),
              GoRoute(path: MyModerationRoute.path, builder: (context, state) => const MyModerationPage()),
              GoRoute(path: SettingsRoute.path, builder: (context, state) => const SettingsPage()),
              GoRoute(
                path: PlanRoute.path,
                builder: (context, state) => PlanPage(onChooseMaterial: () => const PlanMaterialRoute().go(context)),
                routes: [GoRoute(path: PlanMaterialRoute.path, builder: (context, state) => const PlanMaterialPage())],
              ),
            ],
          ),
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

StatefulShellBranch _tab(String path, Widget page, {List<RouteBase> routes = const []}) => StatefulShellBranch(
  routes: [GoRoute(path: path, builder: (context, state) => page, routes: routes)],
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
    if (location == SignInRoute.path) {
      return null;
    }

    // A link to an album is what the person came for (S15): back to it after signing in. Other places are not
    // kept, so the next person signing in on this device does not land where the previous one was.
    final wanted = location == startPath ? requested : uri.toString();
    return wanted != null && wanted.startsWith('${AlbumsRoute.path}/link/')
        ? Uri(path: SignInRoute.path, queryParameters: {'from': wanted}).toString()
        : SignInRoute.path;
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
