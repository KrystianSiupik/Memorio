using Memorio.SharedKernel.Messaging;

namespace Memorio.Modules.Archive.Contracts;

/// <summary>E25: this many memories of an account or album are in the trash for 30 days. Only the effect, with the sequence of the plan's order.</summary>
/// <param name="TrimmedMemoryIds">Which ones (P34: what was made from them goes); null in events sent before E6.</param>
public sealed record ArchiveTrimmedEvent(
    Guid EventId,
    DateTimeOffset OccurredAt,
    Guid ResourceId,
    bool IsAlbum,
    int MemoriesTrimmed,
    int Sequence,
    IReadOnlyList<Guid>? TrimmedMemoryIds = null) : IIntegrationEvent;
