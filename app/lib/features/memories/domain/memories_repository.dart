import '../../../core/result.dart';
import 'memory.dart';
import 'memory_failure.dart';

/// Memories on the private map, API v1. Ids come from the client, so a repeated call has one effect.
abstract interface class MemoriesRepository {
  Future<Result<List<TrashedMemory>, MemoryFailure>> loadTrash();

  Future<Result<void, MemoryFailure>> restore(String id);

  Future<Result<MemoryDetails, MemoryFailure>> loadDetails(String id);

  /// To the trash for 30 days.
  Future<Result<void, MemoryFailure>> moveToTrash(String id);

  /// The formal request: gone for good at once.
  Future<Result<void, MemoryFailure>> deleteForGood(String id);
}
