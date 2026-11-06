import 'package:flutter/material.dart';
import 'package:flutter_riverpod/flutter_riverpod.dart';

import '../../../core/l10n/l10n.dart';
import '../../../core/offline/offline.dart';
import '../../../core/result.dart';
import '../../account/account.dart';
import '../../memories/memories.dart' show noAutomaticRetry;
import '../domain/moderation.dart';

/// Overridden in app/ with the API repository, and in tests with a fake.
final moderationRepositoryProvider = Provider<ModerationRepository>(
  (ref) => throw UnimplementedError('moderationRepositoryProvider is overridden in app/'),
);

/// R18 of the signed-in person: follows the account, and stays failed until they ask again.
final myModerationProvider = FutureProvider<MyModeration>((ref) async {
  ref.watch(sessionProvider.select((session) => session.value?.accountId));
  return switch (await ref.watch(moderationRepositoryProvider).loadMine()) {
    Ok(:final value) => value,
    Err(:final failure) => throw failure,
  };
}, retry: noAutomaticRetry);

/// A failure in words, the same on every screen of this feature.
String moderationFailureText(AppLocalizations l10n, Object? failure) => switch (failure) {
  ModerationOffline() => l10n.authOffline,
  ModerationNoAccess() => l10n.reportNoAccess,
  ModerationOwnContent() => l10n.reportOwnContent,
  ModerationSuspended() => l10n.reportSuspended,
  ModerationInvalidInput() => l10n.invalidInput,
  ModerationAlreadyDecided() => l10n.alreadyDecided,
  _ => l10n.serverError,
};

String reportReasonText(AppLocalizations l10n, ReportReason reason) => switch (reason) {
  ReportReason.childAbuse => l10n.reasonChildAbuse,
  ReportReason.terrorism => l10n.reasonTerrorism,
  ReportReason.threat => l10n.reasonThreat,
  ReportReason.incitement => l10n.reasonIncitement,
  ReportReason.otherIllegal => l10n.reasonOtherIllegal,
  ReportReason.someonesImage => l10n.reasonSomeonesImage,
  ReportReason.personalData => l10n.reasonPersonalData,
  ReportReason.defamation => l10n.reasonDefamation,
  ReportReason.copyright => l10n.reasonCopyright,
  ReportReason.other => l10n.reasonOther,
};

/// One action at a time, its failure kept on the screen: what the person typed stays where it was.
mixin ModerationActions<T extends ConsumerStatefulWidget> on ConsumerState<T> {
  var busy = false;
  ModerationFailure? failure;

  bool get offline => ref.watch(isOnlineProvider).value == false;

  ModerationRepository get moderation => ref.read(moderationRepositoryProvider);

  /// True when it worked.
  Future<bool> act(Future<Result<void, ModerationFailure>> Function() action) async {
    setState(() {
      busy = true;
      failure = null;
    });
    final result = await action();
    if (!mounted) {
      return false;
    }
    setState(() {
      busy = false;
      if (result case Err(failure: final refused)) {
        failure = refused;
      }
    });
    return result is Ok;
  }

  List<Widget> failureBlock(BuildContext context) => [
    if (failure case final ModerationFailure refused)
      Padding(
        padding: const EdgeInsets.symmetric(vertical: 8),
        child: Text(
          moderationFailureText(context.l10n, refused),
          key: const Key('moderation-error'),
          style: TextStyle(color: Theme.of(context).colorScheme.error),
        ),
      ),
  ];

  List<Widget> offlineBlock(BuildContext context) => [
    if (offline)
      Padding(
        padding: const EdgeInsets.all(12),
        child: Text(context.l10n.noConnection, key: const Key('moderation-offline')),
      ),
  ];
}
