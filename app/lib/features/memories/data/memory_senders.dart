import 'dart:io';

import '../../../core/offline/offline_queue.dart';
import '../../../core/result.dart';
import '../domain/memories_repository.dart';
import '../domain/memory.dart';
import '../domain/memory_failure.dart';
import '../domain/memory_operations.dart';
import 'image_photo_picker.dart';

/// Sends what was saved without a connection. The ids in the payload make a repeat the same operation.
Map<String, OperationSender> memorySenders(MemoriesRepository memories, PlacesRepository places) => {
  MemoryOperations.addOwnPlace: (operation) async {
    final payload = operation.payload;
    return _sendResultOf(
      await places.addOwn(
        id: payload['id']! as String,
        name: payload['name']! as String,
        latitude: (payload['latitude']! as num).toDouble(),
        longitude: (payload['longitude']! as num).toDouble(),
      ),
    );
  },
  MemoryOperations.createMemory: (operation) async {
    final payload = operation.payload;
    return _sendResultOf(
      await memories.create(
        NewMemory(
          id: payload['id']! as String,
          placeReference: payload['placeReference']! as String,
          precision: MemoryDatePrecision.values.byName(payload['precision']! as String),
          from: payload['from']! as String,
          to: payload['to'] as String?,
          time: payload['time'] as String?,
        ),
      ),
    );
  },
  MemoryOperations.addPhoto: (operation) async {
    final payload = operation.payload;
    final path = payload['path']! as String;
    if (!File(path).existsSync()) {
      return const Rejected('Memory.PhotoGone');
    }
    return _sendResultOf(
      await memories.addPhoto(
        payload['memoryId']! as String,
        payload['photoId']! as String,
        photoAt(path, payload['name']! as String, payload['contentType']! as String),
      ),
    );
  },
  MemoryOperations.approveDescription: (operation) async {
    final payload = operation.payload;
    return _sendResultOf(
      await memories.approveDescription(
        payload['memoryId']! as String,
        payload['text']! as String,
        payload['aiDraftId'] as String?,
      ),
    );
  },
};

SendResult _sendResultOf<T>(Result<T, MemoryFailure> result) => switch (result) {
  Ok() => const Sent(),
  Err(failure: MemoryOffline() || MemoryServerError()) => const RetryLater(),
  Err(:final failure) => Rejected(failure.code),
};
