import 'package:dio/dio.dart';
import 'package:flutter_riverpod/flutter_riverpod.dart';

import '../auth/auth_interceptor.dart';
import '../auth/token_store.dart';
import 'app_version_interceptor.dart';
import 'generated/clients/auth_client.dart';
import 'generated/models/refresh_request.dart';
import 'generated/memorio_api_client.dart';

/// Base URL of the backend, set at build time: `--dart-define=API_BASE_URL=https://...`.
const _apiBaseUrl = String.fromEnvironment('API_BASE_URL', defaultValue: 'http://localhost:5080');

final apiBaseUrlProvider = Provider<String>((ref) => _apiBaseUrl);

/// Version of the installed app ("major.minor.patch"), read once in `main()` and provided by override.
final appVersionProvider = Provider<String>(
  (ref) => throw UnimplementedError('appVersionProvider is overridden in main() and in tests'),
);

/// Minimum app version reported by the backend when this version is no longer served (HTTP 426),
/// or null while the app is supported.
final updateRequiredProvider = NotifierProvider<UpdateRequiredNotifier, String?>(UpdateRequiredNotifier.new);

class UpdateRequiredNotifier extends Notifier<String?> {
  @override
  String? build() => null;

  void require(String minimumVersion) => state = minimumVersion;
}

/// Counts sessions the backend ended (refresh token refused); the account feature restores its session
/// again when it changes, which signs the app out.
final sessionEndedProvider = NotifierProvider<SessionEndedNotifier, int>(SessionEndedNotifier.new);

class SessionEndedNotifier extends Notifier<int> {
  @override
  int build() => 0;

  void end() => state++;
}

Dio _newDio(Ref ref) {
  final dio = Dio(
    BaseOptions(
      baseUrl: ref.watch(apiBaseUrlProvider),
      connectTimeout: const Duration(seconds: 10),
      receiveTimeout: const Duration(seconds: 20),
    ),
  );
  dio.interceptors.add(
    AppVersionInterceptor(
      appVersion: ref.watch(appVersionProvider),
      onUpdateRequired: (minimumVersion) => ref.read(updateRequiredProvider.notifier).require(minimumVersion),
    ),
  );
  ref.onDispose(dio.close);
  return dio;
}

/// Client for renewing tokens only: without the auth interceptor, so a refused refresh never loops.
final refreshDioProvider = Provider<Dio>(_newDio);

final dioProvider = Provider<Dio>((ref) {
  final dio = _newDio(ref);
  final auth = AuthClient(ref.watch(refreshDioProvider));
  dio.interceptors.add(
    AuthInterceptor(
      store: ref.watch(tokenStoreProvider),
      refresh: (refreshToken) async {
        final tokens = await auth.refreshToken(body: RefreshRequest(refreshToken: refreshToken));
        return (accessToken: tokens.accessToken, refreshToken: tokens.refreshToken);
      },
      retry: dio.fetch,
      onSignedOut: () => ref.read(sessionEndedProvider.notifier).end(),
    ),
  );
  return dio;
});

/// Generated client of API v1 (`generated/`, from api/openapi.v1.json). Features use it in their data layer.
final apiClientProvider = Provider<MemorioApiClient>((ref) => MemorioApiClient(ref.watch(dioProvider)));

/// Asks the backend for its version policy once at start. The request carries the app version, so an
/// unsupported app gets 426 and [updateRequiredProvider] is set by the interceptor. Network errors are
/// ignored: the same check happens on every later request.
final appVersionCheckProvider = FutureProvider<void>((ref) async {
  try {
    await ref.watch(apiClientProvider).app.getAppVersionPolicy();
  } on DioException {
    // Offline or update required; see the comment above.
  }
});
