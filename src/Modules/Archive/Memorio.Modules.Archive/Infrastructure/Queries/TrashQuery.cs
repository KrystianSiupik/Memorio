using Dapper;
using Memorio.Modules.Archive.Application.Queries.Trash;
using Memorio.Modules.Archive.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Memorio.Modules.Archive.Infrastructure.Queries;

internal sealed class TrashQuery(ArchiveDbContext database, TimeProvider clock) : ITrashQuery
{
    private const string Sql = """
        SELECT memory.id AS "Id", memory.place_name AS "PlaceName", memory.date_from::text AS "DateFrom", memory.description AS "Description",
               (SELECT count(*)::int FROM archive.photos photo WHERE photo.memory_id = memory.id) AS "Photos",
               memory.trashed_at AS "TrashedAt", memory.window_ends_at AS "WindowEndsAt"
        FROM archive.memories memory
        WHERE memory.author_account_id = @accountId AND memory.state = 'Trashed' AND memory.window_ends_at > @now
        ORDER BY memory.trashed_at DESC
        """;

    public async Task<IReadOnlyList<TrashedMemory>> GetAsync(Guid accountId, CancellationToken cancellationToken)
    {
        var now = clock.GetUtcNow();
        var rows = await database.Database.GetDbConnection().QueryAsync<Row>(new CommandDefinition(Sql, new { accountId, now }, cancellationToken: cancellationToken));
        return [.. rows.Select(row => new TrashedMemory(
            row.Id,
            row.PlaceName,
            row.DateFrom,
            row.Description,
            row.Photos,
            new DateTimeOffset(row.TrashedAt, TimeSpan.Zero),
            (int)Math.Ceiling((new DateTimeOffset(row.WindowEndsAt, TimeSpan.Zero) - now).TotalDays)))];
    }

    // Npgsql gives a timestamptz as a DateTime in UTC.
    private sealed record Row(Guid Id, string PlaceName, string DateFrom, string? Description, int Photos, DateTime TrashedAt, DateTime WindowEndsAt);
}
