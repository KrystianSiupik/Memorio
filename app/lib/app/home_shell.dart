import 'package:flutter/material.dart';
import 'package:flutter_riverpod/flutter_riverpod.dart';
import 'package:go_router/go_router.dart';

import '../core/l10n/l10n.dart';
import '../core/offline/offline.dart';

/// S02: the signed-in shell with the tabs Map · Albums · Shared with me · Account.
/// Each tab keeps its own navigation stack (StatefulShellRoute). Shows the offline banner on every
/// tab and sends the offline queue when the connection comes back.
class HomeShell extends ConsumerWidget {
  const HomeShell({super.key, required this.navigationShell});

  final StatefulNavigationShell navigationShell;

  @override
  Widget build(BuildContext context, WidgetRef ref) {
    ref.watch(offlineSyncProvider);
    final offline = ref.watch(isOnlineProvider).value == false;

    return Scaffold(
      body: Column(
        children: [
          if (offline) const OfflineBanner(),
          Expanded(child: navigationShell),
        ],
      ),
      bottomNavigationBar: NavigationBar(
        selectedIndex: navigationShell.currentIndex,
        // Tapping the current tab again returns to its first screen.
        onDestinationSelected: (index) =>
            navigationShell.goBranch(index, initialLocation: index == navigationShell.currentIndex),
        destinations: [
          NavigationDestination(
            icon: Icon(Icons.public_outlined),
            selectedIcon: Icon(Icons.public),
            label: context.l10n.tabMap,
          ),
          NavigationDestination(
            icon: Icon(Icons.photo_album_outlined),
            selectedIcon: Icon(Icons.photo_album),
            label: context.l10n.tabAlbums,
          ),
          NavigationDestination(
            icon: Icon(Icons.people_outline),
            selectedIcon: Icon(Icons.people),
            label: context.l10n.tabShared,
          ),
          NavigationDestination(
            icon: Icon(Icons.person_outline),
            selectedIcon: Icon(Icons.person),
            label: context.l10n.tabAccount,
          ),
        ],
      ),
    );
  }
}

/// S02 "offline" state: shown on every tab while there is no connection.
class OfflineBanner extends StatelessWidget {
  const OfflineBanner({super.key});

  @override
  Widget build(BuildContext context) {
    final colors = Theme.of(context).colorScheme;
    return Material(
      color: colors.secondaryContainer,
      child: SafeArea(
        bottom: false,
        child: Padding(
          padding: const EdgeInsets.symmetric(horizontal: 16, vertical: 8),
          child: Row(
            children: [
              Icon(Icons.cloud_off, color: colors.onSecondaryContainer),
              const SizedBox(width: 12),
              Expanded(
                child: Text(context.l10n.offlineSaved, style: TextStyle(color: colors.onSecondaryContainer)),
              ),
            ],
          ),
        ),
      ),
    );
  }
}
