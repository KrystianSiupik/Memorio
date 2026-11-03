using Memorio.SharedKernel.Messaging;

namespace Memorio.Modules.Archive.Contracts;

/// <summary>
/// E22: a memory and its photos are gone for good, whatever the reason; modules holding something derived from
/// it react. Only the effect. <paramref name="Sequence"/> orders events about one memory (D-013).
/// </summary>
public sealed record ContentDeleted(Guid EventId, DateTimeOffset OccurredAt, Guid MemoryId, Guid AuthorAccountId, int Sequence) : IIntegrationEvent;
