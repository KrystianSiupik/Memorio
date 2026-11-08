import 'package:flutter/material.dart';
import 'package:flutter_riverpod/flutter_riverpod.dart';

import '../../../core/l10n/l10n.dart';
import '../../memories/memories.dart';

/// MEM-E12-PLAT-01, MEM-E13-PLAT-10: the card next to a hovered point. The first photo fills the card and the text sits
/// white on a grey band over it; without a photo the card is grey. Loading and a failure stay small: the map is what
/// matters.
class MemoryPreviewCard extends ConsumerWidget {
  const MemoryPreviewCard({super.key, required this.memoryId});

  static const width = 260.0;

  /// The grey under the text, a little see-through over a photo.
  static const band = Color(0xD9303236);

  final String memoryId;

  @override
  Widget build(BuildContext context, WidgetRef ref) {
    final l10n = context.l10n;
    final preview = ref.watch(memoryPreviewProvider(memoryId));
    return IgnorePointer(
      child: SizedBox(
        width: width,
        child: Card(
          key: const Key('s03-preview'),
          clipBehavior: Clip.antiAlias,
          elevation: 6,
          color: const Color(0xFF303236),
          shape: RoundedRectangleBorder(borderRadius: BorderRadius.circular(14)),
          child: switch (preview) {
            AsyncValue(:final value?) => _Loaded(preview: value),
            AsyncValue(hasError: true) => Padding(
              padding: const EdgeInsets.all(12),
              child: Text(l10n.mapPreviewUnavailable, style: const TextStyle(color: Colors.white)),
            ),
            _ => const Padding(padding: EdgeInsets.all(12), child: LinearProgressIndicator()),
          },
        ),
      ),
    );
  }
}

class _Loaded extends StatelessWidget {
  const _Loaded({required this.preview});

  final MemoryPreview preview;

  @override
  Widget build(BuildContext context) {
    final l10n = context.l10n;
    final text = Theme.of(context).textTheme;
    final details = Container(
      key: const Key('s03-preview-band'),
      width: double.infinity,
      color: MemoryPreviewCard.band,
      padding: const EdgeInsets.fromLTRB(12, 10, 12, 12),
      child: Column(
        crossAxisAlignment: CrossAxisAlignment.start,
        mainAxisSize: MainAxisSize.min,
        children: [
          Text(
            preview.placeName,
            style: text.titleSmall?.copyWith(color: Colors.white, fontWeight: FontWeight.w700),
          ),
          Text(
            memoryDateText(preview.datePrecision, preview.dateFrom, preview.dateTo, preview.time),
            style: text.bodySmall?.copyWith(color: Colors.white70),
          ),
          if (preview.excerpt case final excerpt?) ...[
            const SizedBox(height: 6),
            Text(
              excerpt,
              key: const Key('s03-preview-excerpt'),
              maxLines: 4,
              overflow: TextOverflow.ellipsis,
              style: text.bodyMedium?.copyWith(color: Colors.white),
            ),
          ],
          if (preview.photos > 1) ...[
            const SizedBox(height: 4),
            Text(l10n.mapPreviewPhotos(preview.photos), style: text.labelSmall?.copyWith(color: Colors.white70)),
          ],
        ],
      ),
    );
    final url = preview.photoUrl;
    if (url == null) {
      return details;
    }
    return ConstrainedBox(
      constraints: const BoxConstraints(minHeight: 200),
      child: Stack(
        children: [
          Positioned.fill(
            child: Image.network(
              url,
              key: const Key('s03-preview-photo'),
              fit: BoxFit.cover,
              errorBuilder: (context, error, stack) => const SizedBox.shrink(),
            ),
          ),
          Positioned(left: 0, right: 0, bottom: 0, child: details),
          // Keeps the card at least as high as the photo needs, the band at the bottom.
          const SizedBox(height: 200, width: MemoryPreviewCard.width),
        ],
      ),
    );
  }
}
