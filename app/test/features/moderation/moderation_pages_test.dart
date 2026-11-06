import 'package:flutter/material.dart';
import 'package:flutter_test/flutter_test.dart';
import 'package:memorio/app/router.dart';
import 'package:memorio/app/routes.dart';
import 'package:memorio/features/memories/memories.dart';
import 'package:memorio/features/moderation/moderation.dart';

import '../../helpers/app.dart';
import '../../helpers/fake_albums.dart';
import '../../helpers/fake_memories.dart';
import '../../helpers/fake_moderation.dart';

void main() {
  late FakeModerationRepository moderation;
  late FakeMemoriesRepository memories;
  late FakeAlbumsRepository albums;

  const theirs = MemoryDetails(
    id: 'memory-2',
    placeName: 'Giewont',
    dateFrom: '2026-07-01',
    dateTo: '2026-07-01',
    isMine: false,
    description: null,
    descriptionIsAi: false,
    photoUrls: [],
  );

  setUp(() {
    moderation = FakeModerationRepository();
    memories = FakeMemoriesRepository()..details = theirs;
    albums = FakeAlbumsRepository();
  });

  Future<void> open(WidgetTester tester, String location, {Stream<bool>? online}) async {
    tester.view.physicalSize = const Size(1000, 2600);
    tester.view.devicePixelRatio = 1;
    addTearDown(tester.view.reset);
    final container = await pumpMemorioApp(
      tester,
      session: signedIn,
      memories: memories,
      albums: albums,
      moderation: moderation,
      online: online,
    );
    container.read(routerProvider).go(location);
    await tester.pumpAndSettle();
  }

  bool isEnabled(WidgetTester tester, String label) =>
      tester
          .widget<ButtonStyleButton>(
            find
                .ancestor(
                  of: find.text(label),
                  matching: find.byWidgetPredicate((widget) => widget is ButtonStyleButton),
                )
                .first,
          )
          .onPressed !=
      null;

  group('S27', () {
    testWidgets('S05 → S27: somebody else\'s memory can be reported, an illegal one is hidden at once', (tester) async {
      await open(tester, const MemoryRoute('memory-2').location);
      await tester.tap(find.byKey(const Key('s05-report')));
      await tester.pumpAndSettle();

      expect(find.text('Zgłoś wspomnienie'), findsOneWidget);
      expect(isEnabled(tester, 'Zgłoś'), isFalse);
      await tester.tap(find.byKey(const Key('s27-reason-Threat')));
      await tester.pump();
      expect(find.byKey(const Key('s27-illegal')), findsOneWidget);
      await tester.enterText(find.widgetWithText(TextField, 'Uzasadnienie'), 'Grozi komuś.');
      await tester.pump();
      await tester.tap(find.text('Zgłoś'));
      await tester.pumpAndSettle();

      expect(moderation.actions.single, matches(RegExp(r'^report:[^:]+:Memory:memory-2:Threat:Grozi komuś\.$')));
      expect(find.byKey(const Key('s27-sent')), findsOneWidget);
      await tester.tap(find.text('Wróć'));
      await tester.pumpAndSettle();
      expect(find.byKey(const Key('s05-report')), findsOneWidget);
    });

    testWidgets('one\'s own memory has no report', (tester) async {
      memories.details = const MemoryDetails(
        id: 'memory-1',
        placeName: 'Giewont',
        dateFrom: '2026-07-01',
        dateTo: '2026-07-01',
        isMine: true,
        description: null,
        descriptionIsAi: false,
        photoUrls: [],
      );
      await open(tester, const MemoryRoute('memory-1').location);

      expect(find.byKey(const Key('s05-report')), findsNothing);
    });

    testWidgets('a refusal keeps what was typed, and sending again is the same report', (tester) async {
      moderation.actionFailure = const ModerationServerError();
      await open(tester, const AlbumReportRoute('album-1').location);
      await tester.tap(find.byKey(const Key('s27-reason-PersonalData')));
      await tester.enterText(find.widgetWithText(TextField, 'Uzasadnienie'), 'Moje dane.');
      await tester.pump();
      await tester.tap(find.text('Zgłoś'));
      await tester.pumpAndSettle();

      expect(find.byKey(const Key('moderation-error')), findsOneWidget);
      expect(find.text('Moje dane.'), findsOneWidget);
      moderation.actionFailure = null;
      await tester.tap(find.text('Zgłoś'));
      await tester.pumpAndSettle();

      expect(moderation.actions, hasLength(2));
      expect(moderation.actions.first.split(':')[1], moderation.actions.last.split(':')[1]);
      expect(moderation.actions.last, contains(':Album:album-1:PersonalData:'));
    });

    testWidgets('offline turns sending off', (tester) async {
      await open(tester, const AlbumReportRoute('album-1').location, online: Stream.value(false));
      await tester.tap(find.byKey(const Key('s27-reason-Other')));
      await tester.enterText(find.widgetWithText(TextField, 'Uzasadnienie'), 'x');
      await tester.pump();

      expect(find.byKey(const Key('moderation-offline')), findsOneWidget);
      expect(isEnabled(tester, 'Zgłoś'), isFalse);
    });

    testWidgets('S10 → S27: someone in the album reports it; the owner has no report', (tester) async {
      albums.details = albumDetails();
      await open(tester, const AlbumRoute('album-1').location);
      await tester.tap(find.byKey(const Key('s10-report')));
      await tester.pumpAndSettle();
      expect(find.text('Zgłoś album'), findsOneWidget);

      albums.details = albumDetails(isOwner: true);
      await open(tester, const AlbumRoute('album-1').location);
      expect(find.byKey(const Key('s10-report')), findsNothing);
    });
  });

  group('S22', () {
    final location = const MyModerationRoute().location;

    testWidgets('empty says so', (tester) async {
      await open(tester, location);

      expect(find.byKey(const Key('s22-empty')), findsOneWidget);
    });

    testWidgets('shows decisions with their reasons and takes an appeal while it is possible', (tester) async {
      moderation.mine = MyModeration(
        aboutMine: [
          ReportAboutMine(
            id: 'r1',
            target: ReportTarget.memory,
            reason: ReportReason.personalData,
            state: 'Decided',
            hidden: true,
            verdict: 'Removed',
            statement: 'Dane osobowe bez zgody.',
            reportedAt: DateTime(2026, 11, 1, 12),
            appealUntil: DateTime.now().add(const Duration(days: 100)),
          ),
          ReportAboutMine(
            id: 'r2',
            target: ReportTarget.album,
            reason: ReportReason.other,
            state: 'Decided',
            hidden: false,
            verdict: 'Kept',
            statement: 'Brak naruszenia.',
            reportedAt: DateTime(2026, 11, 2, 12),
          ),
        ],
        reports: [
          MyReport(id: 'r3', reason: ReportReason.copyright, state: 'Open', reportedAt: DateTime(2026, 11, 3, 12)),
          MyReport(
            id: 'r4',
            reason: ReportReason.someonesImage,
            state: 'Decided',
            verdict: 'Kept',
            statement: 'Nie widać twarzy.',
            reportedAt: DateTime(2026, 11, 3, 12),
            appealUntil: DateTime.now().add(const Duration(days: 100)),
          ),
        ],
      );
      await open(tester, location);

      expect(find.text('Moje dane osobowe, zgłoszone 01.11.2026'), findsOneWidget);
      expect(find.text('Uzasadnienie: Dane osobowe bez zgody.'), findsOneWidget);
      expect(find.text('Teraz widzisz to tylko Ty.'), findsOneWidget);
      expect(find.byKey(const Key('s22-appeal-r2')), findsNothing);
      expect(find.byKey(const Key('s22-report-r3')), findsOneWidget);
      expect(tester.widget<FilledButton>(find.byKey(const Key('s22-appeal-r1'))).onPressed, isNull);

      await tester.enterText(find.byKey(const Key('s22-appeal-text-r1')), 'To moje zdjęcie.');
      await tester.pump();
      await tester.tap(find.byKey(const Key('s22-appeal-r1')));
      await tester.pumpAndSettle();

      expect(moderation.actions, ['appeal:r1:To moje zdjęcie.']);
      expect(find.byKey(const Key('s22-appeal-r3')), findsNothing);
      await tester.ensureVisible(find.byKey(const Key('s22-appeal-text-r4')));
      await tester.enterText(find.byKey(const Key('s22-appeal-text-r4')), 'Widać mnie wyraźnie.');
      await tester.pump();
      await tester.tap(find.byKey(const Key('s22-appeal-r4')));
      await tester.pumpAndSettle();
      expect(moderation.actions.last, 'appeal:r4:Widać mnie wyraźnie.');
    });

    testWidgets('a failed load says so and Ponów loads again', (tester) async {
      moderation.loadFailure = const ModerationOffline();
      await open(tester, location);
      expect(find.byKey(const Key('s22-load-error')), findsOneWidget);

      moderation.loadFailure = null;
      await tester.tap(find.text('Ponów'));
      await tester.pumpAndSettle();
      expect(find.byKey(const Key('s22-empty')), findsOneWidget);
    });

    testWidgets('S24 → S22 from the account', (tester) async {
      await open(tester, const AccountRoute().location);
      await tester.tap(find.byTooltip('Moje zgłoszenia i decyzje'));
      await tester.pumpAndSettle();

      expect(find.byKey(const Key('s22-empty')), findsOneWidget);
    });
  });
}
