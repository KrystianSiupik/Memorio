import 'package:flutter/foundation.dart';
import 'package:flutter/material.dart';
import 'package:flutter_riverpod/flutter_riverpod.dart';

import '../../../core/l10n/l10n.dart';
import '../../../core/offline/offline.dart';
import '../../../core/result.dart';
import '../../account/account.dart';
import '../../memories/memories.dart' show noAutomaticRetry;
import '../domain/album.dart';
import '../domain/album_failure.dart';
import '../domain/albums_repository.dart';

/// Overridden in app/ with the API repository, and in tests with a fake.
final albumsRepositoryProvider = Provider<AlbumsRepository>(
  (ref) => throw UnimplementedError('albumsRepositoryProvider is overridden in app/'),
);

/// A read of the signed-in person: follows the account, so the next person never sees what the previous one
/// loaded, and stays failed until they ask again.
Future<T> _load<T>(Ref ref, Future<Result<T, AlbumFailure>> Function(AlbumsRepository albums) load) async {
  ref.watch(sessionProvider.select((session) => session.value?.accountId));
  return switch (await load(ref.watch(albumsRepositoryProvider))) {
    Ok(:final value) => value,
    Err(:final failure) => throw failure,
  };
}

final albumsProvider = FutureProvider<List<AlbumSummary>>(
  (ref) => _load(ref, (albums) => albums.loadAlbums()),
  retry: noAutomaticRetry,
);
final sharedAlbumsProvider = FutureProvider<List<SharedAlbum>>(
  (ref) => _load(ref, (albums) => albums.loadShared()),
  retry: noAutomaticRetry,
);
final invitationsProvider = FutureProvider<List<AlbumInvitation>>(
  (ref) => _load(ref, (albums) => albums.loadInvitations()),
  retry: noAutomaticRetry,
);
final keptContributionsProvider = FutureProvider<List<KeptContribution>>(
  (ref) => _load(ref, (albums) => albums.loadKept()),
  retry: noAutomaticRetry,
);
final entrustedProvider = FutureProvider<List<EntrustedMemory>>(
  (ref) => _load(ref, (albums) => albums.loadEntrusted()),
  retry: noAutomaticRetry,
);
final ownMemoriesProvider = FutureProvider<List<OwnMemory>>(
  (ref) => _load(ref, (albums) => albums.loadOwnMemories()),
  retry: noAutomaticRetry,
);
final albumDetailsProvider = FutureProvider.family<AlbumDetails, String>(
  (ref, id) => _load(ref, (albums) => albums.loadAlbum(id)),
  retry: noAutomaticRetry,
);
final albumSeatsProvider = FutureProvider.family<AlbumSeats, String>(
  (ref, id) => _load(ref, (albums) => albums.loadSeats(id)),
  retry: noAutomaticRetry,
);

final albumViewersProvider = FutureProvider.family<List<AlbumViewer>, String>(
  (ref, id) => _load(ref, (albums) => albums.loadViewers(id)),
  retry: noAutomaticRetry,
);
final albumLinkProvider = FutureProvider.family<String?, String>(
  (ref, id) => _load(ref, (albums) => albums.loadLink(id)),
  retry: noAutomaticRetry,
);
final albumLinkPreviewProvider = FutureProvider.family<AlbumLinkPreview, String>(
  (ref, token) => _load(ref, (albums) => albums.previewLink(token)),
  retry: noAutomaticRetry,
);

/// The address of the app, set at build time: --dart-define=APP_URL=https://...; in a browser its own.
const _appUrl = String.fromEnvironment('APP_URL');

/// What the owner sends to people. Without a known address, only the path (D-273).
String albumLinkAddress(String token) {
  final base = _appUrl.isNotEmpty ? _appUrl : (kIsWeb ? Uri.base.origin : '');
  return '$base/albums/link/$token';
}

/// A failure in words, the same on every screen of this feature.
String albumFailureText(AppLocalizations l10n, Object? failure) => switch (failure) {
  AlbumOffline() => l10n.authOffline,
  AlbumNoAccess() => l10n.albumNoAccess,
  AlbumBlocked() => l10n.albumBlocked,
  AlbumLinkGone() => l10n.albumLinkGone,
  AlbumOnlyOwnerBlocks() => l10n.albumOnlyOwnerBlocks,
  AlbumAlreadyDecided() => l10n.alreadyDecided,
  AlbumNoFreeSeats() => l10n.albumNoFreeSeats,
  AlbumInvitationLimit() => l10n.albumInvitationLimit,
  AlbumAddressBlocked() => l10n.albumAddressBlocked,
  AlbumInvitationGone() => l10n.albumInvitationGone,
  AlbumPoolFull() => l10n.albumPoolFull,
  AlbumOwnerCannotLeave() => l10n.albumOwnerCannotLeave,
  AlbumInvalidInput() => l10n.invalidInput,
  _ => l10n.serverError,
};

/// How much space, in words a person reads.
String bytesText(int bytes) {
  const gigabyte = 1024 * 1024 * 1024;
  const megabyte = 1024 * 1024;
  return bytes >= gigabyte
      ? '${(bytes / gigabyte).toStringAsFixed(1)} GB'
      : '${(bytes / megabyte).toStringAsFixed(0)} MB';
}

/// One action at a time, its failure kept on the screen: what the person typed stays where it was.
mixin AlbumActions<T extends ConsumerStatefulWidget> on ConsumerState<T> {
  var busy = false;
  AlbumFailure? failure;

  bool get offline => ref.watch(isOnlineProvider).value == false;

  AlbumsRepository get albums => ref.read(albumsRepositoryProvider);

  /// True when it worked.
  Future<bool> act(Future<Result<void, AlbumFailure>> Function() action) async {
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
    if (failure case final AlbumFailure refused)
      Padding(
        padding: const EdgeInsets.symmetric(vertical: 8),
        child: Text(
          albumFailureText(context.l10n, refused),
          key: const Key('album-error'),
          style: TextStyle(color: Theme.of(context).colorScheme.error),
        ),
      ),
  ];

  List<Widget> offlineBlock(BuildContext context) => [
    if (offline)
      Padding(
        padding: const EdgeInsets.all(12),
        child: Text(context.l10n.noConnection, key: const Key('album-offline')),
      ),
  ];
}

/// Loading, a failed load with its retry, or the content; the last answer stays while it reloads and offline.
class AlbumAsyncBody<T> extends StatelessWidget {
  const AlbumAsyncBody({super.key, required this.value, required this.onRetry, required this.builder});

  final AsyncValue<T> value;
  final VoidCallback onRetry;
  final Widget Function(T value) builder;

  @override
  Widget build(BuildContext context) {
    final l10n = context.l10n;
    final known = value.value;
    if (known != null) {
      return builder(known);
    }
    if (value.hasError) {
      return Center(
        child: Column(
          mainAxisSize: MainAxisSize.min,
          children: [
            Padding(
              padding: const EdgeInsets.all(16),
              child: Text(
                albumFailureText(l10n, value.error),
                key: const Key('album-load-error'),
                textAlign: TextAlign.center,
              ),
            ),
            if (value.error is! AlbumNoAccess && value.error is! AlbumBlocked && value.error is! AlbumLinkGone)
              TextButton(onPressed: onRetry, child: Text(l10n.retry)),
          ],
        ),
      );
    }
    final color = Theme.of(context).colorScheme.surfaceContainerHighest;
    return ListView(
      key: const Key('album-skeleton'),
      padding: const EdgeInsets.all(16),
      children: [
        for (var line = 0; line < 4; line++)
          Container(height: 48, margin: const EdgeInsets.only(bottom: 12), color: color),
      ],
    );
  }
}
