import 'package:flutter/material.dart';
import 'package:flutter_riverpod/flutter_riverpod.dart';

import '../../../core/l10n/l10n.dart';
import '../../../core/settings/settings.dart';

/// MEM-E13-PLAT-05: how memories look on the map, shown before it is chosen: a dot, an arrow, a round photo. The
/// picture follows the switch and the size at once.
class PointStyleSection extends ConsumerWidget {
  const PointStyleSection({super.key});

  @override
  Widget build(BuildContext context, WidgetRef ref) {
    final l10n = context.l10n;
    final settings = ref.watch(settingsProvider);
    final notifier = ref.read(settingsProvider.notifier);
    final colors = Theme.of(context).colorScheme;
    return Column(
      crossAxisAlignment: CrossAxisAlignment.start,
      children: [
        Padding(
          padding: const EdgeInsets.symmetric(vertical: 16),
          child: Row(
            mainAxisAlignment: MainAxisAlignment.center,
            children: [
              _Choice(
                selected: !settings.photoPoints,
                child: _DotPreview(scale: settings.pointScale),
              ),
              Padding(
                padding: const EdgeInsets.symmetric(horizontal: 20),
                child: Icon(Icons.arrow_forward, color: colors.onSurfaceVariant),
              ),
              _Choice(
                selected: settings.photoPoints,
                child: _PhotoPreview(scale: settings.pointScale),
              ),
            ],
          ),
        ),
        SwitchListTile(
          key: const Key('settings-photo-points'),
          value: settings.photoPoints,
          onChanged: (on) => notifier.update((current) => current.copyWith(photoPoints: on)),
          title: Text(l10n.settingsPhotoPoints),
          subtitle: Text(l10n.settingsPhotoPointsExplained),
        ),
        Padding(
          padding: const EdgeInsets.fromLTRB(16, 8, 16, 0),
          child: Text(l10n.settingsPointSize, style: Theme.of(context).textTheme.bodyMedium),
        ),
        Slider(
          key: const Key('settings-point-size'),
          value: settings.pointScale,
          min: AppSettings.minPointScale,
          max: AppSettings.maxPointScale,
          divisions: 4,
          label: '${(settings.pointScale * 100).round()}%',
          onChanged: (scale) => notifier.update((current) => current.copyWith(pointScale: scale)),
        ),
      ],
    );
  }
}

/// What is chosen now stands out; the other waits, paler.
class _Choice extends StatelessWidget {
  const _Choice({required this.selected, required this.child});

  final bool selected;
  final Widget child;

  @override
  Widget build(BuildContext context) {
    final colors = Theme.of(context).colorScheme;
    return AnimatedOpacity(
      duration: const Duration(milliseconds: 200),
      opacity: selected ? 1 : 0.45,
      child: AnimatedContainer(
        duration: const Duration(milliseconds: 200),
        width: 92,
        height: 92,
        alignment: Alignment.center,
        decoration: BoxDecoration(
          color: colors.surfaceContainerHighest.withValues(alpha: 0.5),
          borderRadius: BorderRadius.circular(18),
          border: Border.all(color: selected ? colors.primary : Colors.transparent, width: 2),
        ),
        child: child,
      ),
    );
  }
}

class _DotPreview extends StatelessWidget {
  const _DotPreview({required this.scale});

  final double scale;

  @override
  Widget build(BuildContext context) => Container(
    key: const Key('settings-preview-dot'),
    width: 12 * scale,
    height: 12 * scale,
    decoration: BoxDecoration(
      color: const Color(0xFF00796B),
      shape: BoxShape.circle,
      border: Border.all(color: Colors.white, width: 1.5),
    ),
  );
}

class _PhotoPreview extends StatelessWidget {
  const _PhotoPreview({required this.scale});

  final double scale;

  /// MEM-E13-PLAT-13: an instant print as on the map: white frame, wider at the bottom, a little askew.
  @override
  Widget build(BuildContext context) => Transform.rotate(
    angle: -0.07,
    child: Container(
      key: const Key('settings-preview-photo'),
      width: 29 * scale,
      height: 34 * scale,
      padding: EdgeInsets.fromLTRB(2.5 * scale, 2.5 * scale, 2.5 * scale, 8 * scale),
      decoration: BoxDecoration(
        color: Colors.white,
        borderRadius: BorderRadius.circular(1.5),
        boxShadow: const [BoxShadow(color: Color(0x55000000), blurRadius: 3, offset: Offset(0, 1.5))],
      ),
      child: const DecoratedBox(
        decoration: BoxDecoration(
          gradient: LinearGradient(
            begin: Alignment.topCenter,
            end: Alignment.bottomCenter,
            colors: [Color(0xFF8EC5FC), Color(0xFF7CB342)],
          ),
        ),
        child: FittedBox(
          child: Padding(
            padding: EdgeInsets.all(3),
            child: Icon(Icons.landscape, color: Colors.white),
          ),
        ),
      ),
    ),
  );
}
