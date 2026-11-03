using Dapper;
using Memorio.Modules.Album.Contracts;
using Memorio.Modules.Album.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Memorio.Modules.Album.Infrastructure.Queries;

/// <summary>Counts only the albums the account is in. A person listed in their own album counts once, as its owner.</summary>
internal sealed class AccountAlbumsReader(AlbumDbContext database) : IAccountAlbumsReader
{
    private const string Sql = """
        SELECT
            (SELECT count(*)::int FROM album.albums WHERE owner_account_id = @accountId) AS "OwnAlbums",
            (SELECT count(*)::int FROM album.albums
             WHERE owner_account_id = @accountId AND successor_account_id IS NULL) AS "OwnAlbumsWithoutSuccessor",
            (SELECT count(*)::int FROM album.album_participants participant
             JOIN album.albums album ON album.id = participant.album_id
             WHERE participant.account_id = @accountId AND album.owner_account_id <> @accountId) AS "OtherPeoplesAlbums"
        """;

    public Task<AccountAlbumsSummary> SummarizeAsync(Guid accountId, CancellationToken cancellationToken) =>
        database.Database.GetDbConnection().QuerySingleAsync<AccountAlbumsSummary>(
            new CommandDefinition(Sql, new { accountId }, cancellationToken: cancellationToken));
}
