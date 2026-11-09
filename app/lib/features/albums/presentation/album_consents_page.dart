import 'package:flutter/material.dart';
import 'package:flutter_riverpod/flutter_riverpod.dart';

import '../../../core/l10n/l10n.dart';
import '../../account/account.dart';
import '../../ai_summaries/ai_summaries.dart' show AiConsentTile;
import '../../memories/memories.dart' show memoryPinsProvider;
import 'albums_providers.dart';

/// S12 The person's consents: what stayed in albums they left and, from inside an album, its curator.
/// From inside an album, the person's AI consent for it too (C80, C81).
class AlbumConsentsPage extends ConsumerStatefulWidget {
  const AlbumConsentsPage({super.key, this.albumId});

  /// The album it was opened from; without it only what stayed after leaving is shown.
  final String? albumId;

  @override
  ConsumerState<AlbumConsentsPage> createState() => _AlbumConsentsPageState();
}

class _AlbumConsentsPageState extends ConsumerState<AlbumConsentsPage> with AlbumActions {
  final _confirmed = <String>{};

  Future<void> _revoke(String albumId) async {
    if (await act(() => albums.revokeKept(albumId))) {
      ref.invalidate(keptContributionsProvider);
      ref.invalidate(memoryPinsProvider);
    }
  }

  Future<void> _setCurator(String albumId, String accountId) async {
    if (await act(() => albums.entrust(albumId, accountId))) {
      ref.invalidate(albumDetailsProvider(albumId));
    }
  }

  Future<void> _revokeCurator(String albumId) async {
    if (await act(() => albums.revokeEntrustment(albumId))) {
      ref.invalidate(albumDetailsProvider(albumId));
    }
  }

  @override
  Widget build(BuildContext context) {
    final l10n = context.l10n;
    final kept = ref.watch(keptContributionsProvider);
    final albumId = widget.albumId;
    final me = ref.watch(sessionProvider).value?.accountId;
    final album = albumId == null ? null : ref.watch(albumDetailsProvider(albumId)).value;

    return Scaffold(
      appBar: AppBar(title: Text(l10n.albumConsents)),
      body: Column(
        children: [
          ...offlineBlock(context),
          Expanded(
            child: AlbumAsyncBody(
              value: kept,
              onRetry: () => ref.invalidate(keptContributionsProvider),
              builder: (items) => ListView(
                padding: const EdgeInsets.all(16),
                children: [
                  ...failureBlock(context),
                  if (albumId != null && (album?.isMember ?? false)) ...[
                    AiConsentTile(albumId: albumId),
                    const SizedBox(height: 16),
                  ],
                  if (album != null) ...[
                    Text(l10n.albumCurator, style: Theme.of(context).textTheme.titleMedium),
                    if (album.myCuratorAccountId == null) Text(l10n.albumNoCurator, key: const Key('s12-no-curator')),
                    for (final member in album.members.where((member) => member.accountId != me))
                      ListTile(
                        key: Key('s12-curator-${member.accountId}'),
                        title: Text(member.label),
                        trailing: album.myCuratorAccountId == member.accountId
                            ? TextButton(
                                onPressed: offline || busy ? null : () => _revokeCurator(album.id),
                                child: Text(l10n.albumRevokeEntrustment),
                              )
                            : TextButton(
                                onPressed: offline || busy ? null : () => _setCurator(album.id, member.accountId),
                                child: Text(l10n.albumEntrust),
                              ),
                      ),
                    const SizedBox(height: 16),
                  ],
                  Text(l10n.albumKept, style: Theme.of(context).textTheme.titleMedium),
                  if (items.isEmpty)
                    Padding(padding: const EdgeInsets.symmetric(vertical: 12), child: Text(l10n.albumKeptEmpty)),
                  for (final item in items)
                    Card(
                      key: Key('s12-kept-${item.albumId}'),
                      child: Column(
                        crossAxisAlignment: CrossAxisAlignment.start,
                        children: [
                          ListTile(
                            title: Text(l10n.albumKeptItem(item.albumName, item.memories)),
                            subtitle: item.chosenByAuthor ? null : Text(l10n.albumKeptNotChosen),
                          ),
                          CheckboxListTile(
                            value: _confirmed.contains(item.albumId),
                            title: Text(l10n.albumKeptConfirm),
                            onChanged: (value) => setState(() {
                              value == true ? _confirmed.add(item.albumId) : _confirmed.remove(item.albumId);
                            }),
                          ),
                          TextButton(
                            onPressed: offline || busy || !_confirmed.contains(item.albumId)
                                ? null
                                : () => _revoke(item.albumId),
                            child: Text(l10n.albumRevokeKept),
                          ),
                        ],
                      ),
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
