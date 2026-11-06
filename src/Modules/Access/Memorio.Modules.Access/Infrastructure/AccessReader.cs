using Dapper;
using Memorio.Modules.Access.Contracts;
using Memorio.Modules.Access.Domain.Audiences;
using Memorio.Modules.Access.Infrastructure.Persistence;
using Memorio.Modules.Account.Contracts;
using Memorio.Modules.Album.Contracts;
using Microsoft.EntityFrameworkCore;

namespace Memorio.Modules.Access.Infrastructure;

/// <summary>
/// Access is denied unless a rule grants it (ADR 0011). A block comes first, so whatever raced with it loses at the
/// next read (D-022). Then being in the album, read straight from Album (D-250), or a place on the list of viewers.
/// Last, a hidden scope of Moderation: nobody but the author sees what it hides (E161, P32).
/// </summary>
internal sealed class AccessReader(IAlbumMembership membership, IAccountDirectory directory, AccessDbContext database) : IAccessReader, IBlockList
{
    public async Task<bool> CanSeeAlbumAsync(Guid personId, Guid albumId, CancellationToken cancellationToken)
    {
        if (await IsBlockedAsync(albumId, personId, cancellationToken) || (await HiddenFromAsync(personId, [albumId], cancellationToken)).Count > 0)
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
        var seen = members.Union(viewed).Except(blocked).ToList();
        return [.. seen.Except(await HiddenFromAsync(personId, seen, cancellationToken))];
    }

    public async Task<IReadOnlyCollection<Guid>> AlbumsTakingPartInAsync(Guid personId, CancellationToken cancellationToken)
    {
        var blocked = await database.Database.GetDbConnection().QueryAsync<Guid>(new CommandDefinition(
            "SELECT album_id FROM access.album_blocks WHERE account_id = @personId",
            new { personId },
            cancellationToken: cancellationToken));
        var taking = (await membership.AlbumsOfAsync(personId, cancellationToken)).Except(blocked).ToList();
        return [.. taking.Except(await HiddenFromAsync(personId, taking, cancellationToken))];
    }

    public async Task<IReadOnlyCollection<Guid>> HiddenFromAsync(Guid personId, IReadOnlyCollection<Guid> resourceIds, CancellationToken cancellationToken)
    {
        if (resourceIds.Count == 0)
        {
            return [];
        }

        var hidden = await database.Database.GetDbConnection().QueryAsync<Guid>(new CommandDefinition(
            """
            SELECT DISTINCT resource_id FROM access.hidden_scopes
            WHERE hidden AND resource_id = ANY(@ids) AND author_account_id IS DISTINCT FROM @personId
            """,
            new { ids = resourceIds.ToArray(), personId },
            cancellationToken: cancellationToken));
        return [.. hidden];
    }

    public async Task<IReadOnlyCollection<Guid>> LegacyArchivesAsync(Guid personId, CancellationToken cancellationToken)
    {
        // The address of this moment: the contact was named by address, and may have no account until now.
        if (await directory.EmailOfAsync(personId, cancellationToken) is not { } email)
        {
            return [];
        }

        return [.. await database.Database.GetDbConnection().QueryAsync<Guid>(new CommandDefinition(
            "SELECT owner_account_id FROM access.legacy_grants WHERE contact_hash = @hash",
            new { hash = LegacyGrant.HashOf(email) },
            cancellationToken: cancellationToken))];
    }

    public Task<bool> IsBlockedAsync(Guid albumId, Guid accountId, CancellationToken cancellationToken) =>
        database.Database.GetDbConnection().ExecuteScalarAsync<bool>(new CommandDefinition(
            "SELECT EXISTS (SELECT 1 FROM access.album_blocks WHERE album_id = @albumId AND account_id = @accountId)",
            new { albumId, accountId },
            cancellationToken: cancellationToken));
}
