import 'package:dio/dio.dart';

import '../../../core/api/generated/clients/ai_client.dart';
import '../../../core/api/generated/clients/memories_client.dart';
import '../../../core/api/generated/models/add_photo_request.dart';
import '../../../core/api/generated/models/approve_description_request.dart';
import '../../../core/api/generated/models/create_memory_request.dart';
import '../../../core/api/generated/models/request_ai_draft_request.dart';
import '../../../core/api/generated/models/request_data_deletion_request.dart';
import '../../../core/api/generated/models/start_upload_request.dart';
import '../../../core/result.dart';
import '../domain/memories_repository.dart';
import '../domain/memory.dart';
import '../domain/memory_failure.dart';

/// [MemoriesRepository] on API v1. The contract describes numbers as "integer or string", so the
/// generated models have them dynamic; the backend always sends numbers (D-212).
class HttpMemoriesRepository implements MemoriesRepository {
  const HttpMemoriesRepository({required this.memories, required this.ai, required this.uploads});

  final MemoriesClient memories;
  final AiClient ai;

  /// Sends files straight to the storage: its address is signed, so no sign-in and no API headers.
  final Dio uploads;

  @override
  Future<Result<List<MemoryPin>, MemoryFailure>> loadPins({String? from, String? to, bool withPhotos = false}) =>
      _attempt(() async {
        final points = await memories.getMemoryMap(from: from, to: to, photos: withPhotos ? true : null);
        return [
          for (final point in points)
            MemoryPin(
              id: point.id,
              latitude: (point.latitude as num).toDouble(),
              longitude: (point.longitude as num).toDouble(),
              photoUrl: point.photoUrl,
            ),
        ];
      });

  @override
  Future<Result<void, MemoryFailure>> create(NewMemory memory) => _attempt(
    () => memories.createMemory(
      body: CreateMemoryRequest(
        id: memory.id,
        placeReference: memory.placeReference,
        datePrecision: memory.precision.apiName,
        dateFrom: memory.from,
        dateTo: memory.to,
        time: memory.time,
      ),
    ),
  );

  @override
  Future<Result<void, MemoryFailure>> addPhoto(String memoryId, String photoId, PickedPhoto photo) =>
      _attempt(() async {
        final upload = await memories.startPhotoUpload(body: StartUploadRequest(contentType: photo.contentType));
        await uploads.put<void>(
          upload.url,
          data: await photo.read(),
          options: Options(contentType: upload.contentType),
        );
        await memories.addPhoto(
          id: memoryId,
          body: AddPhotoRequest(photoId: photoId, uploadKey: upload.key),
        );
      });

  @override
  Future<Result<void, MemoryFailure>> approveDescription(String memoryId, String text, String? aiDraftId) => _attempt(
    () => memories.approveDescription(
      id: memoryId,
      body: ApproveDescriptionRequest(text: text, aiDraftId: aiDraftId),
    ),
  );

  @override
  Future<Result<AiDraft, MemoryFailure>> requestAiDraft({
    required String id,
    required String material,
    required bool consentsToAi,
    required List<String> placeNames,
  }) => _attempt(() async {
    final draft = await ai.requestAiDraft(
      body: RequestAiDraftRequest(
        id: id,
        material: material,
        consentsToAi: consentsToAi,
        peopleNames: const [],
        placeNames: placeNames,
      ),
    );
    return AiDraft(id: draft.id, text: draft.text);
  });

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

  @override
  Future<Result<MemoryDetails, MemoryFailure>> loadDetails(String id) => _attempt(() async {
    final memory = await memories.getMemory(id: id);
    return MemoryDetails(
      id: memory.id,
      placeName: memory.placeName,
      dateFrom: memory.dateFrom,
      dateTo: memory.dateTo,
      time: memory.time,
      isMine: memory.isMine,
      description: memory.description,
      descriptionIsAi: memory.descriptionIsAi,
      photoUrls: [for (final photo in memory.photos) photo.url],
    );
  });

  @override
  Future<Result<MemoryPreview, MemoryFailure>> loadPreview(String id) => _attempt(() async {
    final preview = await memories.getMemoryPreview(id: id);
    return MemoryPreview(
      id: preview.id,
      placeName: preview.placeName,
      datePrecision: preview.datePrecision,
      dateFrom: preview.dateFrom,
      dateTo: preview.dateTo,
      time: preview.time,
      excerpt: preview.excerpt,
      photos: preview.photos as int,
      photoUrl: preview.photoUrl,
    );
  });

  @override
  Future<Result<void, MemoryFailure>> moveToTrash(String id) => _attempt(() => memories.trashMemory(id: id));

  @override
  Future<Result<void, MemoryFailure>> deleteForGood(String id) =>
      _attempt(() => memories.requestDataDeletion(id: id, body: const RequestDataDeletionRequest(confirmed: true)));

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
