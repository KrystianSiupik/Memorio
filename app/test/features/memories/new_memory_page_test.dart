import 'package:drift/drift.dart' show driftRuntimeOptions;
import 'package:flutter/material.dart';
import 'package:flutter_riverpod/flutter_riverpod.dart';
import 'package:flutter_test/flutter_test.dart';
import 'package:memorio/app/router.dart';
import 'package:memorio/app/routes.dart';
import 'package:memorio/core/offline/offline.dart';
import 'package:memorio/features/memories/memories.dart';

import '../../helpers/app.dart';
import '../../helpers/fake_account.dart';
import '../../helpers/fake_memories.dart';
import '../../helpers/fake_places.dart';

void main() {
  driftRuntimeOptions.dontWarnAboutMultipleDatabases = true;

  late FakeMemoriesRepository memories;
  late FakePlacesRepository places;
  late FakeAccountRepository account;

  setUp(() {
    memories = FakeMemoriesRepository();
    places = FakePlacesRepository();
    account = FakeAccountRepository();
  });

  Future<ProviderContainer> open(WidgetTester tester, {Stream<bool>? online}) async {
    tester.view.physicalSize = const Size(1000, 2400);
    tester.view.devicePixelRatio = 1;
    addTearDown(tester.view.reset);
    final container = await pumpMemorioApp(
      tester,
      session: signedIn,
      memories: memories,
      places: places,
      account: account,
      online: online,
    );
    container.read(routerProvider).go(const NewMemoryRoute().location);
    await tester.pumpAndSettle();
    return container;
  }

  Future<void> choosePlace(WidgetTester tester) async {
    await tester.enterText(find.widgetWithText(TextField, 'Miejsce'), 'Gie');
    await tester.pump(const Duration(milliseconds: 350));
    await tester.pumpAndSettle();
    await tester.tap(find.text('Giewont'));
    await tester.pumpAndSettle();
  }

  Future<void> chooseToday(WidgetTester tester) async {
    await tester.tap(find.text('Wybierz datę'));
    await tester.pumpAndSettle();
    await tester.tap(find.text('OK'));
    await tester.pumpAndSettle();
  }

  Future<void> toContent(WidgetTester tester) async {
    await choosePlace(tester);
    await chooseToday(tester);
    await tester.tap(find.widgetWithText(FilledButton, 'Dalej'));
    await tester.pumpAndSettle();
  }

  bool isEnabled(WidgetTester tester, String label) => switch (tester.widget(
    find
        .ancestor(of: find.text(label), matching: find.byWidgetPredicate((widget) => widget is ButtonStyleButton))
        .first,
  )) {
    final ButtonStyleButton button => button.onPressed != null,
    _ => false,
  };

  Future<void> save(WidgetTester tester) async {
    await tester.tap(find.widgetWithText(FilledButton, 'Zapisz wspomnienie'));
    await tester.pumpAndSettle();
  }

  testWidgets('shows the fields of both steps', (tester) async {
    await open(tester);

    for (final label in ['Miejsce', 'Nazwa własna miejsca', 'Data', 'Wybierz datę', 'Dodaj miejsce własne']) {
      expect(find.text(label), findsWidgets, reason: label);
    }
    expect(isEnabled(tester, 'Dalej'), isFalse);

    await toContent(tester);

    for (final label in ['Zdjęcia', 'Opis', 'Poproś AI o szkic', 'Zatwierdź opis', 'Zapisz wspomnienie']) {
      expect(find.text(label), findsOneWidget, reason: label);
    }
  });

  testWidgets('suggestions come as the person types', (tester) async {
    await open(tester);

    await choosePlace(tester);

    expect(places.searched, ['Gie']);
    expect(find.text('Wybrano: Giewont'), findsOneWidget);
  });

  testWidgets('an own place needs a name and valid coordinates', (tester) async {
    await open(tester);

    await tester.enterText(find.widgetWithText(TextField, 'Nazwa własna miejsca'), 'Chata');
    await tester.enterText(find.widgetWithText(TextField, 'Szerokość geograficzna'), '95');
    await tester.enterText(find.widgetWithText(TextField, 'Długość geograficzna'), '19,9');
    await tester.tap(find.text('Dodaj miejsce własne'));
    await tester.pumpAndSettle();

    expect(find.text('Podaj nazwę oraz szerokość od -90 do 90 i długość od -180 do 180.'), findsOneWidget);
    expect(places.addedOwn, isEmpty);

    await tester.enterText(find.widgetWithText(TextField, 'Szerokość geograficzna'), '49,3');
    await tester.tap(find.text('Dodaj miejsce własne'));
    await tester.pumpAndSettle();

    expect(places.addedOwn, ['Chata']);
    expect(find.text('Wybrano: Chata'), findsOneWidget);
  });

  testWidgets('S04 → S03: saving sends the memory, its photos and the approved description', (tester) async {
    await open(tester);
    await toContent(tester);
    await tester.tap(find.text('Dodaj zdjęcia'));
    await tester.pumpAndSettle();
    await tester.enterText(find.widgetWithText(TextField, 'Opis'), 'W górach.');
    await tester.pump();
    await tester.tap(find.text('Zatwierdź opis'));
    await tester.pump();

    await save(tester);

    expect(memories.created.single.placeReference, 'osm:N:1');
    expect(memories.created.single.precision, MemoryDatePrecision.day);
    expect(memories.photos, hasLength(1));
    expect(memories.approved.single.text, 'W górach.');
    expect(memories.approved.single.aiDraftId, isNull);
    expect(find.widgetWithText(AppBar, 'Mapa'), findsOneWidget);
  });

  testWidgets('a day may get an hour, which can be taken back; other precisions have none', (tester) async {
    await open(tester);
    await choosePlace(tester);
    await chooseToday(tester);
    await tester.tap(find.byKey(const Key('s04-time')));
    await tester.pumpAndSettle();
    await tester.tap(find.text('OK'));
    await tester.pumpAndSettle();
    expect(find.text('Godzina: 12:00'), findsOneWidget);
    await tester.tap(find.byKey(const Key('s04-time-clear')));
    await tester.pump();
    expect(find.text('Dodaj godzinę (nieobowiązkowo)'), findsOneWidget);

    await tester.tap(find.text('Miesiąc'));
    await tester.pump();
    expect(find.byKey(const Key('s04-time')), findsNothing);
  });

  testWidgets('the hour goes with the memory', (tester) async {
    await open(tester);
    await choosePlace(tester);
    await chooseToday(tester);
    await tester.tap(find.byKey(const Key('s04-time')));
    await tester.pumpAndSettle();
    await tester.tap(find.text('OK'));
    await tester.pumpAndSettle();
    await tester.tap(find.widgetWithText(FilledButton, 'Dalej'));
    await tester.pumpAndSettle();

    await save(tester);

    expect(memories.created.single.time, '12:00');
  });

  testWidgets('a description not approved is not sent', (tester) async {
    await open(tester);
    await toContent(tester);
    await tester.enterText(find.widgetWithText(TextField, 'Opis'), 'W górach.');
    await tester.pump();

    await save(tester);

    expect(memories.approved, isEmpty);
  });

  testWidgets('editing after approving asks for it again', (tester) async {
    await open(tester);
    await toContent(tester);
    await tester.enterText(find.widgetWithText(TextField, 'Opis'), 'W górach.');
    await tester.pump();
    await tester.tap(find.text('Zatwierdź opis'));
    await tester.pump();
    expect(find.text('Opis zatwierdzony'), findsOneWidget);

    await tester.enterText(find.widgetWithText(TextField, 'Opis'), 'Inaczej.');
    await tester.pump();

    expect(find.text('Zatwierdź opis'), findsOneWidget);
  });

  group('the AI draft', () {
    testWidgets('waits for consent and shows with its marker; used and approved it keeps its id', (tester) async {
      await open(tester);
      await toContent(tester);
      await tester.enterText(find.widgetWithText(TextField, 'Opis'), 'Góry');
      await tester.pump();
      expect(isEnabled(tester, 'Poproś AI o szkic'), isFalse);

      await tester.tap(find.byType(CheckboxListTile));
      await tester.pump();
      await tester.tap(find.text('Poproś AI o szkic'));
      await tester.pumpAndSettle();

      expect(find.byKey(const Key('s04-ai-marker')), findsOneWidget);
      expect(find.text('Szkic: Góry'), findsOneWidget);

      await tester.tap(find.text('Użyj szkicu'));
      await tester.pump();
      await tester.tap(find.text('Zatwierdź opis'));
      await tester.pump();
      await save(tester);

      expect(memories.approved.single.text, 'Szkic: Góry');
      expect(memories.approved.single.aiDraftId, memories.draftRequests.single);
    });

    testWidgets('a used up limit says so and turns the action off', (tester) async {
      memories.draftFailure = const AiLimitReached();
      await open(tester);
      await toContent(tester);
      await tester.enterText(find.widgetWithText(TextField, 'Opis'), 'Góry');
      await tester.tap(find.byType(CheckboxListTile));
      await tester.pump();

      await tester.tap(find.text('Poproś AI o szkic'));
      await tester.pumpAndSettle();

      expect(find.text('Limit szkiców AI w tym miesiącu jest wyczerpany.'), findsOneWidget);
      expect(isEnabled(tester, 'Poproś AI o szkic'), isFalse);
    });

    testWidgets('after a provider failure the retry is the same draft', (tester) async {
      memories.draftFailure = const AiProviderDown();
      await open(tester);
      await toContent(tester);
      await tester.enterText(find.widgetWithText(TextField, 'Opis'), 'Góry');
      await tester.tap(find.byType(CheckboxListTile));
      await tester.pump();

      await tester.tap(find.text('Poproś AI o szkic'));
      await tester.pumpAndSettle();
      expect(find.text('Dostawca AI jest teraz niedostępny. Spróbuj później.'), findsOneWidget);

      memories.draftFailure = null;
      await tester.tap(find.text('Poproś AI o szkic'));
      await tester.pumpAndSettle();

      expect(memories.draftRequests.toSet(), hasLength(1));
    });
  });

  group('errors keep what was written, and saving again is the same memory', () {
    const messages = <MemoryFailure, String>{
      MemoryOffline(): 'Brak połączenia. Sprawdź internet i spróbuj ponownie.',
      MemoryNoAccess(): 'Nie masz dostępu do tego wspomnienia.',
      MemoryAlreadyDecided(): 'To już rozstrzygnięte. Odśwież i sprawdź aktualny stan.',
      MemoryServerError(): 'Błąd serwera. Spróbuj ponownie za chwilę.',
    };

    messages.forEach((failure, message) {
      testWidgets(message, (tester) async {
        memories.createFailure = failure;
        await open(tester);
        await toContent(tester);
        await tester.enterText(find.widgetWithText(TextField, 'Opis'), 'W górach.');
        await save(tester);

        expect(find.text(message), findsOneWidget);
        expect(find.text('W górach.'), findsOneWidget);

        memories.createFailure = null;
        await tester.tap(find.text('Ponów'));
        await tester.pumpAndSettle();

        expect(memories.created, hasLength(1));
        expect(find.widgetWithText(AppBar, 'Mapa'), findsOneWidget);
      });
    });
  });

  testWidgets('a full pool says so and turns adding photos off', (tester) async {
    memories.photoFailure = const MemorySpaceFull();
    await open(tester);
    await toContent(tester);
    await tester.tap(find.text('Dodaj zdjęcia'));
    await tester.pumpAndSettle();

    await save(tester);

    expect(find.text('Pula miejsca jest pełna. To zdjęcie się nie zmieści.'), findsOneWidget);
    expect(isEnabled(tester, 'Dodaj zdjęcia'), isFalse);
  });

  testWidgets('terms not accepted: accept them here and the save goes on', (tester) async {
    memories.createFailure = const MemoryTermsNotAccepted();
    await open(tester);
    await toContent(tester);
    await save(tester);

    expect(find.text('Przed pierwszym wspomnieniem zaakceptuj regulamin.'), findsOneWidget);

    memories.createFailure = null;
    await tester.tap(find.text('Akceptuję regulamin'));
    await tester.pumpAndSettle();

    expect(account.termsAccepted, 1);
    expect(memories.created, hasLength(1));
  });

  group('offline', () {
    testWidgets('saves to the queue in order, with only recent places and no AI', (tester) async {
      final container = await open(tester, online: Stream.value(false));
      container.read(recentPlacesProvider.notifier).remember(giewont);
      await tester.pumpAndSettle();

      expect(find.byKey(const Key('s04-offline')), findsOneWidget);
      expect(find.widgetWithText(TextField, 'Miejsce'), findsNothing);
      expect(find.text('Ostatnio użyte'), findsOneWidget);

      await tester.tap(find.text('Giewont'));
      await tester.pumpAndSettle();
      await chooseToday(tester);
      await tester.tap(find.widgetWithText(FilledButton, 'Dalej'));
      await tester.pumpAndSettle();

      expect(find.byKey(const Key('s04-ai-offline')), findsOneWidget);
      expect(find.text('Poproś AI o szkic'), findsNothing);

      await tester.tap(find.text('Dodaj zdjęcia'));
      await tester.pumpAndSettle();
      // The queue is a real database: its work needs real time, which a widget test only has in runAsync.
      // A stream read outside it leaves a timer that never fires, and closing the database then waits forever.
      final saveButton = tester.widget<FilledButton>(find.widgetWithText(FilledButton, 'Zapisz wspomnienie'));
      await tester.runAsync(() async {
        saveButton.onPressed!();
        await Future<void>.delayed(const Duration(milliseconds: 200));
      });
      await tester.pumpAndSettle();

      final queued = (await tester.runAsync(() => container.read(offlineQueueProvider).watch().first))!;
      expect(queued.map((operation) => operation.kind), [MemoryOperations.createMemory, MemoryOperations.addPhoto]);
      expect(queued.first.payload['placeReference'], 'osm:N:1');
      expect(queued.last.payload['memoryId'], queued.first.payload['id']);
      expect(memories.created, isEmpty);
      expect(find.text('Zapisano. Wyślemy, gdy wróci sieć.'), findsOneWidget);
    });
  });
}
