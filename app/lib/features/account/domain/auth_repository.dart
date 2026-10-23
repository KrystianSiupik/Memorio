import '../../../core/result.dart';
import 'session.dart';

/// Sign-in providers whose native sign-in returns an ID token the backend checks (D-034).
enum SignInProvider { google, apple }

/// Why signing in or registering did not work; the screens (MEM-E1-PLAT-01) say it in words.
sealed class AuthFailure {
  const AuthFailure();
}

/// Wrong email or password, or a locked account: one answer for all, by design.
final class InvalidCredentials extends AuthFailure {
  const InvalidCredentials();
}

/// A new account needs the declaration of being at least 16; ask and try again.
final class AgeDeclarationRequired extends AuthFailure {
  const AgeDeclarationRequired();
}

/// An account with this email exists: sign in to it instead.
final class EmailTaken extends AuthFailure {
  const EmailTaken();
}

/// The password is shorter than the rules allow (10 characters).
final class InvalidPassword extends AuthFailure {
  const InvalidPassword();
}

final class InvalidEmail extends AuthFailure {
  const InvalidEmail();
}

/// The provider's sign-in was not accepted (expired or for another app).
final class InvalidProviderToken extends AuthFailure {
  const InvalidProviderToken();
}

/// Sign-in with this provider is not offered now.
final class ProviderUnavailable extends AuthFailure {
  const ProviderUnavailable();
}

final class AuthOffline extends AuthFailure {
  const AuthOffline();
}

final class AuthServerError extends AuthFailure {
  const AuthServerError();
}

/// Signing in, registering and signing out (MEM-E0-PLAT-07); a success keeps the session on the device.
abstract interface class AuthRepository {
  Future<Result<Session, AuthFailure>> signIn({required String email, required String password});

  Future<Result<Session, AuthFailure>> register({
    required String email,
    required String password,
    required bool declaresMinimumAge,
  });

  /// [idToken] comes from the provider's native sign-in; [declaresMinimumAge] matters only when this
  /// creates the account (otherwise [AgeDeclarationRequired]).
  Future<Result<Session, AuthFailure>> signInWithProvider({
    required SignInProvider provider,
    required String idToken,
    required bool declaresMinimumAge,
  });

  Future<void> signOut();
}
