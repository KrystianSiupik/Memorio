import 'package:dio/dio.dart';

import '../../../core/api/generated/clients/account_client.dart';
import '../../../core/api/generated/clients/auth_client.dart';
import '../../../core/api/generated/models/access_token_response.dart';
import '../../../core/api/generated/models/external_sign_in_request.dart';
import '../../../core/api/generated/models/login_request.dart';
import '../../../core/api/generated/models/register_request.dart';
import '../../../core/auth/token_store.dart';
import '../../../core/result.dart';
import '../domain/auth_repository.dart';
import '../domain/session.dart';

/// [AuthRepository] on API v1: the token response is stored with the account id from GET /v1/account/me.
class HttpAuthRepository implements AuthRepository {
  const HttpAuthRepository({required this.auth, required this.account, required this.store});

  final AuthClient auth;
  final AccountClient account;
  final TokenStore store;

  @override
  Future<Result<Session, AuthFailure>> signIn({required String email, required String password}) => _signIn(
    () => auth.login(
      body: LoginRequest(email: email, password: password),
    ),
  );

  @override
  Future<Result<Session, AuthFailure>> register({
    required String email,
    required String password,
    required bool declaresMinimumAge,
  }) => _signIn(
    () => auth.register(
      body: RegisterRequest(email: email, password: password, declaresMinimumAge: declaresMinimumAge),
    ),
  );

  @override
  Future<Result<Session, AuthFailure>> signInWithProvider({
    required SignInProvider provider,
    required String idToken,
    required bool declaresMinimumAge,
  }) => _signIn(
    () => auth.signInWithProvider(
      provider: provider.name,
      body: ExternalSignInRequest(idToken: idToken, declaresMinimumAge: declaresMinimumAge),
    ),
  );

  @override
  Future<void> signOut() => store.clear();

  Future<Result<Session, AuthFailure>> _signIn(Future<AccessTokenResponse> Function() request) async {
    try {
      final tokens = await request();
      // Store the tokens first: GET /v1/account/me is sent with them by the interceptor.
      await store.write(
        StoredTokens(accountId: '', accessToken: tokens.accessToken, refreshToken: tokens.refreshToken),
      );
      final me = await account.getMe();
      final session = StoredTokens(
        accountId: me.accountId,
        accessToken: tokens.accessToken,
        refreshToken: tokens.refreshToken,
      );
      await store.write(session);
      return Ok(Session(accountId: session.accountId));
    } on DioException catch (error) {
      await _discardIncompleteSession();
      return Err(_failureOf(error));
    }
  }

  Future<void> _discardIncompleteSession() async {
    final stored = await store.read();
    if (stored != null && stored.accountId.isEmpty) {
      await store.clear();
    }
  }

  static AuthFailure _failureOf(DioException error) {
    final response = error.response;
    if (response == null) {
      return const AuthOffline();
    }
    final body = response.data;
    final code = body is Map ? body['code'] : null;
    return switch (code) {
      'Account.InvalidCredentials' => const InvalidCredentials(),
      'Account.AgeDeclarationRequired' => const AgeDeclarationRequired(),
      'Account.EmailTaken' => const EmailTaken(),
      'Account.InvalidPassword' => const InvalidPassword(),
      'Account.InvalidEmail' => const InvalidEmail(),
      'Account.InvalidExternalToken' => const InvalidProviderToken(),
      'Account.ProviderUnavailable' => const ProviderUnavailable(),
      _ => const AuthServerError(),
    };
  }
}
