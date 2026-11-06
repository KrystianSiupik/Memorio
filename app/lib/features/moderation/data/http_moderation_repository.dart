import 'package:dio/dio.dart';

import '../../../core/api/generated/clients/moderation_client.dart';
import '../../../core/api/generated/models/appeal_request.dart';
import '../../../core/api/generated/models/report_request.dart';
import '../../../core/result.dart';
import '../domain/moderation.dart';

/// [ModerationRepository] on API v1.
class HttpModerationRepository implements ModerationRepository {
  const HttpModerationRepository({required this.moderation});

  final ModerationClient moderation;

  @override
  Future<Result<void, ModerationFailure>> report(
    String id,
    ReportTarget target,
    String targetId,
    ReportReason reason,
    String explanation,
  ) => _attempt(
    () => moderation.reportContent(
      body: ReportRequest(
        id: id,
        target: target.apiName,
        targetId: targetId,
        reason: reason.apiName,
        explanation: explanation,
      ),
    ),
  );

  @override
  Future<Result<MyModeration, ModerationFailure>> loadMine() => _attempt(() async {
    final mine = await moderation.getMyModeration();
    return MyModeration(
      reports: [
        for (final report in mine.reports)
          MyReport(
            id: report.id,
            reason: ReportReason.of(report.reason),
            state: report.state,
            verdict: report.verdict,
            statement: report.statement,
            reportedAt: report.reportedAt,
          ),
      ],
      aboutMine: [
        for (final about in mine.aboutMine)
          ReportAboutMine(
            id: about.id,
            target: about.target == ReportTarget.album.apiName ? ReportTarget.album : ReportTarget.memory,
            reason: ReportReason.of(about.reason),
            state: about.state,
            hidden: about.hidden,
            verdict: about.verdict,
            statement: about.statement,
            reportedAt: about.reportedAt,
            appealUntil: about.appealUntil,
            appealReversed: about.appealReversed,
            appealStatement: about.appealStatement,
          ),
      ],
    );
  });

  @override
  Future<Result<void, ModerationFailure>> appeal(String reportId, String text) => _attempt(
    () => moderation.appealDecision(
      id: reportId,
      body: AppealRequest(text: text),
    ),
  );

  Future<Result<T, ModerationFailure>> _attempt<T>(Future<T> Function() call) async {
    try {
      return Ok(await call());
    } on DioException catch (error) {
      return Err(_failureOf(error));
    }
  }

  static ModerationFailure _failureOf(DioException error) {
    final response = error.response;
    if (response == null) {
      return const ModerationOffline();
    }
    final body = response.data;
    final code = body is Map ? body['code'] : null;
    return switch (code) {
      'Moderation.OwnContent' => const ModerationOwnContent(),
      'Moderation.AlreadyDecided' => const ModerationAlreadyDecided(),
      _ => switch (response.statusCode) {
        403 || 404 => const ModerationNoAccess(),
        400 => const ModerationInvalidInput(),
        409 => const ModerationAlreadyDecided(),
        _ => const ModerationServerError(),
      },
    };
  }
}
