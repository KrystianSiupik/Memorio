import 'dart:async';

import 'package:dio/dio.dart';

import 'token_store.dart';

/// Signs requests with the stored access token and renews it once when the API answers 401
/// (MEM-E0-PLAT-07). Concurrent 401s share one refresh. When the refresh token is refused too, the
/// stored session is cleared and [onSignedOut] runs, so the app goes back to sign-in.
class AuthInterceptor extends QueuedInterceptor {
  AuthInterceptor({required this.store, required this.refresh, required this.retry, required this.onSignedOut});

  /// Paths that never carry a token (sign-in itself).
  static const anonymousPrefix = '/v1/auth/';

  static const _retriedKey = 'memorio.auth.retried';

  final TokenStore store;

  /// Exchanges a refresh token for new tokens; throws when the backend refuses it.
  final Future<({String accessToken, String refreshToken})> Function(String refreshToken) refresh;

  /// Sends a request again (with the renewed token).
  final Future<Response<Object?>> Function(RequestOptions request) retry;

  final void Function() onSignedOut;

  @override
  Future<void> onRequest(RequestOptions options, RequestInterceptorHandler handler) async {
    if (!options.path.startsWith(anonymousPrefix)) {
      final tokens = await store.read();
      if (tokens != null) {
        options.headers['Authorization'] = 'Bearer ${tokens.accessToken}';
      }
    }
    handler.next(options);
  }

  @override
  Future<void> onError(DioException err, ErrorInterceptorHandler handler) async {
    final request = err.requestOptions;
    if (err.response?.statusCode != 401 ||
        request.path.startsWith(anonymousPrefix) ||
        request.extra[_retriedKey] == true) {
      handler.next(err);
      return;
    }

    final tokens = await store.read();
    if (tokens == null) {
      handler.next(err);
      return;
    }

    // Queued: a request that failed while another one was refreshing finds the new token already.
    final sentWith = request.headers['Authorization'];
    var current = tokens;
    if (sentWith == 'Bearer ${tokens.accessToken}') {
      try {
        final renewed = await refresh(tokens.refreshToken);
        current = tokens.withTokens(accessToken: renewed.accessToken, refreshToken: renewed.refreshToken);
        await store.write(current);
      } on DioException catch (refreshError) {
        if (refreshError.response?.statusCode == 401) {
          await store.clear();
          onSignedOut();
        }
        handler.next(err);
        return;
      }
    }

    request.extra[_retriedKey] = true;
    request.headers['Authorization'] = 'Bearer ${current.accessToken}';
    try {
      handler.resolve(await retry(request));
    } on DioException catch (retryError) {
      handler.next(retryError);
    }
  }
}
