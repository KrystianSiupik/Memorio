import '../../../core/result.dart';

/// What a report says is wrong (S2:C2). The first five are illegal: hidden at once and decided within 24 hours.
enum ReportReason {
  childAbuse('ChildAbuse'),
  terrorism('Terrorism'),
  threat('Threat'),
  incitement('Incitement'),
  otherIllegal('OtherIllegal'),
  someonesImage('SomeonesImage'),
  personalData('PersonalData'),
  defamation('Defamation'),
  copyright('Copyright'),
  other('Other');

  const ReportReason(this.apiName);

  final String apiName;

  bool get isIllegal => index <= ReportReason.otherIllegal.index;

  static ReportReason of(String apiName) =>
      values.firstWhere((reason) => reason.apiName == apiName, orElse: () => other);
}

/// What can be reported from the app: something the person sees.
enum ReportTarget {
  memory('Memory'),
  album('Album');

  const ReportTarget(this.apiName);

  final String apiName;
}

/// R18: a report the person made.
class MyReport {
  const MyReport({
    required this.id,
    required this.reason,
    required this.state,
    this.verdict,
    this.statement,
    required this.reportedAt,
    this.appealUntil,
    this.appealReversed,
    this.appealStatement,
  });

  final String id;
  final ReportReason reason;

  /// Open, Decided, Appealed or Closed.
  final String state;

  /// Removed or Kept, once decided.
  final String? verdict;
  final String? statement;
  final DateTime reportedAt;
  final DateTime? appealUntil;
  final bool? appealReversed;
  final String? appealStatement;

  /// The reporter may appeal keeping the content while the window lasts (DSA art. 20, D-314).
  bool canAppeal(DateTime now) => state == 'Decided' && verdict == 'Kept' && (appealUntil?.isAfter(now) ?? false);
}

/// R18: a report about the person's own content. Who reported it is never known here.
class ReportAboutMine {
  const ReportAboutMine({
    required this.id,
    required this.target,
    required this.reason,
    required this.state,
    required this.hidden,
    this.verdict,
    this.statement,
    required this.reportedAt,
    this.appealUntil,
    this.appealReversed,
    this.appealStatement,
  });

  final String id;
  final ReportTarget target;
  final ReportReason reason;
  final String state;

  /// Nobody else sees it because of this report now.
  final bool hidden;
  final String? verdict;
  final String? statement;
  final DateTime reportedAt;
  final DateTime? appealUntil;
  final bool? appealReversed;
  final String? appealStatement;

  /// The author may appeal a removal while the window lasts (DSA art. 20).
  bool canAppeal(DateTime now) => state == 'Decided' && verdict == 'Removed' && (appealUntil?.isAfter(now) ?? false);
}

class MyModeration {
  const MyModeration({this.reports = const [], this.aboutMine = const []});

  final List<MyReport> reports;
  final List<ReportAboutMine> aboutMine;

  bool get isEmpty => reports.isEmpty && aboutMine.isEmpty;
}

sealed class ModerationFailure implements Exception {
  const ModerationFailure();
}

final class ModerationOffline extends ModerationFailure {
  const ModerationOffline();
}

final class ModerationNoAccess extends ModerationFailure {
  const ModerationNoAccess();
}

/// One's own content is changed or deleted, not reported.
final class ModerationOwnContent extends ModerationFailure {
  const ModerationOwnContent();
}

final class ModerationInvalidInput extends ModerationFailure {
  const ModerationInvalidInput();
}

/// Three manifestly unfounded reports within 90 days (D-315).
final class ModerationSuspended extends ModerationFailure {
  const ModerationSuspended();
}

final class ModerationAlreadyDecided extends ModerationFailure {
  const ModerationAlreadyDecided();
}

final class ModerationServerError extends ModerationFailure {
  const ModerationServerError();
}

abstract interface class ModerationRepository {
  /// C110. [id] comes from the client: the same report again is that report.
  Future<Result<void, ModerationFailure>> report(
    String id,
    ReportTarget target,
    String targetId,
    ReportReason reason,
    String explanation,
  );

  Future<Result<MyModeration, ModerationFailure>> loadMine();

  /// C112.
  Future<Result<void, ModerationFailure>> appeal(String reportId, String text);
}
