import 'package:flutter/material.dart';

import '../../../core/l10n/l10n.dart';

typedef DayRange = ({DateTime from, DateTime to});

/// MEM-E13-PLAT-04: the date filter in a small calendar. On a wide screen it stands at the left, under the filter; on a
/// narrow one in the middle. A tap outside closes it without changing anything. Null: closed without a choice.
Future<({DayRange? range})?> showMiniRangeCalendar(
  BuildContext context, {
  DayRange? initial,
  required DateTime lastDay,
}) {
  final wide = MediaQuery.sizeOf(context).width >= 600;
  return showGeneralDialog<({DayRange? range})>(
    context: context,
    barrierDismissible: true,
    barrierLabel: MaterialLocalizations.of(context).modalBarrierDismissLabel,
    barrierColor: Colors.black.withValues(alpha: wide ? 0.05 : 0.3),
    transitionDuration: const Duration(milliseconds: 150),
    pageBuilder: (context, _, _) => SafeArea(
      child: Align(
        alignment: wide ? Alignment.topLeft : Alignment.center,
        child: Padding(
          // Under the app bar and the filter chips.
          padding: wide ? const EdgeInsets.fromLTRB(8, 112, 8, 8) : const EdgeInsets.all(16),
          child: MiniRangeCalendar(initial: initial, lastDay: lastDay),
        ),
      ),
    ),
    transitionBuilder: (context, animation, _, child) => FadeTransition(
      opacity: animation,
      child: ScaleTransition(
        scale: Tween(begin: 0.96, end: 1.0).animate(animation),
        alignment: Alignment.topLeft,
        child: child,
      ),
    ),
  );
}

/// A month at a time: the first tap is the start, the second the end (in either order); "Apply" gives the range back,
/// "Clear" gives none.
class MiniRangeCalendar extends StatefulWidget {
  const MiniRangeCalendar({super.key, this.initial, required this.lastDay});

  static const width = 296.0;

  final DayRange? initial;
  final DateTime lastDay;

  @override
  State<MiniRangeCalendar> createState() => _MiniRangeCalendarState();
}

class _MiniRangeCalendarState extends State<MiniRangeCalendar> {
  late DateTime _month = DateTime(
    (widget.initial?.from ?? widget.lastDay).year,
    (widget.initial?.from ?? widget.lastDay).month,
  );
  late DateTime? _from = widget.initial?.from;
  late DateTime? _to = widget.initial?.to;

  static DateTime _day(DateTime date) => DateTime(date.year, date.month, date.day);

  void _tap(DateTime day) => setState(() {
    final from = _from;
    if (from == null || _to != null) {
      _from = day;
      _to = null;
    } else if (day.isBefore(from)) {
      _from = day;
      _to = from;
    } else {
      _to = day;
    }
  });

  bool _inRange(DateTime day) {
    final (from, to) = (_from, _to ?? _from);
    return from != null && to != null && !day.isBefore(from) && !day.isAfter(to);
  }

  @override
  Widget build(BuildContext context) {
    final l10n = context.l10n;
    final theme = Theme.of(context);
    final colors = theme.colorScheme;
    final names = MaterialLocalizations.of(context);
    final first = DateTime(_month.year, _month.month);
    // Monday first, as in Poland and most of Europe.
    final leading = (first.weekday + 6) % 7;
    final days = DateUtils.getDaysInMonth(_month.year, _month.month);
    final lastDay = _day(widget.lastDay);
    // narrowWeekdays starts on Sunday; Monday first.
    final weekdays = [for (var day = 1; day <= 7; day++) names.narrowWeekdays[day % 7]];
    final from = _from;

    return Material(
      key: const Key('s03-calendar'),
      elevation: 8,
      color: colors.surfaceContainerHigh,
      borderRadius: BorderRadius.circular(20),
      child: SizedBox(
        width: MiniRangeCalendar.width,
        child: Padding(
          padding: const EdgeInsets.fromLTRB(12, 8, 12, 8),
          child: Column(
            mainAxisSize: MainAxisSize.min,
            children: [
              Row(
                children: [
                  IconButton(
                    key: const Key('s03-calendar-previous'),
                    visualDensity: VisualDensity.compact,
                    icon: const Icon(Icons.chevron_left),
                    onPressed: () => setState(() => _month = DateTime(_month.year, _month.month - 1)),
                  ),
                  Expanded(
                    child: Text(
                      names.formatMonthYear(_month),
                      key: const Key('s03-calendar-month'),
                      textAlign: TextAlign.center,
                      style: theme.textTheme.titleSmall,
                    ),
                  ),
                  IconButton(
                    key: const Key('s03-calendar-next'),
                    visualDensity: VisualDensity.compact,
                    icon: const Icon(Icons.chevron_right),
                    onPressed: DateTime(_month.year, _month.month + 1).isAfter(lastDay)
                        ? null
                        : () => setState(() => _month = DateTime(_month.year, _month.month + 1)),
                  ),
                ],
              ),
              GridView.count(
                crossAxisCount: 7,
                shrinkWrap: true,
                physics: const NeverScrollableScrollPhysics(),
                children: [
                  for (final weekday in weekdays)
                    Center(
                      child: Text(weekday, style: theme.textTheme.labelSmall?.copyWith(color: colors.onSurfaceVariant)),
                    ),
                  for (var blank = 0; blank < leading; blank++) const SizedBox.shrink(),
                  for (var number = 1; number <= days; number++)
                    _DayCell(
                      day: DateTime(_month.year, _month.month, number),
                      inRange: _inRange(DateTime(_month.year, _month.month, number)),
                      edge:
                          DateTime(_month.year, _month.month, number) == from ||
                          DateTime(_month.year, _month.month, number) == _to,
                      enabled: !DateTime(_month.year, _month.month, number).isAfter(lastDay),
                      onTap: _tap,
                    ),
                ],
              ),
              OverflowBar(
                alignment: MainAxisAlignment.spaceBetween,
                overflowAlignment: OverflowBarAlignment.end,
                children: [
                  TextButton(
                    key: const Key('s03-calendar-clear'),
                    onPressed: () => Navigator.of(context).pop((range: null)),
                    child: Text(l10n.calendarClear),
                  ),
                  FilledButton(
                    key: const Key('s03-calendar-apply'),
                    onPressed: from == null
                        ? null
                        : () => Navigator.of(context).pop((range: (from: from, to: _to ?? from))),
                    child: Text(l10n.calendarApply),
                  ),
                ],
              ),
            ],
          ),
        ),
      ),
    );
  }
}

class _DayCell extends StatelessWidget {
  const _DayCell({
    required this.day,
    required this.inRange,
    required this.edge,
    required this.enabled,
    required this.onTap,
  });

  final DateTime day;
  final bool inRange;
  final bool edge;
  final bool enabled;
  final ValueChanged<DateTime> onTap;

  @override
  Widget build(BuildContext context) {
    final colors = Theme.of(context).colorScheme;
    return Padding(
      padding: const EdgeInsets.all(1.5),
      child: Material(
        color: edge ? colors.primary : (inRange ? colors.primary.withValues(alpha: 0.18) : Colors.transparent),
        shape: const CircleBorder(),
        child: InkWell(
          key: Key('s03-calendar-day-${day.year}-${day.month}-${day.day}'),
          customBorder: const CircleBorder(),
          onTap: enabled ? () => onTap(day) : null,
          child: Center(
            child: Text(
              '${day.day}',
              style: TextStyle(
                fontSize: 13,
                color: !enabled
                    ? colors.onSurface.withValues(alpha: 0.3)
                    : (edge ? colors.onPrimary : colors.onSurface),
              ),
            ),
          ),
        ),
      ),
    );
  }
}
