import 'package:flutter/material.dart';
import 'package:flutter_riverpod/flutter_riverpod.dart';

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
        title: const Text('Konto i prywatność'),
        // In the bar: available also when the data did not load, and without a connection.
        actions: [IconButton(tooltip: 'Wyloguj', icon: const Icon(Icons.logout), onPressed: _busy ? null : _signOut)],
      ),
      body: Column(
        children: [
          if (offline)
            const Padding(
              padding: EdgeInsets.all(12),
              child: Text('Brak połączenia', key: Key('s24-offline')),
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
        Text(_messageOf(error), key: const Key('s24-error')),
        TextButton(onPressed: () => ref.invalidate(accountOverviewProvider), child: const Text('Ponów')),
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

    return ListView(
      padding: const EdgeInsets.all(16),
      children: [
        Text('Skutki usunięcia', style: Theme.of(context).textTheme.titleMedium),
        const SizedBox(height: 8),
        Text('Wspomnienia: ${impact.memories}'),
        Text('Zdjęcia: ${impact.photos}'),
        Text('Twoje albumy: ${impact.ownAlbums}, w tym bez następcy: $withoutSuccessor'),
        Text('Cudze albumy, w których uczestniczysz: ${impact.otherPeoplesAlbums}'),
        const SizedBox(height: 24),
        Text('Kontakt spadkowy', style: Theme.of(context).textTheme.titleMedium),
        const SizedBox(height: 8),
        const Text('Nie wskazano'),
        // Comes with succession (MEM-E9-SUKCESJA-01).
        const OutlinedButton(onPressed: null, child: Text('Wskaż kontakt spadkowy')),
        const SizedBox(height: 24),
        if (failure != null) ...[
          Text(
            _messageOf(failure),
            key: const Key('s24-error'),
            style: TextStyle(color: Theme.of(context).colorScheme.error),
          ),
          if (lastAction != null && failure is! DeletionAlreadyDecided)
            TextButton(onPressed: active ? () => _run(lastAction) : null, child: const Text('Ponów')),
          const SizedBox(height: 12),
        ],
        if (windowEnd != null) ...[
          Text('Konto zostanie usunięte ${_dateOf(windowEnd)}.', key: const Key('s24-window')),
          const SizedBox(height: 8),
          FilledButton(onPressed: active ? _cancelDeletion : null, child: const Text('Cofnij usunięcie')),
        ] else ...[
          CheckboxListTile(
            title: const Text('Rozumiem skutki usunięcia konta'),
            value: _understood,
            onChanged: active ? (value) => setState(() => _understood = value ?? false) : null,
          ),
          if (withoutSuccessor > 0) ...[
            const Text('Masz albumy bez następcy. Wskaż następcę w albumie albo zgódź się na ich wygaśnięcie.'),
            CheckboxListTile(
              title: const Text('Zgadzam się na wygaśnięcie albumów bez następcy'),
              value: _consentsToAlbumExpiry,
              onChanged: active ? (value) => setState(() => _consentsToAlbumExpiry = value ?? false) : null,
            ),
          ],
          const Text('Konto zniknie po 30 dniach. Do tego czasu możesz się wycofać.'),
          const SizedBox(height: 8),
          FilledButton(onPressed: canDelete ? _requestDeletion : null, child: const Text('Usuń konto')),
        ],
      ],
    );
  }

  static String _dateOf(DateTime date) {
    final local = date.toLocal();
    String twoDigits(int value) => value.toString().padLeft(2, '0');
    return '${twoDigits(local.day)}.${twoDigits(local.month)}.${local.year}';
  }

  static String _messageOf(Object? failure) => switch (failure) {
    AccountOffline() => 'Brak połączenia. Sprawdź internet i spróbuj ponownie.',
    AccountForbidden() => 'Brak dostępu. Zaloguj się ponownie.',
    DeletionAlreadyDecided() => 'To już rozstrzygnięte: okno usunięcia konta się zakończyło.',
    AlbumWithoutSuccessor() => 'Masz albumy bez następcy. Wskaż następcę albo zgódź się na ich wygaśnięcie.',
    _ => 'Błąd serwera. Spróbuj ponownie za chwilę.',
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
