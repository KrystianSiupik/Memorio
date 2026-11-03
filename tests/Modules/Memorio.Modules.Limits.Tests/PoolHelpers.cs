using Memorio.Modules.Limits.Domain.Pools;
using Memorio.Modules.Limits.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Npgsql;

namespace Memorio.Modules.Limits.Tests;

internal static class PoolHelpers
{
    public static async Task<AccountPool> PoolAsync(this LimitsDatabase database, Guid account)
    {
        await using var scope = database.CreateScope();
        return await scope.ServiceProvider.GetRequiredService<LimitsDbContext>().AccountPools.AsNoTracking()
            .SingleAsync(pool => pool.AccountId == account, TestContext.Current.CancellationToken);
    }

    /// <summary>A pool with this limit, as a plan will set it (E7).</summary>
    public static async Task<Guid> AccountWithLimitAsync(this LimitsDatabase database, long limitBytes)
    {
        var account = Guid.CreateVersion7();
        await using var connection = await database.OpenAsAsync(LimitsDatabase.Role);
        await using var insert = new NpgsqlCommand(
            "INSERT INTO limits.account_pools (account_id, used_bytes, limit_bytes, limit_sequence) VALUES (@account, 0, @limit, 0)",
            connection);
        insert.Parameters.AddWithValue("account", account);
        insert.Parameters.AddWithValue("limit", limitBytes);
        await insert.ExecuteNonQueryAsync(TestContext.Current.CancellationToken);
        return account;
    }
}
