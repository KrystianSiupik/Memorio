import '../../../core/result.dart';

enum SummaryState { draft, saved, invalidated }

/// C82–C84: the AI's story of a period of an album, made only of the memories the person may use.
class PeriodSummary {
  const PeriodSummary({
    required this.id,
    required this.accountId,
    required this.albumId,
    required this.from,
    required this.to,
    required this.state,
    required this.sentences,
    required this.usedMemories,
    required this.leftOutMemories,
    required this.leftOutPeople,
    required this.droppedSentences,
    required this.addedToAlbum,
  });

  final String id;

  /// Who asked for it; only they save it and add it to the album.
  final String accountId;
  final String albumId;

  /// yyyy-MM-dd.
  final String from;
  final String to;
  final SummaryState state;

  /// Each one points to the memories it tells of; an invalidated summary has none.
  final List<String> sentences;
  final int usedMemories;
  final int leftOutMemories;
  final int leftOutPeople;
  final int droppedSentences;
  final bool addedToAlbum;

  String get text => sentences.join(' ');
}

/// Why an operation on summaries did not work; the screen says it in words.
sealed class SummaryFailure implements Exception {
  const SummaryFailure();
}

final class SummaryOffline extends SummaryFailure {
  const SummaryOffline();
}

/// Not a participant of the album, or somebody else's summary.
final class SummaryNoAccess extends SummaryFailure {
  const SummaryNoAccess();
}

final class SummaryConsentRequired extends SummaryFailure {
  const SummaryConsentRequired();
}

/// Nothing in the period may be used.
final class SummaryNoMaterials extends SummaryFailure {
  const SummaryNoMaterials();
}

final class SummaryLimitReached extends SummaryFailure {
  const SummaryLimitReached();
}

/// The AI wrote nothing the memories support.
final class SummaryNothingTold extends SummaryFailure {
  const SummaryNothingTold();
}

/// A memory it tells of may no longer be used: ask for a new one.
final class SummaryMaterialsChanged extends SummaryFailure {
  const SummaryMaterialsChanged();
}

final class SummaryInvalidated extends SummaryFailure {
  const SummaryInvalidated();
}

final class SummaryInvalidPeriod extends SummaryFailure {
  const SummaryInvalidPeriod();
}

final class SummaryAlreadyDecided extends SummaryFailure {
  const SummaryAlreadyDecided();
}

final class SummaryServerError extends SummaryFailure {
  const SummaryServerError();
}

abstract interface class SummariesRepository {
  /// Whether the person gave the AI consent for this album (C80).
  Future<Result<bool, SummaryFailure>> consent(String albumId);

  /// C80 or C81.
  Future<Result<void, SummaryFailure>> setConsent(String albumId, {required bool given});

  /// C82; [id] is the idempotency key: the same id again gets the same summary for one AI operation.
  Future<Result<PeriodSummary, SummaryFailure>> request(String id, String albumId, String from, String to);

  /// C83.
  Future<Result<PeriodSummary, SummaryFailure>> save(String id);

  /// C84.
  Future<Result<PeriodSummary, SummaryFailure>> addToAlbum(String id);

  /// The summaries added to the album that the person may read.
  Future<Result<List<PeriodSummary>, SummaryFailure>> inAlbum(String albumId);
}
