using Dapper;
using Memorio.Modules.Archive.Contracts;
using Memorio.Modules.Archive.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Memorio.Modules.Archive.Infrastructure.Queries;

/// <summary>R8 for AiNarration: a projection without logic, with Dapper on the module's own schema (D-126).</summary>
internal sealed class MemoryFactsQuery(ArchiveDbContext database) : IMemoryFacts
{
    private const string Sql = """
        SELECT id AS "MemoryId", author_account_id AS "AuthorAccountId", place_name AS "PlaceName",
               date_precision AS "DatePrecision", date_from AS "DateFrom", date_to AS "DateTo",
               to_char(time_of_day, 'HH24:MI') AS "Time", description AS "Description"
        FROM archive.memories
        WHERE id = ANY(@ids) AND state = 'Active'
        ORDER BY date_from, time_of_day NULLS FIRST, id
        """;

    public async Task<IReadOnlyList<MemoryFact>> FactsAsync(IReadOnlyCollection<Guid> memoryIds, CancellationToken cancellationToken)
    {
        if (memoryIds.Count == 0)
        {
            return [];
        }

        var rows = await database.Database.GetDbConnection().QueryAsync<Row>(
            new CommandDefinition(Sql, new { ids = memoryIds.ToArray() }, cancellationToken: cancellationToken));
        return [.. rows.Select(row => new MemoryFact(
            row.MemoryId,
            row.AuthorAccountId,
            row.PlaceName,
            row.DatePrecision,
            row.DateFrom,
            row.DateTo,
            row.Time,
            row.Description))];
    }

    private sealed record Row(Guid MemoryId, Guid AuthorAccountId, string PlaceName, string DatePrecision, DateOnly DateFrom, DateOnly DateTo, string? Time, string? Description);
}
