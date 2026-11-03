import 'package:flutter/material.dart';
import 'package:flutter_riverpod/flutter_riverpod.dart';

import '../../../core/l10n/l10n.dart';
import '../../../core/offline/offline.dart';
import '../../../core/result.dart';
import '../domain/memory.dart';
import '../domain/memory_failure.dart';
import 'memories_providers.dart';
import 'memory_texts.dart';

/// S05 Details of a memory. "Remove my tag" appears with albums (MEM-E3-ALBUM-15): nobody can be tagged yet.
class MemoryDetailsPage extends ConsumerStatefulWidget {
  const MemoryDetailsPage({super.key, required this.memoryId, required this.onLeave});

  final String memoryId;

  /// The memory is gone from here (deleted, or access taken away): back to the map.
  final VoidCallback onLeave;

  @override
  ConsumerState<MemoryDetailsPage> createState() => _MemoryDetailsPageState();
}

class _MemoryDetailsPageState extends ConsumerState<MemoryDetailsPage> {
  var _understood = false;
  var _busy = false;
  MemoryFailure? _failure;

  Future<void> _run(Future<Result<void, MemoryFailure>> Function() action) async {
    setState(() {
      _busy = true;
      _failure = null;
    });
    final result = await action();
    if (!mounted) {
      return;
    }
    setState(() => _busy = false);
    switch (result) {
      case Ok():
        ref.invalidate(memoryDetailsProvider(widget.memoryId));
        ref.invalidate(trashProvider);
        ref.invalidate(memoryPinsProvider);
        widget.onLeave();
      case Err(:final failure):
        setState(() => _failure = failure);
    }
  }

  @override
  Widget build(BuildContext context) {
    final l10n = context.l10n;
    final details = ref.watch(memoryDetailsProvider(widget.memoryId));
    final offline = ref.watch(isOnlineProvider).value == false;
    final memory = details.value;

    return Scaffold(
      appBar: AppBar(title: Text(l10n.memoryTitle)),
      body: Column(
        children: [
          if (offline)
            Padding(
              padding: const EdgeInsets.all(12),
              child: Text(l10n.noConnection, key: const Key('s05-offline')),
            ),
          if (_busy) const LinearProgressIndicator(key: Key('s05-busy')),
          Expanded(
            child: switch (memory) {
              final MemoryDetails data when details.error is! MemoryNoAccess => _content(data, offline),
              _ when details.error is MemoryNoAccess => Center(
                child: Column(
                  mainAxisSize: MainAxisSize.min,
                  children: [
                    Text(l10n.memoryGone, key: const Key('s05-forbidden')),
                    TextButton(onPressed: widget.onLeave, child: Text(l10n.memoryBack)),
                  ],
                ),
              ),
              _ when details.hasError => Center(
                child: Column(
                  mainAxisSize: MainAxisSize.min,
                  children: [
                    Text(memoryFailureText(l10n, details.error), key: const Key('s05-error')),
                    TextButton(
                      onPressed: () => ref.invalidate(memoryDetailsProvider(widget.memoryId)),
                      child: Text(l10n.retry),
                    ),
                  ],
                ),
              ),
              _ => const _Skeleton(),
            },
          ),
        ],
      ),
    );
  }

  Widget _content(MemoryDetails memory, bool offline) {
    final l10n = context.l10n;
    final active = !offline && !_busy;
    final failure = _failure;
    final date = memory.dateFrom == memory.dateTo
        ? dayTextOf(memory.dateFrom)
        : '${dayTextOf(memory.dateFrom)} – ${dayTextOf(memory.dateTo)}';

    return ListView(
      padding: const EdgeInsets.all(16),
      children: [
        Text(l10n.memoryPlace(memory.placeName), style: Theme.of(context).textTheme.titleMedium),
        const SizedBox(height: 4),
        Text(l10n.memoryWhen(date)),
        const SizedBox(height: 24),
        Text(l10n.memoryContents, style: Theme.of(context).textTheme.titleMedium),
        const SizedBox(height: 8),
        if (memory.isMine) Text(l10n.memoryAuthorMe),
        const SizedBox(height: 4),
        Text(memory.description ?? l10n.memoryNoDescription),
        if (memory.descriptionIsAi) Chip(key: const Key('s05-ai-marker'), label: Text(l10n.aiMarker)),
        const SizedBox(height: 8),
        for (final url in memory.photoUrls)
          Padding(
            padding: const EdgeInsets.only(bottom: 8),
            child: Image.network(url, errorBuilder: (context, error, stack) => const Icon(Icons.broken_image)),
          ),
        const SizedBox(height: 16),
        Text(l10n.memoryPeople, style: Theme.of(context).textTheme.titleMedium),
        const SizedBox(height: 8),
        Text(l10n.memoryPeopleNone),
        const SizedBox(height: 24),
        if (failure != null) ...[
          Text(
            memoryFailureText(l10n, failure),
            key: const Key('s05-action-error'),
            style: TextStyle(color: Theme.of(context).colorScheme.error),
          ),
          const SizedBox(height: 12),
        ],
        if (memory.isMine) ...[
          OutlinedButton(
            onPressed: active ? () => _run(() => ref.read(memoriesRepositoryProvider).moveToTrash(memory.id)) : null,
            child: Text(l10n.deleteMine),
          ),
          const SizedBox(height: 16),
          CheckboxListTile(
            title: Text(l10n.deleteForGoodConfirm),
            value: _understood,
            onChanged: active ? (value) => setState(() => _understood = value ?? false) : null,
          ),
          FilledButton(
            onPressed: active && _understood
                ? () => _run(() => ref.read(memoriesRepositoryProvider).deleteForGood(memory.id))
                : null,
            child: Text(l10n.deleteForGood),
          ),
        ],
      ],
    );
  }
}

class _Skeleton extends StatelessWidget {
  const _Skeleton();

  @override
  Widget build(BuildContext context) {
    final color = Theme.of(context).colorScheme.surfaceContainerHighest;
    return ListView(
      key: const Key('s05-skeleton'),
      padding: const EdgeInsets.all(16),
      children: [
        for (var line = 0; line < 6; line++)
          Container(height: 16, margin: const EdgeInsets.only(bottom: 12), color: color),
      ],
    );
  }
}
