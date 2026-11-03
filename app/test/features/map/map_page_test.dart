import 'dart:async';

import 'package:flutter/material.dart';
import 'package:flutter_riverpod/flutter_riverpod.dart';
import 'package:flutter_test/flutter_test.dart';
import 'package:memorio/core/offline/offline.dart';
import 'package:memorio/core/result.dart';
import 'package:memorio/features/account/account.dart';
import 'package:memorio/features/map/map.dart';
import 'package:memorio/features/memories/memories.dart';

import '../../helpers/app.dart';
import '../../helpers/fake_memories.dart';

class _Holding extends FakeMemoriesRepository {
  final answer = Completer<void>();

  @override
  Future<Result<List<MemoryPin>, MemoryFailure>> loadPins({String? from, String? to}) async {
    await answer.future;
    return super.loadPins(from: from, to: to);
  }
}

class _SwitchingSessions implements SessionRepository {
  Session? current = const Session(accountId: 'first');

  @override
  Future<Session?> restore() async => current;
}

PendingOperation _operation(String id, {String? rejectionCode, String kind = MemoryOperations.createMemory}) =>
    PendingOperation(
      id: id,
      kind: kind,
      payload: const {},
      createdAt: DateTime.utc(2026),
      attempts: 0,
      rejectionCode: rejectionCode,
    );

void main() {
  const pin = MemoryPin(id: 'memory-1', latitude: 49.2, longitude: 19.9);
  late FakeMemoriesRepository memories;

  setUp(() {
    memories = FakeMemoriesRepository()..pins = [pin];
  });

  Future<ProviderContainer> open(
    WidgetTester tester, {
    Stream<bool>? online,
    Stream<List<PendingOperation>>? pending,
  }) => pumpMemorioApp(tester, session: signedIn, memories: memories, online: online, pending: pending);

  testWidgets('shows the points, the filters and the attribution', (tester) async {
    final container = await open(tester);

    expect(container.read(mapPointsProvider).single.id, 'memory-1');
    for (final label in ['Daty', 'Osoby', 'Albumy', '© OpenStreetMap']) {
      expect(find.text(label), findsWidgets, reason: label);
    }
    expect(find.text('Dodaj pierwsze wspomnienie'), findsNothing);
  });

  testWidgets('people and albums filters are there but off until albums exist', (tester) async {
    await open(tester);

    for (final label in ['Osoby', 'Albumy']) {
      expect(tester.widget<FilterChip>(find.widgetWithText(FilterChip, label)).onSelected, isNull);
    }
    expect(tester.widget<FilterChip>(find.byKey(const Key('s03-filter-dates'))).onSelected, isNotNull);
  });

  testWidgets('while loading the map stays with a message', (tester) async {
    final holding = _Holding();
    memories = holding;
    await pumpMemorioApp(tester, session: signedIn, memories: memories, settle: false);
    await tester.pump();
    await tester.pump();

    expect(find.byKey(const Key('s03-loading')), findsOneWidget);

    holding.answer.complete();
    await tester.pumpAndSettle();
    expect(find.byKey(const Key('s03-loading')), findsNothing);
  });

  group('errors say what happened, and Ponów loads again', () {
    const messages = <MemoryFailure, String>{
      MemoryOffline(): 'Brak połączenia. Sprawdź internet i spróbuj ponownie.',
      MemoryNoAccess(): 'Nie masz dostępu do tego wspomnienia.',
      MemoryAlreadyDecided(): 'To już rozstrzygnięte. Odśwież i sprawdź aktualny stan.',
      MemoryServerError(): 'Błąd serwera. Spróbuj ponownie za chwilę.',
    };

    messages.forEach((failure, message) {
      testWidgets(message, (tester) async {
        memories.loadFailure = failure;
        final container = await open(tester);

        expect(find.text(message), findsOneWidget);
        expect(container.read(mapPointsProvider), isEmpty);

        memories.loadFailure = null;
        await tester.tap(find.text('Ponów'));
        await tester.pumpAndSettle();

        expect(find.byKey(const Key('s03-error')), findsNothing);
        expect(container.read(mapPointsProvider), hasLength(1));
      });
    });
  });

  testWidgets('offline keeps the last points, without an error over them', (tester) async {
    final container = await open(tester, online: Stream.value(false));

    memories.loadFailure = const MemoryOffline();
    container.invalidate(memoryPinsProvider);
    await tester.pumpAndSettle();

    expect(container.read(mapPointsProvider).single.id, 'memory-1');
    expect(find.byKey(const Key('s03-error')), findsNothing);
  });

  testWidgets('a server error over known points still keeps them', (tester) async {
    final container = await open(tester);

    memories.loadFailure = const MemoryServerError();
    container.invalidate(memoryPinsProvider);
    await tester.pumpAndSettle();

    expect(container.read(mapPointsProvider).single.id, 'memory-1');
    expect(find.byKey(const Key('s03-error')), findsOneWidget);
  });

  group('empty', () {
    testWidgets('invites to the first memory, which opens S04', (tester) async {
      memories.pins = [];
      await open(tester);

      await tester.tap(find.text('Dodaj pierwsze wspomnienie'));
      await tester.pumpAndSettle();

      expect(find.widgetWithText(AppBar, 'Nowe wspomnienie'), findsOneWidget);
    });

    testWidgets('with a filter says nothing matches instead', (tester) async {
      memories.pins = [];
      final container = await open(tester);

      container.read(memoryDateFilterProvider.notifier).set((from: DateTime(2020), to: DateTime(2021)));
      await tester.pumpAndSettle();

      expect(find.text('Dodaj pierwsze wspomnienie'), findsNothing);
      expect(find.text('Brak wspomnień w tych datach'), findsOneWidget);
    });
  });

  group('the date filter', () {
    testWidgets('asks the backend for the range and clears', (tester) async {
      final container = await open(tester);

      container.read(memoryDateFilterProvider.notifier).set((from: DateTime(2020, 1, 2), to: DateTime(2021, 3, 4)));
      await tester.pumpAndSettle();

      expect(memories.pinRequests.last, (from: '2020-01-02', to: '2021-03-04'));
      expect(find.text('Daty'), findsNothing);

      await tester.tap(
        find.descendant(of: find.byKey(const Key('s03-filter-dates')), matching: find.byIcon(Icons.clear)),
      );
      await tester.pumpAndSettle();

      expect(memories.pinRequests.last, (from: null, to: null));
      expect(find.text('Daty'), findsOneWidget);
    });

    testWidgets('opens the date range picker', (tester) async {
      await open(tester);

      await tester.tap(find.byKey(const Key('s03-filter-dates')));
      await tester.pumpAndSettle();

      expect(find.byType(DateRangePickerDialog), findsOneWidget);
    });
  });

  group('the queue', () {
    testWidgets('memories waiting to be sent are counted; other operations and rejected ones are not', (tester) async {
      await open(
        tester,
        pending: Stream.value([
          _operation('a'),
          _operation('b'),
          _operation('c', kind: MemoryOperations.addPhoto),
          _operation('d', rejectionCode: 'Limits.PoolFull'),
        ]),
      );

      expect(find.text('Czeka na wysłanie: 2'), findsOneWidget);
    });

    testWidgets('a rejected operation is shown instead of disappearing quietly', (tester) async {
      await open(tester, pending: Stream.value([_operation('a', rejectionCode: 'Limits.PoolFull')]));

      expect(find.text('Nie udało się wysłać: 1'), findsOneWidget);
      expect(find.text('Rozumiem'), findsOneWidget);
    });

    testWidgets('once something is sent the points load again', (tester) async {
      final pending = StreamController<List<PendingOperation>>();
      addTearDown(pending.close);
      await open(tester, pending: pending.stream);

      pending.add([_operation('a')]);
      await tester.pumpAndSettle();
      final loads = memories.pinRequests.length;

      pending.add(const []);
      await tester.pumpAndSettle();

      expect(memories.pinRequests.length, loads + 1);
      expect(find.byKey(const Key('s03-pending')), findsNothing);
    });
  });

  group('transitions', () {
    testWidgets('S03 → S04: the add button', (tester) async {
      await open(tester);

      await tester.tap(find.byType(FloatingActionButton));
      await tester.pumpAndSettle();

      expect(find.widgetWithText(AppBar, 'Nowe wspomnienie'), findsOneWidget);
    });

    testWidgets('S03 → S06: the trash', (tester) async {
      await open(tester);

      await tester.tap(find.byTooltip('Kosz'));
      await tester.pumpAndSettle();

      expect(find.widgetWithText(AppBar, 'Kosz'), findsOneWidget);
    });

    testWidgets('S03 → S05: tapping a point, once', (tester) async {
      memories.loadFailure = null;
      final container = await open(tester);

      container.read(tappedPointProvider.notifier).tap('memory-1');
      await tester.pumpAndSettle();

      expect(find.widgetWithText(AppBar, 'Mapa'), findsNothing);
      expect(container.read(tappedPointProvider), isNull);
    });
  });

  testWidgets('what one person loaded is not kept for the next one', (tester) async {
    final sessions = _SwitchingSessions();
    final container = await pumpMemorioApp(tester, sessions: sessions, memories: memories);
    memories.trash = [
      TrashedMemory(
        id: 'first-trashed',
        placeName: 'Giewont',
        dateFrom: '2026-01-01',
        description: null,
        photos: 0,
        trashedAt: DateTime.utc(2026),
        daysLeft: 3,
      ),
    ];
    container.read(memoryDateFilterProvider.notifier).set((from: DateTime(2020), to: DateTime(2021)));
    expect(await tester.runAsync(() => container.read(trashProvider.future)), hasLength(1));

    memories.trash = [];
    sessions.current = const Session(accountId: 'second');
    container.invalidate(sessionProvider);
    await tester.pumpAndSettle();

    expect(await tester.runAsync(() => container.read(trashProvider.future)), isEmpty);
    expect(container.read(memoryDateFilterProvider), isNull);
  });
}
