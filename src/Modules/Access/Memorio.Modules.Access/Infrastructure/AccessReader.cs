using Memorio.Modules.Access.Contracts;
using Memorio.Modules.Album.Contracts;

namespace Memorio.Modules.Access.Infrastructure;

/// <summary>
/// Access is denied unless a rule grants it (ADR 0011). The one rule so far: the owner and the participants of
/// an album, read straight from Album, so whoever left has no access at the very next query (D-250). The other
/// rules (link, revoked, blocked, hidden scope) come with the E4 and E8 tickets.
/// </summary>
internal sealed class AccessReader(IAlbumMembership membership) : IAccessReader
{
    public Task<bool> CanSeeAlbumAsync(Guid personId, Guid albumId, CancellationToken cancellationToken) =>
        membership.IsMemberAsync(personId, albumId, cancellationToken);

    public Task<IReadOnlyCollection<Guid>> VisibleAlbumsAsync(Guid personId, CancellationToken cancellationToken) =>
        membership.AlbumsOfAsync(personId, cancellationToken);
}
