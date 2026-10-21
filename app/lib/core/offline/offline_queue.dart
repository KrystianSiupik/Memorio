import 'dart:convert';
import 'dart:math';

import 'package:drift/drift.dart';
import 'package:uuid/uuid.dart';

import 'local_database.dart';

/// An operation waiting to be sent.
final class PendingOperation {
  const PendingOperation({
    required this.id,
    required this.kind,
    required this.payload,
    required this.createdAt,
    required this.attempts,
    this.rejectionCode,
  });

  /// Client UUID, sent as the idempotency key.
  final String id;
  final String kind;
  final Map<String, Object?> payload;
  final DateTime createdAt;
  final int attempts;

  /// Set when the backend rejected the operation for good; the user sees it and dismisses it.
  final String? rejectionCode;

  bool get isRejected => rejectionCode != null;
}

/// Outcome of one attempt to send an operation.
sealed class SendResult {
  const SendResult();
}

/// The backend accepted it (also when it had already accepted the same id before).
final class Sent extends SendResult {
  const Sent();
}

/// No network, timeout or server error: try again later, keeping the order.
final class RetryLater extends SendResult {
  const RetryLater();
}

/// The backend refused it for a business reason (e.g. "Limits.PoolFull"); retrying will not help.
final class Rejected extends SendResult {
  const Rejected(this.code);

  final String code;
}

typedef OperationSender = Future<SendResult> Function(PendingOperation operation);

/// Result of [OfflineQueue.flush].
final class FlushReport {
  const FlushReport({required this.sent, required this.rejected, required this.remaining});

  final int sent;
  final int rejected;

  /// Operations still waiting (not due yet, or after the first one that has to wait).
  final int remaining;
}

/// Local queue of operations created offline (ADR 0016). Operations are sent in creation order;
/// the first one that has to wait stops the flush, so photos never arrive before their memory.
class OfflineQueue {
  OfflineQueue(this._db, {DateTime Function()? now, String Function()? newId})
    : _now = now ?? DateTime.now,
      _newId = newId ?? const Uuid().v7;

  static const unknownKindCode = 'Offline.UnknownKind';
  static const _firstRetryDelay = Duration(seconds: 5);
  static const _maxRetryDelay = Duration(minutes: 15);

  final LocalDatabase _db;
  final DateTime Function() _now;
  final String Function() _newId;
  Future<FlushReport>? _flushing;

  /// Stores the operation and returns its id (the idempotency key the sender must pass on).
  Future<String> enqueue(String kind, Map<String, Object?> payload) async {
    final id = _newId();
    await _db
        .into(_db.pendingOperations)
        .insert(PendingOperationsCompanion.insert(id: id, kind: kind, payload: jsonEncode(payload), createdAt: _now()));
    return id;
  }

  /// All waiting and rejected operations, oldest first (for "waiting to send" and rejection messages).
  Stream<List<PendingOperation>> watch() => _ordered().watch().map((rows) => rows.map(_toOperation).toList());

  /// Sends due operations in order with the sender registered for their kind.
  /// Concurrent calls share one run.
  Future<FlushReport> flush(Map<String, OperationSender> senders) {
    return _flushing ??= _flush(senders).whenComplete(() => _flushing = null);
  }

  /// Removes a rejected operation after the user has seen it.
  Future<void> dismiss(String id) async {
    await (_db.delete(_db.pendingOperations)..where((row) => row.id.equals(id) & row.rejectionCode.isNotNull())).go();
  }

  Future<FlushReport> _flush(Map<String, OperationSender> senders) async {
    var sent = 0;
    var rejected = 0;
    final waiting = (await _ordered().get()).where((row) => row.rejectionCode == null).toList();

    for (final (index, row) in waiting.indexed) {
      final due = row.nextAttemptAt == null || !row.nextAttemptAt!.isAfter(_now());
      if (!due) {
        return FlushReport(sent: sent, rejected: rejected, remaining: waiting.length - index);
      }

      final sender = senders[row.kind];
      final result = sender == null ? const Rejected(unknownKindCode) : await sender(_toOperation(row));
      switch (result) {
        case Sent():
          await (_db.delete(_db.pendingOperations)..where((r) => r.id.equals(row.id))).go();
          sent++;
        case Rejected(:final code):
          await _update(row.id, PendingOperationsCompanion(rejectionCode: Value(code)));
          rejected++;
        case RetryLater():
          final attempts = row.attempts + 1;
          await _update(
            row.id,
            PendingOperationsCompanion(
              attempts: Value(attempts),
              nextAttemptAt: Value(_now().add(retryDelay(attempts))),
            ),
          );
          return FlushReport(sent: sent, rejected: rejected, remaining: waiting.length - index);
      }
    }
    return FlushReport(sent: sent, rejected: rejected, remaining: 0);
  }

  /// 5 s, 10 s, 20 s, ... up to 15 min.
  static Duration retryDelay(int attempts) {
    final seconds = _firstRetryDelay.inSeconds * pow(2, max(0, attempts - 1));
    return seconds >= _maxRetryDelay.inSeconds ? _maxRetryDelay : Duration(seconds: seconds.toInt());
  }

  SimpleSelectStatement<$PendingOperationsTable, PendingOperationRow> _ordered() =>
      _db.select(_db.pendingOperations)
        ..orderBy([(row) => OrderingTerm.asc(row.createdAt), (row) => OrderingTerm.asc(row.id)]);

  Future<void> _update(String id, PendingOperationsCompanion changes) =>
      (_db.update(_db.pendingOperations)..where((row) => row.id.equals(id))).write(changes);

  PendingOperation _toOperation(PendingOperationRow row) => PendingOperation(
    id: row.id,
    kind: row.kind,
    payload: (jsonDecode(row.payload) as Map).cast<String, Object?>(),
    createdAt: row.createdAt,
    attempts: row.attempts,
    rejectionCode: row.rejectionCode,
  );
}
