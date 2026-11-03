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

  final item = TrashedMemory(
    id: 'memory-1',
    placeName: 'Giewont',
    dateFrom: '2026-07-01',
    description: 'W górach.',
    photos: 2,
    trashedAt: DateTime(2026, 10, 20, 12),
    daysLeft: 17,
  );

  setUp(() => memories = FakeMemoriesRepository());

  Future<void> open(WidgetTester tester, {Stream<bool>? online, MemoriesRepository? repository}) async {
    final container = await pumpMemorioApp(tester, session: signedIn, memories: repository ?? memories, online: online);
    container.read(routerProvider).go(const TrashRoute().location);
    await tester.pumpAndSettle();
  }

  bool isEnabled(WidgetTester tester) =>
      tester.widget<TextButton>(find.widgetWithText(TextButton, 'Przywróć')).onPressed != null;

  testWidgets('shows the content, when it was deleted and the days left', (tester) async {
    memories.trash = [item];
    await open(tester);

    expect(find.text('Giewont, 01.07.2026'), findsOneWidget);
    expect(find.textContaining('Usunięto: 20.10.2026'), findsOneWidget);
    expect(find.textContaining('Zostało dni: 17'), findsOneWidget);
    expect(find.textContaining('Zdjęcia: 2'), findsOneWidget);
    expect(isEnabled(tester), isTrue);
  });

  testWidgets('restoring takes it off the list', (tester) async {
    memories.trash = [item];
    await open(tester);

    await tester.tap(find.text('Przywróć'));
    await tester.pumpAndSettle();

    expect(memories.restored, ['memory-1']);
    expect(find.text('Kosz jest pusty'), findsOneWidget);
  });

  testWidgets('empty: says so', (tester) async {
    await open(tester);

    expect(find.text('Kosz jest pusty'), findsOneWidget);
  });

  testWidgets('loading: the shape of the screen without actions', (tester) async {
    final gate = Completer<void>();
    memories.trash = [item];
    await open(tester, repository: _WaitingMemories(gate.future, memories));

    expect(find.byKey(const Key('s06-skeleton')), findsOneWidget);
    expect(find.text('Przywróć'), findsNothing);

    gate.complete();
    await tester.pumpAndSettle();

    expect(find.text('Przywróć'), findsOneWidget);
  });

  group('error: the message depends on the failure and it can be retried', () {
    const messages = <MemoryFailure, String>{
      MemoryOffline(): 'Brak połączenia. Sprawdź internet i spróbuj ponownie.',
      MemoryNoAccess(): 'Nie masz dostępu do tego wspomnienia.',
      MemoryServerError(): 'Błąd serwera. Spróbuj ponownie za chwilę.',
    };

    messages.forEach((failure, message) {
      testWidgets(message, (tester) async {
        memories.loadFailure = failure;
        await open(tester);
        await tester.pumpAndSettle();

        expect(find.text(message), findsOneWidget);

        memories.loadFailure = null;
        await tester.tap(find.text('Ponów'));
        await tester.pumpAndSettle();

        expect(find.text('Kosz jest pusty'), findsOneWidget);
      });
    });
  });

  testWidgets('a 409 is a message and the list stays', (tester) async {
    memories.trash = [item];
    await open(tester);
    memories.actionFailure = const MemoryAlreadyDecided();

    await tester.tap(find.text('Przywróć'));
    await tester.pumpAndSettle();

    expect(find.text('To już rozstrzygnięte. Odśwież i sprawdź aktualny stan.'), findsOneWidget);
    expect(find.text('Giewont, 01.07.2026'), findsOneWidget);
  });

  testWidgets('offline: the banner, the data read only, the action off', (tester) async {
    memories.trash = [item];
    await open(tester, online: Stream.value(false));

    expect(find.byKey(const Key('s06-offline')), findsOneWidget);
    expect(find.text('Giewont, 01.07.2026'), findsOneWidget);
    expect(isEnabled(tester), isFalse);
  });
}

class _WaitingMemories extends FakeMemoriesRepository {
  _WaitingMemories(this.gate, this.inner);

  final Future<void> gate;
  final FakeMemoriesRepository inner;

  @override
  Future<Result<List<TrashedMemory>, MemoryFailure>> loadTrash() async {
    await gate;
    return inner.loadTrash();
  }
}
