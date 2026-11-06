import 'package:dio/dio.dart';
import 'package:flutter_riverpod/misc.dart';

import '../core/api/api.dart';
import '../core/auth/token_store.dart';
import '../core/offline/offline.dart';
import '../features/account/account.dart';
import '../features/albums/albums.dart';
import '../features/map/map.dart';
import '../features/memories/memories.dart';
import '../features/plans/plans.dart';
import '../features/replay/replay.dart';
import '../features/moderation/moderation.dart';
import '../features/succession/succession.dart';

/// Wires implementations to the interfaces features depend on. Tests build their own overrides.
List<Override> appOverrides({required String appVersion}) => [
  appVersionProvider.overrideWithValue(appVersion),
  mapViewProvider.overrideWithValue(const MapLibreMapView()),
  tokenStoreProvider.overrideWithValue(SecureTokenStore()),
  sessionRepositoryProvider.overrideWith((ref) => TokenSessionRepository(ref.watch(tokenStoreProvider))),
  authRepositoryProvider.overrideWith((ref) {
    final api = ref.watch(apiClientProvider);
    return HttpAuthRepository(auth: api.auth, account: api.account, store: ref.watch(tokenStoreProvider));
  }),
  memoriesRepositoryProvider.overrideWith((ref) {
    final api = ref.watch(apiClientProvider);
    return HttpMemoriesRepository(memories: api.memories, ai: api.ai, uploads: Dio());
  }),
  placesRepositoryProvider.overrideWith((ref) => HttpPlacesRepository(places: ref.watch(apiClientProvider).places)),
  albumsRepositoryProvider.overrideWith((ref) {
    final api = ref.watch(apiClientProvider);
    return HttpAlbumsRepository(
      albums: api.albums,
      access: api.access,
      notifications: api.notifications,
      memories: api.memories,
      uploads: Dio(),
    );
  }),
  successionRepositoryProvider.overrideWith((ref) {
    final api = ref.watch(apiClientProvider);
    return HttpSuccessionRepository(
      succession: api.succession,
      albums: api.albums,
      access: api.access,
      memories: api.memories,
      uploads: Dio(),
    );
  }),
  moderationRepositoryProvider.overrideWith(
    (ref) => HttpModerationRepository(moderation: ref.watch(apiClientProvider).moderation),
  ),
  replayRepositoryProvider.overrideWith((ref) {
    final api = ref.watch(apiClientProvider);
    return HttpReplayRepository(memories: api.memories, albums: api.albums);
  }),
  plansRepositoryProvider.overrideWith((ref) {
    final api = ref.watch(apiClientProvider);
    return HttpPlansRepository(plans: api.plans, memories: api.memories, albums: api.albums);
  }),
  photoPickerProvider.overrideWithValue(const ImagePhotoPicker()),
  operationSendersProvider.overrideWith(
    (ref) => memorySenders(ref.watch(memoriesRepositoryProvider), ref.watch(placesRepositoryProvider)),
  ),
  accountRepositoryProvider.overrideWith((ref) => HttpAccountRepository(account: ref.watch(apiClientProvider).account)),
];
