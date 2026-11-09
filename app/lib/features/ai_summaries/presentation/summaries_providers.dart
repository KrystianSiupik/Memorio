import 'package:flutter_riverpod/flutter_riverpod.dart';

import '../../../core/l10n/l10n.dart';
import '../../../core/result.dart';
import '../../account/account.dart';
import '../../memories/memories.dart' show noAutomaticRetry;
import '../domain/period_summary.dart';

/// Overridden in app/ with the API repository, and in tests with a fake.
final summariesRepositoryProvider = Provider<SummariesRepository>(
  (ref) => throw UnimplementedError('summariesRepositoryProvider is overridden in app/'),
);

/// AI operations left this month; null when unknown, and then the server decides. Overridden in app/ from the plan.
final aiOperationsLeftProvider = FutureProvider<int?>((ref) async => null);

/// A read of the signed-in person: follows the account and stays failed until they ask again.
Future<T> _load<T>(Ref ref, Future<Result<T, SummaryFailure>> Function(SummariesRepository summaries) load) async {
  ref.watch(sessionProvider.select((session) => session.value?.accountId));
  return switch (await load(ref.watch(summariesRepositoryProvider))) {
    Ok(:final value) => value,
    Err(:final failure) => throw failure,
  };
}

final aiConsentProvider = FutureProvider.family<bool, String>(
  (ref, albumId) => _load(ref, (summaries) => summaries.consent(albumId)),
  retry: noAutomaticRetry,
);

final albumSummariesProvider = FutureProvider.family<List<PeriodSummary>, String>(
  (ref, albumId) => _load(ref, (summaries) => summaries.inAlbum(albumId)),
  retry: noAutomaticRetry,
);

/// A failure in words: no network, no access, "already decided", a server error, or what the AI rules say.
String summaryFailureText(AppLocalizations l10n, Object? failure) => switch (failure) {
  SummaryOffline() => l10n.authOffline,
  SummaryNoAccess() => l10n.summaryNoAccess,
  SummaryConsentRequired() => l10n.summaryConsentNeeded,
  SummaryNoMaterials() => l10n.summaryEmpty,
  SummaryLimitReached() => l10n.summaryLimitUsed,
  SummaryNothingTold() => l10n.summaryNothingTold,
  SummaryMaterialsChanged() => l10n.summaryMaterialsChanged,
  SummaryInvalidated() => l10n.summaryInvalidated,
  SummaryInvalidPeriod() => l10n.summaryInvalidPeriod,
  SummaryAlreadyDecided() => l10n.alreadyDecided,
  _ => l10n.serverError,
};
