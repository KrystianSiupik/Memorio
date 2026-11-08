import 'package:flutter/material.dart';
import 'package:flutter/services.dart';
import 'package:flutter_riverpod/flutter_riverpod.dart';
import 'package:uuid/uuid.dart';

import '../../../core/l10n/l10n.dart';
import '../../../core/result.dart';
import '../../memories/memories.dart' show dayText;
import '../domain/album.dart';
import 'album_link_qr.dart';
import 'albums_providers.dart';

/// S11 Managing an album, for its owner: seats, invitations, who is in it and the successor. What is typed
/// stays after a refusal, and the invitation id with it, so sending again is the same invitation.
class AlbumManagePage extends ConsumerStatefulWidget {
  const AlbumManagePage({super.key, required this.albumId});

  final String albumId;

  @override
  ConsumerState<AlbumManagePage> createState() => _AlbumManagePageState();
}

class _AlbumManagePageState extends ConsumerState<AlbumManagePage> with AlbumActions {
  static const _uuid = Uuid();

  final _email = TextEditingController();
  var _invitationId = _uuid.v7();
  var _emailInvalid = false;

  @override
  void dispose() {
    _email.dispose();
    super.dispose();
  }

  void _refresh() {
    ref.invalidate(albumHandoverProvider(widget.albumId));
    ref.invalidate(albumSeatsProvider(widget.albumId));
    ref.invalidate(albumDetailsProvider(widget.albumId));
    ref.invalidate(albumsProvider);
  }

  Future<void> _invite() async {
    final email = _email.text.trim();
    final valid = RegExp(r'^[^@\s]+@[^@\s]+$').hasMatch(email);
    setState(() => _emailInvalid = !valid);
    if (!valid) {
      return;
    }
    final id = _invitationId;
    if (await act(() => albums.invite(widget.albumId, id, email))) {
      _email.clear();
      _invitationId = _uuid.v7();
      _refresh();
    }
  }

  Future<void> _share() async {
    final shared = await albums.shareByLink(widget.albumId);
    if (!mounted) {
      return;
    }
    switch (shared) {
      case Ok():
        setState(() => failure = null);
        ref.invalidate(albumLinkProvider(widget.albumId));
      case Err(failure: final refused):
        setState(() => failure = refused);
    }
  }

  Future<void> _revokeLink() async {
    if (await act(() => albums.revokeLink(widget.albumId))) {
      ref.invalidate(albumLinkProvider(widget.albumId));
    }
  }

  Future<void> _revokeAccess(String accountId) async {
    if (await act(() => albums.revokeAccess(widget.albumId, accountId))) {
      ref.invalidate(albumViewersProvider(widget.albumId));
    }
  }

  Future<void> _unblock(String accountId) async {
    if (await act(() => albums.unblock(widget.albumId, accountId))) {
      ref.invalidate(albumBlockedProvider(widget.albumId));
    }
  }

  Future<void> _copy(String address) async {
    await Clipboard.setData(ClipboardData(text: address));
    if (mounted) {
      ScaffoldMessenger.of(context).showSnackBar(SnackBar(content: Text(context.l10n.albumLinkCopied)));
    }
  }

  /// S2:B3: the owner says now, while they still can, what becomes of their own contributions.
  Future<void> _handOver(AlbumMember member) async {
    final l10n = context.l10n;
    final keep = await showDialog<bool>(
      context: context,
      builder: (context) => AlertDialog(
        title: Text(l10n.albumHandoverTo(member.label)),
        content: Text(l10n.albumHandoverQuestion),
        actions: [
          TextButton(onPressed: () => Navigator.of(context).pop(), child: Text(l10n.cancel)),
          TextButton(
            key: const Key('s11-handover-take'),
            onPressed: () => Navigator.of(context).pop(false),
            child: Text(l10n.albumHandoverTake),
          ),
          FilledButton(
            key: const Key('s11-handover-keep'),
            onPressed: () => Navigator.of(context).pop(true),
            child: Text(l10n.albumHandoverKeep),
          ),
        ],
      ),
    );
    if (keep != null && mounted) {
      await _then(act(() => albums.offerHandover(widget.albumId, member.accountId, keepContributions: keep)));
    }
  }

  List<Widget> _handoverBlock(AlbumDetails album, AlbumHandover handover) {
    final l10n = context.l10n;
    final member = album.members.where((member) => member.accountId == handover.toAccountId).firstOrNull;
    return [
      const SizedBox(height: 16),
      Text(l10n.albumHandover, style: Theme.of(context).textTheme.titleMedium),
      if (handover.waits) ...[
        Text(
          l10n.albumHandoverWaiting(dayText(handover.until!.toLocal()), member?.label ?? ''),
          key: const Key('s11-handover-waiting'),
        ),
        TextButton(
          onPressed: offline || busy ? null : () => _then(act(() => albums.withdrawHandover(widget.albumId))),
          child: Text(l10n.albumHandoverWithdraw),
        ),
      ] else
        Text(l10n.albumHandoverExplain),
    ];
  }

  Future<void> _then(Future<bool> action) async {
    if (await action) {
      _refresh();
    }
  }

  @override
  Widget build(BuildContext context) {
    final l10n = context.l10n;
    final seats = ref.watch(albumSeatsProvider(widget.albumId));
    final album = ref.watch(albumDetailsProvider(widget.albumId)).value;
    final link = ref.watch(albumLinkProvider(widget.albumId)).value;
    final viewers = ref.watch(albumViewersProvider(widget.albumId)).value ?? const <AlbumViewer>[];
    final blocked = ref.watch(albumBlockedProvider(widget.albumId)).value ?? const <AlbumViewer>[];
    final handover = ref.watch(albumHandoverProvider(widget.albumId)).value ?? AlbumHandover.none;
    return Scaffold(
      appBar: AppBar(title: Text(l10n.albumManage)),
      body: Column(
        children: [
          ...offlineBlock(context),
          Expanded(
            child: AlbumAsyncBody(
              value: seats,
              onRetry: () => ref.invalidate(albumSeatsProvider(widget.albumId)),
              builder: (seats) => ListView(
                padding: const EdgeInsets.all(16),
                children: [
                  Text(l10n.albumSeats(seats.free, seats.invited, seats.taken), key: const Key('s11-seats')),
                  const SizedBox(height: 16),
                  TextField(
                    controller: _email,
                    keyboardType: TextInputType.emailAddress,
                    decoration: InputDecoration(
                      labelText: l10n.albumInviteEmail,
                      errorText: _emailInvalid ? l10n.albumInviteInvalid : null,
                    ),
                  ),
                  ...failureBlock(context),
                  const SizedBox(height: 8),
                  FilledButton(
                    onPressed: offline || busy || seats.free <= 0 ? null : _invite,
                    child: Text(l10n.albumInvite),
                  ),
                  if (seats.invitations.isNotEmpty) ...[
                    const SizedBox(height: 16),
                    Text(l10n.albumInvited, style: Theme.of(context).textTheme.titleMedium),
                    for (final invited in seats.invitations)
                      ListTile(
                        title: Text(invited.email),
                        trailing: TextButton(
                          onPressed: offline || busy
                              ? null
                              : () => _then(act(() => albums.withdrawInvitation(widget.albumId, invited.id))),
                          child: Text(l10n.albumWithdrawInvitation),
                        ),
                      ),
                  ],
                  if (album != null) ...[
                    const SizedBox(height: 16),
                    Text(l10n.albumMembers, style: Theme.of(context).textTheme.titleMedium),
                    for (final member in album.members.where((member) => !member.isOwner))
                      ListTile(
                        key: Key('s11-member-${member.accountId}'),
                        title: Text(member.label),
                        subtitle: album.successorAccountId == member.accountId ? Text(l10n.albumSuccessor) : null,
                        trailing: Wrap(
                          children: [
                            if (album.successorAccountId != member.accountId)
                              TextButton(
                                onPressed: offline || busy
                                    ? null
                                    : () => _then(act(() => albums.nameSuccessor(widget.albumId, member.accountId))),
                                child: Text(l10n.albumNameSuccessor),
                              ),
                            if (!handover.waits)
                              TextButton(
                                key: Key('s11-hand-over-${member.accountId}'),
                                onPressed: offline || busy ? null : () => _handOver(member),
                                child: Text(l10n.albumHandOver),
                              ),
                            TextButton(
                              onPressed: offline || busy
                                  ? null
                                  : () => _then(act(() => albums.removeParticipant(widget.albumId, member.accountId))),
                              child: Text(l10n.albumRemoveParticipant),
                            ),
                          ],
                        ),
                      ),
                    if (album.successorAccountId == null)
                      Text(l10n.albumNoSuccessor, key: const Key('s11-no-successor')),
                    ..._handoverBlock(album, handover),
                  ],
                  const SizedBox(height: 16),
                  Text(l10n.albumLink, style: Theme.of(context).textTheme.titleMedium),
                  if (link == null) ...[
                    Text(l10n.albumLinkNone, key: const Key('s11-no-link')),
                    OutlinedButton(onPressed: offline || busy ? null : _share, child: Text(l10n.albumShareByLink)),
                  ] else ...[
                    SelectableText(albumLinkAddress(link), key: const Key('s11-link')),
                    Wrap(
                      spacing: 8,
                      children: [
                        TextButton(onPressed: () => _copy(albumLinkAddress(link)), child: Text(l10n.albumCopyLink)),
                        TextButton.icon(
                          key: const Key('s11-qr'),
                          onPressed: () =>
                              showAlbumLinkQr(context, albumName: album?.name ?? '', address: albumLinkAddress(link)),
                          icon: const Icon(Icons.qr_code_2),
                          label: Text(l10n.albumShowQr),
                        ),
                      ],
                    ),
                    OutlinedButton(onPressed: offline || busy ? null : _revokeLink, child: Text(l10n.albumRevokeLink)),
                  ],
                  const SizedBox(height: 16),
                  Text(l10n.albumViewers, style: Theme.of(context).textTheme.titleMedium),
                  if (viewers.isEmpty) Text(l10n.albumViewersEmpty),
                  for (final viewer in viewers)
                    ListTile(
                      key: Key('s11-viewer-${viewer.accountId}'),
                      title: Text(viewer.label),
                      trailing: TextButton(
                        onPressed: offline || busy ? null : () => _revokeAccess(viewer.accountId),
                        child: Text(l10n.albumRevokeAccess),
                      ),
                    ),
                  if (blocked.isNotEmpty) ...[
                    const SizedBox(height: 16),
                    Text(l10n.albumBlockedPeople, style: Theme.of(context).textTheme.titleMedium),
                    for (final person in blocked)
                      ListTile(
                        key: Key('s11-blocked-${person.accountId}'),
                        title: Text(person.label),
                        trailing: TextButton(
                          onPressed: offline || busy ? null : () => _unblock(person.accountId),
                          child: Text(l10n.albumUnblock),
                        ),
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
