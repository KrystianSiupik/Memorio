import 'dart:async';

import 'package:flutter/material.dart';
import 'package:flutter_riverpod/flutter_riverpod.dart';
import 'package:uuid/uuid.dart';

import '../../../core/l10n/l10n.dart';
import '../../../core/offline/offline.dart';
import '../../../core/result.dart';
import '../../account/account.dart';
import '../../map/map.dart';
import '../../memories/memories.dart' show dayTextOf, noAutomaticRetry;
import '../domain/replay.dart';

/// Overridden in app/ with the API repository, and in tests with a fake.
final replayRepositoryProvider = Provider<ReplayRepository>(
  (ref) => throw UnimplementedError('replayRepositoryProvider is overridden in app/'),
);

/// A period as two days, yyyy-MM-dd.
typedef Period = ({String from, String to});

final periodProvider = FutureProvider.family<List<PeriodMemory>, Period>((ref, period) async {
  ref.watch(sessionProvider.select((session) => session.value?.accountId));
  return switch (await ref.watch(replayRepositoryProvider).loadPeriod(period.from, period.to)) {
    Ok(:final value) => value,
    Err(:final failure) => throw failure,
  };
}, retry: noAutomaticRetry);

String _day(DateTime date) =>
    '${date.year.toString().padLeft(4, '0')}-${date.month.toString().padLeft(2, '0')}-${date.day.toString().padLeft(2, '0')}';

/// The date as exactly as the person gave it, never more (S2:C1).
String periodDateText(PeriodMemory memory) {
  final from = memory.dateFrom.split('-');
  return switch (memory.datePrecision) {
    'Year' => from[0],
    'Month' => '${from[1]}.${from[0]}',
    'Period' => '${dayTextOf(memory.dateFrom)} – ${dayTextOf(memory.dateTo)}',
    _ => dayTextOf(memory.dateFrom),
  };
}

String replayFailureText(AppLocalizations l10n, Object? failure) => switch (failure) {
  ReplayOffline() => l10n.authOffline,
  ReplayNoAccess() => l10n.albumNoAccess,
  ReplayTooLong() => l10n.replayTooLong,
  ReplayInvalidInput() => l10n.invalidInput,
  _ => l10n.serverError,
};

/// S08 Replay a period: the memories one after another, the map going to each (§8). It waits for "play", and the
/// map shows only this period while the screen is open.
class ReplayPage extends ConsumerStatefulWidget {
  const ReplayPage({super.key, required this.onOpenMemory, required this.onOpenAlbum, this.today});

  final void Function(String memoryId) onOpenMemory;
  final void Function(String albumId) onOpenAlbum;

  /// For tests; otherwise the day of the device.
  final DateTime? today;

  @override
  ConsumerState<ReplayPage> createState() => _ReplayPageState();
}

class _ReplayPageState extends ConsumerState<ReplayPage> {
  static const _tempos = [6, 3];

  late final DateTime _today = widget.today ?? DateTime.now();
  late Period _period = (from: '${_today.year}-01-01', to: _day(_today));
  var _index = 0;
  var _playing = false;
  var _tempo = 0;
  Timer? _timer;
  var _busy = false;
  ReplayFailure? _failure;
  PeriodAlbum? _created;
  String? _createdId;
  late final ReplayPointsNotifier _points;
  late final MapFocusNotifier _focus;
  ProviderSubscription<AsyncValue<List<PeriodMemory>>>? _loaded;

  @override
  void initState() {
    super.initState();
    _points = ref.read(replayPointsProvider.notifier);
    _focus = ref.read(mapFocusProvider.notifier);
    _follow();
  }

  /// The map follows the period: its points, and the first memory. After the frame, as providers may not change during one.
  void _follow() {
    _loaded?.close();
    _loaded = ref.listenManual(periodProvider(_period), (previous, next) {
      final memories = next.value;
      if (memories == null) {
        return;
      }
      Future.microtask(() {
        if (!mounted) {
          return;
        }
        _points.show([
          for (final memory in memories)
            MapPoint(id: memory.id, latitude: memory.latitude, longitude: memory.longitude),
        ]);
        _go(0, memories);
      });
    }, fireImmediately: true);
  }

  @override
  void dispose() {
    _timer?.cancel();
    _loaded?.close();
    // The map tab goes back to all memories, after the frame: providers may not change while the tree changes.
    final (points, focus) = (_points, _focus);
    Future.microtask(() {
      points.clear();
      focus.clear();
    });
    super.dispose();
  }

  void _choose(Period period) {
    _timer?.cancel();
    setState(() {
      _period = period;
      _index = 0;
      _playing = false;
      _created = null;
      _createdId = null;
      _failure = null;
    });
    _follow();
  }

  Future<void> _pickRange() async {
    final picked = await showDateRangePicker(
      context: context,
      firstDate: DateTime(1900),
      lastDate: DateTime(_today.year, _today.month, _today.day),
    );
    if (picked != null) {
      _choose((from: _day(picked.start), to: _day(picked.end)));
    }
  }

  void _go(int index, List<PeriodMemory> memories) {
    if (memories.isEmpty) {
      return;
    }
    final last = index >= memories.length;
    setState(() => _index = last ? memories.length : index.clamp(0, memories.length - 1));
    if (last) {
      _stop();
      return;
    }
    final memory = memories[_index];
    _focus.focus(MapPoint(id: memory.id, latitude: memory.latitude, longitude: memory.longitude));
  }

  void _stop() {
    _timer?.cancel();
    setState(() => _playing = false);
  }

  void _play(List<PeriodMemory> memories) {
    if (_index >= memories.length) {
      _go(0, memories);
    }
    setState(() => _playing = true);
    _restartTimer(memories);
  }

  void _restartTimer(List<PeriodMemory> memories) {
    _timer?.cancel();
    _timer = Timer.periodic(Duration(seconds: _tempos[_tempo]), (_) {
      if (mounted) {
        _go(_index + 1, memories);
      }
    });
  }

  Future<void> _createAlbum(BuildContext context) async {
    final l10n = context.l10n;
    final name = TextEditingController(text: l10n.replayAlbumName(_periodText(l10n)));
    final confirmed = await showDialog<bool>(
      context: context,
      builder: (dialog) => AlertDialog(
        title: Text(l10n.replayCreateAlbum),
        content: Column(
          mainAxisSize: MainAxisSize.min,
          crossAxisAlignment: CrossAxisAlignment.start,
          children: [
            Text(l10n.replayCreateAlbumExplained),
            TextField(
              controller: name,
              decoration: InputDecoration(labelText: l10n.albumNewName),
            ),
          ],
        ),
        actions: [
          TextButton(onPressed: () => Navigator.of(dialog).pop(false), child: Text(l10n.cancel)),
          FilledButton(onPressed: () => Navigator.of(dialog).pop(true), child: Text(l10n.replayCreateAlbum)),
        ],
      ),
    );
    final typed = name.text.trim();
    name.dispose();
    if (confirmed != true || typed.isEmpty || !mounted) {
      return;
    }
    // Made once per period, so sending again after a failure is the same album.
    final id = _createdId ??= const Uuid().v7();
    setState(() {
      _busy = true;
      _failure = null;
    });
    final created = await ref.read(replayRepositoryProvider).createAlbum(id, typed, _period.from, _period.to);
    if (!mounted) {
      return;
    }
    setState(() {
      _busy = false;
      switch (created) {
        case Ok(:final value):
          _created = value;
        case Err(:final failure):
          _failure = failure;
      }
    });
  }

  String _periodText(AppLocalizations l10n) {
    final from = _period.from;
    final to = _period.to;
    final wholeYear =
        from.endsWith('-01-01') &&
        (to.endsWith('-12-31') || to == _day(_today)) &&
        from.substring(0, 4) == to.substring(0, 4);
    return wholeYear ? from.substring(0, 4) : '${dayTextOf(from)} – ${dayTextOf(to)}';
  }

  @override
  Widget build(BuildContext context) {
    final l10n = context.l10n;
    final offline = ref.watch(isOnlineProvider).value == false;
    final loaded = ref.watch(periodProvider(_period));

    return Scaffold(
      appBar: AppBar(title: Text(l10n.replayTitle)),
      body: Column(
        children: [
          if (offline)
            Padding(
              padding: const EdgeInsets.all(12),
              child: Text(l10n.noConnection, key: const Key('s08-offline')),
            ),
          Padding(
            padding: const EdgeInsets.symmetric(horizontal: 12, vertical: 4),
            child: Wrap(
              spacing: 8,
              crossAxisAlignment: WrapCrossAlignment.center,
              children: [
                Text(l10n.replayPeriod(_periodText(l10n)), key: const Key('s08-period')),
                for (final year in [_today.year, _today.year - 1, _today.year - 2])
                  ChoiceChip(
                    label: Text('$year'),
                    selected:
                        _period.from == '$year-01-01' &&
                        _period.to == (year == _today.year ? _day(_today) : '$year-12-31'),
                    onSelected: (_) =>
                        _choose((from: '$year-01-01', to: year == _today.year ? _day(_today) : '$year-12-31')),
                  ),
                ActionChip(label: Text(l10n.replayOtherRange), onPressed: _pickRange),
              ],
            ),
          ),
          Expanded(child: ref.watch(mapViewProvider)),
          Expanded(child: _panel(context, loaded, offline)),
        ],
      ),
    );
  }

  Widget _panel(BuildContext context, AsyncValue<List<PeriodMemory>> loaded, bool offline) {
    final l10n = context.l10n;
    final memories = loaded.value;
    if (memories == null) {
      if (loaded.hasError) {
        return Center(
          child: Column(
            mainAxisSize: MainAxisSize.min,
            children: [
              Text(replayFailureText(l10n, loaded.error), key: const Key('s08-error')),
              TextButton(onPressed: () => ref.invalidate(periodProvider(_period)), child: Text(l10n.retry)),
            ],
          ),
        );
      }
      final color = Theme.of(context).colorScheme.surfaceContainerHighest;
      return ListView(
        key: const Key('s08-skeleton'),
        padding: const EdgeInsets.all(16),
        children: [
          for (var line = 0; line < 3; line++)
            Container(height: 40, margin: const EdgeInsets.only(bottom: 12), color: color),
        ],
      );
    }
    if (memories.isEmpty) {
      return Center(child: Text(l10n.replayEmpty, key: const Key('s08-empty')));
    }
    if (_index >= memories.length) {
      return _end(context, memories, offline);
    }
    final memory = memories[_index];
    return ListView(
      padding: const EdgeInsets.all(16),
      children: [
        Text(l10n.replayStep(_index + 1, memories.length), key: const Key('s08-step')),
        InkWell(
          onTap: () => widget.onOpenMemory(memory.id),
          child: Text(memory.placeName, key: const Key('s08-place'), style: Theme.of(context).textTheme.titleLarge),
        ),
        Text(periodDateText(memory), key: const Key('s08-date')),
        if (memory.description case final String description)
          Padding(padding: const EdgeInsets.only(top: 8), child: Text(description)),
        if (memory.photoUrls.isNotEmpty)
          SizedBox(
            height: 96,
            child: ListView(
              scrollDirection: Axis.horizontal,
              children: [
                for (final url in memory.photoUrls)
                  Padding(
                    padding: const EdgeInsets.only(top: 8, right: 8),
                    child: Image.network(
                      url,
                      height: 88,
                      fit: BoxFit.cover,
                      errorBuilder: (context, error, stack) => const SizedBox(width: 88),
                    ),
                  ),
              ],
            ),
          ),
        const SizedBox(height: 8),
        Row(
          children: [
            IconButton(
              tooltip: l10n.replayPrevious,
              icon: const Icon(Icons.skip_previous),
              onPressed: _index == 0 ? null : () => _go(_index - 1, memories),
            ),
            IconButton(
              tooltip: _playing ? l10n.replayPause : l10n.replayPlay,
              icon: Icon(_playing ? Icons.pause : Icons.play_arrow),
              onPressed: _playing ? _stop : () => _play(memories),
            ),
            IconButton(
              tooltip: l10n.replayNext,
              icon: const Icon(Icons.skip_next),
              onPressed: () => _go(_index + 1, memories),
            ),
            TextButton(
              onPressed: () {
                setState(() => _tempo = (_tempo + 1) % _tempos.length);
                if (_playing) {
                  _restartTimer(memories);
                }
              },
              child: Text(l10n.replayTempo(_tempo + 1)),
            ),
          ],
        ),
      ],
    );
  }

  Widget _end(BuildContext context, List<PeriodMemory> memories, bool offline) {
    final l10n = context.l10n;
    final created = _created;
    final failure = _failure;
    final places = memories.map((memory) => memory.placeName).toSet().length;
    final mine = memories.where((memory) => memory.isMine).length;
    return ListView(
      key: const Key('s08-end'),
      padding: const EdgeInsets.all(16),
      children: [
        Text(l10n.replayEnd, style: Theme.of(context).textTheme.titleMedium),
        Text(l10n.replayStats(memories.length, places), key: const Key('s08-stats')),
        if (failure != null)
          Text(
            replayFailureText(l10n, failure),
            key: const Key('s08-failure'),
            style: TextStyle(color: Theme.of(context).colorScheme.error),
          ),
        if (created != null) ...[
          Text(l10n.replayAlbumCreated(created.added), key: const Key('s08-created')),
          if (created.notFitting > 0) Text(l10n.replayAlbumNotFitting(created.notFitting)),
          TextButton(onPressed: () => widget.onOpenAlbum(_createdId!), child: Text(l10n.replayOpenAlbum)),
        ] else if (mine > 0)
          FilledButton(
            onPressed: offline || _busy ? null : () => _createAlbum(context),
            child: Text(l10n.replayCreateAlbum),
          ),
        TextButton(onPressed: () => _go(0, memories), child: Text(l10n.replayAgain)),
      ],
    );
  }
}
