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

  List<MemoryPin> pins = [];
  final pinRequests = <({String? from, String? to})>[];
  MemoryDetails? details;
  final created = <NewMemory>[];
  final photos = <String, PickedPhoto>{};
  final approved = <({String text, String? aiDraftId})>[];
  final draftRequests = <String>[];
  MemoryFailure? createFailure;
  MemoryFailure? photoFailure;
  MemoryFailure? draftFailure;

  @override
  Future<Result<List<MemoryPin>, MemoryFailure>> loadPins({String? from, String? to}) async {
    pinRequests.add((from: from, to: to));
    final failure = loadFailure;
    return failure == null ? Ok(pins) : Err(failure);
  }

  @override
  Future<Result<void, MemoryFailure>> create(NewMemory memory) async {
    final failure = createFailure;
    if (failure != null) {
      return Err(failure);
    }
    created.add(memory);
    return const Ok(null);
  }

  @override
  Future<Result<void, MemoryFailure>> addPhoto(String memoryId, String photoId, PickedPhoto photo) async {
    final failure = photoFailure;
    if (failure != null) {
      return Err(failure);
    }
    photos[photoId] = photo;
    return const Ok(null);
  }

  @override
  Future<Result<void, MemoryFailure>> approveDescription(String memoryId, String text, String? aiDraftId) async {
    approved.add((text: text, aiDraftId: aiDraftId));
    return const Ok(null);
  }

  @override
  Future<Result<AiDraft, MemoryFailure>> requestAiDraft({
    required String id,
    required String material,
    required bool consentsToAi,
    required List<String> placeNames,
  }) async {
    draftRequests.add(id);
    final failure = draftFailure;
    return failure == null ? Ok(AiDraft(id: id, text: 'Szkic: $material')) : Err(failure);
  }

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

  /// Previews by memory id; a missing one is no access.
  final previews = <String, MemoryPreview>{};

  @override
  Future<Result<MemoryPreview, MemoryFailure>> loadPreview(String id) async {
    final preview = previews[id];
    return preview == null ? const Err(MemoryNoAccess()) : Ok(preview);
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
