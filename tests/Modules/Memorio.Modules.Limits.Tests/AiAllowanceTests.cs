using Memorio.Modules.Limits.Contracts;
using Memorio.Modules.Limits.Domain.Pools;
using Microsoft.Extensions.DependencyInjection;

namespace Memorio.Modules.Limits.Tests;

/// <summary>MEM-E2-LIMITY-03 on a real database.</summary>
public sealed class AiAllowanceTests(LimitsDatabase database)
{
    private static CancellationToken Cancellation => TestContext.Current.CancellationToken;

    [Fact]
    public async Task The_free_plan_has_no_ai()
    {
        Assert.False(await ConsumeAsync(Guid.CreateVersion7(), Guid.CreateVersion7()));
    }

    [Fact]
    public async Task The_allowance_runs_out_and_a_retry_costs_nothing_more()
    {
        var account = await WithAiLimitAsync(2);
        var key = Guid.CreateVersion7();

        Assert.True(await ConsumeAsync(account, key));
        Assert.True(await ConsumeAsync(account, key));
        Assert.True(await ConsumeAsync(account, Guid.CreateVersion7()));
        Assert.False(await ConsumeAsync(account, Guid.CreateVersion7()));
    }

    [Fact]
    public async Task At_once_it_never_goes_over_the_allowance()
    {
        var account = await WithAiLimitAsync(3);

        var results = await Task.WhenAll(Enumerable.Range(0, 6).Select(_ => ConsumeAsync(account, Guid.CreateVersion7())));

        Assert.Equal(3, results.Count(result => result));
    }

    [Fact]
    public async Task A_new_month_starts_from_zero()
    {
        var account = await WithAiLimitAsync(1);
        await using var scope = database.CreateScope();
        var pools = scope.ServiceProvider.GetRequiredService<IPools>();
        var november = new DateTimeOffset(2026, 11, 30, 23, 0, 0, TimeSpan.Zero);

        Assert.True(await pools.ConsumeAiAsync(account, Guid.CreateVersion7(), november, Cancellation));
        Assert.False(await pools.ConsumeAiAsync(account, Guid.CreateVersion7(), november, Cancellation));
        Assert.True(await pools.ConsumeAiAsync(account, Guid.CreateVersion7(), november.AddHours(2), Cancellation));
    }

    private async Task<Guid> WithAiLimitAsync(int limit)
    {
        var account = Guid.CreateVersion7();
        await using var scope = database.CreateScope();
        await scope.ServiceProvider.GetRequiredService<IPools>().SetLimitAsync(account, 100, limit, sequence: 1, Cancellation);
        return account;
    }

    private async Task<bool> ConsumeAsync(Guid account, Guid key)
    {
        await using var scope = database.CreateScope();
        return await scope.ServiceProvider.GetRequiredService<IAiAllowance>().TryConsumeAsync(account, key, Cancellation);
    }
}
