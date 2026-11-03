using Memorio.Modules.Limits.Application.Commands.ReserveUnits;
using Memorio.Modules.Limits.Domain.Pools;
using Microsoft.Extensions.DependencyInjection;

namespace Memorio.Modules.Limits.Tests;

/// <summary>MEM-E3-LIMITY-01 on a real database.</summary>
public sealed class ReserveUnitsTests(LimitsDatabase database)
{
    [Fact]
    public async Task A_first_reservation_opens_a_pool_with_the_free_limit_and_gives_the_event()
    {
        var account = Guid.CreateVersion7();
        var key = Guid.CreateVersion7();

        var reserved = await ReserveAsync(account, key, 100);

        Assert.Equal(key, reserved.Value.OperationKey);
        var pool = await database.PoolAsync(account);
        Assert.Equal(100, pool.UsedBytes);
        Assert.Equal(AccountPool.DefaultLimitBytes, pool.LimitBytes);
    }

    [Fact]
    public async Task What_does_not_fit_is_refused_and_nothing_changes()
    {
        var account = await database.AccountWithLimitAsync(100);
        await ReserveAsync(account, Guid.CreateVersion7(), 60);

        var refused = await ReserveAsync(account, Guid.CreateVersion7(), 41);

        Assert.Equal(PoolErrors.PoolFull, refused.Error);
        Assert.Equal(60, (await database.PoolAsync(account)).UsedBytes);
    }

    [Fact]
    public async Task A_refused_operation_can_fit_later_under_the_same_key()
    {
        var account = await database.AccountWithLimitAsync(100);
        var key = Guid.CreateVersion7();

        Assert.True((await ReserveAsync(account, key, 101)).IsFailure);
        Assert.True((await ReserveAsync(account, key, 100)).IsSuccess);
    }

    [Fact]
    public async Task The_same_operation_twice_has_one_effect()
    {
        var account = await database.AccountWithLimitAsync(100);
        var key = Guid.CreateVersion7();

        await ReserveAsync(account, key, 60);
        var again = await ReserveAsync(account, key, 60);

        Assert.True(again.IsSuccess);
        Assert.Equal(60, (await database.PoolAsync(account)).UsedBytes);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public async Task An_operation_takes_something(long bytes)
    {
        var refused = await ReserveAsync(Guid.CreateVersion7(), Guid.CreateVersion7(), bytes);

        Assert.Equal(PoolErrors.InvalidUnits, refused.Error);
    }

    [Fact]
    public async Task Parallel_reservations_never_go_over_the_limit()
    {
        var account = await database.AccountWithLimitAsync(100);

        var results = await Task.WhenAll(Enumerable.Range(0, 5).Select(_ => ReserveAsync(account, Guid.CreateVersion7(), 30)));

        Assert.Equal(3, results.Count(result => result.IsSuccess));
        Assert.Equal(90, (await database.PoolAsync(account)).UsedBytes);
    }

    private async Task<SharedKernel.Results.Result<UnitsReserved>> ReserveAsync(Guid account, Guid key, long bytes)
    {
        await using var scope = database.CreateScope();
        return await scope.ServiceProvider.GetRequiredService<ReserveUnitsHandler>()
            .HandleAsync(new ReserveUnits(account, key, bytes), TestContext.Current.CancellationToken);
    }
}
