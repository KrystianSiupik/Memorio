/// Kinds of operations in the offline queue (ADR 0016); the queue sends them in the order they were made,
/// so a place comes before its memory, and a memory before its photos.
abstract final class MemoryOperations {
  static const addOwnPlace = 'place.addOwn';
  static const createMemory = 'memory.create';
  static const addPhoto = 'memory.addPhoto';
  static const approveDescription = 'memory.approveDescription';
}
