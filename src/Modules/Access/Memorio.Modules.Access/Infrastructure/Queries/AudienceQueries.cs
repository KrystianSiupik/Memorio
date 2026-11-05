using Dapper;
using Memorio.Modules.Access.Application.Queries;
using Memorio.Modules.Access.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Memorio.Modules.Access.Infrastructure.Queries;

internal sealed class AudienceQueries(AccessDbContext database) : IAudienceQueries
{
    public async Task<IReadOnlyList<ViewerRow>> ViewersAsync(Guid albumId, CancellationToken cancellationToken) =>
        [.. await database.Database.GetDbConnection().QueryAsync<ViewerRow>(new CommandDefinition(
            """
            SELECT account_id AS "AccountId", entered_at AS "EnteredAt" FROM access.album_viewers
            WHERE album_id = @albumId ORDER BY entered_at, account_id
            """,
            new { albumId },
            cancellationToken: cancellationToken))];

    public async Task<IReadOnlyList<Guid>> BlockedAsync(Guid albumId, CancellationToken cancellationToken) =>
        [.. await database.Database.GetDbConnection().QueryAsync<Guid>(new CommandDefinition(
            "SELECT account_id FROM access.album_blocks WHERE album_id = @albumId ORDER BY blocked_at, account_id",
            new { albumId },
            cancellationToken: cancellationToken))];

    public Task<string?> LinkTokenAsync(Guid albumId, CancellationToken cancellationToken) =>
        database.Database.GetDbConnection().ExecuteScalarAsync<string?>(new CommandDefinition(
            "SELECT link_token FROM access.album_audiences WHERE album_id = @albumId",
            new { albumId },
            cancellationToken: cancellationToken));

    public Task<Guid?> AlbumOfTokenAsync(string token, CancellationToken cancellationToken) =>
        database.Database.GetDbConnection().ExecuteScalarAsync<Guid?>(new CommandDefinition(
            "SELECT album_id FROM access.album_audiences WHERE link_token = @token",
            new { token },
            cancellationToken: cancellationToken));
}
