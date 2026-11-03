import '../../../core/result.dart';
import 'memory.dart';
import 'memory_failure.dart';

/// Places for a memory, API v1.
abstract interface class PlacesRepository {
  Future<Result<List<PlaceOption>, MemoryFailure>> suggestions(String text);

  /// Checks the place a person chose.
  Future<Result<PlaceOption, MemoryFailure>> choose(String reference);

  /// [id] comes from the client, so a retry is the same place.
  Future<Result<PlaceOption, MemoryFailure>> addOwn({
    required String id,
    required String name,
    required double latitude,
    required double longitude,
  });
}

/// Choosing photos on the device.
abstract interface class PhotoPicker {
  Future<List<PickedPhoto>> pick();
}

/// Memories on the private map, API v1. Ids come from the client, so a repeated call has one effect.
abstract interface class MemoriesRepository {
  Future<Result<List<TrashedMemory>, MemoryFailure>> loadTrash();

  Future<Result<void, MemoryFailure>> restore(String id);

  /// The points of the map; the dates (yyyy-MM-dd) filter by overlap.
  Future<Result<List<MemoryPin>, MemoryFailure>> loadPins({String? from, String? to});

  Future<Result<void, MemoryFailure>> create(NewMemory memory);

  /// Sends the file and adds it to the memory; [photoId] makes a retry the same photo.
  Future<Result<void, MemoryFailure>> addPhoto(String memoryId, String photoId, PickedPhoto photo);

  /// [aiDraftId] names the AI draft the text started from, for its marker.
  Future<Result<void, MemoryFailure>> approveDescription(String memoryId, String text, String? aiDraftId);

  /// [id] makes a retry the same draft; [placeNames] never reach the AI provider.
  Future<Result<AiDraft, MemoryFailure>> requestAiDraft({
    required String id,
    required String material,
    required bool consentsToAi,
    required List<String> placeNames,
  });

  Future<Result<MemoryDetails, MemoryFailure>> loadDetails(String id);

  /// To the trash for 30 days.
  Future<Result<void, MemoryFailure>> moveToTrash(String id);

  /// The formal request: gone for good at once.
  Future<Result<void, MemoryFailure>> deleteForGood(String id);
}
