import '../../../core/result.dart';

/// One memory of a period (R7), in the order it is replayed.
class PeriodMemory {
  const PeriodMemory({
    required this.id,
    required this.placeName,
    required this.latitude,
    required this.longitude,
    required this.datePrecision,
    required this.dateFrom,
    required this.dateTo,
    this.description,
    required this.isMine,
    required this.photos,
    this.photoUrls = const [],
  });

  final String id;
  final String placeName;
  final double latitude;
  final double longitude;

  /// Day, Month, Year or Period: as exactly as the person remembers (S2:C1).
  final String datePrecision;
  final String dateFrom;
  final String dateTo;
  final String? description;
  final bool isMine;
  final int photos;
  final List<String> photoUrls;
}

/// What C71 put in the album.
class PeriodAlbum {
  const PeriodAlbum({required this.added, required this.notFitting});

  final int added;
  final int notFitting;
}

sealed class ReplayFailure implements Exception {
  const ReplayFailure();
}

final class ReplayOffline extends ReplayFailure {
  const ReplayOffline();
}

final class ReplayNoAccess extends ReplayFailure {
  const ReplayNoAccess();
}

final class ReplayTooLong extends ReplayFailure {
  const ReplayTooLong();
}

final class ReplayInvalidInput extends ReplayFailure {
  const ReplayInvalidInput();
}

final class ReplayServerError extends ReplayFailure {
  const ReplayServerError();
}

abstract interface class ReplayRepository {
  /// Days as yyyy-MM-dd, both in.
  Future<Result<List<PeriodMemory>, ReplayFailure>> loadPeriod(String from, String to);

  /// The id comes from the caller, so sending again is the same album.
  Future<Result<PeriodAlbum, ReplayFailure>> createAlbum(String id, String name, String from, String to);
}
