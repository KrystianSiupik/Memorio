import 'package:dio/dio.dart';
import 'package:flutter_riverpod/flutter_riverpod.dart';
import 'package:flutter_test/flutter_test.dart';
import 'package:memorio/app/memorio_app.dart';
import 'package:memorio/core/api/api.dart';

import '../helpers/fake_http.dart';

void main() {
  Future<void> pumpApp(WidgetTester tester, FakeHttpAdapter adapter) async {
    final container = ProviderContainer(
      overrides: [
        appVersionProvider.overrideWithValue('1.0.0'),
        apiBaseUrlProvider.overrideWithValue('https://api.example.invalid'),
      ],
    );
    addTearDown(container.dispose);
    // The real Dio with its interceptors, answering from memory.
    container.read(dioProvider).httpClientAdapter = adapter;

    await tester.pumpWidget(UncontrolledProviderScope(container: container, child: const MemorioApp()));
    await tester.pumpAndSettle();
  }

  testWidgets('a supported app starts on the start page', (tester) async {
    await pumpApp(tester, FakeHttpAdapter((_) => (200, {'minimumVersion': '0.1.0'})));

    expect(find.text('Memorio'), findsOneWidget);
    expect(find.text('Please update Memorio'), findsNothing);
  });

  testWidgets('an app the backend no longer serves shows the update screen', (tester) async {
    await pumpApp(tester, FakeHttpAdapter((_) => (426, {'code': 'App.UpdateRequired', 'minimumVersion': '1.2.0'})));

    expect(find.text('Please update Memorio'), findsOneWidget);
    expect(find.textContaining('Version 1.2.0 or newer'), findsOneWidget);
  });

  testWidgets('the app starts when the backend cannot be reached', (tester) async {
    await pumpApp(
      tester,
      FakeHttpAdapter((request) => throw DioException.connectionError(requestOptions: request, reason: 'offline')),
    );

    expect(find.text('Memorio'), findsOneWidget);
  });
}
