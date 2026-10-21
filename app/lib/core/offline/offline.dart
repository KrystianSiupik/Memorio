import 'package:connectivity_plus/connectivity_plus.dart';
import 'package:drift_flutter/drift_flutter.dart';
import 'package:flutter/foundation.dart';
import 'package:flutter_riverpod/flutter_riverpod.dart';

import 'local_database.dart';
import 'offline_queue.dart';

export 'offline_queue.dart';

/// True while the device has a network connection (Wi-Fi, mobile, ethernet...). It says nothing
/// about the backend being reachable: senders still return [RetryLater] on network errors.
final isOnlineProvider = StreamProvider<bool>((ref) async* {
  final connectivity = Connectivity();
  yield _online(await connectivity.checkConnectivity());
  yield* connectivity.onConnectivityChanged.map(_online).distinct();
});

bool _online(List<ConnectivityResult> results) => results.any((result) => result != ConnectivityResult.none);

final localDatabaseProvider = Provider<LocalDatabase>((ref) {
  if (kIsWeb) {
    // Drift on the web needs sqlite3.wasm and drift_worker.js in web/; added with the first
    // feature that queues operations on the web (D-115).
    throw UnsupportedError('The local database is not set up for the web yet.');
  }
  final database = LocalDatabase(driftDatabase(name: 'memorio'));
  ref.onDispose(database.close);
  return database;
});

final offlineQueueProvider = Provider<OfflineQueue>((ref) => OfflineQueue(ref.watch(localDatabaseProvider)));

/// Senders per operation kind; features add theirs in app/composition.dart.
final operationSendersProvider = Provider<Map<String, OperationSender>>((ref) => const {});

/// Sends the queue when the connection comes back. Kept alive by the app shell.
final offlineSyncProvider = Provider<void>((ref) {
  ref.listen<AsyncValue<bool>>(isOnlineProvider, (previous, next) {
    final cameOnline = next.value == true && previous?.value != true;
    if (cameOnline) {
      ref.read(offlineQueueProvider).flush(ref.read(operationSendersProvider));
    }
  });
});
