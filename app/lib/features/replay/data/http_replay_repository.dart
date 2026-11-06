import 'package:dio/dio.dart';

import '../../../core/api/generated/clients/albums_client.dart';
import '../../../core/api/generated/clients/memories_client.dart';
import '../../../core/api/generated/models/create_album_from_period_request.dart';
import '../../../core/result.dart';
import '../domain/replay.dart';

/// [ReplayRepository] on API v1. Numbers are "integer or string" in the contract, so the generated models have them
/// dynamic; the backend always sends numbers (D-212).
class HttpReplayRepository implements ReplayRepository {
  const HttpReplayRepository({required this.memories, required this.albums});

  final MemoriesClient memories;
  final AlbumsClient albums;

  @override
  Future<Result<List<PeriodMemory>, ReplayFailure>> loadPeriod(String from, String to) => _attempt(() async {
    return [
      for (final memory in await memories.getPeriod(from: from, to: to))
        PeriodMemory(
          id: memory.id,
          placeName: memory.placeName,
          latitude: (memory.latitude as num).toDouble(),
          longitude: (memory.longitude as num).toDouble(),
          datePrecision: memory.datePrecision,
          dateFrom: memory.dateFrom,
          dateTo: memory.dateTo,
          time: memory.time,
          description: memory.description,
          isMine: memory.isMine,
          photos: memory.photos as int,
          photoUrls: memory.photoUrls,
        ),
    ];
  });

  @override
  Future<Result<PeriodAlbum, ReplayFailure>> createAlbum(String id, String name, String from, String to) =>
      _attempt(() async {
        final created = await albums.createAlbumFromPeriod(
          body: CreateAlbumFromPeriodRequest(id: id, name: name, from: from, to: to),
        );
        return PeriodAlbum(added: created.added as int, notFitting: created.notFitting as int);
      });

  Future<Result<T, ReplayFailure>> _attempt<T>(Future<T> Function() call) async {
    try {
      return Ok(await call());
    } on DioException catch (error) {
      final response = error.response;
      if (response == null) {
        return const Err(ReplayOffline());
      }
      final body = response.data;
      final code = body is Map ? body['code'] : null;
      return Err(switch (code) {
        'Album.PeriodTooLong' => const ReplayTooLong(),
        _ => switch (response.statusCode) {
          403 || 404 => const ReplayNoAccess(),
          400 => const ReplayInvalidInput(),
          _ => const ReplayServerError(),
        },
      });
    }
  }
}
