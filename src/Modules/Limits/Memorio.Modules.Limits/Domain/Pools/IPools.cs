namespace Memorio.Modules.Limits.Domain.Pools;

internal interface IPools
{
    /// <summary>False when it does not fit: nothing changed. The same key again changes nothing and answers true.</summary>
    Task<bool> ReserveAsync(Guid accountId, Guid operationKey, long bytes, DateTimeOffset at, CancellationToken cancellationToken);
}
