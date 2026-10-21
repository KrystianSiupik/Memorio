import 'package:drift/drift.dart';

part 'local_database.g.dart';

/// Operations created offline and not yet accepted by the backend (ADR 0016).
/// Only creating memories and adding photos are queued (D-039); everything else needs the network.
@DataClassName('PendingOperationRow')
class PendingOperations extends Table {
  /// UUID created on the device; the backend uses it as the idempotency key, so a retry has one effect.
  TextColumn get id => text()();

  /// What to send, e.g. "archive.createMemory"; each kind has a sender registered by its feature.
  TextColumn get kind => text()();

  /// The request body as JSON. User content stays on the device until it is sent.
  TextColumn get payload => text()();

  DateTimeColumn get createdAt => dateTime()();

  IntColumn get attempts => integer().withDefault(const Constant(0))();

  /// Earliest time of the next attempt after a failed one (backoff); null means due now.
  DateTimeColumn get nextAttemptAt => dateTime().nullable()();

  /// Error code when the backend rejected the operation for good (e.g. pool full); kept for the user.
  TextColumn get rejectionCode => text().nullable()();

  @override
  Set<Column<Object>> get primaryKey => {id};
}

/// The app's local SQLite database (Drift).
@DriftDatabase(tables: [PendingOperations])
class LocalDatabase extends _$LocalDatabase {
  LocalDatabase(super.executor);

  @override
  int get schemaVersion => 1;
}
