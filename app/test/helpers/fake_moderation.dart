import 'package:memorio/core/result.dart';
import 'package:memorio/features/moderation/moderation.dart';

/// Moderation from memory; tests set the next answers.
class FakeModerationRepository implements ModerationRepository {
  MyModeration mine = const MyModeration();
  ModerationFailure? loadFailure;
  ModerationFailure? actionFailure;

  /// Every action, as "name:arguments", in order.
  final actions = <String>[];

  Future<Result<void, ModerationFailure>> _act(String action) async {
    actions.add(action);
    final failure = actionFailure;
    return failure == null ? const Ok(null) : Err(failure);
  }

  @override
  Future<Result<void, ModerationFailure>> report(
    String id,
    ReportTarget target,
    String targetId,
    ReportReason reason,
    String explanation,
  ) => _act('report:$id:${target.apiName}:$targetId:${reason.apiName}:$explanation');

  @override
  Future<Result<MyModeration, ModerationFailure>> loadMine() async {
    final failure = loadFailure;
    return failure == null ? Ok(mine) : Err(failure);
  }

  @override
  Future<Result<void, ModerationFailure>> appeal(String reportId, String text) => _act('appeal:$reportId:$text');
}
