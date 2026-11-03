using Memorio.Modules.Limits.Contracts;
using Microsoft.Extensions.DependencyInjection;

namespace Memorio.Modules.Limits.Tests;

/// <summary>MEM-E2-LIMITY-02: the port as Archive sees it.</summary>
public sealed class SpacePoolTests(LimitsDatabase database)
{
    private static CancellationToken Cancellation => TestContext.Current.CancellationToken;

    [Fact]
    public async Task Reserve_refuse_release_and_reserve_again()
    {
        var account = await database.AccountWithLimitAsync(100);
        await using var scope = database.CreateScope();
        var pool = scope.ServiceProvider.GetRequiredService<ISpacePool>();
        var first = Guid.CreateVersion7();

        Assert.True(await pool.TryReserveAsync(account, first, 100, Cancellation));
        Assert.False(await pool.TryReserveAsync(account, Guid.CreateVersion7(), 1, Cancellation));

        await pool.ReleaseAsync(first, Cancellation);
        await pool.ReleaseAsync(first, Cancellation);

        Assert.True(await pool.TryReserveAsync(account, Guid.CreateVersion7(), 100, Cancellation));
    }
}
