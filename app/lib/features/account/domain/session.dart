/// A signed-in account on this device.
final class Session {
  const Session({required this.accountId});

  final String accountId;
}

/// Where the session lives (secure storage and the backend, MEM-E0-PLAT-07).
abstract interface class SessionRepository {
  /// The session kept on this device, or null when nobody is signed in.
  Future<Session?> restore();
}
