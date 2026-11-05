using Memorio.Modules.Plans.Domain.Entitlements;

namespace Memorio.Modules.Plans.Application.Queries;

/// <summary>R16.</summary>
internal interface IPlanStateQuery
{
    /// <summary>A resource that never paid has no cycle: it is simply free.</summary>
    Task<PlanState> GetAsync(EntitledResource resource, CancellationToken cancellationToken);
}

internal sealed record PlanState(
    string Mode,
    DateTimeOffset? EntitledUntil,
    DateTimeOffset? ReadOnlySince,
    DateTimeOffset? TrimAt,
    DateTimeOffset? ChoiceOpensAt,
    bool IsHeld,
    DateTimeOffset? LastCheckedAt,
    IReadOnlyList<Guid> KeptMemoryIds);
