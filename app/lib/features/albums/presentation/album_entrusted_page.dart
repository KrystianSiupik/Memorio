import 'package:flutter/material.dart';
import 'package:flutter_riverpod/flutter_riverpod.dart';

import '../../../core/l10n/l10n.dart';
import '../../memories/memories.dart' show dayTextOf;
import '../domain/album.dart';
import 'albums_providers.dart';

/// S13 What others entrusted to this person, album by album; the order changes here and is sent with "Uporządkuj".
class AlbumEntrustedPage extends ConsumerStatefulWidget {
  const AlbumEntrustedPage({super.key});

  @override
  ConsumerState<AlbumEntrustedPage> createState() => _AlbumEntrustedPageState();
}

class _AlbumEntrustedPageState extends ConsumerState<AlbumEntrustedPage> with AlbumActions {
  final _orders = <String, List<EntrustedMemory>>{};

  List<EntrustedMemory> _orderOf(String albumId, List<EntrustedMemory> known) => _orders[albumId] ?? known;

  void _move(String albumId, List<EntrustedMemory> current, int from, int to) {
    final moved = [...current];
    moved.insert(to, moved.removeAt(from));
    setState(() => _orders[albumId] = moved);
  }

  Future<void> _arrange(String albumId, List<EntrustedMemory> order) async {
    if (await act(() => albums.arrange(albumId, [for (final memory in order) memory.memoryId]))) {
      setState(() => _orders.remove(albumId));
      ref.invalidate(entrustedProvider);
      ref.invalidate(albumDetailsProvider(albumId));
    }
  }

  @override
  Widget build(BuildContext context) {
    final l10n = context.l10n;
    final entrusted = ref.watch(entrustedProvider);

    return Scaffold(
      appBar: AppBar(title: Text(l10n.albumEntrusted)),
      body: Column(
        children: [
          ...offlineBlock(context),
          Expanded(
            child: AlbumAsyncBody(
              value: entrusted,
              onRetry: () => ref.invalidate(entrustedProvider),
              builder: (items) {
                if (items.isEmpty) {
                  return Center(child: Text(l10n.albumEntrustedEmpty));
                }
                final byAlbum = <String, List<EntrustedMemory>>{};
                for (final item in items) {
                  byAlbum.putIfAbsent(item.albumId, () => []).add(item);
                }
                return ListView(
                  padding: const EdgeInsets.all(16),
                  children: [
                    ...failureBlock(context),
                    for (final MapEntry(key: albumId, value: known) in byAlbum.entries) ...[
                      Text(known.first.albumName, style: Theme.of(context).textTheme.titleMedium),
                      for (final (index, memory) in _orderOf(albumId, known).indexed)
                        ListTile(
                          key: Key('s13-memory-${memory.memoryId}'),
                          title: Text(memory.placeName),
                          subtitle: Text(dayTextOf(memory.dateFrom)),
                          trailing: Wrap(
                            children: [
                              IconButton(
                                tooltip: l10n.albumMoveUp,
                                icon: const Icon(Icons.arrow_upward),
                                onPressed: index == 0
                                    ? null
                                    : () => _move(albumId, _orderOf(albumId, known), index, index - 1),
                              ),
                              IconButton(
                                tooltip: l10n.albumMoveDown,
                                icon: const Icon(Icons.arrow_downward),
                                onPressed: index == known.length - 1
                                    ? null
                                    : () => _move(albumId, _orderOf(albumId, known), index, index + 1),
                              ),
                            ],
                          ),
                        ),
                      FilledButton(
                        onPressed: offline || busy ? null : () => _arrange(albumId, _orderOf(albumId, known)),
                        child: Text(l10n.albumArrange),
                      ),
                      const SizedBox(height: 16),
                    ],
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
