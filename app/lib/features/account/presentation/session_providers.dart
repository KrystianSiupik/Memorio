import 'package:flutter_riverpod/flutter_riverpod.dart';

import '../domain/session.dart';

/// Overridden in app/ (composition root) with the real repository, and in tests with a fake.
final sessionRepositoryProvider = Provider<SessionRepository>(
  (ref) => throw UnimplementedError('sessionRepositoryProvider is overridden in app/'),
);

/// The current session: loading while it is restored, then the session or null (signed out).
final sessionProvider = FutureProvider<Session?>((ref) => ref.watch(sessionRepositoryProvider).restore());
