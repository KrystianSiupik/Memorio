import 'package:flutter/material.dart';
import 'package:flutter_riverpod/flutter_riverpod.dart';
import 'package:flutter_riverpod/misc.dart';
import 'package:flutter_test/flutter_test.dart';
import 'package:memorio/app/router.dart';
import 'package:memorio/app/routes.dart';
import 'package:memorio/features/map/map.dart';
import 'package:memorio/features/replay/replay.dart';

import '../../helpers/app.dart';
import '../../helpers/fake_replay.dart';

void main() {
  late FakeReplayRepository replay;

  setUp(() => replay = FakeReplayRepository());

  Future<ProviderContainer> open(
    WidgetTester tester, {
    Stream<bool>? online,
    List<Override> overrides = const [],
  }) async {
    tester.view.physicalSize = const Size(1000, 2400);
    tester.view.devicePixelRatio = 1;
    addTearDown(tester.view.reset);
    final container = await pumpMemorioApp(
      tester,
      session: signedIn,
      replay: replay,
      online: online,
      overrides: overrides,
    );
    container.read(routerProvider).go(const ReplayRoute().location);
    await tester.pumpAndSettle();
    return container;
  }

  void threeMemories() => replay.memories = [
    periodMemory('m1', 'Warszawa', precision: 'Period', from: '2025-12-31', to: '2026-01-01'),
    periodMemory('m2', 'Wrocław', precision: 'Month', from: '2026-03-01', to: '2026-03-31'),
    periodMemory('m3', 'Ślęża', mine: false),
  ];

  testWidgets('this year is replayed from the first memory, with the date as exact as given', (tester) async {
    threeMemories();
    final container = await open(tester);

    expect(replay.periods.single, startsWith('${DateTime.now().year}-01-01..'));
    expect(find.text('1 z 3'), findsOneWidget);
    expect(find.text('Warszawa'), findsOneWidget);
    expect(find.text('31.12.2025 – 01.01.2026'), findsOneWidget);
    expect(container.read(mapPointsProvider).map((point) => point.id), ['m1', 'm2', 'm3']);
    expect(container.read(mapFocusProvider)?.id, 'm1');
  });

  testWidgets('next and previous move the map to the memory', (tester) async {
    threeMemories();
    final container = await open(tester);

    await tester.tap(find.byTooltip('Następne'));
    await tester.pump();
    expect(find.text('Wrocław'), findsOneWidget);
    expect(find.text('03.2026'), findsOneWidget);
    expect(container.read(mapFocusProvider)?.id, 'm2');

    await tester.tap(find.byTooltip('Poprzednie'));
    await tester.pump();
    expect(find.text('Warszawa'), findsOneWidget);
  });

  testWidgets('playing goes on by itself, faster at the higher tempo, and pauses', (tester) async {
    threeMemories();
    await open(tester);

    await tester.tap(find.byTooltip('Odtwarzaj'));
    await tester.pump(const Duration(seconds: 6));
    expect(find.text('Wrocław'), findsOneWidget);

    await tester.tap(find.text('Tempo 1×'));
    await tester.pump(const Duration(seconds: 3));
    expect(find.text('Ślęża'), findsOneWidget);

    await tester.tap(find.byTooltip('Zatrzymaj'));
    await tester.pump(const Duration(seconds: 10));
    expect(find.text('Ślęża'), findsOneWidget);
  });

  testWidgets('the end shows the period in numbers and makes an album of it', (tester) async {
    threeMemories();
    final container = await open(tester);
    for (var step = 0; step < 3; step++) {
      await tester.tap(find.byTooltip('Następne'));
      await tester.pump();
    }

    expect(find.text('Wspomnienia: 3, miejsca: 3'), findsOneWidget);
    await tester.tap(find.text('Utwórz album z okresu'));
    await tester.pumpAndSettle();
    await tester.tap(find.widgetWithText(FilledButton, 'Utwórz album z okresu').last);
    await tester.pumpAndSettle();

    expect(replay.actions.single, startsWith('album:Prześledź ${DateTime.now().year}:'));
    expect(find.text('Album gotowy, dodano wspomnień: 2.'), findsOneWidget);
    await tester.tap(find.text('Otwórz album'));
    await tester.pumpAndSettle();
    expect(container.read(routerProvider).state.uri.path, startsWith('/albums/'));
  });

  testWidgets('a refusal is a message on the screen', (tester) async {
    threeMemories();
    replay.actionFailure = const ReplayTooLong();
    await open(tester);
    for (var step = 0; step < 3; step++) {
      await tester.tap(find.byTooltip('Następne'));
      await tester.pump();
    }

    await tester.tap(find.text('Utwórz album z okresu'));
    await tester.pumpAndSettle();
    await tester.tap(find.widgetWithText(FilledButton, 'Utwórz album z okresu').last);
    await tester.pumpAndSettle();

    expect(find.text('W tym okresie jest za dużo wspomnień na jeden album. Wybierz krótszy.'), findsOneWidget);
  });

  testWidgets('another year is another period', (tester) async {
    await open(tester);
    final last = DateTime.now().year - 1;

    await tester.tap(find.text('$last'));
    await tester.pumpAndSettle();

    expect(replay.periods.last, '$last-01-01..$last-12-31');
    expect(find.text('Okres: $last'), findsOneWidget);
  });

  testWidgets('an empty period says so', (tester) async {
    await open(tester);

    expect(find.text('W tym okresie nie ma wspomnień'), findsOneWidget);
  });

  testWidgets('a failed load says why and the retry works', (tester) async {
    replay.loadFailure = const ReplayServerError();
    await open(tester);

    expect(find.text('Błąd serwera. Spróbuj ponownie za chwilę.'), findsOneWidget);
    replay.loadFailure = null;
    threeMemories();
    await tester.tap(find.text('Ponów'));
    await tester.pumpAndSettle();
    expect(find.text('Warszawa'), findsOneWidget);
  });

  testWidgets('offline shows a banner and an album cannot be made', (tester) async {
    replay.memories = [periodMemory('m1', 'Warszawa')];
    await open(tester, online: Stream.value(false));

    expect(find.byKey(const Key('s08-offline')), findsOneWidget);
    await tester.tap(find.byTooltip('Następne'));
    await tester.pump();
    final button = tester.widget<FilledButton>(find.widgetWithText(FilledButton, 'Utwórz album z okresu'));
    expect(button.onPressed, isNull);
  });

  testWidgets('the place opens the memory, and leaving gives the map back all points', (tester) async {
    replay.memories = [periodMemory('m1', 'Warszawa')];
    final container = await open(tester);

    await tester.tap(find.text('Warszawa'));
    await tester.pumpAndSettle();

    expect(container.read(routerProvider).state.uri.path, const MemoryRoute('m1').location);
    expect(container.read(replayPointsProvider), isNull);
  });

  testWidgets('S03 leads here', (tester) async {
    tester.view.physicalSize = const Size(1000, 2400);
    tester.view.devicePixelRatio = 1;
    addTearDown(tester.view.reset);
    final container = await pumpMemorioApp(tester, session: signedIn, replay: replay);
    container.read(routerProvider).go(const MapRoute().location);
    await tester.pumpAndSettle();

    await tester.tap(find.byTooltip('Prześledź'));
    await tester.pumpAndSettle();

    expect(container.read(routerProvider).state.uri.path, const ReplayRoute().location);
  });

  test('a date says only what the person gave', () {
    expect(periodDateText(periodMemory('m', 'x', precision: 'Year', from: '2026-01-01', to: '2026-12-31')), '2026');
    expect(periodDateText(periodMemory('m', 'x', precision: 'Day', from: '2026-05-14')), '14.05.2026');
    expect(
      periodDateText(periodMemory('m', 'x', precision: 'Day', from: '2026-05-14', time: '08:15')),
      '14.05.2026, 08:15',
    );
  });
}
