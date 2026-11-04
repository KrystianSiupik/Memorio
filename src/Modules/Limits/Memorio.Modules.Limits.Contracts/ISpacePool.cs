namespace Memorio.Modules.Limits.Contracts;

/// <summary>Write port for Archive: the space of an account. The key is the id of the content (D-015).</summary>
public interface ISpacePool
{
    /// <summary>False when it does not fit. The same key again changes nothing and answers true; a key given back counts anew.</summary>
    Task<bool> TryReserveAsync(Guid accountId, Guid operationKey, long bytes, CancellationToken cancellationToken);

    /// <summary>
    /// Content already stored comes back to its author (D-264): it counts even over the limit, because taking it
    /// along must not lose it; nothing new fits then. The same key again changes nothing.
    /// </summary>
    Task ReserveOverLimitAsync(Guid accountId, Guid operationKey, long bytes, CancellationToken cancellationToken);

    /// <summary>The space of this content comes back; safe to call again.</summary>
    Task ReleaseAsync(Guid operationKey, CancellationToken cancellationToken);
}
