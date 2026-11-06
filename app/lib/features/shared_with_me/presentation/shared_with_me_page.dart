import 'package:flutter/material.dart';
import 'package:flutter_riverpod/flutter_riverpod.dart';

import '../../../core/l10n/l10n.dart';
import '../../../core/offline/offline.dart';
import '../../albums/albums.dart';

/// S16 Shared with me: albums the person reads without being in them.
class SharedWithMePage extends ConsumerWidget {
  const SharedWithMePage({super.key, required this.onOpenAlbum, this.header});

  final void Function(String albumId) onOpenAlbum;

  /// What comes before the albums: the archives of people who named this person.
  final Widget? header;

  @override
  Widget build(BuildContext context, WidgetRef ref) {
    final l10n = context.l10n;
    final shared = ref.watch(sharedAlbumsProvider);
    final offline = ref.watch(isOnlineProvider).value == false;

    return Scaffold(
      appBar: AppBar(title: Text(l10n.sharedTitle)),
      body: Column(
        children: [
          if (offline)
            Padding(
              padding: const EdgeInsets.all(12),
              child: Text(l10n.noConnection, key: const Key('s16-offline')),
            ),
          ?header,
          Expanded(
            child: AlbumAsyncBody(
              value: shared,
              onRetry: () => ref.invalidate(sharedAlbumsProvider),
              builder: (albums) => albums.isEmpty
                  ? Center(child: Text(l10n.sharedEmpty))
                  : ListView(
                      children: [
                        for (final album in albums)
                          ListTile(
                            leading: const Icon(Icons.photo_album_outlined),
                            title: Text(album.name),
                            onTap: () => onOpenAlbum(album.id),
                          ),
                      ],
                    ),
            ),
          ),
        ],
      ),
    );
  }
}
