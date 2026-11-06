import 'package:flutter/material.dart';
import 'package:flutter_riverpod/flutter_riverpod.dart';
import 'package:uuid/uuid.dart';

import '../../../core/l10n/l10n.dart';
import 'albums_providers.dart';

/// S09 Albums: mine, shared with me, invitations waiting, and a new one. The id is made once per name typed,
/// so sending again after a failure is the same album.
class AlbumsPage extends ConsumerStatefulWidget {
  const AlbumsPage({
    super.key,
    required this.onOpenAlbum,
    required this.onOpenInvitation,
    required this.onOpenConsents,
    required this.onOpenEntrusted,
    this.header,
  });

  /// What comes before the albums: offers waiting for this person.
  final Widget? header;

  final void Function(String albumId) onOpenAlbum;
  final void Function(String invitationId) onOpenInvitation;
  final VoidCallback onOpenConsents;
  final VoidCallback onOpenEntrusted;

  @override
  ConsumerState<AlbumsPage> createState() => _AlbumsPageState();
}

class _AlbumsPageState extends ConsumerState<AlbumsPage> with AlbumActions {
  static const _uuid = Uuid();

  final _name = TextEditingController();
  var _newId = _uuid.v7();
  var _nameMissing = false;

  @override
  void dispose() {
    _name.dispose();
    super.dispose();
  }

  Future<void> _create() async {
    final name = _name.text.trim();
    setState(() => _nameMissing = name.isEmpty);
    if (name.isEmpty) {
      return;
    }
    final id = _newId;
    if (await act(() => albums.create(id, name))) {
      _name.clear();
      _newId = _uuid.v7();
      ref.invalidate(albumsProvider);
      widget.onOpenAlbum(id);
    }
  }

  @override
  Widget build(BuildContext context) {
    final l10n = context.l10n;
    final mine = ref.watch(albumsProvider);
    final shared = ref.watch(sharedAlbumsProvider).value ?? const [];
    final invitations = ref.watch(invitationsProvider).value ?? const [];
    final loaded = mine.hasValue;

    return Scaffold(
      appBar: AppBar(
        title: Text(l10n.albumsTitle),
        actions: [
          IconButton(
            tooltip: l10n.albumEntrusted,
            icon: const Icon(Icons.folder_shared_outlined),
            onPressed: widget.onOpenEntrusted,
          ),
          IconButton(
            tooltip: l10n.albumConsents,
            icon: const Icon(Icons.verified_user_outlined),
            onPressed: widget.onOpenConsents,
          ),
        ],
      ),
      body: Column(
        children: [
          ...offlineBlock(context),
          Expanded(
            child: AlbumAsyncBody(
              value: mine,
              onRetry: () {
                ref.invalidate(albumsProvider);
                ref.invalidate(sharedAlbumsProvider);
                ref.invalidate(invitationsProvider);
              },
              builder: (list) => ListView(
                padding: const EdgeInsets.all(16),
                children: [
                  ?widget.header,
                  if (invitations.isNotEmpty) ...[
                    Text(l10n.albumInvitations, style: Theme.of(context).textTheme.titleMedium),
                    for (final invitation in invitations)
                      ListTile(
                        leading: const Icon(Icons.mail_outline),
                        title: Text(invitation.albumName),
                        onTap: () => widget.onOpenInvitation(invitation.id),
                      ),
                    const SizedBox(height: 16),
                  ],
                  Text(l10n.albumMine, style: Theme.of(context).textTheme.titleMedium),
                  if (list.isEmpty)
                    Padding(padding: const EdgeInsets.symmetric(vertical: 12), child: Text(l10n.albumsEmpty)),
                  for (final album in list)
                    ListTile(
                      title: Text(album.name),
                      subtitle: Text(l10n.albumMembersCount(album.members, album.memories)),
                      onTap: () => widget.onOpenAlbum(album.id),
                    ),
                  const SizedBox(height: 16),
                  Text(l10n.albumShared, style: Theme.of(context).textTheme.titleMedium),
                  if (shared.isEmpty)
                    Padding(padding: const EdgeInsets.symmetric(vertical: 12), child: Text(l10n.albumSharedEmpty)),
                  for (final album in shared)
                    ListTile(title: Text(album.name), onTap: () => widget.onOpenAlbum(album.id)),
                  const SizedBox(height: 16),
                  TextField(
                    controller: _name,
                    decoration: InputDecoration(
                      labelText: l10n.albumNewName,
                      errorText: _nameMissing ? l10n.albumNameRequired : null,
                    ),
                  ),
                  ...failureBlock(context),
                  const SizedBox(height: 8),
                  FilledButton(onPressed: offline || busy || !loaded ? null : _create, child: Text(l10n.albumCreate)),
                ],
              ),
            ),
          ),
        ],
      ),
    );
  }
}
