using Dapper;
using Memorio.Modules.Access.Contracts;
using Memorio.Modules.Access.Infrastructure.Persistence;
using Memorio.Modules.Album.Contracts;
using Microsoft.EntityFrameworkCore;

namespace Memorio.Modules.Access.Infrastructure;

/// <summary>
/// Access is denied unless a rule grants it (ADR 0011). A block comes first, so whatever raced with it loses at the
/// next read (D-022). Then being in the album, read straight from Album (D-250), or a place on the list of viewers.
/// The hidden scope of E8 is still to come.
/// </summary>
internal sealed class AccessReader(IAlbumMembership membership, AccessDbContext database) : IAccessReader, IBlockList
{
    public async Task<bool> CanSeeAlbumAsync(Guid personId, Guid albumId, CancellationToken cancellationToken)
    {
        if (await IsBlockedAsync(albumId, personId, cancellationToken))
        {
            return false;
        }

        return await membership.IsMemberAsync(personId, albumId, cancellationToken)
            || await database.Database.GetDbConnection().ExecuteScalarAsync<bool>(new CommandDefinition(
                "SELECT EXISTS (SELECT 1 FROM access.album_viewers WHERE album_id = @albumId AND account_id = @personId)",
                new { albumId, personId },
                cancellationToken: cancellationToken));
    }

    public async Task<IReadOnlyCollection<Guid>> VisibleAlbumsAsync(Guid personId, CancellationToken cancellationToken)
    {
        var connection = database.Database.GetDbConnection();
        var viewed = await connection.QueryAsync<Guid>(new CommandDefinition(
            "SELECT album_id FROM access.album_viewers WHERE account_id = @personId",
            new { personId },
            cancellationToken: cancellationToken));
        var blocked = await connection.QueryAsync<Guid>(new CommandDefinition(
            "SELECT album_id FROM access.album_blocks WHERE account_id = @personId",
            new { personId },
            cancellationToken: cancellationToken));
        var members = await membership.AlbumsOfAsync(personId, cancellationToken);
        return [.. members.Union(viewed).Except(blocked)];
    }

    public async Task<IReadOnlyCollection<Guid>> AlbumsTakingPartInAsync(Guid personId, CancellationToken cancellationToken)
    {
        var blocked = await database.Database.GetDbConnection().QueryAsync<Guid>(new CommandDefinition(
            "SELECT album_id FROM access.album_blocks WHERE account_id = @personId",
            new { personId },
            cancellationToken: cancellationToken));
        return [.. (await membership.AlbumsOfAsync(personId, cancellationToken)).Except(blocked)];
    }

    public Task<bool> IsBlockedAsync(Guid albumId, Guid accountId, CancellationToken cancellationToken) =>
        database.Database.GetDbConnection().ExecuteScalarAsync<bool>(new CommandDefinition(
            "SELECT EXISTS (SELECT 1 FROM access.album_blocks WHERE album_id = @albumId AND account_id = @accountId)",
            new { albumId, accountId },
            cancellationToken: cancellationToken));
}
