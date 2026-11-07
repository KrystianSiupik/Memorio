import 'package:flutter/material.dart';

/// One tab of [AppNavigationBar].
class AppNavigationItem {
  const AppNavigationItem({required this.icon, required this.selectedIcon, required this.label});

  final IconData icon;
  final IconData selectedIcon;
  final String label;
}

/// MEM-E13-PLAT-03: the tabs as a small floating bar in the middle, white in the light look and dark grey in the dark
/// one, with a border and a shadow. The icon under the mouse takes the accent (blue, or gold in the dark), grows a
/// little and rises out of the bar; the chosen tab keeps the accent.
class AppNavigationBar extends StatelessWidget {
  const AppNavigationBar({super.key, required this.items, required this.selectedIndex, required this.onSelected});

  static const maxWidth = 440.0;

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
        padding: const EdgeInsets.fromLTRB(16, 4, 16, 12),
        child: Center(
          heightFactor: 1,
          child: ConstrainedBox(
            constraints: const BoxConstraints(maxWidth: maxWidth),
            child: DecoratedBox(
              key: const Key('app-navigation'),
              decoration: BoxDecoration(
                color: dark ? const Color(0xFF2A2B30) : Colors.white,
                borderRadius: BorderRadius.circular(24),
                border: Border.all(color: colors.outlineVariant),
                boxShadow: [
                  BoxShadow(
                    color: Colors.black.withValues(alpha: dark ? 0.45 : 0.12),
                    blurRadius: 18,
                    offset: const Offset(0, 6),
                  ),
                ],
              ),
              child: Padding(
                padding: const EdgeInsets.symmetric(horizontal: 6, vertical: 6),
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
            ),
          ),
        ),
      ),
    );
  }
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
    final lit = _hovered || widget.selected;
    final color = lit ? colors.primary : colors.onSurfaceVariant;
    const duration = Duration(milliseconds: 160);
    return Semantics(
      button: true,
      selected: widget.selected,
      label: widget.item.label,
      excludeSemantics: true,
      child: MouseRegion(
        cursor: SystemMouseCursors.click,
        onEnter: (_) => setState(() => _hovered = true),
        onExit: (_) => setState(() => _hovered = false),
        child: GestureDetector(
          behavior: HitTestBehavior.opaque,
          onTap: widget.onTap,
          child: Padding(
            padding: const EdgeInsets.symmetric(vertical: 4),
            child: Column(
              mainAxisSize: MainAxisSize.min,
              children: [
                AnimatedSlide(
                  key: Key('app-navigation-icon-${widget.item.label}'),
                  duration: duration,
                  curve: Curves.easeOut,
                  offset: _hovered ? const Offset(0, -0.25) : Offset.zero,
                  child: AnimatedScale(
                    duration: duration,
                    curve: Curves.easeOut,
                    scale: _hovered ? 1.2 : 1,
                    child: AnimatedContainer(
                      duration: duration,
                      padding: const EdgeInsets.symmetric(horizontal: 14, vertical: 3),
                      decoration: BoxDecoration(
                        color: widget.selected ? colors.primary.withValues(alpha: 0.14) : Colors.transparent,
                        borderRadius: BorderRadius.circular(14),
                      ),
                      child: Icon(
                        widget.selected ? widget.item.selectedIcon : widget.item.icon,
                        color: color,
                        size: 22,
                      ),
                    ),
                  ),
                ),
                const SizedBox(height: 2),
                Text(
                  widget.item.label,
                  maxLines: 1,
                  overflow: TextOverflow.ellipsis,
                  style: Theme.of(context).textTheme.labelSmall
                      ?.copyWith(color: color, fontWeight: widget.selected ? FontWeight.w600 : FontWeight.w400),
                ),
              ],
            ),
          ),
        ),
      ),
    );
  }
}
