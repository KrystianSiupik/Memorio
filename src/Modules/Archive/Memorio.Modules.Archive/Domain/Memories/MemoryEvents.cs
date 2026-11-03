using Memorio.SharedKernel.Domain;

namespace Memorio.Modules.Archive.Domain.Memories;

/// <summary>E19.</summary>
internal sealed record DescriptionApproved(Guid EventId, DateTimeOffset OccurredAt, MemoryId MemoryId) : IDomainEvent;

/// <summary>E20.</summary>
internal sealed record MemoryMovedToTrash(Guid EventId, DateTimeOffset OccurredAt, MemoryId MemoryId, Guid AuthorAccountId, int Sequence, DateTimeOffset WindowEndsAt) : IDomainEvent;

/// <summary>E12.</summary>
internal sealed record MemoryCreated(Guid EventId, DateTimeOffset OccurredAt, MemoryId MemoryId, Guid AuthorAccountId) : IDomainEvent;

/// <summary>E14.</summary>
internal sealed record PhotoAdded(Guid EventId, DateTimeOffset OccurredAt, MemoryId MemoryId, Guid PhotoId) : IDomainEvent;
