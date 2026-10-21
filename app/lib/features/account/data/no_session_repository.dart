import '../domain/session.dart';

/// Stand-in until sign-in exists (MEM-E0-PLAT-07, MEM-E1-PLAT-01): nobody is ever signed in.
class NoSessionRepository implements SessionRepository {
  const NoSessionRepository();

  @override
  Future<Session?> restore() async => null;
}
