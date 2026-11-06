import 'package:flutter/material.dart';
import 'package:flutter_riverpod/flutter_riverpod.dart';

import '../../../core/l10n/l10n.dart';
import '../../../core/offline/offline.dart';
import '../../memories/memories.dart';
import 'map_providers.dart';

/// S03 Map and globe. Points stay on the map through a reload and a lost connection: the provider
/// keeps the last answer, so only the message around them changes.
class MapPage extends ConsumerWidget {
  const MapPage({
    super.key,
    required this.onAddMemory,
    required this.onOpenTrash,
    required this.onOpenMemory,
    required this.onReplay,
  });

  final VoidCallback onReplay;

  final VoidCallback onAddMemory;
  final VoidCallback onOpenTrash;
  final void Function(String memoryId) onOpenMemory;

  static bool _isWaiting(PendingOperation operation) => !operation.isRejected;

  Future<void> _pickDates(BuildContext context, WidgetRef ref) async {
    final now = DateTime.now();
    final range = ref.read(memoryDateFilterProvider);
    final picked = await showDateRangePicker(
      context: context,
      firstDate: DateTime(1900),
      lastDate: DateTime(now.year, now.month, now.day),
      initialDateRange: range == null ? null : DateTimeRange(start: range.from, end: range.to),
    );
    if (picked != null) {
      ref.read(memoryDateFilterProvider.notifier).set((from: picked.start, to: picked.end));
    }
  }

  Future<void> _dismissRejected(WidgetRef ref, List<PendingOperation> rejected) async {
    final queue = ref.read(offlineQueueProvider);
    for (final operation in rejected) {
      await queue.dismiss(operation.id);
    }
  }

  @override
  Widget build(BuildContext context, WidgetRef ref) {
    final l10n = context.l10n;

    ref.listen(tappedPointProvider, (previous, memoryId) {
      if (memoryId != null) {
        ref.read(tappedPointProvider.notifier).clear();
        onOpenMemory(memoryId);
      }
    });
    // What the queue has just sent is now on the server, so the points are out of date.
    ref.listen(pendingOperationsProvider, (previous, next) {
      final before = previous?.value?.where(_isWaiting).length ?? 0;
      final after = next.value?.where(_isWaiting).length ?? before;
      if (after < before) {
        ref.invalidate(memoryPinsProvider);
      }
    });

    final pins = ref.watch(memoryPinsProvider);
    final range = ref.watch(memoryDateFilterProvider);
    final pending = ref.watch(pendingOperationsProvider).value ?? const <PendingOperation>[];
    final waitingMemories = pending.where(
      (operation) => _isWaiting(operation) && operation.kind == MemoryOperations.createMemory,
    );
    final rejected = pending.where((operation) => operation.isRejected).toList();

    final error = pins.error;
    final failure = pins.isLoading || error is! MemoryFailure ? null : error;
    // The last points are still on the map, and the shell already says the connection is gone.
    final showFailure = failure != null && !(failure is MemoryOffline && pins.hasValue);
    final isEmpty = !pins.isLoading && !pins.hasError && (pins.value?.isEmpty ?? false);

    return Scaffold(
      appBar: AppBar(
        title: Text(l10n.mapTitle),
        actions: [
          IconButton(tooltip: l10n.replayTitle, icon: const Icon(Icons.play_circle_outline), onPressed: onReplay),
          IconButton(tooltip: l10n.mapOpenTrash, icon: const Icon(Icons.delete_outline), onPressed: onOpenTrash),
        ],
      ),
      floatingActionButton: FloatingActionButton(
        tooltip: l10n.mapAddMemory,
        onPressed: onAddMemory,
        child: const Icon(Icons.add),
      ),
      body: Stack(
        children: [
          Positioned.fill(child: ref.watch(mapViewProvider)),
          Positioned(
            top: 8,
            left: 8,
            right: 8,
            child: Column(
              crossAxisAlignment: CrossAxisAlignment.start,
              children: [
                Wrap(
                  spacing: 8,
                  children: [
                    FilterChip(
                      key: const Key('s03-filter-dates'),
                      label: Text(
                        range == null
                            ? l10n.mapFilterDates
                            : l10n.mapFilterDatesRange(dayText(range.from), dayText(range.to)),
                      ),
                      selected: range != null,
                      onSelected: (_) => _pickDates(context, ref),
                      onDeleted: range == null ? null : () => ref.read(memoryDateFilterProvider.notifier).set(null),
                    ),
                    // They start working with albums and people (D-231).
                    FilterChip(label: Text(l10n.mapFilterPeople), onSelected: null),
                    FilterChip(label: Text(l10n.mapFilterAlbums), onSelected: null),
                  ],
                ),
                if (pins.isLoading)
                  Card(
                    key: const Key('s03-loading'),
                    child: Padding(
                      padding: const EdgeInsets.all(12),
                      child: Column(
                        crossAxisAlignment: CrossAxisAlignment.start,
                        mainAxisSize: MainAxisSize.min,
                        children: [Text(l10n.mapLoadingPoints), const LinearProgressIndicator()],
                      ),
                    ),
                  ),
                if (showFailure)
                  Card(
                    child: Padding(
                      padding: const EdgeInsets.all(12),
                      child: Row(
                        children: [
                          Expanded(child: Text(memoryFailureText(l10n, failure), key: const Key('s03-error'))),
                          TextButton(onPressed: () => ref.invalidate(memoryPinsProvider), child: Text(l10n.retry)),
                        ],
                      ),
                    ),
                  ),
                if (waitingMemories.isNotEmpty)
                  Card(
                    child: Padding(
                      padding: const EdgeInsets.all(12),
                      child: Text(l10n.mapPendingUpload(waitingMemories.length), key: const Key('s03-pending')),
                    ),
                  ),
                if (rejected.isNotEmpty)
                  Card(
                    child: Padding(
                      padding: const EdgeInsets.all(12),
                      child: Row(
                        children: [
                          Expanded(child: Text(l10n.mapRejected(rejected.length), key: const Key('s03-rejected'))),
                          TextButton(
                            onPressed: () => _dismissRejected(ref, rejected),
                            child: Text(l10n.mapRejectedDismiss),
                          ),
                        ],
                      ),
                    ),
                  ),
              ],
            ),
          ),
          if (isEmpty)
            Center(
              child: range == null
                  ? FilledButton(onPressed: onAddMemory, child: Text(l10n.mapEmpty))
                  : Card(
                      child: Padding(padding: const EdgeInsets.all(12), child: Text(l10n.mapEmptyFiltered)),
                    ),
            ),
          Positioned(
            left: 8,
            bottom: 8,
            child: ColoredBox(
              color: Theme.of(context).colorScheme.surface.withValues(alpha: 0.8),
              child: Padding(
                padding: const EdgeInsets.symmetric(horizontal: 4),
                child: Text(l10n.mapAttribution, style: Theme.of(context).textTheme.labelSmall),
              ),
            ),
          ),
        ],
      ),
    );
  }
}
