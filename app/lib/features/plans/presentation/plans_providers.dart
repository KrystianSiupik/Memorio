import 'package:flutter/material.dart';
import 'package:flutter_riverpod/flutter_riverpod.dart';

import '../../../core/l10n/l10n.dart';
import '../../../core/result.dart';
import '../../account/account.dart';
import '../../memories/memories.dart' show noAutomaticRetry;
import '../domain/plan.dart';

/// Overridden in app/ with the API repository, and in tests with a fake.
final plansRepositoryProvider = Provider<PlansRepository>(
  (ref) => throw UnimplementedError('plansRepositoryProvider is overridden in app/'),
);

/// A read of the signed-in person: follows the account, so the next person never sees what the previous one
/// loaded, and stays failed until they ask again.
Future<T> _load<T>(Ref ref, Future<Result<T, PlanFailure>> Function(PlansRepository plans) load) async {
  ref.watch(sessionProvider.select((session) => session.value?.accountId));
  return switch (await load(ref.watch(plansRepositoryProvider))) {
    Ok(:final value) => value,
    Err(:final failure) => throw failure,
  };
}

/// The key is the album's id; null is the person's own plan.
final planProvider = FutureProvider.family<PlanState, String?>(
  (ref, albumId) => _load(ref, (plans) => plans.load(albumId)),
  retry: noAutomaticRetry,
);
final trimMaterialProvider = FutureProvider.family<List<TrimMaterial>, String?>(
  (ref, albumId) => _load(ref, (plans) => plans.loadMaterial(albumId)),
  retry: noAutomaticRetry,
);

/// A failure in words, the same on every screen of this feature.
String planFailureText(AppLocalizations l10n, Object? failure) => switch (failure) {
  PlanOffline() => l10n.authOffline,
  PlanNoAccess() => l10n.planNoAccess,
  PlanChoiceClosed() => l10n.planChoiceClosed,
  PlanAlreadyDecided() => l10n.alreadyDecided,
  _ => l10n.serverError,
};

/// Loading, a failed load with its retry, or the content; the last answer stays while it reloads and offline.
class PlanAsyncBody<T> extends StatelessWidget {
  const PlanAsyncBody({super.key, required this.value, required this.onRetry, required this.builder});

  final AsyncValue<T> value;
  final VoidCallback onRetry;
  final Widget Function(T value) builder;

  @override
  Widget build(BuildContext context) {
    final l10n = context.l10n;
    final known = value.value;
    if (known != null) {
      return builder(known);
    }
    if (value.hasError) {
      return Center(
        child: Column(
          mainAxisSize: MainAxisSize.min,
          children: [
            Padding(
              padding: const EdgeInsets.all(16),
              child: Text(
                planFailureText(l10n, value.error),
                key: const Key('plan-load-error'),
                textAlign: TextAlign.center,
              ),
            ),
            if (value.error is! PlanNoAccess) TextButton(onPressed: onRetry, child: Text(l10n.retry)),
          ],
        ),
      );
    }
    final color = Theme.of(context).colorScheme.surfaceContainerHighest;
    return ListView(
      key: const Key('plan-skeleton'),
      padding: const EdgeInsets.all(16),
      children: [
        for (var line = 0; line < 4; line++)
          Container(height: 48, margin: const EdgeInsets.only(bottom: 12), color: color),
      ],
    );
  }
}
