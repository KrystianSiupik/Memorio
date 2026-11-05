using Dapper;
using Memorio.Modules.Access.Contracts;
using Memorio.Modules.Archive.Application.Queries.MemoryMap;
using Memorio.Modules.Archive.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Memorio.Modules.Archive.Infrastructure.Queries;

/// <summary>The person's own memories and those in albums they take part in now; an album read through a link stays out of it (ADR 0011, D-277).</summary>
internal sealed class MemoryMapQuery(ArchiveDbContext database, IAccessReader access) : IMemoryMapQuery
{
    private const string Sql = """
        SELECT id AS "Id", latitude AS "Latitude", longitude AS "Longitude"
        FROM archive.memories
        WHERE state = 'Active'
          AND (author_account_id = @accountId
               OR EXISTS (SELECT 1 FROM archive.memory_albums link WHERE link.memory_id = archive.memories.id AND link.album_id = ANY(@visibleAlbums)))
          AND (@from::date IS NULL OR date_to >= @from::date)
          AND (@to::date IS NULL OR date_from <= @to::date)
        """;

    public async Task<IReadOnlyList<MemoryPoint>> GetAsync(Guid accountId, DateOnly? from, DateOnly? to, CancellationToken cancellationToken)
    {
        var visibleAlbums = (await access.AlbumsTakingPartInAsync(accountId, cancellationToken)).ToArray();
        var points = await database.Database.GetDbConnection().QueryAsync<MemoryPoint>(new CommandDefinition(
            Sql,
            new { accountId, visibleAlbums, from = from?.ToString("yyyy-MM-dd", System.Globalization.CultureInfo.InvariantCulture), to = to?.ToString("yyyy-MM-dd", System.Globalization.CultureInfo.InvariantCulture) },
            cancellationToken: cancellationToken));
        return [.. points];
    }
}
