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
