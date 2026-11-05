import 'package:dio/dio.dart';

import '../../../core/api/generated/clients/albums_client.dart';
import '../../../core/api/generated/clients/memories_client.dart';
import '../../../core/api/generated/clients/plans_client.dart';
import '../../../core/api/generated/models/keep_material_request.dart';
import '../../../core/api/generated/models/plan_response.dart';
import '../../../core/result.dart';
import '../domain/plan.dart';

/// [PlansRepository] on API v1. The contract describes numbers as "integer or string", so the generated models
/// have them dynamic; the backend always sends numbers (D-212).
class HttpPlansRepository implements PlansRepository {
  const HttpPlansRepository({required this.plans, required this.memories, required this.albums});

  final PlansClient plans;
  final MemoriesClient memories;
  final AlbumsClient albums;

  @override
  Future<Result<PlanState, PlanFailure>> load(String? albumId) => _attempt(() async {
    return _stateOf(albumId == null ? await plans.getPlan() : await plans.getAlbumPlan(id: albumId));
  });

  @override
  Future<Result<PlanState, PlanFailure>> refresh(String? albumId) => _attempt(() async {
    return _stateOf(albumId == null ? await plans.refreshPlan() : await plans.refreshAlbumPlan(id: albumId));
  });

  @override
  Future<Result<List<TrimMaterial>, PlanFailure>> loadMaterial(String? albumId) => _attempt(() async {
    if (albumId == null) {
      return [
        for (final item in await memories.getTrimMaterial())
          TrimMaterial(
            memoryId: item.memoryId,
            placeName: item.placeName,
            dateFrom: item.dateFrom,
            sizeBytes: item.sizeBytes as int,
          ),
      ];
    }
    return [
      for (final item in await albums.getAlbumTrimMaterial(id: albumId))
        TrimMaterial(
          memoryId: item.memoryId,
          placeName: item.placeName,
          dateFrom: item.dateFrom,
          sizeBytes: item.sizeBytes as int,
        ),
    ];
  });

  @override
  Future<Result<void, PlanFailure>> keep(String? albumId, List<String> memoryIds) => _attempt(() async {
    final body = KeepMaterialRequest(memoryIds: memoryIds);
    if (albumId == null) {
      await plans.keepMaterial(body: body);
    } else {
      await plans.keepAlbumMaterial(id: albumId, body: body);
    }
  });

  static PlanState _stateOf(PlanResponse plan) => PlanState(
    mode: switch (plan.mode) {
      'Active' => PlanMode.active,
      'ReadOnly' => PlanMode.readOnly,
      'Trimmed' => PlanMode.trimmed,
      _ => PlanMode.free,
    },
    entitledUntil: plan.entitledUntil,
    readOnlySince: plan.readOnlySince,
    trimAt: plan.trimAt,
    choiceOpensAt: plan.choiceOpensAt,
    isChoiceOpen: plan.isChoiceOpen,
    isHeld: plan.isHeld,
    lastCheckedAt: plan.lastCheckedAt,
    keptMemoryIds: plan.keptMemoryIds,
    usedBytes: plan.usedBytes as int,
    limitBytes: plan.limitBytes as int,
    freeLimitBytes: plan.freeLimitBytes as int,
    aiUsed: plan.aiUsed as int,
    aiLimit: plan.aiLimit as int,
  );

  Future<Result<T, PlanFailure>> _attempt<T>(Future<T> Function() call) async {
    try {
      return Ok(await call());
    } on DioException catch (error) {
      return Err(_failureOf(error));
    }
  }

  static PlanFailure _failureOf(DioException error) {
    final response = error.response;
    if (response == null) {
      return const PlanOffline();
    }
    final body = response.data;
    final code = body is Map ? body['code'] : null;
    return switch (code) {
      'Plans.ChoiceClosed' => const PlanChoiceClosed(),
      'Plans.AlreadyDecided' => const PlanAlreadyDecided(),
      _ => switch (response.statusCode) {
        403 || 404 => const PlanNoAccess(),
        409 => const PlanAlreadyDecided(),
        _ => const PlanServerError(),
      },
    };
  }
}
