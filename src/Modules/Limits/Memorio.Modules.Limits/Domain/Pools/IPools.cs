namespace Memorio.Modules.Limits.Domain.Pools;

internal interface IPools
{
    /// <summary>False when it does not fit: nothing changed. The same key again changes nothing and answers true.</summary>
    Task<bool> ReserveAsync(Guid accountId, Guid operationKey, long bytes, DateTimeOffset at, CancellationToken cancellationToken);

    /// <summary>True when this call gave the space back; an unknown key or one given back before changes nothing.</summary>
    Task<bool> ReleaseAsync(Guid operationKey, DateTimeOffset at, CancellationToken cancellationToken);

    /// <summary>False when this month's allowance is used up. The same key again changes nothing and answers true.</summary>
    Task<bool> ConsumeAiAsync(Guid accountId, Guid operationKey, DateTimeOffset at, CancellationToken cancellationToken);

    /// <summary>Only a newer limit overwrites (D-016). A lower limit may leave the pool over it: nothing new fits then.</summary>
    Task<bool> SetLimitAsync(Guid accountId, long limitBytes, int aiLimit, long sequence, CancellationToken cancellationToken);
}
