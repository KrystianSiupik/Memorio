import 'package:drift/drift.dart' show driftRuntimeOptions;
import 'package:drift/native.dart';
import 'package:flutter_test/flutter_test.dart';
import 'package:memorio/core/offline/local_database.dart';
import 'package:memorio/core/offline/offline_queue.dart';

void main() {
  driftRuntimeOptions.dontWarnAboutMultipleDatabases = true;

  late LocalDatabase db;
  late DateTime now;
  late int nextId;
  late OfflineQueue queue;

  setUp(() {
    db = LocalDatabase(NativeDatabase.memory());
    now = DateTime.utc(2026, 10, 22, 12);
    nextId = 0;
    queue = OfflineQueue(db, now: () => now, newId: () => 'op-${++nextId}');
  });

  tearDown(() => db.close());

  /// Records what each kind's sender received and answers with [answer].
  Map<String, OperationSender> senders(List<String> log, SendResult Function(PendingOperation) answer) => {
    for (final kind in ['memory', 'photos'])
      kind: (operation) async {
        log.add('${operation.kind}:${operation.id}');
        return answer(operation);
      },
  };

  Future<List<PendingOperation>> pending() => queue.watch().first;

  test('an enqueued operation survives until sent and keeps its client id and payload', () async {
    final id = await queue.enqueue('memory', {'title': 'Summit', 'photos': 3});

    final [operation] = await pending();
    expect(operation.id, id);
    expect(operation.payload, {'title': 'Summit', 'photos': 3});
    expect(operation.attempts, 0);
  });

  test('ids are UUIDs by default', () async {
    final id = await OfflineQueue(db).enqueue('memory', {});

    expect(id, matches(RegExp(r'^[0-9a-f]{8}-[0-9a-f]{4}-7[0-9a-f]{3}-[89ab][0-9a-f]{3}-[0-9a-f]{12}$')));
  });

  test('flush sends in creation order and removes what was sent', () async {
    await queue.enqueue('memory', {});
    now = now.add(const Duration(seconds: 1));
    await queue.enqueue('photos', {});
    final log = <String>[];

    final report = await queue.flush(senders(log, (_) => const Sent()));

    expect(log, ['memory:op-1', 'photos:op-2']);
    expect((report.sent, report.rejected, report.remaining), (2, 0, 0));
    expect(await pending(), isEmpty);
  });

  test('a retry stops the flush so later operations keep their order', () async {
    await queue.enqueue('memory', {});
    await queue.enqueue('photos', {});
    final log = <String>[];

    final report = await queue.flush(senders(log, (_) => const RetryLater()));

    expect(log, ['memory:op-1']);
    expect(report.remaining, 2);
    final [first, second] = await pending();
    expect(first.attempts, 1);
    expect(second.attempts, 0);
  });

  test('a retried operation waits for its backoff, then is sent with the same id', () async {
    await queue.enqueue('memory', {});
    final log = <String>[];
    await queue.flush(senders(log, (_) => const RetryLater()));

    now = now.add(const Duration(seconds: 4));
    await queue.flush(senders(log, (_) => const Sent()));
    expect(log, ['memory:op-1'], reason: 'not due before 5 s');

    now = now.add(const Duration(seconds: 1));
    await queue.flush(senders(log, (_) => const Sent()));
    expect(log, ['memory:op-1', 'memory:op-1']);
    expect(await pending(), isEmpty);
  });

  test('a rejected operation is kept for the user, skipped by later flushes and can be dismissed', () async {
    await queue.enqueue('photos', {});
    await queue.enqueue('memory', {});
    final log = <String>[];

    final report = await queue.flush(
      senders(log, (op) => op.kind == 'photos' ? const Rejected('Limits.PoolFull') : const Sent()),
    );

    expect((report.sent, report.rejected), (1, 1));
    final [rejected] = await pending();
    expect(rejected.rejectionCode, 'Limits.PoolFull');

    await queue.flush(senders(log, (_) => const Sent()));
    expect(log, ['photos:op-1', 'memory:op-2'], reason: 'a rejected operation is not sent again');

    await queue.dismiss(rejected.id);
    expect(await pending(), isEmpty);
  });

  test('an operation without a sender is rejected, not lost', () async {
    await queue.enqueue('unknown', {});

    await queue.flush(const {});

    final [operation] = await pending();
    expect(operation.rejectionCode, OfflineQueue.unknownKindCode);
  });

  test('concurrent flushes share one run, so nothing is sent twice', () async {
    await queue.enqueue('memory', {});
    final log = <String>[];

    await Future.wait([queue.flush(senders(log, (_) => const Sent())), queue.flush(senders(log, (_) => const Sent()))]);

    expect(log, ['memory:op-1']);
  });

  test('retry delay doubles from 5 s up to 15 min', () {
    expect(OfflineQueue.retryDelay(1), const Duration(seconds: 5));
    expect(OfflineQueue.retryDelay(2), const Duration(seconds: 10));
    expect(OfflineQueue.retryDelay(3), const Duration(seconds: 20));
    expect(OfflineQueue.retryDelay(20), const Duration(minutes: 15));
  });
}
