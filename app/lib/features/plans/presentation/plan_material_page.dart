import 'package:flutter/material.dart';
import 'package:flutter_riverpod/flutter_riverpod.dart';

import '../../../core/l10n/l10n.dart';
import '../../../core/offline/offline.dart';
import '../../../core/result.dart';
import '../../albums/albums.dart' show bytesText;
import '../../memories/memories.dart' show dayTextOf;
import '../domain/plan.dart';
import 'plan_page.dart';
import 'plans_providers.dart';

/// S19 Choose what stays after the trim. Without a choice the oldest stays, so that is what starts selected (S2:B1).
class PlanMaterialPage extends ConsumerStatefulWidget {
  const PlanMaterialPage({super.key, this.albumId});

  final String? albumId;

  @override
  ConsumerState<PlanMaterialPage> createState() => _PlanMaterialPageState();
}

class _PlanMaterialPageState extends ConsumerState<PlanMaterialPage> {
  /// Null until the person changes something: the selection comes from what was stored or from the oldest.
  Set<String>? _selected;
  var _busy = false;
  var _saved = false;
  PlanFailure? _failure;

  static Set<String> _start(PlanState plan, List<TrimMaterial> material) {
    final known = material.map((item) => item.memoryId).toSet();
    final kept = plan.keptMemoryIds.where(known.contains).toSet();
    if (kept.isNotEmpty) {
      return kept;
    }
    var left = plan.freeLimitBytes;
    final oldest = <String>{};
    for (final item in material) {
      if (item.sizeBytes <= left) {
        left -= item.sizeBytes;
        oldest.add(item.memoryId);
      }
    }
    return oldest;
  }

  Future<void> _save(Set<String> selected) async {
    setState(() {
      _busy = true;
      _saved = false;
      _failure = null;
    });
    final stored = await ref.read(plansRepositoryProvider).keep(widget.albumId, selected.toList());
    if (!mounted) {
      return;
    }
    setState(() {
      _busy = false;
      switch (stored) {
        case Ok():
          _saved = true;
        case Err(:final failure):
          // The selection stays: a refusal never costs what the person marked.
          _failure = failure;
      }
    });
    if (stored is Ok) {
      ref.invalidate(planProvider(widget.albumId));
    }
  }

  @override
  Widget build(BuildContext context) {
    final l10n = context.l10n;
    final plan = ref.watch(planProvider(widget.albumId));
    final material = ref.watch(trimMaterialProvider(widget.albumId));
    final offline = ref.watch(isOnlineProvider).value == false;
    final failure = _failure ?? plan.error;
    final known = plan.value;
    final AsyncValue<List<TrimMaterial>> shown = known == null && plan.hasError
        ? AsyncValue.error(plan.error!, plan.stackTrace ?? StackTrace.empty)
        : known == null
        ? const AsyncValue.loading()
        : material;

    return Scaffold(
      appBar: AppBar(title: Text(l10n.planMaterialTitle)),
      body: Column(
        children: [
          if (offline)
            Padding(
              padding: const EdgeInsets.all(12),
              child: Text(l10n.noConnection, key: const Key('s19-offline')),
            ),
          Expanded(
            child: PlanAsyncBody(
              value: shown,
              onRetry: () {
                ref.invalidate(planProvider(widget.albumId));
                ref.invalidate(trimMaterialProvider(widget.albumId));
              },
              builder: (items) {
                final state = known!;
                final selected = _selected ?? _start(state, items);
                final used = items
                    .where((item) => selected.contains(item.memoryId))
                    .fold(0, (sum, item) => sum + item.sizeBytes);
                final tooMuch = used > state.freeLimitBytes;
                return ListView(
                  padding: const EdgeInsets.all(16),
                  children: [
                    Text(
                      l10n.planMaterialLimit(bytesText(used), bytesText(state.freeLimitBytes)),
                      key: const Key('s19-limit'),
                    ),
                    if (state.trimAt case final DateTime trimAt when !state.isHeld)
                      Text(l10n.planMaterialDeadline(planDayText(trimAt)), key: const Key('s19-deadline')),
                    if (tooMuch)
                      Text(
                        l10n.planMaterialTooMuch,
                        key: const Key('s19-too-much'),
                        style: TextStyle(color: Theme.of(context).colorScheme.error),
                      ),
                    if (!state.isChoiceOpen) Text(l10n.planChoiceClosed, key: const Key('s19-closed')),
                    const SizedBox(height: 8),
                    for (final item in items)
                      CheckboxListTile(
                        key: Key('s19-item-${item.memoryId}'),
                        value: selected.contains(item.memoryId),
                        title: Text(item.placeName),
                        subtitle: Text('${dayTextOf(item.dateFrom)} · ${bytesText(item.sizeBytes)}'),
                        onChanged: !state.isChoiceOpen || offline || _busy
                            ? null
                            : (checked) => setState(() {
                                _saved = false;
                                _selected = {...selected};
                                if (checked ?? false) {
                                  _selected!.add(item.memoryId);
                                } else {
                                  _selected!.remove(item.memoryId);
                                }
                              }),
                      ),
                    if (failure != null)
                      Padding(
                        padding: const EdgeInsets.symmetric(vertical: 8),
                        child: Text(
                          planFailureText(l10n, failure),
                          key: const Key('plan-error'),
                          style: TextStyle(color: Theme.of(context).colorScheme.error),
                        ),
                      ),
                    if (_saved) Text(l10n.planMaterialSaved, key: const Key('s19-saved')),
                    const SizedBox(height: 16),
                    FilledButton(
                      onPressed: !state.isChoiceOpen || offline || _busy || tooMuch ? null : () => _save(selected),
                      child: Text(l10n.planMaterialKeep),
                    ),
                  ],
                );
              },
            ),
          ),
        ],
      ),
    );
  }
}
