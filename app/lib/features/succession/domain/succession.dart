import '../../../core/result.dart';
import '../../memories/memories.dart' show PickedPhoto;

/// R17: what the successor decides about.
class TakeoverOffer {
  const TakeoverOffer({
    required this.id,
    required this.albumId,
    required this.albumName,
    required this.answerBy,
    required this.founderMemories,
  });

  final String id;
  final String albumId;
  final String albumName;
  final DateTime answerBy;

  /// What the founder brought: it stays in the album.
  final int founderMemories;
}

/// R20.
class AlbumTakeover {
  const AlbumTakeover({required this.canReserve, required this.reservedByMe, this.reservedUntil});

  final bool canReserve;
  final bool reservedByMe;
  final DateTime? reservedUntil;
}

/// Someone who named this person and has died.
class LegacyArchive {
  const LegacyArchive({required this.ownerAccountId, this.email});

  final String ownerAccountId;
  final String? email;
}

class LegacyMemory {
  const LegacyMemory({required this.id, required this.placeName, required this.dateFrom});

  final String id;
  final String placeName;
  final String dateFrom;
}

/// Why an operation did not work; the screens say it in words.
sealed class SuccessionFailure implements Exception {
  const SuccessionFailure();
}

final class SuccessionOffline extends SuccessionFailure {
  const SuccessionOffline();
}

final class SuccessionNoAccess extends SuccessionFailure {
  const SuccessionNoAccess();
}

/// "You are not the successor named."
final class SuccessionNotSuccessor extends SuccessionFailure {
  const SuccessionNotSuccessor();
}

final class SuccessionAlreadyDecided extends SuccessionFailure {
  const SuccessionAlreadyDecided();
}

/// A report about this album is already being handled.
final class SuccessionAlreadyReported extends SuccessionFailure {
  const SuccessionAlreadyReported();
}

/// Someone else holds the album right now.
final class SuccessionReservedByOther extends SuccessionFailure {
  const SuccessionReservedByOther();
}

final class SuccessionInvalidEmail extends SuccessionFailure {
  const SuccessionInvalidEmail();
}

final class SuccessionServerError extends SuccessionFailure {
  const SuccessionServerError();
}

abstract interface class SuccessionRepository {
  Future<Result<List<TakeoverOffer>, SuccessionFailure>> loadOffers();

  Future<Result<TakeoverOffer, SuccessionFailure>> loadOffer(String id);

  Future<Result<void, SuccessionFailure>> answer(String id, {required bool accepted});

  /// The id comes from the caller, so sending again is the same report.
  Future<Result<void, SuccessionFailure>> report(String reportId, String albumId, PickedPhoto certificate);

  Future<Result<AlbumTakeover, SuccessionFailure>> loadTakeover(String albumId);

  Future<Result<void, SuccessionFailure>> reserve(String albumId);

  Future<Result<String?, SuccessionFailure>> loadContact();

  Future<Result<void, SuccessionFailure>> nameContact(String email);

  Future<Result<void, SuccessionFailure>> removeContact();

  Future<Result<List<LegacyArchive>, SuccessionFailure>> loadArchives();

  Future<Result<List<LegacyMemory>, SuccessionFailure>> loadArchive(String ownerAccountId);
}
