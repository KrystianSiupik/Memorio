import 'dart:async';

import 'package:flutter/material.dart';
import 'package:flutter_test/flutter_test.dart';
import 'package:memorio/app/router.dart';
import 'package:memorio/app/routes.dart';
import 'package:memorio/core/result.dart';
import 'package:memorio/features/memories/memories.dart';

import '../../helpers/app.dart';
import '../../helpers/fake_memories.dart';

void main() {
  late FakeMemoriesRepository memories;

  const mine = MemoryDetails(
    id: 'memory-1',
    placeName: 'Giewont',
    dateFrom: '2026-07-01',
    dateTo: '2026-07-20',
    isMine: true,
    description: 'W górach.',
    descriptionIsAi: true,
    photoUrls: [],
  );

  setUp(() {
    memories = FakeMemoriesRepository()..details = mine;
  });

  Future<void> open(WidgetTester tester, {Stream<bool>? online, MemoriesRepository? repository}) async {
    tester.view.physicalSize = const Size(1000, 2400);
    tester.view.devicePixelRatio = 1;
    addTearDown(tester.view.reset);
    final container = await pumpMemorioApp(tester, session: signedIn, memories: repository ?? memories, online: online);
    container.read(routerProvider).go(const MemoryRoute('memory-1').location);
    await tester.pumpAndSettle();
  }

  bool isEnabled(WidgetTester tester, Finder finder) => switch (tester.widget(finder)) {
    final FilledButton button => button.onPressed != null,
    final OutlinedButton button => button.onPressed != null,
    _ => throw StateError('Not a button'),
  };

  Finder trash() => find.widgetWithText(OutlinedButton, 'Usuń moją treść');
  Finder forGood() => find.widgetWithText(FilledButton, 'Usuń trwale (formalne żądanie)');

  testWidgets('shows the place, the date, the content with its author, the people and the AI marker', (tester) async {
    await open(tester);

    expect(find.text('Miejsce: Giewont'), findsOneWidget);
    expect(find.text('Data: 01.07.2026 – 20.07.2026'), findsOneWidget);
    expect(find.text('Autor: Ty'), findsOneWidget);
    expect(find.text('W górach.'), findsOneWidget);
    expect(find.text('Nikt nie jest oznaczony'), findsOneWidget);
    expect(find.byKey(const Key('s05-ai-marker')), findsOneWidget);
  });

  testWidgets('S05 → S03: moving to the trash leaves the screen', (tester) async {
    await open(tester);

    await tester.tap(trash());
    await tester.pumpAndSettle();

    expect(memories.trashed, ['memory-1']);
    expect(find.widgetWithText(AppBar, 'Mapa'), findsOneWidget);
  });

  testWidgets('S05 → S03: deleting for good waits for the confirmation', (tester) async {
    await open(tester);
    expect(isEnabled(tester, forGood()), isFalse);

    await tester.tap(find.text('Rozumiem, że tego nie da się cofnąć'));
    await tester.pumpAndSettle();
    await tester.tap(forGood());
    await tester.pumpAndSettle();

    expect(memories.deleted, ['memory-1']);
    expect(find.widgetWithText(AppBar, 'Mapa'), findsOneWidget);
  });

  testWidgets('no actions on content of someone else', (tester) async {
    memories.details = const MemoryDetails(
      id: 'memory-1',
      placeName: 'Giewont',
      dateFrom: '2026-07-01',
      dateTo: '2026-07-01',
      isMine: false,
      description: null,
      descriptionIsAi: false,
      photoUrls: [],
    );
    await open(tester);

    expect(trash(), findsNothing);
    expect(forGood(), findsNothing);
    expect(find.text('Bez opisu'), findsOneWidget);
  });

  testWidgets('loading: the shape of the screen without actions', (tester) async {
    final gate = Completer<void>();
    await open(tester, repository: _WaitingMemories(gate.future, memories));

    expect(find.byKey(const Key('s05-skeleton')), findsOneWidget);
    expect(trash(), findsNothing);

    gate.complete();
    await tester.pumpAndSettle();

    expect(trash(), findsOneWidget);
  });

  testWidgets('forbidden: says so and goes back', (tester) async {
    memories.loadFailure = const MemoryNoAccess();
    await open(tester);

    expect(find.text('Nie masz już dostępu do tego wspomnienia'), findsOneWidget);

    await tester.tap(find.text('Wróć'));
    await tester.pumpAndSettle();

    expect(find.widgetWithText(AppBar, 'Mapa'), findsOneWidget);
  });

  group('error: the message depends on the failure and it can be retried', () {
    const messages = <MemoryFailure, String>{
      MemoryOffline(): 'Brak połączenia. Sprawdź internet i spróbuj ponownie.',
      MemoryServerError(): 'Błąd serwera. Spróbuj ponownie za chwilę.',
    };

    messages.forEach((failure, message) {
      testWidgets(message, (tester) async {
        memories.loadFailure = failure;
        await open(tester);

        expect(find.text(message), findsOneWidget);

        memories.loadFailure = null;
        await tester.tap(find.text('Ponów'));
        await tester.pumpAndSettle();

        expect(find.text('Miejsce: Giewont'), findsOneWidget);
      });
    });
  });

  testWidgets('a 409 is a message and the screen stays', (tester) async {
    await open(tester);
    memories.actionFailure = const MemoryAlreadyDecided();

    await tester.tap(trash());
    await tester.pumpAndSettle();

    expect(find.text('To już rozstrzygnięte. Odśwież i sprawdź aktualny stan.'), findsOneWidget);
    expect(find.text('Miejsce: Giewont'), findsOneWidget);
  });

  testWidgets('offline: the banner, the data read only, the actions off', (tester) async {
    await open(tester, online: Stream.value(false));

    expect(find.byKey(const Key('s05-offline')), findsOneWidget);
    expect(find.text('Miejsce: Giewont'), findsOneWidget);
    expect(isEnabled(tester, trash()), isFalse);
    expect(tester.widget<CheckboxListTile>(find.byType(CheckboxListTile)).onChanged, isNull);
  });
}

class _WaitingMemories extends FakeMemoriesRepository {
  _WaitingMemories(this.gate, this.inner);

  final Future<void> gate;
  final FakeMemoriesRepository inner;

  @override
  Future<Result<MemoryDetails, MemoryFailure>> loadDetails(String id) async {
    await gate;
    return inner.loadDetails(id);
  }
}
