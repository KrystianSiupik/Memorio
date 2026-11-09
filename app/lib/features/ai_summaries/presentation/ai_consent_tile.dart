import 'package:flutter/material.dart';
import 'package:flutter_riverpod/flutter_riverpod.dart';

import '../../../core/l10n/l10n.dart';
import '../../../core/offline/offline.dart';
import '../../../core/result.dart';
import '../domain/period_summary.dart';
import 'summaries_providers.dart';

/// C80 and C81: the person's AI consent for one album, on S12 and S17.
class AiConsentTile extends ConsumerStatefulWidget {
  const AiConsentTile({super.key, required this.albumId});

  final String albumId;

  @override
  ConsumerState<AiConsentTile> createState() => _AiConsentTileState();
}

class _AiConsentTileState extends ConsumerState<AiConsentTile> {
  var _busy = false;
  SummaryFailure? _failure;

  Future<void> _set(bool given) async {
    setState(() {
      _busy = true;
      _failure = null;
    });
    final set = await ref.read(summariesRepositoryProvider).setConsent(widget.albumId, given: given);
    if (!mounted) {
      return;
    }
    setState(() {
      _busy = false;
      if (set case Err(:final failure)) {
        _failure = failure;
      }
    });
    ref.invalidate(aiConsentProvider(widget.albumId));
    // Taken back, the summaries made from this person's memories go (P12).
    ref.invalidate(albumSummariesProvider(widget.albumId));
  }

  @override
  Widget build(BuildContext context) {
    final l10n = context.l10n;
    final consent = ref.watch(aiConsentProvider(widget.albumId));
    final offline = ref.watch(isOnlineProvider).value == false;
    final failure = _failure ?? (consent.hasError && consent.value == null ? consent.error : null);
    return Column(
      crossAxisAlignment: CrossAxisAlignment.start,
      children: [
        SwitchListTile(
          key: const Key('ai-consent'),
          contentPadding: EdgeInsets.zero,
          title: Text(l10n.summaryConsent),
          subtitle: Text(l10n.summaryConsentHint),
          value: consent.value ?? false,
          onChanged: offline || _busy || consent.value == null ? null : _set,
        ),
        if (failure != null)
          Text(
            summaryFailureText(l10n, failure),
            key: const Key('ai-consent-error'),
            style: TextStyle(color: Theme.of(context).colorScheme.error),
          ),
      ],
    );
  }
}
