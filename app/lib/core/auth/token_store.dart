import 'dart:convert';

import 'package:flutter_riverpod/flutter_riverpod.dart';
import 'package:flutter_secure_storage/flutter_secure_storage.dart';

/// What a signed-in device keeps (MEM-E0-PLAT-07): the backend's bearer tokens and whose they are,
/// so the session is known offline too.
final class StoredTokens {
  const StoredTokens({required this.accountId, required this.accessToken, required this.refreshToken});

  final String accountId;
  final String accessToken;
  final String refreshToken;

  StoredTokens withTokens({required String accessToken, required String refreshToken}) =>
      StoredTokens(accountId: accountId, accessToken: accessToken, refreshToken: refreshToken);
}

abstract interface class TokenStore {
  Future<StoredTokens?> read();

  Future<void> write(StoredTokens tokens);

  Future<void> clear();
}

/// Overridden in app/ with [SecureTokenStore] and in tests with a fake.
final tokenStoreProvider = Provider<TokenStore>(
  (ref) => throw UnimplementedError('tokenStoreProvider is overridden in app/'),
);

/// Tokens in the platform's secure storage (Keychain, Android Keystore; on the web encrypted browser
/// storage, see D-151).
class SecureTokenStore implements TokenStore {
  SecureTokenStore([FlutterSecureStorage storage = const FlutterSecureStorage()]) : _storage = storage;

  static const _key = 'memorio.session';

  final FlutterSecureStorage _storage;

  @override
  Future<StoredTokens?> read() async {
    final value = await _storage.read(key: _key);
    if (value == null) {
      return null;
    }
    try {
      final json = jsonDecode(value) as Map<String, Object?>;
      return StoredTokens(
        accountId: json['accountId']! as String,
        accessToken: json['accessToken']! as String,
        refreshToken: json['refreshToken']! as String,
      );
    } on Object {
      // Unreadable (older format, damaged): treat as signed out rather than fail the app start.
      await clear();
      return null;
    }
  }

  @override
  Future<void> write(StoredTokens tokens) => _storage.write(
    key: _key,
    value: jsonEncode({
      'accountId': tokens.accountId,
      'accessToken': tokens.accessToken,
      'refreshToken': tokens.refreshToken,
    }),
  );

  @override
  Future<void> clear() => _storage.delete(key: _key);
}
