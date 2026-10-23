import 'package:dio/dio.dart';
import 'package:flutter_riverpod/flutter_riverpod.dart';
import 'package:flutter_test/flutter_test.dart';
import 'package:memorio/core/api/api.dart';
import 'package:memorio/core/auth/token_store.dart';
import 'package:memorio/core/result.dart';
import 'package:memorio/features/account/account.dart';

import '../../helpers/fake_http.dart';
import '../../helpers/fake_token_store.dart';

const _tokens = {'tokenType': 'Bearer', 'accessToken': 'access-1', 'expiresIn': 3600, 'refreshToken': 'refresh-1'};

void main() {
  late InMemoryTokenStore store;
  late FakeHttpAdapter adapter;
  late AuthRepository auth;

  void serve(FakeHandler handler) {
    store = InMemoryTokenStore();
    adapter = FakeHttpAdapter(handler);
    final container = ProviderContainer(
      overrides: [
        appVersionProvider.overrideWithValue('1.0.0'),
        apiBaseUrlProvider.overrideWithValue('https://api.example.invalid'),
        tokenStoreProvider.overrideWithValue(store),
      ],
    );
    addTearDown(container.dispose);
    container.read(dioProvider).httpClientAdapter = adapter;
    container.read(refreshDioProvider).httpClientAdapter = adapter;
    final api = container.read(apiClientProvider);
    auth = HttpAuthRepository(auth: api.auth, account: api.account, store: store);
  }

  (int, Object?) signedInServer(RequestOptions request) => switch (request.path) {
    '/v1/account/me' => (200, {'accountId': 'account-1', 'email': 'ola@example.com'}),
    _ => (200, _tokens),
  };

  test('signing in keeps the tokens with the account id and returns the session', () async {
    serve(signedInServer);

    final result = await auth.signIn(email: 'ola@example.com', password: 'long-enough-password');

    expect((result as Ok<Session, AuthFailure>).value.accountId, 'account-1');
    expect(store.tokens?.accountId, 'account-1');
    expect(store.tokens?.accessToken, 'access-1');
    expect(adapter.requests.last.headers['Authorization'], 'Bearer access-1');
  });

  test('registering sends the 16+ declaration', () async {
    serve(signedInServer);

    await auth.register(email: 'ola@example.com', password: 'long-enough-password', declaresMinimumAge: true);

    expect(adapter.requests.first.path, '/v1/auth/register');
    expect((adapter.requests.first.data as Map)['declaresMinimumAge'], isTrue);
  });

  test('a provider sign-in goes to the provider path with the ID token', () async {
    serve(signedInServer);

    await auth.signInWithProvider(provider: SignInProvider.apple, idToken: 'id-token', declaresMinimumAge: false);

    expect(adapter.requests.first.path, '/v1/auth/external/apple');
    expect((adapter.requests.first.data as Map)['idToken'], 'id-token');
  });

  for (final (code, failure) in [
    ('Account.InvalidCredentials', isA<InvalidCredentials>()),
    ('Account.AgeDeclarationRequired', isA<AgeDeclarationRequired>()),
    ('Account.EmailTaken', isA<EmailTaken>()),
    ('Account.InvalidPassword', isA<InvalidPassword>()),
    ('Account.InvalidExternalToken', isA<InvalidProviderToken>()),
    ('Account.ProviderUnavailable', isA<ProviderUnavailable>()),
    ('Something.Else', isA<AuthServerError>()),
  ]) {
    test('the API code $code becomes a typed failure and nothing is kept', () async {
      serve((_) => (code == 'Account.InvalidCredentials' ? 401 : 400, {'code': code}));

      final result = await auth.signIn(email: 'ola@example.com', password: 'whatever-password');

      expect((result as Err<Session, AuthFailure>).failure, failure);
      expect(store.tokens, isNull);
    });
  }

  test('no network is its own failure', () async {
    serve((request) => throw Exception('offline'));

    final result = await auth.signIn(email: 'ola@example.com', password: 'whatever-password');

    expect((result as Err<Session, AuthFailure>).failure, isA<AuthOffline>());
  });

  test('a failure after the tokens arrived leaves no half session behind', () async {
    serve((request) => request.path == '/v1/account/me' ? (500, null) : (200, _tokens));

    final result = await auth.signIn(email: 'ola@example.com', password: 'long-enough-password');

    expect((result as Err<Session, AuthFailure>).failure, isA<AuthServerError>());
    expect(store.tokens, isNull);
  });

  test('signing out forgets the tokens', () async {
    serve(signedInServer);
    await auth.signIn(email: 'ola@example.com', password: 'long-enough-password');

    await auth.signOut();

    expect(store.tokens, isNull);
  });

  test('the session is restored from the device, also offline', () async {
    final restored = await const TokenSessionRepository(
      _StaticStore(StoredTokens(accountId: 'account-1', accessToken: 'a', refreshToken: 'r')),
    ).restore();
    final inProgress = await const TokenSessionRepository(
      _StaticStore(StoredTokens(accountId: '', accessToken: 'a', refreshToken: 'r')),
    ).restore();

    expect(restored?.accountId, 'account-1');
    expect(inProgress, isNull);
  });
}

class _StaticStore implements TokenStore {
  const _StaticStore(this.tokens);

  final StoredTokens tokens;

  @override
  Future<StoredTokens?> read() async => tokens;

  @override
  Future<void> write(StoredTokens tokens) async {}

  @override
  Future<void> clear() async {}
}
