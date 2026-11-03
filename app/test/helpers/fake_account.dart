import 'package:memorio/core/result.dart';
import 'package:memorio/features/account/account.dart';

/// Account answers from memory; tests set the next answers.
class FakeAccountRepository implements AccountRepository {
  AccountOverview overview = const AccountOverview(
    impact: DeletionImpact(memories: 3, photos: 12, ownAlbums: 1, ownAlbumsWithoutSuccessor: 0, otherPeoplesAlbums: 2),
  );

  AccountFailure? loadFailure;
  AccountFailure? actionFailure;
  var termsAccepted = 0;
  final deletionRequests = <bool>[];
  var cancellations = 0;

  @override
  Future<Result<void, AccountFailure>> acceptCurrentTerms() async {
    termsAccepted++;
    return const Ok(null);
  }

  @override
  Future<Result<AccountOverview, AccountFailure>> loadOverview() async {
    final failure = loadFailure;
    return failure == null ? Ok(overview) : Err(failure);
  }

  @override
  Future<Result<void, AccountFailure>> requestDeletion({required bool consentsToAlbumExpiry}) async {
    final failure = actionFailure;
    if (failure != null) {
      return Err(failure);
    }
    deletionRequests.add(consentsToAlbumExpiry);
    overview = AccountOverview(impact: overview.impact, deletionWindowEndsAt: DateTime.utc(2026, 12, 3));
    return const Ok(null);
  }

  @override
  Future<Result<void, AccountFailure>> cancelDeletion() async {
    final failure = actionFailure;
    if (failure != null) {
      return Err(failure);
    }
    cancellations++;
    overview = AccountOverview(impact: overview.impact);
    return const Ok(null);
  }
}

/// Registering and signing in from memory; what the screen sent is kept for the test.
class FakeAuthRepository implements AuthRepository {
  FakeAuthRepository({this.sessions});

  /// Becomes signed in on a success, as the real repository stores the tokens.
  final MutableSessionRepository? sessions;

  AuthFailure? failure;
  final registered = <({String email, String password, bool declaresMinimumAge})>[];
  final signedIn = <({String email, String password})>[];

  Result<Session, AuthFailure> _answer() {
    final refusal = failure;
    if (refusal != null) {
      return Err(refusal);
    }
    const session = Session(accountId: 'account-1');
    sessions?.session = session;
    return const Ok(session);
  }

  @override
  Future<Result<Session, AuthFailure>> register({
    required String email,
    required String password,
    required bool declaresMinimumAge,
  }) async {
    registered.add((email: email, password: password, declaresMinimumAge: declaresMinimumAge));
    return _answer();
  }

  @override
  Future<Result<Session, AuthFailure>> signIn({required String email, required String password}) async {
    signedIn.add((email: email, password: password));
    return _answer();
  }

  @override
  Future<Result<Session, AuthFailure>> signInWithProvider({
    required SignInProvider provider,
    required String idToken,
    required bool declaresMinimumAge,
  }) => throw UnimplementedError();

  @override
  Future<void> signOut() async {}
}

class MutableSessionRepository implements SessionRepository {
  Session? session;

  @override
  Future<Session?> restore() async => session;
}
