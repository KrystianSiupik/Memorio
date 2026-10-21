import 'dart:async';

import 'package:drift/drift.dart' show driftRuntimeOptions;
import 'package:flutter_test/flutter_test.dart';
import 'package:memorio/core/offline/offline.dart';

import '../helpers/app.dart';

void main() {
  driftRuntimeOptions.dontWarnAboutMultipleDatabases = true;

  const bannerText = 'You are offline. New memories and photos are saved and sent when you are back online.';

  testWidgets('the offline banner shows on the tabs while there is no connection', (tester) async {
    final online = StreamController<bool>();
    addTearDown(online.close);
    await pumpMemorioApp(tester, session: signedIn, online: online.stream);

    online.add(false);
    await tester.pumpAndSettle();
    expect(find.text(bannerText), findsOneWidget);

    await tester.tap(find.text('Albums'));
    await tester.pumpAndSettle();
    expect(find.text(bannerText), findsOneWidget);

    online.add(true);
    await tester.pumpAndSettle();
    expect(find.text(bannerText), findsNothing);
  });

  testWidgets('coming back online sends the queued operations', (tester) async {
    final online = StreamController<bool>();
    addTearDown(online.close);
    final sent = <String>[];
    final container = await pumpMemorioApp(
      tester,
      session: signedIn,
      online: online.stream,
      overrides: [
        operationSendersProvider.overrideWithValue({
          'memory': (operation) async {
            sent.add(operation.id);
            return const Sent();
          },
        }),
      ],
    );
    online.add(false);
    await tester.pumpAndSettle();
    final id = await tester.runAsync(() => container.read(offlineQueueProvider).enqueue('memory', {'title': 'Summit'}));

    online.add(true);
    await tester.runAsync(() => Future<void>.delayed(const Duration(milliseconds: 100)));
    await tester.pumpAndSettle();

    expect(sent, [id]);
  });
}
