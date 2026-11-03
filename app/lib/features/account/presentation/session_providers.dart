import 'package:flutter_riverpod/flutter_riverpod.dart';

import '../../../core/api/api.dart';
import '../../../core/result.dart';
import '../domain/account_repository.dart';
import '../domain/auth_repository.dart';
import '../domain/session.dart';

/// Overridden in app/ (composition root) with the real repository, and in tests with a fake.
final sessionRepositoryProvider = Provider<SessionRepository>(
  (ref) => throw UnimplementedError('sessionRepositoryProvider is overridden in app/'),
);

/// Overridden in app/ with the API repository, and in tests with a fake.
final authRepositoryProvider = Provider<AuthRepository>(
  (ref) => throw UnimplementedError('authRepositoryProvider is overridden in app/'),
);

/// Overridden in app/ with the API repository, and in tests with a fake.
final accountRepositoryProvider = Provider<AccountRepository>(
  (ref) => throw UnimplementedError('accountRepositoryProvider is overridden in app/'),
);

/// What S24 shows. Kept for the whole session: without a connection the screen reads the last answer.
/// Follows the signed-in account, so the next person never sees the numbers of the previous one.
final accountOverviewProvider = FutureProvider<AccountOverview>((ref) async {
  final accountId = ref.watch(sessionProvider.select((session) => session.value?.accountId));
  if (accountId == null) {
    throw const AccountForbidden();
  }
  final loaded = await ref.watch(accountRepositoryProvider).loadOverview();
  return switch (loaded) {
    Ok(:final value) => value,
    Err(:final failure) => throw failure,
  };
}, retry: (retryCount, error) => null);

/// The current session: loading while it is restored, then the session or null (signed out). Restored
/// again when the backend ends the session; screens invalidate it after signing in or out.
final sessionProvider = FutureProvider<Session?>((ref) {
  ref.watch(sessionEndedProvider);
  return ref.watch(sessionRepositoryProvider).restore();
});
