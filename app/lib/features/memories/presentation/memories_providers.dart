import 'package:flutter_riverpod/flutter_riverpod.dart';

import '../../../core/result.dart';
import '../../../core/settings/settings.dart';
import '../../account/account.dart';
import '../domain/memories_repository.dart';
import '../domain/memory.dart';

/// A failed load stays failed until the person asks again ("Ponów"): Riverpod would otherwise keep retrying.
Duration? noAutomaticRetry(int retryCount, Object error) => null;

/// Overridden in app/ with the API repository, and in tests with a fake.
final memoriesRepositoryProvider = Provider<MemoriesRepository>(
  (ref) => throw UnimplementedError('memoriesRepositoryProvider is overridden in app/'),
);

/// Overridden in app/ with the API repository, and in tests with a fake.
final placesRepositoryProvider = Provider<PlacesRepository>(
  (ref) => throw UnimplementedError('placesRepositoryProvider is overridden in app/'),
);

/// Overridden in app/ with the system's gallery, and in tests with a fake.
final photoPickerProvider = Provider<PhotoPicker>(
  (ref) => throw UnimplementedError('photoPickerProvider is overridden in app/'),
);

/// Places chosen in this session: what S04 suggests without a connection.
final recentPlacesProvider = NotifierProvider<RecentPlacesNotifier, List<PlaceOption>>(RecentPlacesNotifier.new);

class RecentPlacesNotifier extends Notifier<List<PlaceOption>> {
  static const _kept = 10;

  @override
  List<PlaceOption> build() {
    ref.watch(sessionProvider.select((session) => session.value?.accountId));
    return const [];
  }

  void remember(PlaceOption place) {
    state = [place, ...state.where((known) => known.reference != place.reference)].take(_kept).toList();
  }
}

/// The filter of the map: a range of days, or everything.
final memoryDateFilterProvider = NotifierProvider<MemoryDateFilterNotifier, ({DateTime from, DateTime to})?>(
  MemoryDateFilterNotifier.new,
);

class MemoryDateFilterNotifier extends Notifier<({DateTime from, DateTime to})?> {
  @override
  ({DateTime from, DateTime to})? build() {
    ref.watch(sessionProvider.select((session) => session.value?.accountId));
    return null;
  }

  void set(({DateTime from, DateTime to})? range) => state = range;
}

/// The points of the map. Follows the signed-in account, so the next person never sees those of the previous one.
final memoryPinsProvider = FutureProvider<List<MemoryPin>>((ref) async {
  final accountId = ref.watch(sessionProvider.select((session) => session.value?.accountId));
  if (accountId == null) {
    return const [];
  }
  final range = ref.watch(memoryDateFilterProvider);
  String day(DateTime date) =>
      '${date.year.toString().padLeft(4, '0')}-${date.month.toString().padLeft(2, '0')}-${date.day.toString().padLeft(2, '0')}';
  final loaded = await ref
      .watch(memoriesRepositoryProvider)
      .loadPins(
        from: range == null ? null : day(range.from),
        to: range == null ? null : day(range.to),
        withPhotos: ref.watch(settingsProvider.select((settings) => settings.photoPoints)),
      );
  return switch (loaded) {
    Ok(:final value) => value,
    Err(:final failure) => throw failure,
  };
}, retry: noAutomaticRetry);

/// What S05 shows. Kept for the session: without a connection the screen reads the last answer.
final memoryDetailsProvider = FutureProvider.family<MemoryDetails, String>((ref, id) async {
  ref.watch(sessionProvider.select((session) => session.value?.accountId));
  final loaded = await ref.watch(memoriesRepositoryProvider).loadDetails(id);
  return switch (loaded) {
    Ok(:final value) => value,
    Err(:final failure) => throw failure,
  };
}, retry: noAutomaticRetry);

/// MEM-E12-PLAT-01: kept while the map is open, so moving over the same point again asks nothing.
final memoryPreviewProvider = FutureProvider.family<MemoryPreview, String>((ref, id) async {
  ref.watch(sessionProvider.select((session) => session.value?.accountId));
  final loaded = await ref.watch(memoriesRepositoryProvider).loadPreview(id);
  return switch (loaded) {
    Ok(:final value) => value,
    Err(:final failure) => throw failure,
  };
}, retry: noAutomaticRetry);

/// What S06 shows. Kept for the session: without a connection the screen reads the last answer.
final trashProvider = FutureProvider<List<TrashedMemory>>((ref) async {
  ref.watch(sessionProvider.select((session) => session.value?.accountId));
  final loaded = await ref.watch(memoriesRepositoryProvider).loadTrash();
  return switch (loaded) {
    Ok(:final value) => value,
    Err(:final failure) => throw failure,
  };
}, retry: noAutomaticRetry);
