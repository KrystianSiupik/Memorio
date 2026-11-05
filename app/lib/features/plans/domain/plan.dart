import '../../../core/result.dart';

enum PlanMode { free, active, readOnly, trimmed }

/// R16: a plan as whoever pays for it sees it.
class PlanState {
  const PlanState({
    required this.mode,
    this.entitledUntil,
    this.readOnlySince,
    this.trimAt,
    this.choiceOpensAt,
    this.isChoiceOpen = false,
    this.isHeld = false,
    this.lastCheckedAt,
    this.keptMemoryIds = const [],
    required this.usedBytes,
    required this.limitBytes,
    required this.freeLimitBytes,
    required this.aiUsed,
    required this.aiLimit,
  });

  final PlanMode mode;
  final DateTime? entitledUntil;
  final DateTime? readOnlySince;
  final DateTime? trimAt;
  final DateTime? choiceOpensAt;
  final bool isChoiceOpen;

  /// A warning did not reach someone: the trim waits (S2:C10).
  final bool isHeld;
  final DateTime? lastCheckedAt;
  final List<String> keptMemoryIds;
  final int usedBytes;
  final int limitBytes;
  final int freeLimitBytes;
  final int aiUsed;
  final int aiLimit;
}

/// What a trim could take; the oldest stays when nobody chooses.
class TrimMaterial {
  const TrimMaterial({
    required this.memoryId,
    required this.placeName,
    required this.dateFrom,
    required this.sizeBytes,
  });

  final String memoryId;
  final String placeName;
  final String dateFrom;
  final int sizeBytes;
}

/// Why an operation on a plan did not work; the screens say it in words.
sealed class PlanFailure implements Exception {
  const PlanFailure();
}

final class PlanOffline extends PlanFailure {
  const PlanOffline();
}

/// Only whoever pays for the plan sees it.
final class PlanNoAccess extends PlanFailure {
  const PlanNoAccess();
}

/// The window to choose what stays is not open.
final class PlanChoiceClosed extends PlanFailure {
  const PlanChoiceClosed();
}

final class PlanAlreadyDecided extends PlanFailure {
  const PlanAlreadyDecided();
}

final class PlanServerError extends PlanFailure {
  const PlanServerError();
}

/// The person's plan, or with an album id the plan of an album they own.
abstract interface class PlansRepository {
  Future<Result<PlanState, PlanFailure>> load(String? albumId);

  /// C96: asks the payment source now.
  Future<Result<PlanState, PlanFailure>> refresh(String? albumId);

  Future<Result<List<TrimMaterial>, PlanFailure>> loadMaterial(String? albumId);

  /// C94: the memories to keep first.
  Future<Result<void, PlanFailure>> keep(String? albumId, List<String> memoryIds);
}
