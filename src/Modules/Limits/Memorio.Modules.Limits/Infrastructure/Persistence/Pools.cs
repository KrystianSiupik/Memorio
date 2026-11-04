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

    public async Task<bool> ReleaseAsync(Guid operationKey, DateTimeOffset at, CancellationToken cancellationToken)
    {
        await using var transaction = await database.Database.BeginTransactionAsync(cancellationToken);

        // The operation first: its row lock makes a parallel release wait and then find it released.
        var released = await database.Database.ExecuteSqlAsync(
            $"UPDATE limits.pool_operations SET released_at = {at} WHERE operation_key = {operationKey} AND released_at IS NULL",
            cancellationToken) == 1;
        if (released)
        {
            await database.Database.ExecuteSqlAsync(
                $"""
                UPDATE limits.account_pools pool SET used_bytes = pool.used_bytes - operation.bytes
                FROM limits.pool_operations operation
                WHERE operation.operation_key = {operationKey} AND pool.account_id = operation.account_id
                """,
                cancellationToken);
        }

        await transaction.CommitAsync(cancellationToken);
        return released;
    }

    public async Task<bool> ConsumeAiAsync(Guid accountId, Guid operationKey, DateTimeOffset at, CancellationToken cancellationToken)
    {
        var period = new DateOnly(at.UtcDateTime.Year, at.UtcDateTime.Month, 1);
        await using var transaction = await database.Database.BeginTransactionAsync(cancellationToken);
        await EnsurePoolAsync(accountId, cancellationToken);

        var isNew = await database.Database.ExecuteSqlAsync(
            $"""
            INSERT INTO limits.ai_operations (operation_key, account_id, consumed_at)
            VALUES ({operationKey}, {accountId}, {at})
            ON CONFLICT (operation_key) DO NOTHING
            """,
            cancellationToken) == 1;
        if (!isNew)
        {
            await transaction.CommitAsync(cancellationToken);
            return true;
        }

        var allowed = await database.Database.ExecuteSqlAsync(
            $"""
            UPDATE limits.account_pools
            SET ai_used = CASE WHEN ai_period = {period} THEN ai_used + 1 ELSE 1 END, ai_period = {period}
            WHERE account_id = {accountId} AND CASE WHEN ai_period = {period} THEN ai_used ELSE 0 END < ai_limit
            """,
            cancellationToken) == 1;
        if (!allowed)
        {
            await transaction.RollbackAsync(cancellationToken);
            return false;
        }

        await transaction.CommitAsync(cancellationToken);
        return true;
    }

    public async Task<(long UsedBytes, long LimitBytes)> GetUsageAsync(Guid poolId, CancellationToken cancellationToken)
    {
        var pool = await database.Set<AccountPool>().AsNoTracking().FirstOrDefaultAsync(item => item.AccountId == poolId, cancellationToken);
        return pool is null ? (0, AccountPool.DefaultLimitBytes) : (pool.UsedBytes, pool.LimitBytes);
    }

    public async Task<bool> SetLimitAsync(Guid accountId, long limitBytes, int aiLimit, long sequence, CancellationToken cancellationToken)
    {
        await EnsurePoolAsync(accountId, cancellationToken);
        return await database.Database.ExecuteSqlAsync(
            $"""
            UPDATE limits.account_pools SET limit_bytes = {limitBytes}, ai_limit = {aiLimit}, limit_sequence = {sequence}
            WHERE account_id = {accountId} AND limit_sequence < {sequence}
            """,
            cancellationToken) == 1;
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
