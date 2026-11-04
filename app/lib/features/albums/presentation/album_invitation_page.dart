import 'package:flutter/material.dart';
import 'package:flutter_riverpod/flutter_riverpod.dart';

import '../../../core/l10n/l10n.dart';
import '../../memories/memories.dart' show memoryPinsProvider;
import '../domain/album.dart';
import 'albums_providers.dart';

/// S14 An invitation: who invites and to what, with no content of the album. Once it is gone, the screen says so.
class AlbumInvitationPage extends ConsumerStatefulWidget {
  const AlbumInvitationPage({super.key, required this.invitationId, required this.onAccepted, required this.onDone});

  final String invitationId;
  final void Function(String albumId) onAccepted;
  final VoidCallback onDone;

  @override
  ConsumerState<AlbumInvitationPage> createState() => _AlbumInvitationPageState();
}

class _AlbumInvitationPageState extends ConsumerState<AlbumInvitationPage> with AlbumActions {
  void _refresh() {
    ref.invalidate(invitationsProvider);
    ref.invalidate(albumsProvider);
  }

  Future<void> _accept(AlbumInvitation invitation) async {
    if (await act(() => albums.accept(invitation.id))) {
      _refresh();
      ref.invalidate(memoryPinsProvider);
      widget.onAccepted(invitation.albumId);
    }
  }

  Future<void> _decline() async {
    if (await act(() => albums.decline(widget.invitationId))) {
      _refresh();
      widget.onDone();
    }
  }

  /// Blocking also answers this invitation: its seat should not wait for an answer that will not come.
  Future<void> _block() async {
    if (await act(() => albums.blockInvitations()) && await act(() => albums.decline(widget.invitationId))) {
      _refresh();
      widget.onDone();
    }
  }

  @override
  Widget build(BuildContext context) {
    final l10n = context.l10n;
    final invitations = ref.watch(invitationsProvider);

    return Scaffold(
      appBar: AppBar(title: Text(l10n.albumInvitationTitle)),
      body: Column(
        children: [
          ...offlineBlock(context),
          Expanded(
            child: AlbumAsyncBody(
              value: invitations,
              onRetry: () => ref.invalidate(invitationsProvider),
              builder: (list) {
                final invitation = list.where((item) => item.id == widget.invitationId).firstOrNull;
                if (invitation == null) {
                  return Center(child: Text(l10n.albumInvitationGone, key: const Key('s14-gone')));
                }
                return ListView(
                  padding: const EdgeInsets.all(16),
                  children: [
                    Text(
                      l10n.albumInvitationAlbum(invitation.albumName),
                      style: Theme.of(context).textTheme.titleMedium,
                    ),
                    if (invitation.invitedBy case final String person) Text(l10n.albumInvitedBy(person)),
                    ...failureBlock(context),
                    const SizedBox(height: 16),
                    FilledButton(
                      onPressed: offline || busy ? null : () => _accept(invitation),
                      child: Text(l10n.albumAccept),
                    ),
                    OutlinedButton(onPressed: offline || busy ? null : _decline, child: Text(l10n.albumDecline)),
                    TextButton(onPressed: offline || busy ? null : _block, child: Text(l10n.albumBlockInvitations)),
                  ],
                );
              },
            ),
          ),
        ],
      ),
    );
  }
}
