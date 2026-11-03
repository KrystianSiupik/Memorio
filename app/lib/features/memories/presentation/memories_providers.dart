import 'package:flutter_riverpod/flutter_riverpod.dart';

import '../../../core/result.dart';
import '../domain/memories_repository.dart';
import '../domain/memory.dart';

/// Overridden in app/ with the API repository, and in tests with a fake.
final memoriesRepositoryProvider = Provider<MemoriesRepository>(
  (ref) => throw UnimplementedError('memoriesRepositoryProvider is overridden in app/'),
);

/// What S06 shows. Kept for the session: without a connection the screen reads the last answer.
final trashProvider = FutureProvider<List<TrashedMemory>>((ref) async {
  final loaded = await ref.watch(memoriesRepositoryProvider).loadTrash();
  return switch (loaded) {
    Ok(:final value) => value,
    Err(:final failure) => throw failure,
  };
});
