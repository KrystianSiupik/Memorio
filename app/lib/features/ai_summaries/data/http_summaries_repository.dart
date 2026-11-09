import 'package:dio/dio.dart';

import '../../../core/api/generated/clients/ai_client.dart';
import '../../../core/api/generated/models/ai_summary_response.dart';
import '../../../core/api/generated/models/request_ai_summary_request.dart';
import '../../../core/result.dart';
import '../domain/period_summary.dart';

/// [SummariesRepository] on API v1; numbers come as "integer or string" in the contract (D-212).
class HttpSummariesRepository implements SummariesRepository {
  const HttpSummariesRepository({required this.ai});

  final AiClient ai;

  @override
  Future<Result<bool, SummaryFailure>> consent(String albumId) =>
      _attempt(() async => (await ai.getAiConsent(albumId: albumId)).given);

  @override
  Future<Result<void, SummaryFailure>> setConsent(String albumId, {required bool given}) =>
      _attempt(() => given ? ai.giveAiConsent(albumId: albumId) : ai.revokeAiConsent(albumId: albumId));

  @override
  Future<Result<PeriodSummary, SummaryFailure>> request(String id, String albumId, String from, String to) => _attempt(
    () async => _summaryOf(
      await ai.requestAiSummary(
        body: RequestAiSummaryRequest(id: id, albumId: albumId, from: from, to: to),
      ),
    ),
  );

  @override
  Future<Result<PeriodSummary, SummaryFailure>> save(String id) =>
      _attempt(() async => _summaryOf(await ai.saveAiSummary(id: id)));

  @override
  Future<Result<PeriodSummary, SummaryFailure>> addToAlbum(String id) =>
      _attempt(() async => _summaryOf(await ai.addAiSummaryToAlbum(id: id)));

  @override
  Future<Result<List<PeriodSummary>, SummaryFailure>> inAlbum(String albumId) =>
      _attempt(() async => [for (final item in await ai.listAlbumAiSummaries(albumId: albumId)) _summaryOf(item)]);

  static PeriodSummary _summaryOf(AiSummaryResponse summary) => PeriodSummary(
    id: summary.id,
    accountId: summary.accountId,
    albumId: summary.albumId,
    from: summary.from,
    to: summary.to,
    state: switch (summary.state) {
      'Saved' => SummaryState.saved,
      'Invalidated' => SummaryState.invalidated,
      _ => SummaryState.draft,
    },
    sentences: [for (final sentence in summary.sentences) sentence.text],
    usedMemories: summary.usedMemories as int,
    leftOutMemories: summary.leftOutMemories as int,
    leftOutPeople: summary.leftOutPeople as int,
    droppedSentences: summary.droppedSentences as int,
    addedToAlbum: summary.addedToAlbum,
  );

  Future<Result<T, SummaryFailure>> _attempt<T>(Future<T> Function() call) async {
    try {
      return Ok(await call());
    } on DioException catch (error) {
      return Err(_failureOf(error));
    }
  }

  static SummaryFailure _failureOf(DioException error) {
    final response = error.response;
    if (response == null) {
      return const SummaryOffline();
    }
    final body = response.data;
    final code = body is Map ? body['code'] : null;
    return switch (code) {
      'AiNarration.ConsentRequired' => const SummaryConsentRequired(),
      'AiNarration.NoMaterials' => const SummaryNoMaterials(),
      'AiNarration.LimitReached' => const SummaryLimitReached(),
      'AiNarration.NothingTold' => const SummaryNothingTold(),
      'AiNarration.MaterialsChanged' => const SummaryMaterialsChanged(),
      'AiNarration.Invalidated' => const SummaryInvalidated(),
      'AiNarration.InvalidPeriod' => const SummaryInvalidPeriod(),
      _ => switch (response.statusCode) {
        403 || 404 => const SummaryNoAccess(),
        409 => const SummaryAlreadyDecided(),
        _ => const SummaryServerError(),
      },
    };
  }
}
