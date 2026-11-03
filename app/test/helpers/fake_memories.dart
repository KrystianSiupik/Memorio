import 'package:memorio/core/result.dart';
import 'package:memorio/features/memories/memories.dart';

/// Memories from memory; tests set the next answers.
class FakeMemoriesRepository implements MemoriesRepository {
  List<TrashedMemory> trash = [];
  MemoryFailure? loadFailure;
  MemoryFailure? actionFailure;
  final restored = <String>[];

  @override
  Future<Result<List<TrashedMemory>, MemoryFailure>> loadTrash() async {
    final failure = loadFailure;
    return failure == null ? Ok(trash) : Err(failure);
  }

  MemoryDetails? details;
  final trashed = <String>[];
  final deleted = <String>[];

  Future<Result<void, MemoryFailure>> _act(List<String> into, String id) async {
    final failure = actionFailure;
    if (failure != null) {
      return Err(failure);
    }
    into.add(id);
    return const Ok(null);
  }

  @override
  Future<Result<MemoryDetails, MemoryFailure>> loadDetails(String id) async {
    final failure = loadFailure;
    return failure == null ? Ok(details!) : Err(failure);
  }

  @override
  Future<Result<void, MemoryFailure>> moveToTrash(String id) => _act(trashed, id);

  @override
  Future<Result<void, MemoryFailure>> deleteForGood(String id) => _act(deleted, id);

  @override
  Future<Result<void, MemoryFailure>> restore(String id) async {
    final failure = actionFailure;
    if (failure != null) {
      return Err(failure);
    }
    restored.add(id);
    trash = [...trash.where((item) => item.id != id)];
    return const Ok(null);
  }
}
