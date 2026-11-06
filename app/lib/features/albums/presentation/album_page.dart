import 'package:flutter/material.dart';
import 'package:flutter_riverpod/flutter_riverpod.dart';
import 'package:uuid/uuid.dart';

import '../../../core/l10n/l10n.dart';
import '../../account/account.dart';
import '../../memories/memories.dart' show dayText, dayTextOf, memoryPinsProvider, photoPickerProvider;
import '../domain/album.dart';
import 'albums_providers.dart';

/// S10 An album: memories, who is in it, what the person brought and what is left in the pool.
class AlbumPage extends ConsumerStatefulWidget {
  const AlbumPage({
    super.key,
    required this.albumId,
    required this.onOpenMemory,
    required this.onManage,
    required this.onConsents,
    required this.onPlan,
    required this.onReportDeath,
    this.onReport,
    required this.ownerlessBlock,
    required this.onLeft,
  });

  final String albumId;
  final void Function(String memoryId) onOpenMemory;
  final VoidCallback onManage;
  final VoidCallback onConsents;
  final VoidCallback onPlan;
  final VoidCallback onReportDeath;

  /// S27: someone in the album, or reading it through a link, can report it (L19).
  final VoidCallback? onReport;

  /// What someone in an album without an owner can do about it.
  final Widget ownerlessBlock;
  final VoidCallback onLeft;

  @override
  ConsumerState<AlbumPage> createState() => _AlbumPageState();
}

class _AlbumPageState extends ConsumerState<AlbumPage> with AlbumActions {
  static const _uuid = Uuid();

  var _keep = true;
  var _leaveConfirmed = false;

  void _refresh() {
    ref.invalidate(albumDetailsProvider(widget.albumId));
    ref.invalidate(albumsProvider);
  }

  Future<void> _addMemory(BuildContext context) async {
    final own = await ref.read(ownMemoriesProvider.future);
    if (!context.mounted) {
      return;
    }
    final l10n = context.l10n;
    final chosen = await showModalBottomSheet<OwnMemory>(
      context: context,
      builder: (sheet) => own.isEmpty
          ? Padding(padding: const EdgeInsets.all(24), child: Text(l10n.albumNoMemoriesToAdd))
          : ListView(
              shrinkWrap: true,
              children: [
                for (final memory in own)
                  ListTile(
                    title: Text(memory.placeName),
                    subtitle: Text(dayTextOf(memory.dateFrom)),
                    onTap: () => Navigator.of(sheet).pop(memory),
                  ),
              ],
            ),
    );
    if (chosen != null && await act(() => albums.addMemory(widget.albumId, chosen.id))) {
      _refresh();
    }
  }

  Future<void> _addPhotos(String memoryId) async {
    final picked = await ref.read(photoPickerProvider).pick();
    for (final photo in picked) {
      final id = _uuid.v7();
      if (!await act(() => albums.addPhoto(widget.albumId, memoryId, id, photo))) {
        break;
      }
    }
    _refresh();
  }

  Future<void> _tag(BuildContext context, AlbumDetails album, AlbumMemory memory) async {
    final person = await showModalBottomSheet<AlbumMember>(
      context: context,
      builder: (sheet) => ListView(
        shrinkWrap: true,
        children: [
          for (final member in album.members.where((member) => !memory.taggedAccountIds.contains(member.accountId)))
            ListTile(title: Text(member.label), onTap: () => Navigator.of(sheet).pop(member)),
        ],
      ),
    );
    if (person != null && await act(() => albums.tag(widget.albumId, memory.id, person.accountId))) {
      _refresh();
    }
  }

  Future<void> _removeMyTag(String memoryId) async {
    if (await act(() => albums.removeMyTag(widget.albumId, memoryId))) {
      _refresh();
    }
  }

  Future<void> _block(BuildContext context, AlbumDetails album) async {
    final l10n = context.l10n;
    final viewers = await ref.read(albumViewersProvider(widget.albumId).future);
    if (!context.mounted) {
      return;
    }
    final me = ref.read(sessionProvider).value?.accountId;

    // Only the owner blocks someone in the album (D-270).
    final people = <(String, String)>[
      for (final viewer in viewers) (viewer.accountId, viewer.label),
      if (album.isOwner)
        for (final member in album.members.where((member) => !member.isOwner && member.accountId != me))
          (member.accountId, member.label),
    ];
    final chosen = await showModalBottomSheet<(String, String)>(
      context: context,
      builder: (sheet) => people.isEmpty
          ? Padding(padding: const EdgeInsets.all(24), child: Text(l10n.albumBlockNobody))
          : ListView(
              shrinkWrap: true,
              children: [
                for (final person in people)
                  ListTile(title: Text(person.$2), onTap: () => Navigator.of(sheet).pop(person)),
              ],
            ),
    );
    if (chosen == null || !context.mounted) {
      return;
    }
    final confirmed = await showDialog<bool>(
      context: context,
      builder: (dialog) => AlertDialog(
        title: Text(l10n.albumBlockConfirmTitle),
        content: Text(l10n.albumBlockConfirmBody(chosen.$2)),
        actions: [
          TextButton(onPressed: () => Navigator.of(dialog).pop(false), child: Text(l10n.cancel)),
          FilledButton(onPressed: () => Navigator.of(dialog).pop(true), child: Text(l10n.albumBlockConfirm)),
        ],
      ),
    );
    if (confirmed == true && await act(() => albums.block(widget.albumId, chosen.$1))) {
      _refresh();
      ref.invalidate(albumViewersProvider(widget.albumId));
      ref.invalidate(albumBlockedProvider(widget.albumId));
    }
  }

  Future<void> _leave() async {
    if (await act(() => albums.leave(widget.albumId, keepContributions: _keep))) {
      ref.invalidate(albumsProvider);
      ref.invalidate(memoryPinsProvider);
      ref.invalidate(keptContributionsProvider);
      widget.onLeft();
    }
  }

  /// D-307: the person the living owner offers the album to answers here.
  Future<void> _answerHandover({required bool accepted}) async {
    if (await act(() => albums.answerHandover(widget.albumId, accepted: accepted))) {
      ref.invalidate(albumHandoverProvider(widget.albumId));
      _refresh();
    }
  }

  Widget _handoverCard(AlbumHandover handover) {
    final l10n = context.l10n;
    return Card(
      key: const Key('s10-handover'),
      child: Padding(
        padding: const EdgeInsets.all(12),
        child: Column(
          crossAxisAlignment: CrossAxisAlignment.start,
          children: [
            Text(l10n.albumHandoverForMe(dayText(handover.until!.toLocal()))),
            Text(handover.keepsContributions ? l10n.albumHandoverOwnerKeeps : l10n.albumHandoverOwnerTakes),
            Wrap(
              spacing: 8,
              children: [
                FilledButton(
                  onPressed: offline || busy ? null : () => _answerHandover(accepted: true),
                  child: Text(l10n.albumHandoverAccept),
                ),
                OutlinedButton(
                  onPressed: offline || busy ? null : () => _answerHandover(accepted: false),
                  child: Text(l10n.albumHandoverRefuse),
                ),
              ],
            ),
          ],
        ),
      ),
    );
  }

  @override
  Widget build(BuildContext context) {
    final l10n = context.l10n;
    final details = ref.watch(albumDetailsProvider(widget.albumId));
    final handover = ref.watch(albumHandoverProvider(widget.albumId)).value ?? AlbumHandover.none;
    final me = ref.watch(sessionProvider).value?.accountId;

    return Scaffold(
      appBar: AppBar(
        title: Text(details.value?.name ?? l10n.tabAlbums),
        actions: [
          if (details.value?.isOwner ?? false)
            IconButton(
              tooltip: l10n.albumManage,
              icon: const Icon(Icons.settings_outlined),
              onPressed: widget.onManage,
            ),
          if (details.value?.isOwner ?? false)
            IconButton(
              tooltip: l10n.planAlbumTitle,
              icon: const Icon(Icons.workspace_premium_outlined),
              onPressed: widget.onPlan,
            ),
          if (details.value?.isMember ?? false)
            IconButton(
              tooltip: l10n.albumConsents,
              icon: const Icon(Icons.verified_user_outlined),
              onPressed: widget.onConsents,
            ),
        ],
      ),
      body: Column(
        children: [
          ...offlineBlock(context),
          Expanded(
            child: AlbumAsyncBody(
              value: details,
              onRetry: () => ref.invalidate(albumDetailsProvider(widget.albumId)),
              builder: (album) => ListView(
                padding: const EdgeInsets.all(16),
                children: [
                  if (!album.isMember)
                    Text(l10n.albumReadOnlyViewer, key: const Key('s10-viewer'))
                  else ...[
                    Text(l10n.albumMyContributions(album.myMemories, album.myPhotos), key: const Key('s10-mine')),
                    Text(
                      l10n.albumPoolLeft(bytesText(album.freeBytes), bytesText(album.limitBytes)),
                      key: const Key('s10-pool'),
                    ),
                    if (album.isOwnerless) widget.ownerlessBlock else Text(l10n.albumNoOwnerState),
                  ],
                  if (album.isReadOnly) Text(l10n.albumReadOnly, key: const Key('s10-read-only')),
                  if (handover.forMe && handover.waits) _handoverCard(handover),
                  if (album.isMember && !album.isOwner && !album.isOwnerless)
                    TextButton(onPressed: widget.onReportDeath, child: Text(l10n.albumReportDeath)),
                  if (!album.isOwner && widget.onReport != null)
                    TextButton(
                      key: const Key('s10-report'),
                      onPressed: widget.onReport,
                      child: Text(l10n.reportContent),
                    ),
                  ...failureBlock(context),
                  if (album.isMember) ...[
                    const SizedBox(height: 8),
                    FilledButton(
                      onPressed: offline || busy || album.isReadOnly ? null : () => _addMemory(context),
                      child: Text(l10n.albumAddMemory),
                    ),
                  ],
                  const SizedBox(height: 16),
                  Text(l10n.albumMemories, style: Theme.of(context).textTheme.titleMedium),
                  if (album.memories.isEmpty)
                    Padding(padding: const EdgeInsets.symmetric(vertical: 12), child: Text(l10n.albumEmpty)),
                  for (final memory in album.memories)
                    Card(
                      key: Key('s10-memory-${memory.id}'),
                      child: Column(
                        crossAxisAlignment: CrossAxisAlignment.start,
                        children: [
                          ListTile(
                            title: Text(memory.placeName),
                            subtitle: Text(
                              [
                                dayTextOf(memory.dateFrom),
                                if (memory.authorEmail case final String author) author,
                                l10n.trashPhotos(memory.photos),
                              ].join(' · '),
                            ),
                            onTap: () => widget.onOpenMemory(memory.id),
                          ),
                          if (album.isMember)
                            Wrap(
                              spacing: 8,
                              children: [
                                if (memory.isMine)
                                  TextButton(
                                    onPressed: offline || busy || album.poolIsFull || album.isReadOnly
                                        ? null
                                        : () => _addPhotos(memory.id),
                                    child: Text(l10n.albumAddPhotos),
                                  ),
                                TextButton(
                                  onPressed: offline || busy ? null : () => _tag(context, album, memory),
                                  child: Text(l10n.albumTag),
                                ),
                                if (memory.taggedAccountIds.contains(me))
                                  TextButton(
                                    onPressed: offline || busy ? null : () => _removeMyTag(memory.id),
                                    child: Text(l10n.albumRemoveMyTag),
                                  ),
                              ],
                            ),
                        ],
                      ),
                    ),
                  if (album.isMember) ...[
                    const SizedBox(height: 16),
                    Text(l10n.albumMembers, style: Theme.of(context).textTheme.titleMedium),
                    for (final member in album.members)
                      ListTile(
                        title: Text(member.label),
                        subtitle: Text(member.isOwner ? l10n.albumOwnerRole : l10n.albumParticipantRole),
                      ),
                    OutlinedButton(
                      onPressed: offline || busy ? null : () => _block(context, album),
                      child: Text(l10n.albumBlock),
                    ),
                  ],
                  if (album.isMember && !album.isOwner) ...[
                    const SizedBox(height: 16),
                    Text(l10n.albumLeave, style: Theme.of(context).textTheme.titleMedium),
                    RadioGroup<bool>(
                      groupValue: _keep,
                      onChanged: (value) => setState(() => _keep = value ?? true),
                      child: Column(
                        children: [
                          RadioListTile<bool>(value: true, title: Text(l10n.albumLeaveKeep)),
                          RadioListTile<bool>(value: false, title: Text(l10n.albumLeaveTake)),
                        ],
                      ),
                    ),
                    CheckboxListTile(
                      value: _leaveConfirmed,
                      title: Text(l10n.albumLeaveConfirm),
                      onChanged: (value) => setState(() => _leaveConfirmed = value ?? false),
                    ),
                    OutlinedButton(
                      onPressed: offline || busy || !_leaveConfirmed ? null : _leave,
                      child: Text(l10n.albumLeave),
                    ),
                  ],
                ],
              ),
            ),
          ),
        ],
      ),
    );
  }
}
