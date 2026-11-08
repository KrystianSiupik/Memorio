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
  const MemoryDetailsPage({super.key, required this.memoryId, required this.onLeave, this.onReport});

  final String memoryId;

  /// S27: somebody else's memory can be reported (L19).
  final VoidCallback? onReport;

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
    final colors = Theme.of(context).colorScheme;
    final text = Theme.of(context).textTheme;
    final active = !offline && !_busy;
    final failure = _failure;
    final day = memory.dateFrom == memory.dateTo
        ? dayTextOf(memory.dateFrom)
        : '${dayTextOf(memory.dateFrom)} – ${dayTextOf(memory.dateTo)}';
    final date = memory.time == null ? day : '$day, ${memory.time}';

    return ListView(
      padding: EdgeInsets.zero,
      children: [
        _Header(placeName: memory.placeName, date: date, photoUrls: memory.photoUrls),
        Center(
          child: ConstrainedBox(
            constraints: const BoxConstraints(maxWidth: 760),
            child: Padding(
              padding: const EdgeInsets.fromLTRB(20, 20, 20, 32),
              child: Column(
                crossAxisAlignment: CrossAxisAlignment.stretch,
                spacing: 16,
                children: [
                  Wrap(
                    spacing: 8,
                    runSpacing: 8,
                    children: [
                      if (memory.isMine)
                        Chip(avatar: const Icon(Icons.person_outline, size: 18), label: Text(l10n.memoryAuthorMe)),
                      if (memory.photoUrls.isNotEmpty)
                        Chip(
                          avatar: const Icon(Icons.photo_library_outlined, size: 18),
                          label: Text(l10n.mapPreviewPhotos(memory.photoUrls.length)),
                        ),
                      if (memory.descriptionIsAi)
                        Chip(
                          key: const Key('s05-ai-marker'),
                          avatar: const Icon(Icons.auto_awesome_outlined, size: 18),
                          label: Text(l10n.aiMarker),
                        ),
                    ],
                  ),
                  _Panel(
                    icon: Icons.notes_outlined,
                    title: l10n.memoryStory,
                    child: Text(
                      memory.description ?? l10n.memoryNoDescription,
                      style: memory.description == null
                          ? text.bodyLarge?.copyWith(color: colors.onSurfaceVariant, fontStyle: FontStyle.italic)
                          : text.bodyLarge?.copyWith(height: 1.5),
                    ),
                  ),
                  _Panel(
                    icon: Icons.people_outline,
                    title: l10n.memoryPeople,
                    child: Text(l10n.memoryPeopleNone, style: TextStyle(color: colors.onSurfaceVariant)),
                  ),
                  if (failure != null)
                    Text(
                      memoryFailureText(l10n, failure),
                      key: const Key('s05-action-error'),
                      style: TextStyle(color: colors.error),
                    ),
                  if (memory.isMine)
                    _Panel(
                      key: const Key('s05-delete-zone'),
                      icon: Icons.delete_outline,
                      title: l10n.memoryDeleting,
                      accent: colors.error,
                      child: Column(
                        crossAxisAlignment: CrossAxisAlignment.start,
                        children: [
                          OutlinedButton.icon(
                            onPressed: active
                                ? () => _run(() => ref.read(memoriesRepositoryProvider).moveToTrash(memory.id))
                                : null,
                            icon: const Icon(Icons.delete_outline),
                            label: Text(l10n.deleteMine),
                          ),
                          const SizedBox(height: 12),
                          const Divider(),
                          CheckboxListTile(
                            contentPadding: EdgeInsets.zero,
                            controlAffinity: ListTileControlAffinity.leading,
                            title: Text(l10n.deleteForGoodConfirm),
                            value: _understood,
                            onChanged: active ? (value) => setState(() => _understood = value ?? false) : null,
                          ),
                          FilledButton.icon(
                            style: FilledButton.styleFrom(
                              backgroundColor: colors.error,
                              foregroundColor: colors.onError,
                            ),
                            onPressed: active && _understood
                                ? () => _run(() => ref.read(memoriesRepositoryProvider).deleteForGood(memory.id))
                                : null,
                            icon: const Icon(Icons.delete_forever_outlined),
                            label: Text(l10n.deleteForGood),
                          ),
                        ],
                      ),
                    ),
                  if (!memory.isMine && widget.onReport != null)
                    Align(
                      alignment: Alignment.centerLeft,
                      child: OutlinedButton.icon(
                        key: const Key('s05-report'),
                        onPressed: widget.onReport,
                        icon: const Icon(Icons.flag_outlined),
                        label: Text(l10n.reportContent),
                      ),
                    ),
                ],
              ),
            ),
          ),
        ),
      ],
    );
  }
}

/// MEM-E13-PLAT-11: the photos big at the top, the place and the date written on them; without photos a coloured band.
class _Header extends StatefulWidget {
  const _Header({required this.placeName, required this.date, required this.photoUrls});

  final String placeName;
  final String date;
  final List<String> photoUrls;

  @override
  State<_Header> createState() => _HeaderState();
}

class _HeaderState extends State<_Header> {
  final _pages = PageController();
  var _current = 0;

  @override
  void dispose() {
    _pages.dispose();
    super.dispose();
  }

  void _show(int index) =>
      _pages.animateToPage(index, duration: const Duration(milliseconds: 250), curve: Curves.easeOut);

  @override
  Widget build(BuildContext context) {
    final colors = Theme.of(context).colorScheme;
    final text = Theme.of(context).textTheme;
    final photos = widget.photoUrls;
    final wide = MediaQuery.sizeOf(context).width >= 720;
    final height = photos.isEmpty ? 160.0 : (wide ? 380.0 : 280.0);
    return SizedBox(
      key: const Key('s05-header'),
      height: height,
      child: Stack(
        children: [
          Positioned.fill(
            child: photos.isEmpty
                ? DecoratedBox(
                    decoration: BoxDecoration(
                      gradient: LinearGradient(
                        begin: Alignment.topLeft,
                        end: Alignment.bottomRight,
                        colors: [colors.primary, colors.tertiary],
                      ),
                    ),
                  )
                : ColoredBox(
                    color: Colors.black,
                    child: PageView.builder(
                      key: const Key('s05-photos'),
                      controller: _pages,
                      itemCount: photos.length,
                      onPageChanged: (index) => setState(() => _current = index),
                      itemBuilder: (context, index) => GestureDetector(
                        onTap: () => _openFull(context, index),
                        child: Image.network(
                          photos[index],
                          fit: BoxFit.cover,
                          errorBuilder: (context, error, stack) =>
                              const Center(child: Icon(Icons.broken_image_outlined, color: Colors.white54, size: 48)),
                        ),
                      ),
                    ),
                  ),
          ),
          // Shade at the foot so the white words read on any photo.
          const Positioned(
            left: 0,
            right: 0,
            bottom: 0,
            height: 140,
            child: IgnorePointer(
              child: DecoratedBox(
                decoration: BoxDecoration(
                  gradient: LinearGradient(
                    begin: Alignment.topCenter,
                    end: Alignment.bottomCenter,
                    colors: [Color(0x00000000), Color(0xB3000000)],
                  ),
                ),
              ),
            ),
          ),
          Positioned(
            left: 20,
            right: 20,
            bottom: 16,
            child: IgnorePointer(
              child: Column(
                crossAxisAlignment: CrossAxisAlignment.start,
                children: [
                  Text(
                    widget.placeName,
                    key: const Key('s05-place'),
                    style: text.headlineMedium?.copyWith(color: Colors.white, fontWeight: FontWeight.w700),
                  ),
                  const SizedBox(height: 6),
                  Row(
                    children: [
                      const Icon(Icons.event_outlined, color: Colors.white, size: 18),
                      const SizedBox(width: 6),
                      Flexible(
                        child: Text(
                          widget.date,
                          key: const Key('s05-date'),
                          style: text.titleSmall?.copyWith(color: Colors.white),
                        ),
                      ),
                      if (photos.length > 1) ...[
                        const Spacer(),
                        Text(
                          '${_current + 1} / ${photos.length}',
                          key: const Key('s05-photo-counter'),
                          style: text.labelLarge?.copyWith(color: Colors.white),
                        ),
                      ],
                    ],
                  ),
                ],
              ),
            ),
          ),
          if (photos.length > 1 && _current > 0)
            Positioned(
              left: 12,
              top: 0,
              bottom: 60,
              child: Center(
                child: _RoundButton(
                  key: const Key('s05-photo-previous'),
                  icon: Icons.chevron_left,
                  onPressed: () => _show(_current - 1),
                ),
              ),
            ),
          if (photos.length > 1 && _current < photos.length - 1)
            Positioned(
              right: 12,
              top: 0,
              bottom: 60,
              child: Center(
                child: _RoundButton(
                  key: const Key('s05-photo-next'),
                  icon: Icons.chevron_right,
                  onPressed: () => _show(_current + 1),
                ),
              ),
            ),
        ],
      ),
    );
  }

  Future<void> _openFull(BuildContext context, int index) => showDialog<void>(
    context: context,
    builder: (context) => Dialog.fullscreen(
      key: const Key('s05-photo-full'),
      backgroundColor: Colors.black,
      child: Stack(
        children: [
          PageView.builder(
            controller: PageController(initialPage: index),
            itemCount: widget.photoUrls.length,
            itemBuilder: (context, page) => InteractiveViewer(
              child: Center(
                child: Image.network(
                  widget.photoUrls[page],
                  fit: BoxFit.contain,
                  errorBuilder: (context, error, stack) =>
                      const Icon(Icons.broken_image_outlined, color: Colors.white54, size: 48),
                ),
              ),
            ),
          ),
          Positioned(
            top: 12,
            right: 12,
            child: SafeArea(
              child: _RoundButton(
                key: const Key('s05-photo-full-close'),
                icon: Icons.close,
                onPressed: () => Navigator.of(context).pop(),
              ),
            ),
          ),
        ],
      ),
    ),
  );
}

class _RoundButton extends StatelessWidget {
  const _RoundButton({super.key, required this.icon, required this.onPressed});

  final IconData icon;
  final VoidCallback onPressed;

  @override
  Widget build(BuildContext context) => IconButton.filled(
    onPressed: onPressed,
    style: IconButton.styleFrom(backgroundColor: Colors.white.withValues(alpha: 0.9), foregroundColor: Colors.black87),
    icon: Icon(icon),
  );
}

/// A part of the details in a soft frame, with an icon and a title.
class _Panel extends StatelessWidget {
  const _Panel({super.key, required this.icon, required this.title, required this.child, this.accent});

  final IconData icon;
  final String title;
  final Widget child;
  final Color? accent;

  @override
  Widget build(BuildContext context) {
    final colors = Theme.of(context).colorScheme;
    final tone = accent ?? colors.primary;
    return Material(
      color: accent == null ? colors.surfaceContainerLow : colors.errorContainer.withValues(alpha: 0.25),
      shape: RoundedRectangleBorder(
        borderRadius: BorderRadius.circular(16),
        side: BorderSide(color: accent == null ? colors.outlineVariant : tone.withValues(alpha: 0.5)),
      ),
      child: Padding(
        padding: const EdgeInsets.all(16),
        child: Column(
          crossAxisAlignment: CrossAxisAlignment.start,
          children: [
            Row(
              children: [
                Icon(icon, color: tone, size: 20),
                const SizedBox(width: 8),
                Text(title, style: Theme.of(context).textTheme.titleMedium?.copyWith(fontWeight: FontWeight.w700)),
              ],
            ),
            const SizedBox(height: 10),
            child,
          ],
        ),
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
      key: const Key('s05-skeleton'),
      padding: const EdgeInsets.all(16),
      children: [
        for (var line = 0; line < 6; line++)
          Container(height: 16, margin: const EdgeInsets.only(bottom: 12), color: color),
      ],
    );
  }
}
