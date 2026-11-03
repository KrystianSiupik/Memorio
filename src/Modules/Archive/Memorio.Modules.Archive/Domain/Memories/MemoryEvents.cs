using Memorio.SharedKernel.Domain;

namespace Memorio.Modules.Archive.Domain.Memories;

/// <summary>E12.</summary>
internal sealed record MemoryCreated(Guid EventId, DateTimeOffset OccurredAt, MemoryId MemoryId, Guid AuthorAccountId) : IDomainEvent;
