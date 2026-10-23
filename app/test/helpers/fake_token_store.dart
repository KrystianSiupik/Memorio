import 'package:memorio/core/auth/token_store.dart';

/// Token store in memory, starting with [tokens] (null: signed out).
class InMemoryTokenStore implements TokenStore {
  InMemoryTokenStore([this.tokens]);

  StoredTokens? tokens;

  @override
  Future<StoredTokens?> read() async => tokens;

  @override
  Future<void> write(StoredTokens tokens) async => this.tokens = tokens;

  @override
  Future<void> clear() async => tokens = null;
}
