namespace Memorio.Modules.Limits.Contracts;

/// <summary>Ports for Album and for Archive on its behalf: the shared space of an album. The key is the id of the content (D-015).</summary>
public interface IAlbumPool
{
    /// <summary>False when it does not fit. The same key again changes nothing and answers true.</summary>
    Task<bool> TryReserveAsync(Guid albumId, Guid operationKey, long bytes, CancellationToken cancellationToken);

    /// <summary>The space of this content comes back; safe to call again.</summary>
    Task ReleaseAsync(Guid operationKey, CancellationToken cancellationToken);

    /// <summary>R6: how the pool stands. The caller has checked that the person may see the album.</summary>
    Task<AlbumPoolState> GetStateAsync(Guid albumId, CancellationToken cancellationToken);
}

/// <param name="UsedBytes">Above the limit only after the limit was lowered.</param>
public sealed record AlbumPoolState(long UsedBytes, long LimitBytes)
{
    public long FreeBytes => Math.Max(0, LimitBytes - UsedBytes);
}
