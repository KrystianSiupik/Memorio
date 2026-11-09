import 'dart:async';

import 'package:flutter/material.dart';
import 'package:flutter_riverpod/flutter_riverpod.dart';
import 'package:flutter_riverpod/misc.dart';
import 'package:flutter_test/flutter_test.dart';
import 'package:memorio/app/router.dart';
import 'package:memorio/app/routes.dart';
import 'package:memorio/features/ai_summaries/ai_summaries.dart';

import '../../helpers/app.dart';
import '../../helpers/fake_albums.dart';
import '../../helpers/fake_summaries.dart';

void main() {
  late FakeSummariesRepository summaries;
  late FakeAlbumsRepository albums;
  final location = const AlbumSummaryRoute('album-1').location;

  setUp(() {
    summaries = FakeSummariesRepository();
    albums = FakeAlbumsRepository()..details = albumDetails();
  });

  Future<ProviderContainer> open(
    WidgetTester tester,
    String at, {
    Stream<bool>? online,
    List<Override> overrides = const [],
  }) async {
    tester.view.physicalSize = const Size(1000, 2400);
    tester.view.devicePixelRatio = 1;
    addTearDown(tester.view.reset);
    final container = await pumpMemorioApp(
      tester,
      session: signedIn,
      albums: albums,
      summaries: summaries,
      online: online,
      overrides: overrides,
    );
    container.read(routerProvider).go(at);
    await tester.pumpAndSettle();
    return container;
  }

  bool isEnabled(WidgetTester tester, String key) => switch (tester.widget(find.byKey(Key(key)))) {
    final ButtonStyleButton button => button.onPressed != null,
    _ => false,
  };

  Future<void> request(WidgetTester tester) async {
    await tester.tap(find.byKey(const Key('s17-request')));
    await tester.pumpAndSettle();
  }

  group('S17', () {
    testWidgets('the story shows the period, the AI marker, the materials and who was left out', (tester) async {
      summaries.requested = periodSummary(leftOutPeople: 2, droppedSentences: 1);
      await open(tester, location);

      expect(find.text('Podsumowanie okresu (AI)'), findsOneWidget);
      expect(find.byKey(const Key('s17-from')), findsOneWidget);
      expect(find.byKey(const Key('s17-to')), findsOneWidget);
      await request(tester);

      final card = find.byKey(const Key('s17-summary'));
      expect(find.descendant(of: card, matching: find.text('01.07.2026 – 31.07.2026')), findsOneWidget);
      expect(find.descendant(of: card, matching: find.text('Tekst napisany przez AI')), findsOneWidget);
      expect(find.text('W lipcu byliście w górach. Potem nad morzem.'), findsOneWidget);
      expect(find.text('Materiały dozwolone (wspomnienia): 3'), findsOneWidget);
      expect(find.text('Pominięto uczestników (brak zgody): 2'), findsOneWidget);
      expect(find.text('Pominięte zdania AI bez źródła: 1'), findsOneWidget);
      expect(summaries.actions.single, startsWith('request:'));
      expect(summaries.actions.single, endsWith(':album-1'));
    });

    testWidgets('approved and saved, then added to the album where the participants see it', (tester) async {
      await open(tester, location);
      expect(find.byKey(const Key('s17-album-empty')), findsOneWidget);
      await request(tester);

      await tester.tap(find.text('Zatwierdź i zapisz'));
      await tester.pumpAndSettle();
      expect(find.text('Zapisane jako pamiątka'), findsOneWidget);
      expect(find.byKey(const Key('s17-save')), findsNothing);

      await tester.tap(find.text('Dodaj do albumu (widzą uczestnicy)'));
      await tester.pumpAndSettle();
      expect(find.byKey(const Key('s17-added')), findsOneWidget);
      expect(find.byKey(const Key('s17-album-summary-1')), findsOneWidget);
      expect(summaries.actions.skip(1), ['save:summary-1', 'add:summary-1']);
    });

    testWidgets('asking needs the AI allowance left and the consent, which is given right there', (tester) async {
      await open(tester, location, overrides: [aiOperationsLeftProvider.overrideWith((ref) async => 0)]);

      expect(isEnabled(tester, 's17-request'), isFalse);
      expect(find.text('Limit AI w tym miesiącu jest wykorzystany.'), findsOneWidget);
    });

    testWidgets('without the consent asking waits for it', (tester) async {
      summaries.given = false;
      await open(tester, location);

      expect(isEnabled(tester, 's17-request'), isFalse);
      expect(find.byKey(const Key('s17-consent-needed')), findsOneWidget);

      await tester.tap(find.byKey(const Key('ai-consent')));
      await tester.pumpAndSettle();
      expect(summaries.actions, ['consent:album-1:true']);
      expect(isEnabled(tester, 's17-request'), isTrue);
      expect(find.byKey(const Key('s17-consent-needed')), findsNothing);
    });

    testWidgets('loading says it is being prepared', (tester) async {
      summaries.writing = Completer<void>();
      await open(tester, location);

      await tester.tap(find.byKey(const Key('s17-request')));
      await tester.pump();
      expect(find.text('Przygotowujemy podsumowanie — damy znać powiadomieniem'), findsOneWidget);
      expect(isEnabled(tester, 's17-request'), isFalse);

      summaries.writing!.complete();
      await tester.pumpAndSettle();
      expect(find.byKey(const Key('s17-loading')), findsNothing);
      expect(find.byKey(const Key('s17-summary')), findsOneWidget);
    });

    const messages = <SummaryFailure, String>{
      SummaryOffline(): 'Brak połączenia. Sprawdź internet i spróbuj ponownie.',
      SummaryNoAccess(): 'Nie masz dostępu do tego podsumowania ani albumu.',
      SummaryAlreadyDecided(): 'To już rozstrzygnięte. Odśwież i sprawdź aktualny stan.',
      SummaryServerError(): 'Błąd serwera. Spróbuj ponownie za chwilę.',
      SummaryLimitReached(): 'Limit AI w tym miesiącu jest wykorzystany.',
      SummaryNothingTold(): 'AI nie napisało nic, co potwierdzają wspomnienia. Spróbuj ponownie.',
    };
    for (final MapEntry(key: failure, value: message) in messages.entries) {
      testWidgets('error: ${failure.runtimeType} says so', (tester) async {
        summaries.actionFailure = failure;
        await open(tester, location);
        await request(tester);

        expect(
          find.descendant(of: find.byKey(const Key('s17-error')), matching: find.text(message), matchRoot: true),
          findsOneWidget,
        );
        expect(find.byKey(const Key('s17-summary')), findsNothing);
      });
    }

    testWidgets('a request lost on the way is asked again with the same id, for one AI operation', (tester) async {
      summaries.actionFailure = const SummaryServerError();
      await open(tester, location);
      await request(tester);

      summaries.actionFailure = null;
      await tester.tap(find.byKey(const Key('s17-retry')));
      await tester.pumpAndSettle();

      expect(summaries.actions, hasLength(2));
      expect(summaries.actions.first, summaries.actions.last);
      expect(find.byKey(const Key('s17-summary')), findsOneWidget);
    });

    testWidgets('approving a summary whose memories changed says to ask anew', (tester) async {
      await open(tester, location);
      await request(tester);
      summaries.actionFailure = const SummaryMaterialsChanged();

      await tester.tap(find.text('Zatwierdź i zapisz'));
      await tester.pumpAndSettle();

      expect(find.text('Części wspomnień nie wolno już użyć. Zamów nowe podsumowanie.'), findsOneWidget);
      expect(find.byKey(const Key('s17-retry')), findsNothing);
    });

    testWidgets('empty: nothing that may be used', (tester) async {
      summaries.actionFailure = const SummaryNoMaterials();
      await open(tester, location);
      await request(tester);

      expect(find.text('Brak materiałów, które wolno użyć'), findsOneWidget);
      expect(find.byKey(const Key('s17-error')), findsNothing);
    });

    testWidgets('offline: asking is off and the saved summaries stay readable', (tester) async {
      summaries.album = [periodSummary(id: 'kept', state: SummaryState.saved, addedToAlbum: true)];
      await open(tester, location, online: Stream.value(false));

      expect(find.byKey(const Key('s17-offline')), findsOneWidget);
      expect(isEnabled(tester, 's17-request'), isFalse);
      expect(find.byKey(const Key('s17-album-kept')), findsOneWidget);
      expect(
        find.descendant(of: find.byKey(const Key('s17-album-kept')), matching: find.byKey(const Key('s17-text'))),
        findsOneWidget,
      );
    });
  });

  group('S17 transitions', () {
    testWidgets('a participant opens it from the album', (tester) async {
      await open(tester, const AlbumRoute('album-1').location);

      await tester.tap(find.byKey(const Key('album-summaries')));
      await tester.pumpAndSettle();

      expect(find.text('Podsumowanie okresu (AI)'), findsOneWidget);
      expect(find.byType(AlbumSummaryPage), findsOneWidget);
    });

    testWidgets('who does not take part has no entry', (tester) async {
      albums.details = albumDetails(isMember: false);
      await open(tester, const AlbumRoute('album-1').location);

      expect(find.byKey(const Key('album-summaries')), findsNothing);
    });
  });

  group('S12 AI consent', () {
    testWidgets('from an album the consent is taken back there', (tester) async {
      await open(tester, const AlbumConsentsRoute('album-1').location);

      await tester.tap(find.byKey(const Key('ai-consent')));
      await tester.pumpAndSettle();

      expect(summaries.actions, ['consent:album-1:false']);
      expect(tester.widget<SwitchListTile>(find.byKey(const Key('ai-consent'))).value, isFalse);
    });

    testWidgets('from the list there is no album to consent for', (tester) async {
      await open(tester, const MyConsentsRoute().location);

      expect(find.byKey(const Key('ai-consent')), findsNothing);
    });
  });
}
