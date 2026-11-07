import 'dart:typed_data';

import 'package:flutter/material.dart';

import '../../../core/l10n/l10n.dart';
import '../domain/memory.dart';

/// MEM-E13-PLAT-08: the photos picked for a new memory, before they are sent: a big picture with arrows and a
/// counter, small ones below to jump to, and a button to take the shown one out. With none yet, a field asks for them.
class PhotoCarousel extends StatefulWidget {
  const PhotoCarousel({super.key, required this.photos, required this.onRemove, required this.onAdd});

  /// The photos as (id, photo), in the order they were picked.
  final List<(String, PickedPhoto)> photos;
  final ValueChanged<String> onRemove;

  /// Null while photos cannot be added (busy, or the pool is full).
  final VoidCallback? onAdd;

  @override
  State<PhotoCarousel> createState() => _PhotoCarouselState();
}

class _PhotoCarouselState extends State<PhotoCarousel> {
  final _pages = PageController();
  final _bytes = <String, Future<Uint8List>>{};
  var _current = 0;

  @override
  void dispose() {
    _pages.dispose();
    super.dispose();
  }

  @override
  void didUpdateWidget(PhotoCarousel oldWidget) {
    super.didUpdateWidget(oldWidget);
    final count = widget.photos.length;
    if (count > oldWidget.photos.length && count > 0) {
      // New photos: show the first of them.
      _current = oldWidget.photos.length;
      WidgetsBinding.instance.addPostFrameCallback((_) {
        if (_pages.hasClients) {
          _pages.jumpToPage(_current);
        }
      });
    } else if (_current >= count) {
      _current = count == 0 ? 0 : count - 1;
    }
  }

  Future<Uint8List> _read(String id, PickedPhoto photo) =>
      _bytes.putIfAbsent(id, () async => Uint8List.fromList(await photo.read()));

  void _show(int index) {
    setState(() => _current = index);
    _pages.animateToPage(index, duration: const Duration(milliseconds: 250), curve: Curves.easeOut);
  }

  @override
  Widget build(BuildContext context) {
    final l10n = context.l10n;
    final colors = Theme.of(context).colorScheme;
    final photos = widget.photos;
    if (photos.isEmpty) {
      return InkWell(
        key: const Key('s04-photos-empty'),
        onTap: widget.onAdd,
        borderRadius: BorderRadius.circular(14),
        child: Container(
          height: 140,
          alignment: Alignment.center,
          decoration: BoxDecoration(
            color: colors.surfaceContainerHighest.withValues(alpha: 0.4),
            borderRadius: BorderRadius.circular(14),
            border: Border.all(color: colors.outlineVariant, width: 1.5),
          ),
          child: Column(
            mainAxisSize: MainAxisSize.min,
            children: [
              Icon(Icons.add_photo_alternate_outlined, size: 40, color: colors.primary),
              const SizedBox(height: 8),
              Padding(
                padding: const EdgeInsets.symmetric(horizontal: 16),
                child: Text(
                  l10n.photosEmpty,
                  textAlign: TextAlign.center,
                  style: TextStyle(color: colors.onSurfaceVariant),
                ),
              ),
            ],
          ),
        ),
      );
    }
    final (shownId, _) = photos[_current];
    return Column(
      crossAxisAlignment: CrossAxisAlignment.stretch,
      children: [
        SizedBox(
          height: 240,
          child: ClipRRect(
            borderRadius: BorderRadius.circular(14),
            child: Stack(
              children: [
                Positioned.fill(
                  child: ColoredBox(
                    color: Colors.black,
                    child: PageView.builder(
                      key: const Key('s04-carousel'),
                      controller: _pages,
                      itemCount: photos.length,
                      onPageChanged: (index) => setState(() => _current = index),
                      itemBuilder: (context, index) {
                        final (id, photo) = photos[index];
                        return _PhotoImage(bytes: _read(id, photo), fit: BoxFit.contain);
                      },
                    ),
                  ),
                ),
                if (_current > 0)
                  Positioned(
                    left: 8,
                    top: 0,
                    bottom: 0,
                    child: Center(
                      child: _OverlayButton(
                        key: const Key('s04-photo-previous'),
                        icon: Icons.chevron_left,
                        tooltip: l10n.photoPrevious,
                        onPressed: () => _show(_current - 1),
                      ),
                    ),
                  ),
                if (_current < photos.length - 1)
                  Positioned(
                    right: 8,
                    top: 0,
                    bottom: 0,
                    child: Center(
                      child: _OverlayButton(
                        key: const Key('s04-photo-next'),
                        icon: Icons.chevron_right,
                        tooltip: l10n.photoNext,
                        onPressed: () => _show(_current + 1),
                      ),
                    ),
                  ),
                Positioned(
                  top: 8,
                  right: 8,
                  child: _OverlayButton(
                    key: const Key('s04-photo-remove'),
                    icon: Icons.delete_outline,
                    tooltip: l10n.photoRemove,
                    onPressed: () => widget.onRemove(shownId),
                  ),
                ),
                Positioned(
                  bottom: 8,
                  left: 0,
                  right: 0,
                  child: Center(
                    child: Container(
                      padding: const EdgeInsets.symmetric(horizontal: 10, vertical: 3),
                      decoration: BoxDecoration(color: Colors.black54, borderRadius: BorderRadius.circular(12)),
                      child: Text(
                        '${_current + 1} / ${photos.length}',
                        key: const Key('s04-photo-counter'),
                        style: const TextStyle(color: Colors.white, fontWeight: FontWeight.w600),
                      ),
                    ),
                  ),
                ),
              ],
            ),
          ),
        ),
        const SizedBox(height: 8),
        SizedBox(
          height: 60,
          child: ListView.separated(
            scrollDirection: Axis.horizontal,
            itemCount: photos.length,
            separatorBuilder: (context, index) => const SizedBox(width: 8),
            itemBuilder: (context, index) {
              final (id, photo) = photos[index];
              final chosen = index == _current;
              return GestureDetector(
                key: Key('s04-photo-thumb-$index'),
                onTap: () => _show(index),
                child: AnimatedContainer(
                  duration: const Duration(milliseconds: 150),
                  width: 60,
                  decoration: BoxDecoration(
                    borderRadius: BorderRadius.circular(10),
                    border: Border.all(color: chosen ? colors.primary : colors.outlineVariant, width: chosen ? 2.5 : 1),
                  ),
                  child: ClipRRect(
                    borderRadius: BorderRadius.circular(8),
                    child: _PhotoImage(bytes: _read(id, photo), fit: BoxFit.cover),
                  ),
                ),
              );
            },
          ),
        ),
      ],
    );
  }
}

class _PhotoImage extends StatelessWidget {
  const _PhotoImage({required this.bytes, required this.fit});

  final Future<Uint8List> bytes;
  final BoxFit fit;

  @override
  Widget build(BuildContext context) => FutureBuilder<Uint8List>(
    future: bytes,
    builder: (context, read) {
      final data = read.data;
      if (data == null) {
        return const Center(child: Icon(Icons.image_outlined, color: Colors.white54));
      }
      return Image.memory(
        data,
        fit: fit,
        gaplessPlayback: true,
        errorBuilder: (context, error, stack) =>
            const Center(child: Icon(Icons.broken_image_outlined, color: Colors.white54)),
      );
    },
  );
}

class _OverlayButton extends StatelessWidget {
  const _OverlayButton({super.key, required this.icon, required this.tooltip, required this.onPressed});

  final IconData icon;
  final String tooltip;
  final VoidCallback onPressed;

  @override
  Widget build(BuildContext context) => IconButton.filled(
    tooltip: tooltip,
    onPressed: onPressed,
    style: IconButton.styleFrom(backgroundColor: Colors.white.withValues(alpha: 0.9), foregroundColor: Colors.black87),
    icon: Icon(icon),
  );
}
