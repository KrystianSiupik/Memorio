import 'package:flutter/material.dart';
import 'package:flutter_riverpod/flutter_riverpod.dart';

import '../../../core/l10n/l10n.dart';
import '../../../core/offline/offline.dart';
import '../../../core/result.dart';
import '../domain/memory.dart';
import '../domain/memory_failure.dart';
import 'memories_providers.dart';
import 'memory_texts.dart';

/// S06 The trash: the person's own content while its 30 days last.
class TrashPage extends ConsumerStatefulWidget {
  const TrashPage({super.key, this.onRestored});

  /// A memory came back, so what shows it must refresh.
  final VoidCallback? onRestored;

  @override
  ConsumerState<TrashPage> createState() => _TrashPageState();
}

class _TrashPageState extends ConsumerState<TrashPage> {
  String? _restoring;
  MemoryFailure? _failure;

  Future<void> _restore(String id) async {
    setState(() {
      _restoring = id;
      _failure = null;
    });
    final result = await ref.read(memoriesRepositoryProvider).restore(id);
    if (!mounted) {
      return;
    }
    setState(() {
      _restoring = null;
      if (result case Err(:final failure)) {
        _failure = failure;
      }
    });
    ref.invalidate(trashProvider);
    ref.invalidate(memoryPinsProvider);
    if (result is Ok) {
      widget.onRestored?.call();
    }
  }

  @override
  Widget build(BuildContext context) {
    final l10n = context.l10n;
    final trash = ref.watch(trashProvider);
    final offline = ref.watch(isOnlineProvider).value == false;
    final items = trash.value;
    final failure = _failure;

    return Scaffold(
      appBar: AppBar(title: Text(l10n.trashTitle)),
      body: Column(
        children: [
          if (offline)
            Padding(
              padding: const EdgeInsets.all(12),
              child: Text(l10n.noConnection, key: const Key('s06-offline')),
            ),
          if (failure != null)
            Padding(
              padding: const EdgeInsets.all(12),
              child: Text(
                memoryFailureText(l10n, failure),
                key: const Key('s06-error'),
                style: TextStyle(color: Theme.of(context).colorScheme.error),
              ),
            ),
          Expanded(
            child: switch (items) {
              null when trash.hasError => Center(
                child: Column(
                  mainAxisSize: MainAxisSize.min,
                  children: [
                    Text(memoryFailureText(l10n, trash.error), key: const Key('s06-error')),
                    TextButton(onPressed: () => ref.invalidate(trashProvider), child: Text(l10n.retry)),
                  ],
                ),
              ),
              null => const _Skeleton(),
              [] => Center(child: Text(l10n.trashEmpty)),
              final List<TrashedMemory> list => ListView.separated(
                itemCount: list.length,
                separatorBuilder: (context, index) => const Divider(height: 1),
                itemBuilder: (context, index) {
                  final item = list[index];
                  return ListTile(
                    title: Text(l10n.trashItem(dayTextOf(item.dateFrom), item.placeName)),
                    subtitle: Text(
                      [
                        if (item.description case final String description) description,
                        l10n.trashPhotos(item.photos),
                        l10n.trashDeletedAt(dayText(item.trashedAt)),
                        l10n.trashDaysLeft(item.daysLeft),
                      ].join('\n'),
                    ),
                    isThreeLine: true,
                    trailing: TextButton(
                      onPressed: offline || _restoring != null ? null : () => _restore(item.id),
                      child: Text(l10n.restore),
                    ),
                  );
                },
              ),
            },
          ),
        ],
      ),
    );
  }
}

class _Skeleton extends StatelessWidget {
  const _Skeleton();

  @override
  Widget build(BuildContext context) {
    final color = Theme.of(context).colorScheme.surfaceContainerHighest;
    return ListView(
      key: const Key('s06-skeleton'),
      padding: const EdgeInsets.all(16),
      children: [
        for (var line = 0; line < 4; line++)
          Container(height: 48, margin: const EdgeInsets.only(bottom: 12), color: color),
      ],
    );
  }
}
