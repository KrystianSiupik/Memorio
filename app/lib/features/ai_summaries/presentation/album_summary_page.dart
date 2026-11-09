import 'package:flutter/material.dart';
import 'package:flutter_riverpod/flutter_riverpod.dart';
import 'package:uuid/uuid.dart';

import '../../../core/l10n/l10n.dart';
import '../../../core/offline/offline.dart';
import '../../../core/result.dart';
import '../../memories/memories.dart' show dayTextOf;
import '../domain/period_summary.dart';
import 'ai_consent_tile.dart';
import 'summaries_providers.dart';

String _dateOf(DateTime day) =>
    '${day.year}-${day.month.toString().padLeft(2, '0')}-${day.day.toString().padLeft(2, '0')}';

/// S17 Period summary (AI): the person asks for the story of a period of an album, approves it and shows it to the
/// album's participants. Only the materials they may use go to the AI (R8); what was left out is said.
class AlbumSummaryPage extends ConsumerStatefulWidget {
  const AlbumSummaryPage({super.key, required this.albumId});

  final String albumId;

  @override
  ConsumerState<AlbumSummaryPage> createState() => _AlbumSummaryPageState();
}

class _AlbumSummaryPageState extends ConsumerState<AlbumSummaryPage> {
  static const _uuid = Uuid();

  late DateTime _to;
  late DateTime _from;

  /// The id of the request being asked: kept while it fails, so asking again costs one AI operation (D-226).
  String? _requestId;
  PeriodSummary? _current;
  var _preparing = false;
  var _busy = false;
  SummaryFailure? _failure;
  Future<void> Function()? _retry;

  @override
  void initState() {
    super.initState();
    final now = DateTime.now();
    _to = DateTime(now.year, now.month, now.day);
    _from = DateTime(now.year - 1, now.month, now.day);
  }

  Future<void> _request() async {
    final id = _requestId ??= _uuid.v7();
    setState(() {
      _preparing = true;
      _failure = null;
      _retry = null;
    });
    final requested = await ref
        .read(summariesRepositoryProvider)
        .request(id, widget.albumId, _dateOf(_from), _dateOf(_to));
    if (!mounted) {
      return;
    }
    setState(() {
      _preparing = false;
      switch (requested) {
        case Ok(:final value):
          _current = value;
          _requestId = null;
        case Err(:final failure):
          _failure = failure;
          _retry = _request;
          // A refused request never reached the AI: the next one is a new request.
          if (failure is! SummaryOffline && failure is! SummaryServerError) {
            _requestId = null;
          }
      }
    });
    ref.invalidate(aiOperationsLeftProvider);
  }

  Future<void> _decide(Future<Result<PeriodSummary, SummaryFailure>> Function(String id) action) async {
    final current = _current;
    if (current == null) {
      return;
    }
    setState(() {
      _busy = true;
      _failure = null;
      _retry = null;
    });
    final decided = await action(current.id);
    if (!mounted) {
      return;
    }
    setState(() {
      _busy = false;
      switch (decided) {
        case Ok(:final value):
          _current = value;
        case Err(:final failure):
          _failure = failure;
          _retry = () => _decide(action);
      }
    });
    ref.invalidate(albumSummariesProvider(widget.albumId));
  }

  Future<void> _pick({required bool from}) async {
    final picked = await showDatePicker(
      context: context,
      initialDate: from ? _from : _to,
      firstDate: DateTime(1900),
      lastDate: DateTime(_to.year + 1, 12, 31),
    );
    if (picked == null || !mounted) {
      return;
    }
    setState(() {
      if (from) {
        _from = picked;
      } else {
        _to = picked;
      }
      _requestId = null;
    });
  }

  @override
  Widget build(BuildContext context) {
    final l10n = context.l10n;
    final theme = Theme.of(context);
    final offline = ref.watch(isOnlineProvider).value == false;
    final consent = ref.watch(aiConsentProvider(widget.albumId)).value;
    final left = ref.watch(aiOperationsLeftProvider).value;
    final current = _current;
    final failure = _failure;
    final canRequest = !offline && consent == true && (left == null || left > 0) && !_preparing && !_busy;

    return Scaffold(
      appBar: AppBar(title: Text(l10n.summaryTitle)),
      body: ListView(
        padding: const EdgeInsets.all(16),
        children: [
          if (offline) Text(l10n.noConnection, key: const Key('s17-offline')),
          AiConsentTile(albumId: widget.albumId),
          const Divider(height: 32),
          Wrap(
            spacing: 8,
            runSpacing: 8,
            children: [
              OutlinedButton(
                key: const Key('s17-from'),
                onPressed: _preparing ? null : () => _pick(from: true),
                child: Text(l10n.summaryFrom(dayTextOf(_dateOf(_from)))),
              ),
              OutlinedButton(
                key: const Key('s17-to'),
                onPressed: _preparing ? null : () => _pick(from: false),
                child: Text(l10n.summaryTo(dayTextOf(_dateOf(_to)))),
              ),
            ],
          ),
          const SizedBox(height: 12),
          FilledButton.icon(
            key: const Key('s17-request'),
            onPressed: canRequest ? _request : null,
            icon: const Icon(Icons.auto_awesome_outlined),
            label: Text(l10n.summaryRequest),
          ),
          if (consent == false)
            Padding(
              padding: const EdgeInsets.only(top: 8),
              child: Text(l10n.summaryConsentNeeded, key: const Key('s17-consent-needed')),
            ),
          if (left == 0)
            Padding(
              padding: const EdgeInsets.only(top: 8),
              child: Text(l10n.summaryLimitUsed, key: const Key('s17-limit')),
            ),
          if (_preparing)
            Padding(
              padding: const EdgeInsets.symmetric(vertical: 16),
              child: Row(
                key: const Key('s17-loading'),
                children: [
                  const SizedBox(width: 20, height: 20, child: CircularProgressIndicator(strokeWidth: 2)),
                  const SizedBox(width: 12),
                  Expanded(child: Text(l10n.summaryPreparing)),
                ],
              ),
            ),
          if (failure is SummaryNoMaterials)
            Padding(
              padding: const EdgeInsets.symmetric(vertical: 16),
              child: Text(l10n.summaryEmpty, key: const Key('s17-empty'), style: theme.textTheme.titleMedium),
            )
          else if (failure != null)
            Padding(
              padding: const EdgeInsets.symmetric(vertical: 12),
              child: Column(
                crossAxisAlignment: CrossAxisAlignment.start,
                children: [
                  Text(
                    summaryFailureText(l10n, failure),
                    key: const Key('s17-error'),
                    style: TextStyle(color: theme.colorScheme.error),
                  ),
                  if (_retry case final retry?
                      when failure is SummaryOffline || failure is SummaryServerError || failure is SummaryNothingTold)
                    TextButton(key: const Key('s17-retry'), onPressed: offline ? null : retry, child: Text(l10n.retry)),
                ],
              ),
            ),
          if (current != null) ...[
            const SizedBox(height: 12),
            _SummaryCard(
              key: const Key('s17-summary'),
              summary: current,
              actions: [
                if (current.state == SummaryState.draft)
                  FilledButton(
                    key: const Key('s17-save'),
                    onPressed: offline || _busy ? null : () => _decide(ref.read(summariesRepositoryProvider).save),
                    child: Text(l10n.summarySave),
                  ),
                if (current.state == SummaryState.saved) ...[
                  Text(l10n.summarySaved, key: const Key('s17-saved')),
                  const SizedBox(height: 8),
                  if (current.addedToAlbum)
                    Text(l10n.summaryAdded, key: const Key('s17-added'))
                  else
                    OutlinedButton(
                      key: const Key('s17-add'),
                      onPressed: offline || _busy
                          ? null
                          : () => _decide(ref.read(summariesRepositoryProvider).addToAlbum),
                      child: Text(l10n.summaryAddToAlbum),
                    ),
                ],
              ],
            ),
          ],
          const Divider(height: 32),
          Text(l10n.summaryInAlbum, style: theme.textTheme.titleMedium),
          const SizedBox(height: 8),
          _AlbumSummaries(albumId: widget.albumId),
        ],
      ),
    );
  }
}

/// The summaries of the album, to read; the last answer stays offline.
class _AlbumSummaries extends ConsumerWidget {
  const _AlbumSummaries({required this.albumId});

  final String albumId;

  @override
  Widget build(BuildContext context, WidgetRef ref) {
    final l10n = context.l10n;
    final summaries = ref.watch(albumSummariesProvider(albumId));
    final known = summaries.value;
    if (known != null) {
      if (known.isEmpty) {
        return Text(l10n.summaryInAlbumEmpty, key: const Key('s17-album-empty'));
      }
      return Column(
        children: [for (final summary in known) _SummaryCard(key: Key('s17-album-${summary.id}'), summary: summary)],
      );
    }
    if (summaries.hasError) {
      return Column(
        crossAxisAlignment: CrossAxisAlignment.start,
        children: [
          Text(summaryFailureText(l10n, summaries.error), key: const Key('s17-album-error')),
          if (summaries.error is! SummaryNoAccess)
            TextButton(onPressed: () => ref.invalidate(albumSummariesProvider(albumId)), child: Text(l10n.retry)),
        ],
      );
    }
    return const LinearProgressIndicator(key: Key('s17-album-loading'));
  }
}

/// One summary: its period, the AI marker, the text and what it was made from.
class _SummaryCard extends StatelessWidget {
  const _SummaryCard({super.key, required this.summary, this.actions = const []});

  final PeriodSummary summary;
  final List<Widget> actions;

  @override
  Widget build(BuildContext context) {
    final l10n = context.l10n;
    final theme = Theme.of(context);
    return Card(
      margin: const EdgeInsets.only(bottom: 12),
      child: Padding(
        padding: const EdgeInsets.all(16),
        child: Column(
          crossAxisAlignment: CrossAxisAlignment.start,
          children: [
            Text('${dayTextOf(summary.from)} – ${dayTextOf(summary.to)}', style: theme.textTheme.labelLarge),
            const SizedBox(height: 8),
            Row(
              key: const Key('s17-ai-badge'),
              children: [
                Icon(Icons.auto_awesome, size: 16, color: theme.colorScheme.primary),
                const SizedBox(width: 6),
                Text(
                  l10n.summaryAiBadge,
                  style: theme.textTheme.labelMedium?.copyWith(color: theme.colorScheme.primary),
                ),
              ],
            ),
            const SizedBox(height: 8),
            if (summary.state == SummaryState.invalidated)
              Text(l10n.summaryInvalidated, key: const Key('s17-invalidated'))
            else
              Text(summary.text, key: const Key('s17-text'), style: theme.textTheme.bodyLarge),
            const SizedBox(height: 12),
            Text(l10n.summaryMaterials(summary.usedMemories), key: const Key('s17-materials')),
            if (summary.leftOutPeople > 0)
              Text(l10n.summaryLeftOut(summary.leftOutPeople), key: const Key('s17-left-out')),
            if (summary.droppedSentences > 0)
              Text(l10n.summaryDropped(summary.droppedSentences), key: const Key('s17-dropped')),
            if (actions.isNotEmpty) ...[const SizedBox(height: 12), ...actions],
          ],
        ),
      ),
    );
  }
}
