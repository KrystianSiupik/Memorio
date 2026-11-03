import 'package:flutter_riverpod/misc.dart';

import '../core/api/api.dart';
import '../core/auth/token_store.dart';
import '../features/account/account.dart';
import '../features/map/map.dart';

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
  accountRepositoryProvider.overrideWith((ref) => HttpAccountRepository(account: ref.watch(apiClientProvider).account)),
];
