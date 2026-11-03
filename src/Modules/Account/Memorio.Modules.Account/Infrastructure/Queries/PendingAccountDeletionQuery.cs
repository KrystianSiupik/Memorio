using Dapper;
using Memorio.Modules.Account.Application.Queries.PendingAccountDeletion;
using Memorio.Modules.Account.Domain.Accounts;
using Memorio.Modules.Account.Domain.Deletions;
using Memorio.Modules.Account.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Memorio.Modules.Account.Infrastructure.Queries;

internal sealed class PendingAccountDeletionQuery(AccountDbContext database) : IPendingAccountDeletionQuery
{
    private const string Sql = """
        SELECT window_ends_at FROM account.account_deletions
        WHERE account_id = @accountId AND state = @pending
        """;

    public async Task<DateTimeOffset?> GetWindowEndAsync(AccountId accountId, CancellationToken cancellationToken)
    {
        // Npgsql gives a timestamptz as a DateTime in UTC.
        var windowEnd = await database.Database.GetDbConnection().QuerySingleOrDefaultAsync<DateTime?>(new CommandDefinition(
            Sql,
            new { accountId = accountId.Value, pending = nameof(DeletionState.Pending) },
            cancellationToken: cancellationToken));
        return windowEnd is { } utc ? new DateTimeOffset(utc, TimeSpan.Zero) : null;
    }
}
