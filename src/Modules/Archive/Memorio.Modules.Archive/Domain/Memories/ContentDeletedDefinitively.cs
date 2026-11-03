using Memorio.SharedKernel.Domain;

namespace Memorio.Modules.Archive.Domain.Memories;

/// <summary>E22: a memory and its photos are gone for good.</summary>
internal sealed record ContentDeletedDefinitively(Guid EventId, DateTimeOffset OccurredAt, MemoryId MemoryId, Guid AuthorAccountId) : IDomainEvent;
