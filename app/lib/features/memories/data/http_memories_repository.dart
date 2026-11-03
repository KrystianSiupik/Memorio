import 'package:dio/dio.dart';

import '../../../core/api/generated/clients/memories_client.dart';
import '../../../core/result.dart';
import '../domain/memories_repository.dart';
import '../domain/memory.dart';
import '../domain/memory_failure.dart';

/// [MemoriesRepository] on API v1. The contract describes numbers as "integer or string", so the
/// generated models have them dynamic; the backend always sends numbers (D-212).
class HttpMemoriesRepository implements MemoriesRepository {
  const HttpMemoriesRepository({required this.memories});

  final MemoriesClient memories;

  @override
  Future<Result<List<TrashedMemory>, MemoryFailure>> loadTrash() => _attempt(() async {
    final items = await memories.getTrash();
    return [
      for (final item in items)
        TrashedMemory(
          id: item.id,
          placeName: item.placeName,
          dateFrom: item.dateFrom,
          description: item.description,
          photos: item.photos as int,
          trashedAt: item.trashedAt,
          daysLeft: item.daysLeft as int,
        ),
    ];
  });

  @override
  Future<Result<void, MemoryFailure>> restore(String id) => _attempt(() => memories.restoreMemory(id: id));

  static Future<Result<T, MemoryFailure>> _attempt<T>(Future<T> Function() request) async {
    try {
      return Ok(await request());
    } on DioException catch (error) {
      return Err(memoryFailureOf(error));
    }
  }
}

/// What the backend's answer means for a screen.
MemoryFailure memoryFailureOf(DioException error) {
  final response = error.response;
  if (response == null) {
    return const MemoryOffline();
  }
  final body = response.data;
  final code = body is Map ? body['code'] as String? : null;
  return switch ((response.statusCode, code)) {
    (401 || 403, _) => const MemoryNoAccess(),
    (_, 'Archive.AlreadyDecided') => const MemoryAlreadyDecided(),
    (_, 'Archive.TermsNotAccepted') => const MemoryTermsNotAccepted(),
    (_, 'Archive.DateInFuture') => const MemoryDateInFuture(),
    (_, 'Archive.PlaceNotFound' || 'Places.NotFound') => const MemoryPlaceMissing(),
    (_, 'Archive.SpaceLimitExceeded') => const MemorySpaceFull(),
    (_, 'AiNarration.LimitReached') => const AiLimitReached(),
    (_, 'AiNarration.ProviderUnavailable') => const AiProviderDown(),
    (400, final String refused) => MemoryInvalidInput(refused),
    _ => const MemoryServerError(),
  };
}
