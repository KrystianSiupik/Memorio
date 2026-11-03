using Dapper;
using Memorio.Modules.Archive.Contracts;
using Memorio.Modules.Archive.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Memorio.Modules.Archive.Infrastructure.Queries;

/// <summary>Counts what the account is the author of, and nothing of anybody else.</summary>
internal sealed class AccountContentReader(ArchiveDbContext database) : IAccountContentReader
{
    private const string Sql = """
        SELECT
            (SELECT count(*)::int FROM archive.memories WHERE author_account_id = @accountId) AS memories,
            (SELECT count(*)::int FROM archive.photos photo
             JOIN archive.memories memory ON memory.id = photo.memory_id
             WHERE memory.author_account_id = @accountId) AS photos
        """;

    public Task<AccountContentSummary> SummarizeAsync(Guid accountId, CancellationToken cancellationToken) =>
        database.Database.GetDbConnection().QuerySingleAsync<AccountContentSummary>(
            new CommandDefinition(Sql, new { accountId }, cancellationToken: cancellationToken));
}
