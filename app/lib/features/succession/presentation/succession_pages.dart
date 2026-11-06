import 'package:flutter/material.dart';
import 'package:flutter_riverpod/flutter_riverpod.dart';
import 'package:uuid/uuid.dart';

import '../../../core/l10n/l10n.dart';
import '../../albums/albums.dart' show albumDetailsProvider, albumsProvider;
import '../../memories/memories.dart' show dayTextOf, photoPickerProvider, PickedPhoto;
import 'succession_providers.dart';

String _dayText(DateTime date) {
  final local = date.toLocal();
  final month = local.month.toString().padLeft(2, '0');
  final day = local.day.toString().padLeft(2, '0');
  return dayTextOf('${local.year}-$month-$day');
}

/// S23 Taking an album over: only for the successor named when the death was confirmed.
class TakeoverOfferPage extends ConsumerStatefulWidget {
  const TakeoverOfferPage({super.key, required this.caseId, required this.onDone});

  final String caseId;
  final VoidCallback onDone;

  @override
  ConsumerState<TakeoverOfferPage> createState() => _TakeoverOfferPageState();
}

class _TakeoverOfferPageState extends ConsumerState<TakeoverOfferPage> with SuccessionActions {
  Future<void> _answer({required bool accepted}) async {
    if (await act(() => succession.answer(widget.caseId, accepted: accepted))) {
      ref.invalidate(takeoverOffersProvider);
      ref.invalidate(albumsProvider);
      widget.onDone();
    }
  }

  @override
  Widget build(BuildContext context) {
    final l10n = context.l10n;
    final offer = ref.watch(takeoverOfferProvider(widget.caseId));

    return Scaffold(
      appBar: AppBar(title: Text(l10n.takeoverTitle)),
      body: Column(
        children: [
          ...offlineBlock(context),
          Expanded(
            child: SuccessionAsyncBody(
              value: offer,
              onRetry: () => ref.invalidate(takeoverOfferProvider(widget.caseId)),
              builder: (known) => ListView(
                padding: const EdgeInsets.all(16),
                children: [
                  Text(known.albumName, key: const Key('s23-album'), style: Theme.of(context).textTheme.titleMedium),
                  const SizedBox(height: 8),
                  Text(l10n.takeoverDeadline(_dayText(known.answerBy)), key: const Key('s23-deadline')),
                  Text(l10n.takeoverFounder(known.founderMemories), key: const Key('s23-founder')),
                  const SizedBox(height: 8),
                  Text(l10n.takeoverExplained),
                  Text(l10n.takeoverStoreAdvice),
                  ...failureBlock(context),
                  const SizedBox(height: 16),
                  FilledButton(
                    onPressed: offline || busy ? null : () => _answer(accepted: true),
                    child: Text(l10n.takeoverAccept),
                  ),
                  const SizedBox(height: 8),
                  OutlinedButton(
                    onPressed: offline || busy ? null : () => _answer(accepted: false),
                    child: Text(l10n.takeoverRefuse),
                  ),
                ],
              ),
            ),
          ),
        ],
      ),
    );
  }
}

/// S25 Reporting the death of an owner: someone in the album, with the certificate; the service checks it.
class ReportDeathPage extends ConsumerStatefulWidget {
  const ReportDeathPage({super.key, required this.albumId, required this.onDone});

  final String albumId;
  final VoidCallback onDone;

  @override
  ConsumerState<ReportDeathPage> createState() => _ReportDeathPageState();
}

class _ReportDeathPageState extends ConsumerState<ReportDeathPage> with SuccessionActions {
  // Made once, so sending again after a failure is the same report.
  final _reportId = const Uuid().v7();
  PickedPhoto? _certificate;
  var _sent = false;

  Future<void> _pick() async {
    final picked = await ref.read(photoPickerProvider).pick();
    if (mounted && picked.isNotEmpty) {
      setState(() => _certificate = picked.first);
    }
  }

  Future<void> _send(PickedPhoto certificate) async {
    if (await act(() => succession.report(_reportId, widget.albumId, certificate))) {
      setState(() => _sent = true);
    }
  }

  @override
  Widget build(BuildContext context) {
    final l10n = context.l10n;
    final album = ref.watch(albumDetailsProvider(widget.albumId));
    final certificate = _certificate;

    return Scaffold(
      appBar: AppBar(title: Text(l10n.deathReportTitle)),
      body: Column(
        children: [
          ...offlineBlock(context),
          Expanded(
            child: SuccessionAsyncBody(
              value: album,
              onRetry: () => ref.invalidate(albumDetailsProvider(widget.albumId)),
              builder: (known) => ListView(
                padding: const EdgeInsets.all(16),
                children: [
                  Text(known.name, key: const Key('s25-album'), style: Theme.of(context).textTheme.titleMedium),
                  const SizedBox(height: 8),
                  Text(l10n.deathReportExplained),
                  const SizedBox(height: 8),
                  if (_sent) ...[
                    Text(l10n.deathReportSent, key: const Key('s25-sent')),
                    const SizedBox(height: 16),
                    FilledButton(onPressed: widget.onDone, child: Text(l10n.deathReportBack)),
                  ] else ...[
                    OutlinedButton.icon(
                      icon: const Icon(Icons.attach_file),
                      onPressed: offline || busy ? null : _pick,
                      label: Text(certificate?.name ?? l10n.deathReportPick),
                    ),
                    ...failureBlock(context),
                    const SizedBox(height: 16),
                    FilledButton(
                      onPressed: offline || busy || certificate == null ? null : () => _send(certificate),
                      child: Text(l10n.deathReportSend),
                    ),
                  ],
                ],
              ),
            ),
          ),
        ],
      ),
    );
  }
}

/// In S10 of an album without an owner: taking it over (D-004).
class AlbumTakeoverBlock extends ConsumerStatefulWidget {
  const AlbumTakeoverBlock({super.key, required this.albumId});

  final String albumId;

  @override
  ConsumerState<AlbumTakeoverBlock> createState() => _AlbumTakeoverBlockState();
}

class _AlbumTakeoverBlockState extends ConsumerState<AlbumTakeoverBlock> with SuccessionActions {
  Future<void> _reserve() async {
    await act(() => succession.reserve(widget.albumId));
    ref.invalidate(albumTakeoverProvider(widget.albumId));
  }

  @override
  Widget build(BuildContext context) {
    final l10n = context.l10n;
    final takeover = ref.watch(albumTakeoverProvider(widget.albumId)).value;
    final until = takeover?.reservedUntil?.toLocal();
    final time = until == null
        ? ''
        : '${until.hour.toString().padLeft(2, '0')}:${until.minute.toString().padLeft(2, '0')}';

    return Column(
      key: const Key('s10-ownerless'),
      crossAxisAlignment: CrossAxisAlignment.start,
      children: [
        Text(l10n.takeoverNoOwner),
        if (takeover?.reservedByMe ?? false)
          Text(l10n.takeoverReservedByMe(time), key: const Key('s10-reserved-mine'))
        else if (until != null)
          Text(l10n.takeoverReservedByOther, key: const Key('s10-reserved-other')),
        ...failureBlock(context),
        if (takeover?.canReserve ?? false)
          FilledButton(onPressed: offline || busy ? null : _reserve, child: Text(l10n.takeoverReserve)),
      ],
    );
  }
}

/// On top of S09: what waits for this person's answer. Nothing when nothing waits, or when it did not load.
class TakeoverOffersList extends ConsumerWidget {
  const TakeoverOffersList({super.key, required this.onOpen});

  final void Function(String caseId) onOpen;

  @override
  Widget build(BuildContext context, WidgetRef ref) {
    final offers = ref.watch(takeoverOffersProvider).value ?? const [];
    return Column(
      children: [
        for (final offer in offers)
          Card(
            child: ListTile(
              key: Key('s09-offer-${offer.id}'),
              leading: const Icon(Icons.how_to_reg_outlined),
              title: Text(context.l10n.takeoverOfferTile(offer.albumName)),
              subtitle: Text(context.l10n.takeoverDeadline(_dayText(offer.answerBy))),
              onTap: () => onOpen(offer.id),
            ),
          ),
      ],
    );
  }
}

/// Who may read the person's private memories after their death (S2:C8).
class LegacyContactPage extends ConsumerStatefulWidget {
  const LegacyContactPage({super.key});

  @override
  ConsumerState<LegacyContactPage> createState() => _LegacyContactPageState();
}

class _LegacyContactPageState extends ConsumerState<LegacyContactPage> with SuccessionActions {
  final _email = TextEditingController();

  @override
  void dispose() {
    _email.dispose();
    super.dispose();
  }

  Future<void> _save() async {
    if (await act(() => succession.nameContact(_email.text))) {
      _email.clear();
      ref.invalidate(legacyContactProvider);
    }
  }

  Future<void> _remove() async {
    if (await act(succession.removeContact)) {
      ref.invalidate(legacyContactProvider);
    }
  }

  @override
  Widget build(BuildContext context) {
    final l10n = context.l10n;
    final contact = ref.watch(legacyContactProvider);

    return Scaffold(
      appBar: AppBar(title: Text(l10n.legacyContactTitle)),
      body: Column(
        children: [
          ...offlineBlock(context),
          Expanded(
            child: SuccessionAsyncBody(
              value: contact,
              onRetry: () => ref.invalidate(legacyContactProvider),
              builder: (email) => ListView(
                padding: const EdgeInsets.all(16),
                children: [
                  Text(l10n.legacyContactExplained),
                  const SizedBox(height: 12),
                  Text(
                    email == null ? l10n.legacyContactNone : l10n.legacyContactCurrent(email),
                    key: const Key('legacy-current'),
                  ),
                  const SizedBox(height: 12),
                  TextField(
                    controller: _email,
                    keyboardType: TextInputType.emailAddress,
                    decoration: InputDecoration(labelText: l10n.legacyContactEmail),
                  ),
                  ...failureBlock(context),
                  const SizedBox(height: 12),
                  FilledButton(onPressed: offline || busy ? null : _save, child: Text(l10n.legacyContactSave)),
                  if (email != null)
                    TextButton(onPressed: offline || busy ? null : _remove, child: Text(l10n.legacyContactRemove)),
                ],
              ),
            ),
          ),
        ],
      ),
    );
  }
}

/// On top of S16: people who named this person and have died.
class LegacyArchivesList extends ConsumerWidget {
  const LegacyArchivesList({super.key, required this.onOpen});

  final void Function(String ownerAccountId) onOpen;

  @override
  Widget build(BuildContext context, WidgetRef ref) {
    final archives = ref.watch(legacyArchivesProvider).value ?? const [];
    return Column(
      children: [
        for (final archive in archives)
          ListTile(
            key: Key('s16-legacy-${archive.ownerAccountId}'),
            leading: const Icon(Icons.auto_stories_outlined),
            title: Text(archive.email ?? context.l10n.legacyArchiveUnknown),
            subtitle: Text(context.l10n.legacyArchiveSubtitle),
            onTap: () => onOpen(archive.ownerAccountId),
          ),
      ],
    );
  }
}

/// The private memories of someone who named this person: read only.
class LegacyArchivePage extends ConsumerWidget {
  const LegacyArchivePage({super.key, required this.ownerAccountId, required this.onOpenMemory});

  final String ownerAccountId;
  final void Function(String memoryId) onOpenMemory;

  @override
  Widget build(BuildContext context, WidgetRef ref) {
    final l10n = context.l10n;
    final memories = ref.watch(legacyArchiveProvider(ownerAccountId));

    return Scaffold(
      appBar: AppBar(title: Text(l10n.legacyArchiveTitle)),
      body: SuccessionAsyncBody(
        value: memories,
        onRetry: () => ref.invalidate(legacyArchiveProvider(ownerAccountId)),
        builder: (items) => items.isEmpty
            ? Center(child: Text(l10n.legacyArchiveEmpty))
            : ListView(
                children: [
                  for (final item in items)
                    ListTile(
                      title: Text(item.placeName),
                      subtitle: Text(dayTextOf(item.dateFrom)),
                      onTap: () => onOpenMemory(item.id),
                    ),
                ],
              ),
      ),
    );
  }
}
