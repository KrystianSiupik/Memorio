using Memorio.SharedKernel.Domain;

namespace Memorio.Modules.Archive.Domain.Memories;

/// <summary>E22: a memory and its photos are gone for good.</summary>
internal sealed record ContentDeletedDefinitively(Guid EventId, DateTimeOffset OccurredAt, MemoryId MemoryId, Guid AuthorAccountId, int Sequence) : IDomainEvent;

/// <summary>E23: its last content went, so the memory no longer exists.</summary>
internal sealed record MemoryCeasedToExist(Guid EventId, DateTimeOffset OccurredAt, MemoryId MemoryId) : IDomainEvent;
