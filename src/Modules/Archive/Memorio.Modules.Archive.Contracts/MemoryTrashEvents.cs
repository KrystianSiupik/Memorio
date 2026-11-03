using Memorio.SharedKernel.Messaging;

namespace Memorio.Modules.Archive.Contracts;

/// <summary>E20: the memory went to the trash; carried through the outbox so its deadline is never lost (D-157).</summary>
public sealed record MemoryMovedToTrashEvent(Guid EventId, DateTimeOffset OccurredAt, Guid MemoryId, int Sequence, DateTimeOffset WindowEndsAt) : IIntegrationEvent;

/// <summary>E24: the memory came back from the trash.</summary>
public sealed record MemoryRestoredEvent(Guid EventId, DateTimeOffset OccurredAt, Guid MemoryId, int Sequence) : IIntegrationEvent;
