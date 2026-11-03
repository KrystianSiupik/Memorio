using Memorio.Modules.Limits.Domain.Pools;
using Microsoft.EntityFrameworkCore;

namespace Memorio.Modules.Limits.Infrastructure.Persistence;

/// <summary>Conditional updates (D-016) with the ledger of operations under a unique key, in one transaction (D-015).</summary>
internal sealed class Pools(LimitsDbContext database) : IPools
{
    public async Task<bool> ReserveAsync(Guid accountId, Guid operationKey, long bytes, DateTimeOffset at, CancellationToken cancellationToken)
    {
        await using var transaction = await database.Database.BeginTransactionAsync(cancellationToken);
        await EnsurePoolAsync(accountId, cancellationToken);

        var isNew = await database.Database.ExecuteSqlAsync(
            $"""
            INSERT INTO limits.pool_operations (operation_key, account_id, bytes, reserved_at)
            VALUES ({operationKey}, {accountId}, {bytes}, {at})
            ON CONFLICT (operation_key) DO NOTHING
            """,
            cancellationToken) == 1;
        if (!isNew)
        {
            await transaction.CommitAsync(cancellationToken);
            return true;
        }

        var fits = await database.Database.ExecuteSqlAsync(
            $"""
            UPDATE limits.account_pools SET used_bytes = used_bytes + {bytes}
            WHERE account_id = {accountId} AND used_bytes + {bytes} <= limit_bytes
            """,
            cancellationToken) == 1;
        if (!fits)
        {
            await transaction.RollbackAsync(cancellationToken);
            return false;
        }

        await transaction.CommitAsync(cancellationToken);
        return true;
    }

    private Task<int> EnsurePoolAsync(Guid accountId, CancellationToken cancellationToken) =>
        database.Database.ExecuteSqlAsync(
            $"""
            INSERT INTO limits.account_pools (account_id, used_bytes, limit_bytes, limit_sequence)
            VALUES ({accountId}, 0, {AccountPool.DefaultLimitBytes}, 0)
            ON CONFLICT (account_id) DO NOTHING
            """,
            cancellationToken);
}
