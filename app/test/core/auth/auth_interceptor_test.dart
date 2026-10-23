import 'package:dio/dio.dart';
import 'package:flutter_riverpod/flutter_riverpod.dart';
import 'package:flutter_test/flutter_test.dart';
import 'package:memorio/core/api/api.dart';
import 'package:memorio/core/auth/token_store.dart';

import '../../helpers/fake_http.dart';
import '../../helpers/fake_token_store.dart';

const _signedIn = StoredTokens(accountId: 'account-1', accessToken: 'access-1', refreshToken: 'refresh-1');

void main() {
  late InMemoryTokenStore store;
  late ProviderContainer container;

  /// The real Dio of the app (both clients: API and refresh) answering from [handler].
  FakeHttpAdapter wire(FakeHandler handler) {
    final adapter = FakeHttpAdapter(handler);
    container = ProviderContainer(
      overrides: [
        appVersionProvider.overrideWithValue('1.0.0'),
        apiBaseUrlProvider.overrideWithValue('https://api.example.invalid'),
        tokenStoreProvider.overrideWithValue(store),
      ],
    );
    addTearDown(container.dispose);
    container.read(dioProvider).httpClientAdapter = adapter;
    container.read(refreshDioProvider).httpClientAdapter = adapter;
    return adapter;
  }

  setUp(() => store = InMemoryTokenStore(_signedIn));

  test('requests carry the access token, sign-in requests do not', () async {
    final adapter = wire(
      (_) => (200, {'accountId': 'account-1', 'email': null, 'accessToken': 'x', 'refreshToken': 'y'}),
    );

    await container.read(apiClientProvider).account.getMe();
    await container.read(dioProvider).post<Object?>('/v1/auth/login', data: {'email': 'a@b.c', 'password': 'p'});

    expect(adapter.requests[0].headers['Authorization'], 'Bearer access-1');
    expect(adapter.requests[1].headers.containsKey('Authorization'), isFalse);
  });

  test('a 401 renews the tokens once and repeats the request with the new token', () async {
    final adapter = wire((request) {
      if (request.path == '/v1/auth/refresh') {
        return (
          200,
          {'tokenType': 'Bearer', 'accessToken': 'access-2', 'expiresIn': 3600, 'refreshToken': 'refresh-2'},
        );
      }
      return request.headers['Authorization'] == 'Bearer access-2'
          ? (200, {'accountId': 'account-1', 'email': null})
          : (401, null);
    });

    final me = await container.read(apiClientProvider).account.getMe();

    expect(me.accountId, 'account-1');
    expect(adapter.requests.map((request) => request.path), ['/v1/account/me', '/v1/auth/refresh', '/v1/account/me']);
    expect(adapter.requests[1].data, {'refreshToken': 'refresh-1'});
    expect(store.tokens?.accessToken, 'access-2');
    expect(store.tokens?.refreshToken, 'refresh-2');
    expect(store.tokens?.accountId, 'account-1');
  });

  test('a refused refresh token signs the device out', () async {
    wire((_) => (401, {'code': 'Account.InvalidCredentials'}));

    await expectLater(container.read(apiClientProvider).account.getMe(), throwsA(isA<DioException>()));

    expect(store.tokens, isNull);
    expect(container.read(sessionEndedProvider), 1);
  });

  test('offline during the refresh keeps the session', () async {
    wire((request) {
      if (request.path == '/v1/auth/refresh') {
        throw DioException.connectionError(requestOptions: request, reason: 'offline');
      }
      return (401, null);
    });

    await expectLater(container.read(apiClientProvider).account.getMe(), throwsA(isA<DioException>()));

    expect(store.tokens?.accessToken, 'access-1');
    expect(container.read(sessionEndedProvider), 0);
  });

  test('requests failing together share one refresh', () async {
    final adapter = wire((request) {
      if (request.path == '/v1/auth/refresh') {
        return (
          200,
          {'tokenType': 'Bearer', 'accessToken': 'access-2', 'expiresIn': 3600, 'refreshToken': 'refresh-2'},
        );
      }
      return request.headers['Authorization'] == 'Bearer access-2'
          ? (200, {'accountId': 'account-1', 'email': null})
          : (401, null);
    });
    final account = container.read(apiClientProvider).account;

    await Future.wait([account.getMe(), account.getMe(), account.getMe()]);

    expect(adapter.requests.where((request) => request.path == '/v1/auth/refresh'), hasLength(1));
  });
}
