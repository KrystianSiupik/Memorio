using Memorio.SharedKernel.Messaging;

namespace Memorio.Modules.Archive.Contracts;

/// <summary>E25: this many memories of an account or album are in the trash for 30 days. Only the effect, with the sequence of the plan's order.</summary>
public sealed record ArchiveTrimmedEvent(Guid EventId, DateTimeOffset OccurredAt, Guid ResourceId, bool IsAlbum, int MemoriesTrimmed, int Sequence) : IIntegrationEvent;
