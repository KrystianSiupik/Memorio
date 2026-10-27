using Memorio.Modules.Access.Contracts;

namespace Memorio.Modules.Access.Infrastructure;

/// <summary>
/// Access is denied unless a rule grants it (ADR 0011). The rules (owner, participant, link, revoked,
/// blocked, hidden scope) come with the E4 and E8 tickets, each with its rows in the module's access
/// matrix; until then nobody sees anything.
/// </summary>
internal sealed class AccessReader : IAccessReader
{
    public Task<bool> CanSeeAlbumAsync(Guid personId, Guid albumId, CancellationToken cancellationToken) =>
        Task.FromResult(false);

    public Task<IReadOnlyCollection<Guid>> VisibleAlbumsAsync(Guid personId, CancellationToken cancellationToken) =>
        Task.FromResult<IReadOnlyCollection<Guid>>([]);
}
