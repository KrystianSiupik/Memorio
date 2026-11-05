import 'package:flutter/material.dart';
import 'package:flutter_riverpod/flutter_riverpod.dart';

import '../../../core/l10n/l10n.dart';
import '../../../core/result.dart';
import '../../memories/memories.dart' show memoryPinsProvider;
import 'albums_providers.dart';

/// S15 Coming into an album through a link. Signed-out people come back here after signing in (router, D-107).
class AlbumLinkPage extends ConsumerStatefulWidget {
  const AlbumLinkPage({super.key, required this.token, required this.onEntered});

  final String token;
  final void Function(String albumId) onEntered;

  @override
  ConsumerState<AlbumLinkPage> createState() => _AlbumLinkPageState();
}

class _AlbumLinkPageState extends ConsumerState<AlbumLinkPage> with AlbumActions {
  Future<void> _enter() async {
    setState(() {
      busy = true;
      failure = null;
    });
    final entered = await albums.enterByLink(widget.token);
    if (!mounted) {
      return;
    }
    setState(() => busy = false);
    switch (entered) {
      case Ok(:final value):
        ref.invalidate(sharedAlbumsProvider);
        ref.invalidate(memoryPinsProvider);
        widget.onEntered(value);
      case Err(failure: final refused):
        setState(() => failure = refused);
    }
  }

  @override
  Widget build(BuildContext context) {
    final l10n = context.l10n;
    final preview = ref.watch(albumLinkPreviewProvider(widget.token));

    return Scaffold(
      appBar: AppBar(title: Text(l10n.albumEntryTitle)),
      body: Column(
        children: [
          ...offlineBlock(context),
          Expanded(
            child: AlbumAsyncBody(
              value: preview,
              onRetry: () => ref.invalidate(albumLinkPreviewProvider(widget.token)),
              builder: (album) => ListView(
                padding: const EdgeInsets.all(16),
                children: [
                  Text(l10n.albumEntryAlbum(album.albumName), style: Theme.of(context).textTheme.titleMedium),
                  const SizedBox(height: 8),
                  Text(l10n.albumEntryNamed),
                  ...failureBlock(context),
                  const SizedBox(height: 16),
                  FilledButton(onPressed: offline || busy ? null : _enter, child: Text(l10n.albumEntryOpen)),
                ],
              ),
            ),
          ),
        ],
      ),
    );
  }
}
