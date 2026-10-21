import 'package:flutter_riverpod/flutter_riverpod.dart';
import 'package:flutter_test/flutter_test.dart';
import 'package:memorio/app/memorio_app.dart';
import 'package:memorio/core/api/api.dart';
import 'package:memorio/features/account/account.dart';

import 'fake_http.dart';

/// Session store answering with a fixed session; null means signed out.
class FakeSessionRepository implements SessionRepository {
  const FakeSessionRepository(this.session);

  final Session? session;

  @override
  Future<Session?> restore() async => session;
}

const signedIn = Session(accountId: 'account-1');

/// Renders the whole app with fakes: no network, no real storage.
Future<ProviderContainer> pumpMemorioApp(WidgetTester tester, {Session? session, FakeHttpAdapter? http}) async {
  final container = ProviderContainer(
    overrides: [
      appVersionProvider.overrideWithValue('1.0.0'),
      apiBaseUrlProvider.overrideWithValue('https://api.example.invalid'),
      sessionRepositoryProvider.overrideWithValue(FakeSessionRepository(session)),
    ],
  );
  addTearDown(container.dispose);
  // The real Dio with its interceptors, answering from memory.
  container.read(dioProvider).httpClientAdapter = http ?? FakeHttpAdapter((_) => (200, {'minimumVersion': '0.1.0'}));

  await tester.pumpWidget(UncontrolledProviderScope(container: container, child: const MemorioApp()));
  await tester.pumpAndSettle();
  return container;
}
