import 'package:flutter/material.dart';
import 'package:flutter_riverpod/flutter_riverpod.dart';
import 'package:uuid/uuid.dart';

import '../../../core/l10n/l10n.dart';
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
