using Dapper;
using Memorio.Modules.Archive.Application.Queries.MyMemories;
using Memorio.Modules.Archive.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Memorio.Modules.Archive.Infrastructure.Queries;

internal sealed class MyMemoriesQuery(ArchiveDbContext database) : IMyMemoriesQuery
{
    private const string Sql = """
        SELECT id AS "Id", place_name AS "PlaceName", date_from::text AS "DateFrom"
        FROM archive.memories
        WHERE author_account_id = @accountId AND state = 'Active'
        ORDER BY date_from DESC, id
        """;

    public async Task<IReadOnlyList<MyMemory>> GetAsync(Guid accountId, CancellationToken cancellationToken) =>
        [.. await database.Database.GetDbConnection().QueryAsync<MyMemory>(new CommandDefinition(Sql, new { accountId }, cancellationToken: cancellationToken))];
}
