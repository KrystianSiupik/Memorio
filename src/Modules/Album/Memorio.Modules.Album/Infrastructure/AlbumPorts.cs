using Dapper;
using Memorio.Modules.Album.Contracts;
using Memorio.Modules.Album.Domain.Albums;
using Memorio.Modules.Album.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Memorio.Modules.Album.Infrastructure;

internal sealed class AlbumMembership(AlbumDbContext database) : IAlbumMembership
{
    private const string MemberSql = """
        SELECT EXISTS (SELECT 1 FROM album.albums WHERE id = @albumId AND owner_account_id = @accountId)
            OR EXISTS (SELECT 1 FROM album.album_participants WHERE album_id = @albumId AND account_id = @accountId)
        """;

    private const string AlbumsSql = """
        SELECT id FROM album.albums WHERE owner_account_id = @accountId
        UNION
        SELECT album_id FROM album.album_participants WHERE account_id = @accountId
        """;

    public Task<bool> IsMemberAsync(Guid accountId, Guid albumId, CancellationToken cancellationToken) =>
        database.Database.GetDbConnection().ExecuteScalarAsync<bool>(
            new CommandDefinition(MemberSql, new { accountId, albumId }, cancellationToken: cancellationToken));

    public Task<Guid?> OwnerOfAsync(Guid albumId, CancellationToken cancellationToken) =>
        database.Database.GetDbConnection().ExecuteScalarAsync<Guid?>(
            new CommandDefinition("SELECT owner_account_id FROM album.albums WHERE id = @albumId", new { albumId }, cancellationToken: cancellationToken));

    public async Task<IReadOnlyCollection<Guid>> AlbumsOfAsync(Guid accountId, CancellationToken cancellationToken) =>
        [.. await database.Database.GetDbConnection().QueryAsync<Guid>(
            new CommandDefinition(AlbumsSql, new { accountId }, cancellationToken: cancellationToken))];
}

internal sealed class AlbumNames(AlbumDbContext database) : IAlbumNames
{
    public async Task<IReadOnlyDictionary<Guid, string>> NamesOfAsync(IReadOnlyCollection<Guid> albumIds, CancellationToken cancellationToken)
    {
        var rows = await database.Database.GetDbConnection().QueryAsync<(Guid Id, string Name)>(new CommandDefinition(
            "SELECT id, name FROM album.albums WHERE id = ANY(@albumIds)",
            new { albumIds = albumIds.ToArray() },
            cancellationToken: cancellationToken));
        return rows.ToDictionary(row => row.Id, row => row.Name);
    }
}

internal sealed class AlbumMessaging(AlbumDbContext database) : IAlbumMessaging
{
    public Task<PendingInvitation?> FindPendingInvitationAsync(Guid invitationId, CancellationToken cancellationToken) =>
        database.Database.GetDbConnection().QuerySingleOrDefaultAsync<PendingInvitation>(new CommandDefinition(
            """
            SELECT album_id AS "AlbumId", email AS "Email", invited_by_account_id AS "InvitedByAccountId"
            FROM album.album_invitations WHERE id = @invitationId AND state = 'Pending'
            """,
            new { invitationId },
            cancellationToken: cancellationToken));
}

internal sealed class AlbumSuccession(IAlbums albums, TimeProvider clock) : IAlbumSuccession
{
    public async Task<SuccessorOutcome> NameAsync(Guid albumId, Guid ownerAccountId, Guid successorAccountId, CancellationToken cancellationToken)
    {
        var album = await albums.FindAsync(new AlbumId(albumId), cancellationToken);
        if (album is null)
        {
            return SuccessorOutcome.NoAccess;
        }

        var named = album.NameSuccessor(ownerAccountId, successorAccountId, clock.GetUtcNow());
        if (named.IsFailure)
        {
            return named.Error == AlbumErrors.NoAccess ? SuccessorOutcome.NoAccess : SuccessorOutcome.NotParticipant;
        }

        return await albums.SaveAsync(cancellationToken) ? SuccessorOutcome.Named : SuccessorOutcome.AlreadyDecided;
    }
}
