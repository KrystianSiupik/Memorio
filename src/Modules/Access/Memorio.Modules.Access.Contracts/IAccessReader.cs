namespace Memorio.Modules.Access.Contracts;

/// <summary>
/// Read port of ADR 0011 (D-035): the only answer in the system to "may this person see this content
/// now?". Asked at every read and never cached by the caller, so access taken away is gone at the very
/// next query (C9, D-031). Covered by the access matrix tests (MEM-E0-PLAT-14, <c>AccessMatrix</c>).
/// Ids of other modules' aggregates cross the boundary as <see cref="Guid"/> (D-187).
/// </summary>
public interface IAccessReader
{
    /// <summary>One album: before showing it or signing an address of its media (ADR 0005).</summary>
    Task<bool> CanSeeAlbumAsync(Guid personId, Guid albumId, CancellationToken cancellationToken);

    /// <summary>
    /// Every album the person may see now: the filter a list or search puts into its own SQL
    /// (<c>album_id = ANY(@visibleAlbums)</c>, D-031, D-126), fetched again for every query.
    /// </summary>
    Task<IReadOnlyCollection<Guid>> VisibleAlbumsAsync(Guid personId, CancellationToken cancellationToken);
}
