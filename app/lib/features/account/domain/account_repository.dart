import '../../../core/result.dart';

/// In numbers, what goes when the account goes (R2).
class DeletionImpact {
  const DeletionImpact({
    required this.memories,
    required this.photos,
    required this.ownAlbums,
    required this.ownAlbumsWithoutSuccessor,
    required this.otherPeoplesAlbums,
  });

  final int memories;
  final int photos;
  final int ownAlbums;
  final int ownAlbumsWithoutSuccessor;
  final int otherPeoplesAlbums;
}

/// What S24 shows: the effects of deleting and the open window, if any.
class AccountOverview {
  const AccountOverview({required this.impact, this.deletionWindowEndsAt});

  final DeletionImpact impact;
  final DateTime? deletionWindowEndsAt;
}

/// Why an account operation did not work; the screens say it in words.
sealed class AccountFailure {
  const AccountFailure();
}

final class AccountOffline extends AccountFailure {
  const AccountOffline();
}

/// The session is no longer allowed to do this.
final class AccountForbidden extends AccountFailure {
  const AccountForbidden();
}

/// The window already has its result: the account went or the time is over.
final class DeletionAlreadyDecided extends AccountFailure {
  const DeletionAlreadyDecided();
}

/// Albums without a successor hold the deletion back until the person chooses.
final class AlbumWithoutSuccessor extends AccountFailure {
  const AlbumWithoutSuccessor();
}

final class AccountServerError extends AccountFailure {
  const AccountServerError();
}

/// The signed-in person's own account (E1).
abstract interface class AccountRepository {
  /// Accepts the version of the terms the backend currently offers.
  Future<Result<void, AccountFailure>> acceptCurrentTerms();

  Future<Result<AccountOverview, AccountFailure>> loadOverview();

  Future<Result<void, AccountFailure>> requestDeletion({required bool consentsToAlbumExpiry});

  Future<Result<void, AccountFailure>> cancelDeletion();
}
