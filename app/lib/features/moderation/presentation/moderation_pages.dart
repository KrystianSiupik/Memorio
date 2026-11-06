import 'package:flutter/material.dart';
import 'package:flutter_riverpod/flutter_riverpod.dart';
import 'package:uuid/uuid.dart';

import '../../../core/l10n/l10n.dart';
import '../../memories/memories.dart' show dayText;
import '../domain/moderation.dart';
import 'moderation_providers.dart';

/// S27: reporting a memory or an album the person sees (S2:C2, L19). Illegal content is hidden at once; the rest
/// stays seen until the service decides. What was typed stays after a failure, and sending again is the same report.
class ReportContentPage extends ConsumerStatefulWidget {
  const ReportContentPage({super.key, required this.target, required this.targetId, required this.onDone});

  final ReportTarget target;
  final String targetId;
  final VoidCallback onDone;

  @override
  ConsumerState<ReportContentPage> createState() => _ReportContentPageState();
}

class _ReportContentPageState extends ConsumerState<ReportContentPage> with ModerationActions {
  // Made once, so sending again after a failure is the same report.
  final _reportId = const Uuid().v7();
  final _explanation = TextEditingController();
  ReportReason? _reason;
  var _sent = false;

  @override
  void dispose() {
    _explanation.dispose();
    super.dispose();
  }

  Future<void> _send(ReportReason reason) async {
    if (await act(
      () => moderation.report(_reportId, widget.target, widget.targetId, reason, _explanation.text.trim()),
    )) {
      ref.invalidate(myModerationProvider);
      setState(() => _sent = true);
    }
  }

  @override
  Widget build(BuildContext context) {
    final l10n = context.l10n;
    final reason = _reason;
    final ready = reason != null && _explanation.text.trim().isNotEmpty;
    return Scaffold(
      appBar: AppBar(
        title: Text(widget.target == ReportTarget.memory ? l10n.reportMemoryTitle : l10n.reportAlbumTitle),
      ),
      body: Column(
        children: [
          ...offlineBlock(context),
          Expanded(
            child: ListView(
              padding: const EdgeInsets.all(16),
              children: [
                if (_sent) ...[
                  Text(
                    reason?.isIllegal ?? false ? l10n.reportSentIllegal : l10n.reportSent,
                    key: const Key('s27-sent'),
                  ),
                  const SizedBox(height: 16),
                  FilledButton(onPressed: widget.onDone, child: Text(l10n.reportBack)),
                ] else ...[
                  Text(l10n.reportExplained),
                  const SizedBox(height: 8),
                  Text(l10n.reportCategory, style: Theme.of(context).textTheme.titleMedium),
                  RadioGroup<ReportReason>(
                    groupValue: _reason,
                    onChanged: (chosen) => setState(() => _reason = chosen),
                    child: Column(
                      children: [
                        for (final option in ReportReason.values)
                          RadioListTile<ReportReason>(
                            key: Key('s27-reason-${option.apiName}'),
                            value: option,
                            title: Text(reportReasonText(l10n, option)),
                          ),
                      ],
                    ),
                  ),
                  if (reason?.isIllegal ?? false) Text(l10n.reportIllegalHidden, key: const Key('s27-illegal')),
                  const SizedBox(height: 8),
                  TextField(
                    controller: _explanation,
                    maxLength: 4000,
                    maxLines: 4,
                    decoration: InputDecoration(labelText: l10n.reportExplanation),
                    onChanged: (_) => setState(() {}),
                  ),
                  ...failureBlock(context),
                  FilledButton(
                    onPressed: offline || busy || !ready ? null : () => _send(reason),
                    child: Text(l10n.reportSend),
                  ),
                ],
              ],
            ),
          ),
        ],
      ),
    );
  }
}

/// S22: what the person reported, and every decision about their own content, with the reasons and the appeal.
class MyModerationPage extends ConsumerStatefulWidget {
  const MyModerationPage({super.key});

  @override
  ConsumerState<MyModerationPage> createState() => _MyModerationPageState();
}

class _MyModerationPageState extends ConsumerState<MyModerationPage> with ModerationActions {
  final _appeals = <String, TextEditingController>{};

  @override
  void dispose() {
    for (final controller in _appeals.values) {
      controller.dispose();
    }
    super.dispose();
  }

  TextEditingController _appealOf(String id) => _appeals.putIfAbsent(id, TextEditingController.new);

  Future<void> _appeal(String id) async {
    if (await act(() => moderation.appeal(id, _appealOf(id).text.trim()))) {
      ref.invalidate(myModerationProvider);
    }
  }

  String _verdictText(String? verdict) => switch (verdict) {
    'Removed' => context.l10n.verdictRemoved,
    'Kept' => context.l10n.verdictKept,
    _ => context.l10n.verdictPending,
  };

  @override
  Widget build(BuildContext context) {
    final l10n = context.l10n;
    final mine = ref.watch(myModerationProvider);
    return Scaffold(
      appBar: AppBar(title: Text(l10n.myModerationTitle)),
      body: Column(
        children: [
          ...offlineBlock(context),
          Expanded(
            child: switch (mine) {
              AsyncValue(:final value?) => _content(value),
              AsyncValue(:final error?) => Center(
                child: Column(
                  mainAxisSize: MainAxisSize.min,
                  children: [
                    Padding(
                      padding: const EdgeInsets.all(16),
                      child: Text(moderationFailureText(l10n, error), key: const Key('s22-load-error')),
                    ),
                    TextButton(onPressed: () => ref.invalidate(myModerationProvider), child: Text(l10n.retry)),
                  ],
                ),
              ),
              _ => const Center(child: CircularProgressIndicator(key: Key('s22-loading'))),
            },
          ),
        ],
      ),
    );
  }

  Widget _content(MyModeration value) {
    final l10n = context.l10n;
    if (value.isEmpty) {
      return Center(child: Text(l10n.myModerationEmpty, key: const Key('s22-empty')));
    }
    final now = DateTime.now();
    return ListView(
      padding: const EdgeInsets.all(16),
      children: [
        ...failureBlock(context),
        if (value.aboutMine.isNotEmpty)
          Text(l10n.myModerationAboutMine, style: Theme.of(context).textTheme.titleMedium),
        for (final about in value.aboutMine)
          Card(
            key: Key('s22-about-${about.id}'),
            child: Padding(
              padding: const EdgeInsets.all(12),
              child: Column(
                crossAxisAlignment: CrossAxisAlignment.start,
                children: [
                  Text(l10n.myModerationReported(dayText(about.reportedAt), reportReasonText(l10n, about.reason))),
                  if (about.hidden) Text(l10n.myModerationHidden),
                  Text(_verdictText(about.verdict)),
                  if (about.statement case final statement?) Text(l10n.myModerationStatement(statement)),
                  if (about.state == 'Appealed') Text(l10n.myModerationAppealPending),
                  if (about.appealReversed case final reversed?)
                    Text(reversed ? l10n.myModerationAppealReversed : l10n.myModerationAppealUpheld),
                  if (about.appealStatement case final statement?) Text(l10n.myModerationStatement(statement)),
                  if (about.canAppeal(now)) ..._appealBlock(about.id, about.appealUntil!),
                ],
              ),
            ),
          ),
        if (value.reports.isNotEmpty) ...[
          const SizedBox(height: 16),
          Text(l10n.myModerationMine, style: Theme.of(context).textTheme.titleMedium),
        ],
        for (final report in value.reports)
          Card(
            key: Key('s22-report-${report.id}'),
            child: Padding(
              padding: const EdgeInsets.all(12),
              child: Column(
                crossAxisAlignment: CrossAxisAlignment.start,
                children: [
                  Text(l10n.myModerationReported(dayText(report.reportedAt), reportReasonText(l10n, report.reason))),
                  Text(_verdictText(report.verdict)),
                  if (report.statement case final statement?) Text(l10n.myModerationStatement(statement)),
                  if (report.state == 'Appealed') Text(l10n.myModerationAppealPending),
                  if (report.appealReversed case final reversed?)
                    Text(reversed ? l10n.myModerationReporterAppealReversed : l10n.myModerationAppealUpheld),
                  if (report.appealStatement case final statement?) Text(l10n.myModerationStatement(statement)),
                  if (report.canAppeal(now)) ..._appealBlock(report.id, report.appealUntil!),
                ],
              ),
            ),
          ),
      ],
    );
  }

  /// The appeal of either side, while its window lasts.
  List<Widget> _appealBlock(String id, DateTime until) {
    final l10n = context.l10n;
    return [
      const SizedBox(height: 8),
      TextField(
        key: Key('s22-appeal-text-$id'),
        controller: _appealOf(id),
        maxLength: 4000,
        maxLines: 3,
        decoration: InputDecoration(labelText: l10n.myModerationAppealReason),
        onChanged: (_) => setState(() {}),
      ),
      Text(l10n.myModerationAppealUntil(dayText(until))),
      FilledButton(
        key: Key('s22-appeal-$id'),
        onPressed: offline || busy || _appealOf(id).text.trim().isEmpty ? null : () => _appeal(id),
        child: Text(l10n.myModerationAppeal),
      ),
    ];
  }
}
