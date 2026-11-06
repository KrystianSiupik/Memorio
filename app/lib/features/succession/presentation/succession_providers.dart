import 'package:flutter/material.dart';
import 'package:flutter_riverpod/flutter_riverpod.dart';

import '../../../core/l10n/l10n.dart';
import '../../../core/offline/offline.dart';
import '../../../core/result.dart';
import '../../account/account.dart';
import '../../memories/memories.dart' show noAutomaticRetry;
import '../domain/succession.dart';

/// Overridden in app/ with the API repository, and in tests with a fake.
final successionRepositoryProvider = Provider<SuccessionRepository>(
  (ref) => throw UnimplementedError('successionRepositoryProvider is overridden in app/'),
);

/// A read of the signed-in person: follows the account, so the next person never sees what the previous one
/// loaded, and stays failed until they ask again.
Future<T> _load<T>(Ref ref, Future<Result<T, SuccessionFailure>> Function(SuccessionRepository succession) load) async {
  ref.watch(sessionProvider.select((session) => session.value?.accountId));
  return switch (await load(ref.watch(successionRepositoryProvider))) {
    Ok(:final value) => value,
    Err(:final failure) => throw failure,
  };
}

final takeoverOffersProvider = FutureProvider<List<TakeoverOffer>>(
  (ref) => _load(ref, (succession) => succession.loadOffers()),
  retry: noAutomaticRetry,
);
final takeoverOfferProvider = FutureProvider.family<TakeoverOffer, String>(
  (ref, id) => _load(ref, (succession) => succession.loadOffer(id)),
  retry: noAutomaticRetry,
);
final albumTakeoverProvider = FutureProvider.family<AlbumTakeover, String>(
  (ref, albumId) => _load(ref, (succession) => succession.loadTakeover(albumId)),
  retry: noAutomaticRetry,
);
final legacyContactProvider = FutureProvider<String?>(
  (ref) => _load(ref, (succession) => succession.loadContact()),
  retry: noAutomaticRetry,
);
final legacyArchivesProvider = FutureProvider<List<LegacyArchive>>(
  (ref) => _load(ref, (succession) => succession.loadArchives()),
  retry: noAutomaticRetry,
);
final legacyArchiveProvider = FutureProvider.family<List<LegacyMemory>, String>(
  (ref, owner) => _load(ref, (succession) => succession.loadArchive(owner)),
  retry: noAutomaticRetry,
);

/// A failure in words, the same on every screen of this feature.
String successionFailureText(AppLocalizations l10n, Object? failure) => switch (failure) {
  SuccessionOffline() => l10n.authOffline,
  SuccessionNoAccess() => l10n.albumNoAccess,
  SuccessionNotSuccessor() => l10n.takeoverNotSuccessor,
  SuccessionAlreadyDecided() => l10n.takeoverDecided,
  SuccessionAlreadyReported() => l10n.deathReportTaken,
  SuccessionReservedByOther() => l10n.takeoverReservedByOther,
  SuccessionInvalidEmail() => l10n.legacyContactInvalid,
  _ => l10n.serverError,
};

/// One action at a time, its failure kept on the screen: what the person typed stays where it was.
mixin SuccessionActions<T extends ConsumerStatefulWidget> on ConsumerState<T> {
  var busy = false;
  SuccessionFailure? failure;

  bool get offline => ref.watch(isOnlineProvider).value == false;

  SuccessionRepository get succession => ref.read(successionRepositoryProvider);

  /// True when it worked.
  Future<bool> act(Future<Result<void, SuccessionFailure>> Function() action) async {
    setState(() {
      busy = true;
      failure = null;
    });
    final result = await action();
    if (!mounted) {
      return false;
    }
    setState(() {
      busy = false;
      if (result case Err(failure: final refused)) {
        failure = refused;
      }
    });
    return result is Ok;
  }

  List<Widget> failureBlock(BuildContext context) => [
    if (failure case final SuccessionFailure refused)
      Padding(
        padding: const EdgeInsets.symmetric(vertical: 8),
        child: Text(
          successionFailureText(context.l10n, refused),
          key: const Key('succession-error'),
          style: TextStyle(color: Theme.of(context).colorScheme.error),
        ),
      ),
  ];

  List<Widget> offlineBlock(BuildContext context) => [
    if (offline)
      Padding(
        padding: const EdgeInsets.all(12),
        child: Text(context.l10n.noConnection, key: const Key('succession-offline')),
      ),
  ];
}

/// Loading, a failed load with its retry where it can help, or the content.
class SuccessionAsyncBody<T> extends StatelessWidget {
  const SuccessionAsyncBody({super.key, required this.value, required this.onRetry, required this.builder});

  final AsyncValue<T> value;
  final VoidCallback onRetry;
  final Widget Function(T value) builder;

  @override
  Widget build(BuildContext context) {
    final l10n = context.l10n;
    if (value.hasValue) {
      return builder(value.requireValue);
    }
    if (value.hasError) {
      final error = value.error;
      return Center(
        child: Column(
          mainAxisSize: MainAxisSize.min,
          children: [
            Padding(
              padding: const EdgeInsets.all(16),
              child: Text(
                successionFailureText(l10n, error),
                key: const Key('succession-load-error'),
                textAlign: TextAlign.center,
              ),
            ),
            if (error is! SuccessionNoAccess && error is! SuccessionNotSuccessor && error is! SuccessionAlreadyDecided)
              TextButton(onPressed: onRetry, child: Text(l10n.retry)),
          ],
        ),
      );
    }
    final color = Theme.of(context).colorScheme.surfaceContainerHighest;
    return ListView(
      key: const Key('succession-skeleton'),
      padding: const EdgeInsets.all(16),
      children: [
        for (var line = 0; line < 3; line++)
          Container(height: 48, margin: const EdgeInsets.only(bottom: 12), color: color),
      ],
    );
  }
}
