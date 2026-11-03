using Dapper;
using Memorio.Modules.Archive.Application.Queries.MemoryMap;
using Memorio.Modules.Archive.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Memorio.Modules.Archive.Infrastructure.Queries;

internal sealed class MemoryMapQuery(ArchiveDbContext database) : IMemoryMapQuery
{
    private const string Sql = """
        SELECT id AS "Id", latitude AS "Latitude", longitude AS "Longitude"
        FROM archive.memories
        WHERE author_account_id = @accountId AND state = 'Active'
          AND (@from::date IS NULL OR date_to >= @from::date)
          AND (@to::date IS NULL OR date_from <= @to::date)
        """;

    public async Task<IReadOnlyList<MemoryPoint>> GetAsync(Guid accountId, DateOnly? from, DateOnly? to, CancellationToken cancellationToken)
    {
        var points = await database.Database.GetDbConnection().QueryAsync<MemoryPoint>(new CommandDefinition(
            Sql,
            new { accountId, from = from?.ToString("yyyy-MM-dd", System.Globalization.CultureInfo.InvariantCulture), to = to?.ToString("yyyy-MM-dd", System.Globalization.CultureInfo.InvariantCulture) },
            cancellationToken: cancellationToken));
        return [.. points];
    }
}
