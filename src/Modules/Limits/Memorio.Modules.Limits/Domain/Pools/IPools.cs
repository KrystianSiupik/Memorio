using Memorio.Modules.Limits.Contracts;

namespace Memorio.Modules.Limits.Domain.Pools;

internal interface IPools
{
    /// <summary>False when it does not fit: nothing changed. The same key again changes nothing and answers true; a key given back counts anew, also in another pool.</summary>
    Task<bool> ReserveAsync(Guid accountId, Guid operationKey, long bytes, DateTimeOffset at, CancellationToken cancellationToken);

    /// <summary>Like <see cref="ReserveAsync"/> without the limit: for content already stored (D-264).</summary>
    Task ReserveOverLimitAsync(Guid accountId, Guid operationKey, long bytes, DateTimeOffset at, CancellationToken cancellationToken);

    /// <summary>True when this call gave the space back; an unknown key or one given back before changes nothing.</summary>
    Task<bool> ReleaseAsync(Guid operationKey, DateTimeOffset at, CancellationToken cancellationToken);

    /// <summary>False when this month's allowance is used up. The same key again changes nothing and answers true.</summary>
    Task<bool> ConsumeAiAsync(Guid accountId, Guid operationKey, DateTimeOffset at, CancellationToken cancellationToken);

    /// <summary>A pool not used yet has nothing used and the default limit.</summary>
    Task<(long UsedBytes, long LimitBytes)> GetUsageAsync(Guid poolId, CancellationToken cancellationToken);

    /// <summary>A pool not used yet has nothing used and the default limit; AI used counts the month of <paramref name="at"/>.</summary>
    Task<PoolStanding> GetStandingAsync(Guid poolId, DateTimeOffset at, CancellationToken cancellationToken);

    /// <summary>Only a newer limit overwrites (D-016). A lower limit may leave the pool over it: nothing new fits then.</summary>
    Task<bool> SetLimitAsync(Guid accountId, long limitBytes, int aiLimit, long sequence, CancellationToken cancellationToken);
}
