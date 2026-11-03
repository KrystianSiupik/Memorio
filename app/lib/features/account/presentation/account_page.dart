import 'package:flutter/material.dart';
import 'package:flutter_riverpod/flutter_riverpod.dart';

import '../../../core/l10n/l10n.dart';
import '../../../core/offline/offline.dart';
import '../../../core/result.dart';
import '../domain/account_repository.dart';
import 'session_providers.dart';

/// S24 Account and privacy: what goes with the account, deleting it and changing one's mind.
class AccountPage extends ConsumerStatefulWidget {
  const AccountPage({super.key});

  @override
  ConsumerState<AccountPage> createState() => _AccountPageState();
}

class _AccountPageState extends ConsumerState<AccountPage> {
  var _understood = false;
  var _consentsToAlbumExpiry = false;
  var _busy = false;
  AccountFailure? _failure;
  Future<Result<void, AccountFailure>> Function()? _lastAction;

  Future<void> _run(Future<Result<void, AccountFailure>> Function() action) async {
    setState(() {
      _busy = true;
      _failure = null;
      _lastAction = action;
    });
    final result = await action();
    if (!mounted) {
      return;
    }
    setState(() {
      _busy = false;
      switch (result) {
        case Ok():
          _understood = false;
          _consentsToAlbumExpiry = false;
          _lastAction = null;
        case Err(:final failure):
          // The choices stay: a refusal never costs what the person already marked.
          _failure = failure;
      }
    });
    ref.invalidate(accountOverviewProvider);
  }

  void _requestDeletion() {
    final consents = _consentsToAlbumExpiry;
    final account = ref.read(accountRepositoryProvider);
    _run(() => account.requestDeletion(consentsToAlbumExpiry: consents));
  }

  void _cancelDeletion() {
    final account = ref.read(accountRepositoryProvider);
    _run(account.cancelDeletion);
  }

  Future<void> _signOut() async {
    await ref.read(authRepositoryProvider).signOut();
    if (mounted) {
      ref.invalidate(sessionProvider);
    }
  }

  @override
  Widget build(BuildContext context) {
    final overview = ref.watch(accountOverviewProvider);
    final offline = ref.watch(isOnlineProvider).value == false;
    final loaded = overview.value;

    return Scaffold(
      appBar: AppBar(
        title: Text(context.l10n.accountTitle),
        // In the bar: available also when the data did not load, and without a connection.
        actions: [
          IconButton(tooltip: context.l10n.signOut, icon: const Icon(Icons.logout), onPressed: _busy ? null : _signOut),
        ],
      ),
      body: Column(
        children: [
          if (offline)
            Padding(
              padding: const EdgeInsets.all(12),
              child: Text(context.l10n.noConnection, key: const Key('s24-offline')),
            ),
          if (_busy) const LinearProgressIndicator(key: Key('s24-busy')),
          Expanded(
            child: switch (loaded) {
              final AccountOverview data => _content(data, offline),
              null when overview.hasError => _loadError(overview.error, offline),
              null => const _Skeleton(),
            },
          ),
        ],
      ),
    );
  }

  Widget _loadError(Object? error, bool offline) => Center(
    child: Column(
      mainAxisSize: MainAxisSize.min,
      children: [
        Text(_messageOf(context.l10n, error), key: const Key('s24-error')),
        TextButton(onPressed: () => ref.invalidate(accountOverviewProvider), child: Text(context.l10n.retry)),
      ],
    ),
  );

  Widget _content(AccountOverview overview, bool offline) {
    final impact = overview.impact;
    final windowEnd = overview.deletionWindowEndsAt;
    final active = !offline && !_busy;
    final withoutSuccessor = impact.ownAlbumsWithoutSuccessor;
    final canDelete = active && _understood && (withoutSuccessor == 0 || _consentsToAlbumExpiry);
    final failure = _failure;
    final lastAction = _lastAction;
    final l10n = context.l10n;

    return ListView(
      padding: const EdgeInsets.all(16),
      children: [
        Text(l10n.deletionEffects, style: Theme.of(context).textTheme.titleMedium),
        const SizedBox(height: 8),
        Text(l10n.effectMemories(impact.memories)),
        Text(l10n.effectPhotos(impact.photos)),
        Text(l10n.effectOwnAlbums(impact.ownAlbums, withoutSuccessor)),
        Text(l10n.effectOtherAlbums(impact.otherPeoplesAlbums)),
        const SizedBox(height: 24),
        Text(l10n.legacyContact, style: Theme.of(context).textTheme.titleMedium),
        const SizedBox(height: 8),
        Text(l10n.legacyContactNone),
        // Comes with succession (MEM-E9-SUKCESJA-01).
        OutlinedButton(onPressed: null, child: Text(l10n.legacyContactChoose)),
        const SizedBox(height: 24),
        if (failure != null) ...[
          Text(
            _messageOf(l10n, failure),
            key: const Key('s24-error'),
            style: TextStyle(color: Theme.of(context).colorScheme.error),
          ),
          if (lastAction != null && failure is! DeletionAlreadyDecided)
            TextButton(onPressed: active ? () => _run(lastAction) : null, child: Text(context.l10n.retry)),
          const SizedBox(height: 12),
        ],
        if (windowEnd != null) ...[
          Text(l10n.deletionDate(_dateOf(windowEnd)), key: const Key('s24-window')),
          const SizedBox(height: 8),
          FilledButton(onPressed: active ? _cancelDeletion : null, child: Text(l10n.cancelDeletion)),
        ] else ...[
          CheckboxListTile(
            title: Text(l10n.understandDeletion),
            value: _understood,
            onChanged: active ? (value) => setState(() => _understood = value ?? false) : null,
          ),
          if (withoutSuccessor > 0) ...[
            Text(l10n.albumsWithoutSuccessor),
            CheckboxListTile(
              title: Text(l10n.consentAlbumExpiry),
              value: _consentsToAlbumExpiry,
              onChanged: active ? (value) => setState(() => _consentsToAlbumExpiry = value ?? false) : null,
            ),
          ],
          Text(l10n.deletionWindowInfo),
          const SizedBox(height: 8),
          FilledButton(onPressed: canDelete ? _requestDeletion : null, child: Text(l10n.deleteAccount)),
        ],
      ],
    );
  }

  static String _dateOf(DateTime date) {
    final local = date.toLocal();
    String twoDigits(int value) => value.toString().padLeft(2, '0');
    return '${twoDigits(local.day)}.${twoDigits(local.month)}.${local.year}';
  }

  static String _messageOf(AppLocalizations l10n, Object? failure) => switch (failure) {
    AccountOffline() => l10n.authOffline,
    AccountForbidden() => l10n.noAccess,
    DeletionAlreadyDecided() => l10n.alreadyDecidedAccount,
    AlbumWithoutSuccessor() => l10n.albumWithoutSuccessorError,
    _ => l10n.serverError,
  };
}

/// S24 "loading": the shape of the screen, with no actions.
class _Skeleton extends StatelessWidget {
  const _Skeleton();

  @override
  Widget build(BuildContext context) {
    final color = Theme.of(context).colorScheme.surfaceContainerHighest;
    return ListView(
      key: const Key('s24-skeleton'),
      padding: const EdgeInsets.all(16),
      children: [
        for (var line = 0; line < 6; line++)
          Container(height: 16, margin: const EdgeInsets.only(bottom: 12), color: color),
      ],
    );
  }
}
