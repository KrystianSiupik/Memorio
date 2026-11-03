import 'package:flutter_test/flutter_test.dart';
import 'package:memorio/core/offline/offline_queue.dart';
import 'package:memorio/features/memories/memories.dart';

import '../../helpers/fake_memories.dart';
import '../../helpers/fake_places.dart';

void main() {
  late FakeMemoriesRepository memories;
  late Map<String, OperationSender> senders;

  PendingOperation operation(String kind, Map<String, Object?> payload) =>
      PendingOperation(id: 'op', kind: kind, payload: payload, createdAt: DateTime(2026), attempts: 0);

  final create = {'id': 'm1', 'placeReference': 'osm:N:1', 'precision': 'day', 'from': '2026-07-01', 'to': null};

  setUp(() {
    memories = FakeMemoriesRepository();
    senders = memorySenders(memories, FakePlacesRepository());
  });

  test('a memory is created from its payload', () async {
    final result = await senders[MemoryOperations.createMemory]!(operation(MemoryOperations.createMemory, create));

    expect(result, isA<Sent>());
    expect(memories.created.single.precision, MemoryDatePrecision.day);
  });

  test('no connection or a server error waits for later', () async {
    memories.createFailure = const MemoryOffline();
    expect(
      await senders[MemoryOperations.createMemory]!(operation(MemoryOperations.createMemory, create)),
      isA<RetryLater>(),
    );

    memories.createFailure = const MemoryServerError();
    expect(
      await senders[MemoryOperations.createMemory]!(operation(MemoryOperations.createMemory, create)),
      isA<RetryLater>(),
    );
  });

  test('a refusal for good carries its code', () async {
    memories.createFailure = const MemoryDateInFuture();

    final result = await senders[MemoryOperations.createMemory]!(operation(MemoryOperations.createMemory, create));

    expect((result as Rejected).code, 'Archive.DateInFuture');
  });

  test('a photo no longer on the device is refused for good', () async {
    final result = await senders[MemoryOperations.addPhoto]!(
      operation(MemoryOperations.addPhoto, {
        'memoryId': 'm1',
        'photoId': 'p1',
        'path': '/definitely/not/here.jpg',
        'name': 'a.jpg',
        'contentType': 'image/jpeg',
      }),
    );

    expect((result as Rejected).code, 'Memory.PhotoGone');
  });
}
