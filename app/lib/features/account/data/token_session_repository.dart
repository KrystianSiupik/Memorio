import '../../../core/auth/token_store.dart';
import '../domain/session.dart';

/// The session kept on the device with its tokens: known offline too; the backend ends it by refusing
/// the refresh token (the interceptor then clears the store).
class TokenSessionRepository implements SessionRepository {
  const TokenSessionRepository(this._store);

  final TokenStore _store;

  @override
  Future<Session?> restore() async {
    final tokens = await _store.read();
    // An empty account id is a sign-in still in progress (tokens stored before GET /v1/account/me).
    return tokens == null || tokens.accountId.isEmpty ? null : Session(accountId: tokens.accountId);
  }
}
