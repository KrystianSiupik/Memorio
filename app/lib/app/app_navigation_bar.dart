import 'package:flutter/material.dart';

/// One tab of [AppNavigationBar].
class AppNavigationItem {
  const AppNavigationItem({required this.icon, required this.selectedIcon, required this.label});

  final IconData icon;
  final IconData selectedIcon;
  final String label;
}

/// MEM-E13-PLAT-03, MEM-E13-PLAT-06: the tabs on a low pad in the middle, white in the light look and dark grey in
/// the dark one, with a border and a shadow. Each icon sits in a round badge that sticks out above the pad, its label
/// on the pad. The icon under the mouse takes the accent (blue, or gold in the dark), grows a little and rises; the
/// chosen tab has its badge in the accent.
class AppNavigationBar extends StatelessWidget {
  const AppNavigationBar({super.key, required this.items, required this.selectedIndex, required this.onSelected});

  static const maxWidth = 400.0;

  /// The pad is lower than the tabs: the badges stand out of it by about half.
  static const padHeight = 46.0;
  static const height = 80.0;
  static const iconSize = 28.0;

  final List<AppNavigationItem> items;
  final int selectedIndex;
  final ValueChanged<int> onSelected;

  @override
  Widget build(BuildContext context) {
    final colors = Theme.of(context).colorScheme;
    final dark = Theme.of(context).brightness == Brightness.dark;
    return SafeArea(
      top: false,
      child: Padding(
        padding: const EdgeInsets.fromLTRB(16, 0, 16, 10),
        child: Center(
          heightFactor: 1,
          child: ConstrainedBox(
            constraints: const BoxConstraints(maxWidth: maxWidth),
            child: SizedBox(
              key: const Key('app-navigation'),
              height: height,
              child: Stack(
                clipBehavior: Clip.none,
                children: [
                  Positioned(
                    left: 0,
                    right: 0,
                    bottom: 0,
                    height: padHeight,
                    child: DecoratedBox(
                      key: const Key('app-navigation-pad'),
                      decoration: BoxDecoration(
                        color: _padColor(dark),
                        borderRadius: BorderRadius.circular(18),
                        border: Border.all(color: colors.outlineVariant),
                        boxShadow: [
                          BoxShadow(
                            color: Colors.black.withValues(alpha: dark ? 0.45 : 0.12),
                            blurRadius: 16,
                            offset: const Offset(0, 5),
                          ),
                        ],
                      ),
                    ),
                  ),
                  Positioned.fill(
                    child: Row(
                      children: [
                        for (final (index, item) in items.indexed)
                          Expanded(
                            child: _NavigationTab(
                              item: item,
                              selected: index == selectedIndex,
                              onTap: () => onSelected(index),
                            ),
                          ),
                      ],
                    ),
                  ),
                ],
              ),
            ),
          ),
        ),
      ),
    );
  }

  static Color _padColor(bool dark) => dark ? const Color(0xFF2A2B30) : Colors.white;
}

class _NavigationTab extends StatefulWidget {
  const _NavigationTab({required this.item, required this.selected, required this.onTap});

  final AppNavigationItem item;
  final bool selected;
  final VoidCallback onTap;

  @override
  State<_NavigationTab> createState() => _NavigationTabState();
}

class _NavigationTabState extends State<_NavigationTab> {
  var _hovered = false;

  @override
  Widget build(BuildContext context) {
    final colors = Theme.of(context).colorScheme;
    final dark = Theme.of(context).brightness == Brightness.dark;
    final selected = widget.selected;
    final iconColor = selected ? colors.onPrimary : (_hovered ? colors.primary : colors.onSurfaceVariant);
    final labelColor = selected || _hovered ? colors.primary : colors.onSurfaceVariant;
    const duration = Duration(milliseconds: 160);
    return Semantics(
      button: true,
      selected: selected,
      label: widget.item.label,
      excludeSemantics: true,
      child: MouseRegion(
        cursor: SystemMouseCursors.click,
        onEnter: (_) => setState(() => _hovered = true),
        onExit: (_) => setState(() => _hovered = false),
        child: GestureDetector(
          behavior: HitTestBehavior.opaque,
          onTap: widget.onTap,
          child: Column(
            mainAxisAlignment: MainAxisAlignment.end,
            children: [
              AnimatedSlide(
                key: Key('app-navigation-icon-${widget.item.label}'),
                duration: duration,
                curve: Curves.easeOut,
                offset: _hovered ? const Offset(0, -0.15) : Offset.zero,
                child: AnimatedScale(
                  duration: duration,
                  curve: Curves.easeOut,
                  scale: _hovered ? 1.12 : 1,
                  child: AnimatedContainer(
                    key: Key('app-navigation-badge-${widget.item.label}'),
                    duration: duration,
                    width: 48,
                    height: 48,
                    alignment: Alignment.center,
                    decoration: BoxDecoration(
                      shape: BoxShape.circle,
                      color: selected ? colors.primary : AppNavigationBar._padColor(dark),
                      border: Border.all(color: selected ? colors.primary : colors.outlineVariant),
                      boxShadow: [
                        BoxShadow(
                          color: Colors.black.withValues(alpha: dark ? 0.4 : (_hovered ? 0.18 : 0.1)),
                          blurRadius: _hovered ? 10 : 6,
                          offset: const Offset(0, 3),
                        ),
                      ],
                    ),
                    child: Icon(
                      selected ? widget.item.selectedIcon : widget.item.icon,
                      color: iconColor,
                      size: AppNavigationBar.iconSize,
                    ),
                  ),
                ),
              ),
              const SizedBox(height: 2),
              Text(
                widget.item.label,
                key: Key('app-navigation-label-${widget.item.label}'),
                maxLines: 1,
                overflow: TextOverflow.ellipsis,
                style: Theme.of(context).textTheme.labelMedium
                    ?.copyWith(color: labelColor, fontWeight: selected ? FontWeight.w700 : FontWeight.w500),
              ),
              const SizedBox(height: 7),
            ],
          ),
        ),
      ),
    );
  }
}
