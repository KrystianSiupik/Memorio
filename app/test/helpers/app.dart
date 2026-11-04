import 'package:drift/native.dart';
import 'package:flutter/widgets.dart';
import 'package:flutter_riverpod/flutter_riverpod.dart';
import 'package:flutter_riverpod/misc.dart';
import 'package:flutter_test/flutter_test.dart';
import 'package:memorio/app/memorio_app.dart';
import 'package:memorio/core/api/api.dart';
import 'package:memorio/core/auth/token_store.dart';
import 'package:memorio/core/offline/local_database.dart';
import 'package:memorio/core/offline/offline.dart';
import 'package:memorio/features/account/account.dart';
import 'package:memorio/features/albums/albums.dart';
import 'package:memorio/features/memories/memories.dart';
import 'package:memorio/features/map/map.dart';

import 'fake_account.dart';
import 'fake_albums.dart';
import 'fake_http.dart';
import 'fake_memories.dart';
import 'fake_places.dart';
import 'fake_token_store.dart';

/// Session store answering with a fixed session; null means signed out.
class FakeSessionRepository implements SessionRepository {
  const FakeSessionRepository(this.session);

  final Session? session;

  @override
  Future<Session?> restore() async => session;
}

const signedIn = Session(accountId: 'account-1');

/// Renders the whole app with fakes: no network, no real storage, no connectivity plugin.
/// [online] drives the connection state (online by default).
Future<ProviderContainer> pumpMemorioApp(
  WidgetTester tester, {
  Session? session,
  SessionRepository? sessions,
  AccountRepository? account,
  MemoriesRepository? memories,
  PlacesRepository? places,
  AlbumsRepository? albums,
  FakeHttpAdapter? http,
  Stream<bool>? online,
  Stream<List<PendingOperation>>? pending,
  bool settle = true,
  List<Override> overrides = const [],
}) async {
  final database = LocalDatabase(NativeDatabase.memory());
  addTearDown(database.close);
  final container = ProviderContainer(
    overrides: [
      appVersionProvider.overrideWithValue('1.0.0'),
      apiBaseUrlProvider.overrideWithValue('https://api.example.invalid'),
      tokenStoreProvider.overrideWithValue(InMemoryTokenStore()),
      sessionRepositoryProvider.overrideWithValue(sessions ?? FakeSessionRepository(session)),
      accountRepositoryProvider.overrideWithValue(account ?? FakeAccountRepository()),
      memoriesRepositoryProvider.overrideWithValue(memories ?? FakeMemoriesRepository()),
      placesRepositoryProvider.overrideWithValue(places ?? FakePlacesRepository()),
      albumsRepositoryProvider.overrideWithValue(albums ?? FakeAlbumsRepository()),
      photoPickerProvider.overrideWithValue(FakePhotoPicker()),
      localDatabaseProvider.overrideWithValue(database),
      isOnlineProvider.overrideWith((ref) => online ?? Stream.value(true)),
      // A stream on the real database would leave a timer that keeps the test from closing it.
      pendingOperationsProvider.overrideWith((ref) => pending ?? Stream.value(const [])),
      mapViewProvider.overrideWithValue(const SizedBox.expand()),
      ...overrides,
    ],
  );
  addTearDown(container.dispose);
  // The real Dio with its interceptors, answering from memory.
  container.read(dioProvider).httpClientAdapter = http ?? FakeHttpAdapter((_) => (200, {'minimumVersion': '0.1.0'}));

  await tester.pumpWidget(UncontrolledProviderScope(container: container, child: const MemorioApp()));
  if (settle) {
    await tester.pumpAndSettle();
  }
  return container;
}
