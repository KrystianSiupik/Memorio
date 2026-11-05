import 'package:flutter/material.dart';
import 'package:flutter_riverpod/flutter_riverpod.dart';

import '../../../core/l10n/l10n.dart';
import '../../../core/offline/offline.dart';
import '../../../core/result.dart';
import '../../albums/albums.dart' show bytesText;
import '../../memories/memories.dart' show dayTextOf;
import '../domain/plan.dart';
import 'plans_providers.dart';

String planDayText(DateTime date) {
  final local = date.toLocal();
  final month = local.month.toString().padLeft(2, '0');
  final day = local.day.toString().padLeft(2, '0');
  return dayTextOf('${local.year}-$month-$day');
}

/// S18 Plan and subscription: of the person, or with [albumId] of an album they own.
class PlanPage extends ConsumerStatefulWidget {
  const PlanPage({super.key, this.albumId, required this.onChooseMaterial});

  final String? albumId;
  final VoidCallback onChooseMaterial;

  @override
  ConsumerState<PlanPage> createState() => _PlanPageState();
}

class _PlanPageState extends ConsumerState<PlanPage> {
  var _busy = false;
  PlanFailure? _failure;

  Future<void> _refresh() async {
    setState(() {
      _busy = true;
      _failure = null;
    });
    final refreshed = await ref.read(plansRepositoryProvider).refresh(widget.albumId);
    if (!mounted) {
      return;
    }
    setState(() {
      _busy = false;
      if (refreshed case Err(:final failure)) {
        _failure = failure;
      }
    });
    if (refreshed is Ok) {
      ref.invalidate(planProvider(widget.albumId));
    }
  }

  @override
  Widget build(BuildContext context) {
    final l10n = context.l10n;
    final plan = ref.watch(planProvider(widget.albumId));
    final offline = ref.watch(isOnlineProvider).value == false;
    final failure = _failure;

    return Scaffold(
      appBar: AppBar(title: Text(widget.albumId == null ? l10n.planTitle : l10n.planAlbumTitle)),
      body: Column(
        children: [
          if (offline)
            Padding(
              padding: const EdgeInsets.all(12),
              child: Text(l10n.noConnection, key: const Key('s18-offline')),
            ),
          Expanded(
            child: PlanAsyncBody(
              value: plan,
              onRetry: () => ref.invalidate(planProvider(widget.albumId)),
              builder: (state) => ListView(
                padding: const EdgeInsets.all(16),
                children: [
                  Text(
                    _modeText(l10n, state),
                    key: const Key('s18-mode'),
                    style: Theme.of(context).textTheme.titleMedium,
                  ),
                  const SizedBox(height: 8),
                  Text(
                    l10n.planUsage(bytesText(state.usedBytes), bytesText(state.limitBytes)),
                    key: const Key('s18-usage'),
                  ),
                  Text(l10n.planAiUsage(state.aiUsed, state.aiLimit), key: const Key('s18-ai')),
                  if (state.entitledUntil case final DateTime until when state.mode == PlanMode.active)
                    Text(l10n.planPaidUntil(planDayText(until)), key: const Key('s18-paid')),
                  if (state.readOnlySince case final DateTime since) ...[
                    const SizedBox(height: 8),
                    Text(l10n.planReadOnlySince(planDayText(since)), key: const Key('s18-read-only')),
                  ],
                  if (state.trimAt case final DateTime trimAt when !state.isHeld)
                    Text(
                      l10n.planTrimAt(planDayText(trimAt), bytesText(state.freeLimitBytes)),
                      key: const Key('s18-trim'),
                    ),
                  if (state.isHeld) Text(l10n.planHeld, key: const Key('s18-held')),
                  if (state.mode == PlanMode.readOnly) ...[const SizedBox(height: 8), Text(l10n.planNothingLost)],
                  if (failure != null)
                    Padding(
                      padding: const EdgeInsets.symmetric(vertical: 8),
                      child: Text(
                        planFailureText(l10n, failure),
                        key: const Key('plan-error'),
                        style: TextStyle(color: Theme.of(context).colorScheme.error),
                      ),
                    ),
                  const SizedBox(height: 16),
                  if (state.isChoiceOpen) ...[
                    FilledButton(onPressed: widget.onChooseMaterial, child: Text(l10n.planChoose)),
                    const SizedBox(height: 8),
                  ],
                  OutlinedButton(onPressed: offline || _busy ? null : _refresh, child: Text(l10n.planRefresh)),
                ],
              ),
            ),
          ),
        ],
      ),
    );
  }

  String _modeText(AppLocalizations l10n, PlanState state) => switch (state.mode) {
    PlanMode.free => l10n.planModeFree,
    PlanMode.active => l10n.planModeActive,
    PlanMode.readOnly => widget.albumId == null ? l10n.planModeOverLimit : l10n.planModeReadOnly,
    PlanMode.trimmed => l10n.planModeTrimmed,
  };
}
