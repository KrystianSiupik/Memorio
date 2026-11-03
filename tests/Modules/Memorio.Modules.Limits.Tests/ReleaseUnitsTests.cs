using Memorio.Modules.Limits.Application.Commands.ReleaseUnits;
using Memorio.Modules.Limits.Application.Commands.ReserveUnits;
using Memorio.Modules.Limits.Domain.Pools;
using Microsoft.Extensions.DependencyInjection;

namespace Memorio.Modules.Limits.Tests;

/// <summary>MEM-E2-LIMITY-01 on a real database.</summary>
public sealed class ReleaseUnitsTests(LimitsDatabase database)
{
    private static CancellationToken Cancellation => TestContext.Current.CancellationToken;

    [Fact]
    public async Task Giving_back_makes_room_and_gives_the_event()
    {
        var account = await database.AccountWithLimitAsync(100);
        var key = await ReserveAsync(account, 100);

        var released = await ReleaseAsync(key);

        Assert.Equal(key, released?.OperationKey);
        Assert.Equal(0, (await database.PoolAsync(account)).UsedBytes);
        await ReserveAsync(account, 100);
    }

    [Fact]
    public async Task Giving_back_twice_or_at_once_returns_the_space_once()
    {
        var account = await database.AccountWithLimitAsync(100);
        var key = await ReserveAsync(account, 40);
        await ReserveAsync(account, 10);

        var results = await Task.WhenAll(Enumerable.Range(0, 5).Select(_ => ReleaseAsync(key)));
        var later = await ReleaseAsync(key);

        Assert.Single(results, result => result is not null);
        Assert.Null(later);
        Assert.Equal(10, (await database.PoolAsync(account)).UsedBytes);
    }

    [Fact]
    public async Task An_unknown_operation_changes_nothing()
    {
        Assert.Null(await ReleaseAsync(Guid.CreateVersion7()));
    }

    [Fact]
    public async Task Only_a_newer_limit_overwrites()
    {
        var account = await database.AccountWithLimitAsync(100);
        await using var scope = database.CreateScope();
        var pools = scope.ServiceProvider.GetRequiredService<IPools>();

        Assert.True(await pools.SetLimitAsync(account, 500, sequence: 2, Cancellation));
        Assert.False(await pools.SetLimitAsync(account, 50, sequence: 1, Cancellation));

        Assert.Equal(500, (await database.PoolAsync(account)).LimitBytes);
    }

    [Fact]
    public async Task After_lowering_the_limit_the_pool_is_over_it_and_nothing_new_fits()
    {
        var account = await database.AccountWithLimitAsync(100);
        await ReserveAsync(account, 80);
        await using (var scope = database.CreateScope())
        {
            await scope.ServiceProvider.GetRequiredService<IPools>().SetLimitAsync(account, 50, sequence: 1, Cancellation);
        }

        await using var other = database.CreateScope();
        var refused = await other.ServiceProvider.GetRequiredService<ReserveUnitsHandler>()
            .HandleAsync(new ReserveUnits(account, Guid.CreateVersion7(), 1), Cancellation);

        Assert.Equal(PoolErrors.PoolFull, refused.Error);
        Assert.Equal(80, (await database.PoolAsync(account)).UsedBytes);
    }

    private async Task<Guid> ReserveAsync(Guid account, long bytes)
    {
        var key = Guid.CreateVersion7();
        await using var scope = database.CreateScope();
        var reserved = await scope.ServiceProvider.GetRequiredService<ReserveUnitsHandler>()
            .HandleAsync(new ReserveUnits(account, key, bytes), Cancellation);
        Assert.True(reserved.IsSuccess);
        return key;
    }

    private async Task<UnitsReleased?> ReleaseAsync(Guid key)
    {
        await using var scope = database.CreateScope();
        return await scope.ServiceProvider.GetRequiredService<ReleaseUnitsHandler>().HandleAsync(new ReleaseUnits(key), Cancellation);
    }
}
