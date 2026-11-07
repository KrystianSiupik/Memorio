import 'dart:async';

import 'package:flutter/foundation.dart';
import 'package:flutter/material.dart';
import 'package:flutter_riverpod/flutter_riverpod.dart';
import 'package:uuid/uuid.dart';

import '../../../core/l10n/l10n.dart';
import '../../../core/offline/offline.dart';
import '../../../core/result.dart';
import '../../account/account.dart';
import '../domain/memory.dart';
import '../domain/memory_failure.dart';
import '../domain/memory_operations.dart';
import 'memories_providers.dart';
import 'memory_texts.dart';

/// S04 New memory, a wizard: place and date, then content. The ids are made once per screen,
/// so saving again after a failure repeats the same operations instead of doubling them.
class NewMemoryPage extends ConsumerStatefulWidget {
  const NewMemoryPage({
    super.key,
    required this.onSaved,
    required this.onClose,
    this.isWeb = kIsWeb,
    this.initialLatitude,
    this.initialLongitude,
  });

  final VoidCallback onSaved;

  /// MEM-E13-PLAT-07: the X and Cancel; nothing else closes the window.
  final VoidCallback onClose;

  /// MEM-E12-PLAT-02: the point chosen on the map, filled in as an own place; the person names it.
  final double? initialLatitude;
  final double? initialLongitude;

  /// The offline queue has no database in a browser (D-115).
  final bool isWeb;

  @override
  ConsumerState<NewMemoryPage> createState() => _NewMemoryPageState();
}

class _NewMemoryPageState extends ConsumerState<NewMemoryPage> {
  static const _uuid = Uuid();

  final String _memoryId = _uuid.v7();
  final _photos = <(String, PickedPhoto)>[];
  final _placeSearch = TextEditingController();
  final _ownName = TextEditingController();
  late final _ownLatitude = TextEditingController(text: widget.initialLatitude?.toStringAsFixed(6));
  late final _ownLongitude = TextEditingController(text: widget.initialLongitude?.toStringAsFixed(6));
  final _description = TextEditingController();

  var _step = 0;
  PlaceOption? _place;
  Map<String, Object?>? _ownPlaceToQueue;
  var _suggestions = const <PlaceOption>[];
  Timer? _searchDelay;
  MemoryDatePrecision _precision = MemoryDatePrecision.day;
  DateTime? _from;
  DateTime? _to;
  TimeOfDay? _time;
  var _ownPlaceInvalid = false;
  var _consentsToAi = false;
  String _aiDraftRequestId = _uuid.v7();
  AiDraft? _aiDraft;
  String? _usedAiDraftId;
  var _aiExhausted = false;
  var _poolFull = false;
  var _approved = false;
  var _busy = false;
  MemoryFailure? _failure;

  @override
  void dispose() {
    _searchDelay?.cancel();
    _placeSearch.dispose();
    _ownName.dispose();
    _ownLatitude.dispose();
    _ownLongitude.dispose();
    _description.dispose();
    super.dispose();
  }

  bool get _dateIsInFuture {
    final from = _from;
    final now = DateTime.now();
    return from != null && DateTime(from.year, from.month, from.day).isAfter(DateTime(now.year, now.month, now.day));
  }

  bool get _dateIsComplete => _from != null && (_precision != MemoryDatePrecision.period || _to != null);

  bool get _canSave => _place != null && _dateIsComplete && !_dateIsInFuture && !_busy;

  static String _apiDate(DateTime date) {
    String pad(int value, int width) => value.toString().padLeft(width, '0');
    return '${pad(date.year, 4)}-${pad(date.month, 2)}-${pad(date.day, 2)}';
  }

  void _search(String text) {
    _searchDelay?.cancel();
    _searchDelay = Timer(const Duration(milliseconds: 300), () async {
      if (ref.read(isOnlineProvider).value == false) {
        return;
      }
      final found = await ref.read(placesRepositoryProvider).suggestions(text);
      if (!mounted) {
        return;
      }
      setState(() {
        switch (found) {
          case Ok(:final value):
            _suggestions = value;
          case Err(:final failure):
            _failure = failure;
        }
      });
    });
  }

  Future<void> _choose(PlaceOption option, {required bool offline}) async {
    setState(() => _failure = null);
    if (offline) {
      _setPlace(option);
      return;
    }
    final chosen = await ref.read(placesRepositoryProvider).choose(option.reference);
    if (!mounted) {
      return;
    }
    switch (chosen) {
      case Ok(:final value):
        _setPlace(value);
      case Err(:final failure):
        setState(() => _failure = failure);
    }
  }

  void _setPlace(PlaceOption place) {
    ref.read(recentPlacesProvider.notifier).remember(place);
    setState(() {
      _place = place;
      _suggestions = const [];
    });
  }

  /// MEM-E12-PLAT-05: a name and coordinates typed in are enough to go on; "Next" adds the own place itself.
  bool get _ownPlaceFilled =>
      _ownName.text.trim().isNotEmpty &&
      double.tryParse(_ownLatitude.text.replaceAll(',', '.')) != null &&
      double.tryParse(_ownLongitude.text.replaceAll(',', '.')) != null;

  Future<void> _next({required bool offline}) async {
    if (_place == null) {
      await _addOwnPlace(offline: offline);
    }
    if (mounted && _place != null) {
      setState(() => _step = 1);
    }
  }

  Future<void> _addOwnPlace({required bool offline}) async {
    final name = _ownName.text.trim();
    final latitude = double.tryParse(_ownLatitude.text.replaceAll(',', '.'));
    final longitude = double.tryParse(_ownLongitude.text.replaceAll(',', '.'));
    final valid =
        name.isNotEmpty &&
        latitude != null &&
        longitude != null &&
        latitude >= -90 &&
        latitude <= 90 &&
        longitude >= -180 &&
        longitude <= 180;
    setState(() {
      _ownPlaceInvalid = !valid;
      _failure = null;
    });
    if (!valid) {
      return;
    }
    final id = _uuid.v7();
    if (offline) {
      _ownPlaceToQueue = {'id': id, 'name': name, 'latitude': latitude, 'longitude': longitude};
      _setPlace(PlaceOption(reference: 'own:$id', name: name, latitude: latitude, longitude: longitude, isOwn: true));
      return;
    }
    final added = await ref
        .read(placesRepositoryProvider)
        .addOwn(id: id, name: name, latitude: latitude, longitude: longitude);
    if (!mounted) {
      return;
    }
    switch (added) {
      case Ok(:final value):
        _setPlace(value);
      case Err(:final failure):
        setState(() => _failure = failure);
    }
  }

  Future<void> _pickDate({required bool end}) async {
    final now = DateTime.now();
    final picked = await showDatePicker(
      context: context,
      initialDate: (end ? _to : _from) ?? now,
      firstDate: DateTime(1900),
      lastDate: DateTime(now.year + 1, 12, 31),
    );
    if (picked != null && mounted) {
      setState(() => end ? _to = picked : _from = picked);
    }
  }

  Future<void> _pickPhotos() async {
    final picked = await ref.read(photoPickerProvider).pick();
    if (mounted) {
      setState(() => _photos.addAll([for (final photo in picked) (_uuid.v7(), photo)]));
    }
  }

  Future<void> _requestAiDraft() async {
    setState(() {
      _busy = true;
      _failure = null;
    });
    final draft = await ref
        .read(memoriesRepositoryProvider)
        .requestAiDraft(
          id: _aiDraftRequestId,
          material: _description.text,
          consentsToAi: _consentsToAi,
          placeNames: [if (_place case final PlaceOption place) place.name],
        );
    if (!mounted) {
      return;
    }
    setState(() {
      _busy = false;
      switch (draft) {
        case Ok(:final value):
          _aiDraft = value;
          // The next request is another draft; a failed one keeps its id, so its retry costs nothing more.
          _aiDraftRequestId = _uuid.v7();
        case Err(:final failure):
          _failure = failure;
          _aiExhausted = failure is AiLimitReached;
      }
    });
  }

  void _useAiDraft() {
    final draft = _aiDraft;
    if (draft == null) {
      return;
    }
    setState(() {
      _description.text = draft.text;
      _usedAiDraftId = draft.id;
      _approved = false;
    });
  }

  Future<void> _acceptTerms() async {
    setState(() => _busy = true);
    await ref.read(accountRepositoryProvider).acceptCurrentTerms();
    if (mounted) {
      setState(() => _busy = false);
      await _save();
    }
  }

  Future<void> _save() async {
    setState(() {
      _busy = true;
      _failure = null;
    });
    final offline = ref.read(isOnlineProvider).value == false;
    final failure = offline ? await _saveToQueue() : await _send();
    if (!mounted) {
      return;
    }
    setState(() {
      _busy = false;
      _failure = failure;
      if (failure is MemorySpaceFull) {
        _poolFull = true;
      }
    });
    if (failure == null) {
      ref.invalidate(memoryPinsProvider);
      if (offline) {
        ScaffoldMessenger.of(context).showSnackBar(SnackBar(content: Text(context.l10n.savedOffline)));
      }
      widget.onSaved();
    }
  }

  Future<MemoryFailure?> _send() async {
    final memories = ref.read(memoriesRepositoryProvider);
    final created = await memories.create(_newMemory());
    if (created case Err(:final failure)) {
      return failure;
    }
    for (final (id, photo) in _photos) {
      final added = await memories.addPhoto(_memoryId, id, photo);
      if (added case Err(:final failure)) {
        return failure;
      }
    }
    if (_approved) {
      final approved = await memories.approveDescription(_memoryId, _description.text, _usedAiDraftId);
      if (approved case Err(:final failure)) {
        return failure;
      }
    }
    return null;
  }

  Future<MemoryFailure?> _saveToQueue() async {
    final queue = ref.read(offlineQueueProvider);
    if (_ownPlaceToQueue case final Map<String, Object?> place) {
      await queue.enqueue(MemoryOperations.addOwnPlace, place);
    }
    final memory = _newMemory();
    await queue.enqueue(MemoryOperations.createMemory, {
      'id': memory.id,
      'placeReference': memory.placeReference,
      'precision': memory.precision.name,
      'from': memory.from,
      'to': memory.to,
      'time': memory.time,
    });
    for (final (id, photo) in _photos) {
      await queue.enqueue(MemoryOperations.addPhoto, {
        'memoryId': _memoryId,
        'photoId': id,
        'path': photo.path,
        'name': photo.name,
        'contentType': photo.contentType,
      });
    }
    if (_approved) {
      await queue.enqueue(MemoryOperations.approveDescription, {
        'memoryId': _memoryId,
        'text': _description.text,
        'aiDraftId': _usedAiDraftId,
      });
    }
    return null;
  }

  NewMemory _newMemory() => NewMemory(
    id: _memoryId,
    placeReference: _place!.reference,
    precision: _precision,
    from: _apiDate(_from!),
    to: _precision == MemoryDatePrecision.period ? _apiDate(_to!) : null,
    time: _precision == MemoryDatePrecision.day && _time != null ? _hourText(_time!) : null,
  );

  static String _hourText(TimeOfDay time) =>
      '${time.hour.toString().padLeft(2, '0')}:${time.minute.toString().padLeft(2, '0')}';

  Future<void> _pickTime() async {
    final picked = await showTimePicker(context: context, initialTime: _time ?? const TimeOfDay(hour: 12, minute: 0));
    if (picked != null && mounted) {
      setState(() => _time = picked);
    }
  }

  @override
  Widget build(BuildContext context) {
    final l10n = context.l10n;
    final offline = ref.watch(isOnlineProvider).value == false;
    final blockedOffline = offline && widget.isWeb;

    return _ModalFrame(
      child: Column(
        children: [
          Padding(
            padding: const EdgeInsets.fromLTRB(20, 12, 8, 0),
            child: Row(
              children: [
                Expanded(child: Text(l10n.newMemoryTitle, style: Theme.of(context).textTheme.titleLarge)),
                IconButton(
                  key: const Key('s04-close'),
                  tooltip: l10n.cancel,
                  icon: const Icon(Icons.close),
                  onPressed: widget.onClose,
                ),
              ],
            ),
          ),
          Padding(
            padding: const EdgeInsets.fromLTRB(20, 8, 20, 12),
            child: _StepBar(step: _step, labels: [l10n.newMemoryStepPlace, l10n.newMemoryStepContent]),
          ),
          const Divider(height: 1),
          if (offline)
            Padding(
              padding: const EdgeInsets.all(12),
              child: Text(blockedOffline ? l10n.offlineNotOnWeb : l10n.noConnection, key: const Key('s04-offline')),
            ),
          if (_busy) const LinearProgressIndicator(key: Key('s04-busy')),
          Expanded(
            child: ListView(
              padding: const EdgeInsets.all(20),
              children: [
                if (_step == 0) ..._placeAndDate(offline) else ..._content(offline, blockedOffline),
                ..._failureBlock(),
              ],
            ),
          ),
          const Divider(height: 1),
          Padding(padding: const EdgeInsets.fromLTRB(12, 10, 20, 12), child: _footer(offline, blockedOffline)),
        ],
      ),
    );
  }

  /// Cancel on the left; Back, Next or Save on the right, always in view.
  Widget _footer(bool offline, bool blockedOffline) {
    final l10n = context.l10n;
    return Row(
      children: [
        TextButton(key: const Key('s04-cancel'), onPressed: widget.onClose, child: Text(l10n.cancel)),
        const Spacer(),
        if (_step == 0)
          FilledButton(
            onPressed: (_place != null || _ownPlaceFilled) && _dateIsComplete && !_dateIsInFuture
                ? () => _next(offline: offline)
                : null,
            child: Text(l10n.next),
          )
        else ...[
          TextButton(onPressed: _busy ? null : () => setState(() => _step = 0), child: Text(l10n.back)),
          const SizedBox(width: 8),
          FilledButton(onPressed: _canSave && !blockedOffline ? _save : null, child: Text(l10n.saveMemory)),
        ],
      ],
    );
  }

  List<Widget> _failureBlock() {
    final l10n = context.l10n;
    final failure = _failure;
    if (failure == null) {
      return const [];
    }
    return [
      const SizedBox(height: 12),
      Text(
        memoryFailureText(l10n, failure),
        key: const Key('s04-error'),
        style: TextStyle(color: Theme.of(context).colorScheme.error),
      ),
      if (failure is MemoryTermsNotAccepted)
        TextButton(onPressed: _busy ? null : _acceptTerms, child: Text(l10n.acceptTerms))
      else if (_step == 1 && failure is! AiLimitReached && failure is! AiProviderDown)
        TextButton(onPressed: _canSave ? _save : null, child: Text(l10n.retry)),
    ];
  }

  List<Widget> _placeAndDate(bool offline) {
    final l10n = context.l10n;
    final place = _place;
    final recent = ref.watch(recentPlacesProvider);
    final options = offline ? recent : _suggestions;
    return [
      if (place != null) ...[
        Text(l10n.placeChosen(place.name), key: const Key('s04-place')),
        TextButton(onPressed: () => setState(() => _place = null), child: Text(l10n.placeChange)),
      ] else ...[
        if (!offline)
          TextField(
            controller: _placeSearch,
            decoration: InputDecoration(labelText: l10n.place),
            onChanged: _search,
          )
        else if (recent.isNotEmpty)
          Text(l10n.placeRecent),
        for (final option in options)
          ListTile(
            title: Text(option.name),
            leading: Icon(option.isOwn ? Icons.bookmark : Icons.place),
            onTap: () => _choose(option, offline: offline),
          ),
        const SizedBox(height: 12),
        if (widget.initialLatitude != null) Text(l10n.ownPlaceFromMap, key: const Key('s04-from-map')),
        TextField(
          controller: _ownName,
          decoration: InputDecoration(labelText: l10n.ownPlaceName),
          onChanged: (_) => setState(() {}),
        ),
        TextField(
          controller: _ownLatitude,
          keyboardType: const TextInputType.numberWithOptions(decimal: true, signed: true),
          decoration: InputDecoration(labelText: l10n.ownPlaceLatitude),
          onChanged: (_) => setState(() {}),
        ),
        TextField(
          controller: _ownLongitude,
          keyboardType: const TextInputType.numberWithOptions(decimal: true, signed: true),
          decoration: InputDecoration(labelText: l10n.ownPlaceLongitude),
          onChanged: (_) => setState(() {}),
        ),
        if (_ownPlaceInvalid) Text(l10n.ownPlaceInvalid, style: TextStyle(color: Theme.of(context).colorScheme.error)),
        TextButton(
          onPressed: () => _addOwnPlace(offline: offline),
          child: Text(l10n.ownPlaceAdd),
        ),
      ],
      const SizedBox(height: 24),
      Text(l10n.datePrecision, style: Theme.of(context).textTheme.titleMedium),
      const SizedBox(height: 8),
      SegmentedButton<MemoryDatePrecision>(
        segments: [
          ButtonSegment(value: MemoryDatePrecision.day, label: Text(l10n.precisionDay)),
          ButtonSegment(value: MemoryDatePrecision.month, label: Text(l10n.precisionMonth)),
          ButtonSegment(value: MemoryDatePrecision.year, label: Text(l10n.precisionYear)),
          ButtonSegment(value: MemoryDatePrecision.period, label: Text(l10n.precisionPeriod)),
        ],
        selected: {_precision},
        onSelectionChanged: (selected) => setState(() => _precision = selected.single),
      ),
      const SizedBox(height: 8),
      TextButton(
        onPressed: () => _pickDate(end: false),
        child: Text(_from == null ? l10n.dateChoose : l10n.dateChosen(dayText(_from!))),
      ),
      if (_precision == MemoryDatePrecision.day)
        Row(
          children: [
            Flexible(
              child: TextButton(
                key: const Key('s04-time'),
                onPressed: _pickTime,
                child: Text(
                  _time == null ? l10n.timeChoose : l10n.timeChosen(_hourText(_time!)),
                  overflow: TextOverflow.ellipsis,
                ),
              ),
            ),
            if (_time != null)
              IconButton(
                key: const Key('s04-time-clear'),
                tooltip: l10n.timeClear,
                icon: const Icon(Icons.close),
                onPressed: () => setState(() => _time = null),
              ),
          ],
        ),
      if (_precision == MemoryDatePrecision.period)
        TextButton(
          onPressed: () => _pickDate(end: true),
          child: Text(_to == null ? l10n.dateChooseEnd : l10n.dateChosenEnd(dayText(_to!))),
        ),
      if (_dateIsInFuture)
        Text(
          l10n.dateInFuture,
          key: const Key('s04-future'),
          style: TextStyle(color: Theme.of(context).colorScheme.error),
        ),
      const SizedBox(height: 16),
      if (place == null && !_ownPlaceFilled)
        Text(l10n.placeNeeded, key: const Key('s04-place-needed'), style: Theme.of(context).textTheme.bodySmall),
      if (!_dateIsComplete)
        Text(l10n.dateNeeded, key: const Key('s04-date-needed'), style: Theme.of(context).textTheme.bodySmall),
    ];
  }

  List<Widget> _content(bool offline, bool blockedOffline) {
    final l10n = context.l10n;
    final draft = _aiDraft;
    return [
      const SizedBox(height: 12),
      Text(l10n.photos, style: Theme.of(context).textTheme.titleSmall),
      OutlinedButton(onPressed: _poolFull || _busy ? null : _pickPhotos, child: Text(l10n.photosAdd)),
      if (_photos.isNotEmpty) Text(l10n.photosPicked(_photos.length), key: const Key('s04-photos')),
      const SizedBox(height: 16),
      TextField(
        controller: _description,
        minLines: 3,
        maxLines: 8,
        decoration: InputDecoration(labelText: l10n.description),
        // Always rebuilds: the approve and AI actions depend on the text.
        onChanged: (_) => setState(() => _approved = false),
      ),
      const SizedBox(height: 8),
      if (offline)
        Text(l10n.aiUnavailableOffline, key: const Key('s04-ai-offline'))
      else ...[
        CheckboxListTile(
          title: Text(l10n.aiConsent),
          value: _consentsToAi,
          onChanged: (value) => setState(() => _consentsToAi = value ?? false),
        ),
        OutlinedButton(
          onPressed: _consentsToAi && !_aiExhausted && !_busy && _description.text.trim().isNotEmpty
              ? _requestAiDraft
              : null,
          child: Text(l10n.aiRequest),
        ),
      ],
      if (draft != null) ...[
        const SizedBox(height: 12),
        Row(
          children: [
            Text(l10n.aiDraft, style: Theme.of(context).textTheme.titleSmall),
            const SizedBox(width: 8),
            Chip(key: const Key('s04-ai-marker'), label: Text(l10n.aiMarker)),
          ],
        ),
        Text(draft.text, key: const Key('s04-ai-draft')),
        TextButton(onPressed: _useAiDraft, child: Text(l10n.aiUseDraft)),
      ],
      const SizedBox(height: 8),
      OutlinedButton(
        onPressed: _description.text.trim().isEmpty || _approved ? null : () => setState(() => _approved = true),
        child: Text(_approved ? l10n.descriptionApproved : l10n.approveDescription),
      ),
    ];
  }
}

/// MEM-E13-PLAT-07: on a wide screen a window in the middle; on a phone the whole screen.
class _ModalFrame extends StatelessWidget {
  const _ModalFrame({required this.child});

  static const maxWidth = 640.0;

  final Widget child;

  @override
  Widget build(BuildContext context) {
    final size = MediaQuery.sizeOf(context);
    final wide = size.width >= 720;
    final surface = Material(
      key: const Key('s04-window'),
      color: Theme.of(context).colorScheme.surface,
      elevation: 12,
      borderRadius: wide ? BorderRadius.circular(20) : BorderRadius.zero,
      clipBehavior: Clip.antiAlias,
      child: SafeArea(child: child),
    );
    if (!wide) {
      return surface;
    }
    return Center(
      child: Padding(
        padding: const EdgeInsets.symmetric(vertical: 32),
        child: ConstrainedBox(
          constraints: BoxConstraints(maxWidth: maxWidth, maxHeight: size.height * 0.9),
          child: surface,
        ),
      ),
    );
  }
}

/// The steps as numbered circles joined by a line: done ones ticked, the current one in the accent.
class _StepBar extends StatelessWidget {
  const _StepBar({required this.step, required this.labels});

  final int step;
  final List<String> labels;

  @override
  Widget build(BuildContext context) {
    final colors = Theme.of(context).colorScheme;
    return Row(
      key: const Key('s04-steps'),
      children: [
        for (final (index, label) in labels.indexed) ...[
          if (index > 0)
            Expanded(
              child: Container(
                height: 2,
                margin: const EdgeInsets.symmetric(horizontal: 10),
                color: index <= step ? colors.primary : colors.outlineVariant,
              ),
            ),
          _StepDot(index: index, label: label, done: index < step, current: index == step),
        ],
      ],
    );
  }
}

class _StepDot extends StatelessWidget {
  const _StepDot({required this.index, required this.label, required this.done, required this.current});

  final int index;
  final String label;
  final bool done;
  final bool current;

  @override
  Widget build(BuildContext context) {
    final colors = Theme.of(context).colorScheme;
    final lit = done || current;
    return Semantics(
      selected: current,
      child: Row(
        key: Key('s04-step-$index'),
        mainAxisSize: MainAxisSize.min,
        children: [
          AnimatedContainer(
            duration: const Duration(milliseconds: 180),
            width: 28,
            height: 28,
            alignment: Alignment.center,
            decoration: BoxDecoration(
              shape: BoxShape.circle,
              color: lit ? colors.primary : Colors.transparent,
              border: Border.all(color: lit ? colors.primary : colors.outline, width: 1.5),
            ),
            child: done
                ? Icon(Icons.check, size: 16, color: colors.onPrimary)
                : Text(
                    '${index + 1}',
                    style: TextStyle(
                      color: lit ? colors.onPrimary : colors.onSurfaceVariant,
                      fontWeight: FontWeight.w600,
                    ),
                  ),
          ),
          const SizedBox(width: 8),
          Text(
            label,
            style: Theme.of(context).textTheme.labelLarge?.copyWith(
              color: current ? colors.onSurface : colors.onSurfaceVariant,
              fontWeight: current ? FontWeight.w700 : FontWeight.w500,
            ),
          ),
        ],
      ),
    );
  }
}
