import 'package:flutter_riverpod/flutter_riverpod.dart';

import '../../../core/api/api.dart';
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

/// The current session: loading while it is restored, then the session or null (signed out). Restored
/// again when the backend ends the session; screens invalidate it after signing in or out.
final sessionProvider = FutureProvider<Session?>((ref) {
  ref.watch(sessionEndedProvider);
  return ref.watch(sessionRepositoryProvider).restore();
});
