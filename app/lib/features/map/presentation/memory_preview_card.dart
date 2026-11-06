import 'package:flutter/material.dart';
import 'package:flutter_riverpod/flutter_riverpod.dart';

import '../../../core/l10n/l10n.dart';
import '../../memories/memories.dart';

/// MEM-E12-PLAT-01: the card next to a hovered point. Loading and a failure stay small: the map is what matters.
class MemoryPreviewCard extends ConsumerWidget {
  const MemoryPreviewCard({super.key, required this.memoryId});

  static const width = 240.0;

  final String memoryId;

  @override
  Widget build(BuildContext context, WidgetRef ref) {
    final l10n = context.l10n;
    final preview = ref.watch(memoryPreviewProvider(memoryId));
    final theme = Theme.of(context);
    return IgnorePointer(
      child: SizedBox(
        width: width,
        child: Card(
          key: const Key('s03-preview'),
          clipBehavior: Clip.antiAlias,
          elevation: 4,
          child: switch (preview) {
            AsyncValue(:final value?) => Column(
              crossAxisAlignment: CrossAxisAlignment.start,
              mainAxisSize: MainAxisSize.min,
              children: [
                if (value.photoUrl case final url?)
                  Image.network(
                    url,
                    key: const Key('s03-preview-photo'),
                    height: 140,
                    width: width,
                    fit: BoxFit.cover,
                    errorBuilder: (context, error, stack) => const SizedBox.shrink(),
                  ),
                Padding(
                  padding: const EdgeInsets.all(10),
                  child: Column(
                    crossAxisAlignment: CrossAxisAlignment.start,
                    children: [
                      Text(value.placeName, style: theme.textTheme.titleSmall),
                      Text(
                        memoryDateText(value.datePrecision, value.dateFrom, value.dateTo, value.time),
                        style: theme.textTheme.bodySmall,
                      ),
                      if (value.excerpt case final excerpt?) ...[
                        const SizedBox(height: 4),
                        Text(
                          excerpt,
                          key: const Key('s03-preview-excerpt'),
                          maxLines: 4,
                          overflow: TextOverflow.ellipsis,
                        ),
                      ],
                      if (value.photos > 1)
                        Text(l10n.mapPreviewPhotos(value.photos), style: theme.textTheme.labelSmall),
                    ],
                  ),
                ),
              ],
            ),
            AsyncValue(hasError: true) => Padding(
              padding: const EdgeInsets.all(10),
              child: Text(l10n.mapPreviewUnavailable),
            ),
            _ => const Padding(padding: EdgeInsets.all(10), child: LinearProgressIndicator()),
          },
        ),
      ),
    );
  }
}
