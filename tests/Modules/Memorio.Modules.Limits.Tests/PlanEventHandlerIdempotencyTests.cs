using Memorio.Modules.Limits.Application.Commands.SetPoolLimit;
using Memorio.Modules.Limits.Application.Policies;
using Memorio.Modules.Limits.Contracts;
using Memorio.Modules.Plans.Contracts;
using Memorio.SharedKernel.Messaging;
using Microsoft.Extensions.DependencyInjection;

namespace Memorio.Modules.Limits.Tests;

/// <summary>SD6: E94 delivered twice, and a late event not undoing a newer limit (D-016), on a real database.</summary>
public sealed class PlanActivatedHandlerIdempotencyTests(LimitsDatabase database)
{
    private static CancellationToken Cancellation => TestContext.Current.CancellationToken;

    [Fact]
    public async Task The_paid_limit_is_set_once_and_a_late_read_only_does_not_undo_it()
    {
        var pool = Guid.CreateVersion7();
        const long paid = 50L * 1024 * 1024 * 1024;
        await using var scope = database.CreateScope();
        var policy = scope.ServiceProvider.GetRequiredService<SetLimitFromPlan>();
        var activated = new PlanActivatedEvent(Guid.CreateVersion7(), DateTimeOffset.UtcNow, pool, false, paid, 20, 5);

        await policy.HandleAsync(activated, Cancellation);
        await policy.HandleAsync(activated, Cancellation);
        await policy.HandleAsync(new PlanReadOnlyEvent(Guid.CreateVersion7(), DateTimeOffset.UtcNow, pool, false, 1, 0, 4), Cancellation);

        var standing = await scope.ServiceProvider.GetRequiredService<IPoolState>().GetAsync(pool, Cancellation);
        Assert.Equal(paid, standing.LimitBytes);
        Assert.Equal(20, standing.AiLimit);
    }

    [Fact]
    public async Task With_the_limit_set_the_space_and_the_ai_really_are_there()
    {
        var pool = Guid.CreateVersion7();
        const long free = 2L * 1024 * 1024 * 1024;
        await using var scope = database.CreateScope();
        var services = scope.ServiceProvider;
        Assert.False(await services.GetRequiredService<ISpacePool>().TryReserveAsync(pool, Guid.CreateVersion7(), free + 1, Cancellation));
        Assert.False(await services.GetRequiredService<IAiAllowance>().TryConsumeAsync(pool, Guid.CreateVersion7(), Cancellation));

        await services.GetRequiredService<SetLimitFromPlan>().HandleAsync(
            new PlanActivatedEvent(Guid.CreateVersion7(), DateTimeOffset.UtcNow, pool, false, 10 * free, 1, 2),
            Cancellation);

        Assert.True(await services.GetRequiredService<ISpacePool>().TryReserveAsync(pool, Guid.CreateVersion7(), free + 1, Cancellation));
        Assert.True(await services.GetRequiredService<IAiAllowance>().TryConsumeAsync(pool, Guid.CreateVersion7(), Cancellation));
        var standing = await services.GetRequiredService<IPoolState>().GetAsync(pool, Cancellation);
        Assert.Equal(free + 1, standing.UsedBytes);
        Assert.Equal(1, standing.AiUsed);
    }
}

/// <summary>SD6: E95 delivered twice. A lowered limit takes nothing away, and nothing new fits (S2:B1).</summary>
public sealed class PlanReadOnlyHandlerIdempotencyTests(LimitsDatabase database)
{
    private static CancellationToken Cancellation => TestContext.Current.CancellationToken;

    [Fact]
    public async Task What_is_used_stays_and_nothing_new_fits()
    {
        var pool = Guid.CreateVersion7();
        await using var scope = database.CreateScope();
        var services = scope.ServiceProvider;
        var albums = services.GetRequiredService<IAlbumPool>();
        Assert.True(await albums.TryReserveAsync(pool, Guid.CreateVersion7(), 1000, Cancellation));
        var readOnly = new PlanReadOnlyEvent(Guid.CreateVersion7(), DateTimeOffset.UtcNow, pool, true, 0, 0, 2);

        await services.GetRequiredService<SetLimitFromPlan>().HandleAsync(readOnly, Cancellation);
        await services.GetRequiredService<SetLimitFromPlan>().HandleAsync(readOnly, Cancellation);

        var state = await albums.GetStateAsync(pool, Cancellation);
        Assert.Equal(1000, state.UsedBytes);
        Assert.Equal(0, state.LimitBytes);
        Assert.False(await albums.TryReserveAsync(pool, Guid.CreateVersion7(), 1, Cancellation));
    }

    [Fact]
    public async Task What_comes_back_to_an_album_counts_over_the_limit_once()
    {
        var pool = Guid.CreateVersion7();
        var key = Guid.CreateVersion7();
        await using var scope = database.CreateScope();
        var services = scope.ServiceProvider;
        await services.GetRequiredService<SetLimitFromPlan>().HandleAsync(new PlanReadOnlyEvent(Guid.CreateVersion7(), DateTimeOffset.UtcNow, pool, true, 0, 0, 2), Cancellation);
        var albums = services.GetRequiredService<IAlbumPool>();

        await albums.ReserveOverLimitAsync(pool, key, 500, Cancellation);
        await albums.ReserveOverLimitAsync(pool, key, 500, Cancellation);

        Assert.Equal(500, (await albums.GetStateAsync(pool, Cancellation)).UsedBytes);
    }

    [Fact]
    public async Task The_command_says_when_it_changed_nothing()
    {
        var pool = Guid.CreateVersion7();
        await using var scope = database.CreateScope();
        var handler = scope.ServiceProvider.GetRequiredService<SetPoolLimitHandler>();

        Assert.NotNull(await handler.HandleAsync(new SetPoolLimit(pool, 10, 0, 3), Cancellation));
        Assert.Null(await handler.HandleAsync(new SetPoolLimit(pool, 20, 0, 3), Cancellation));
        Assert.Null(await handler.HandleAsync(new SetPoolLimit(pool, 20, 0, 2), Cancellation));
        Assert.NotNull(await handler.HandleAsync(new SetPoolLimit(pool, 20, 0, 4), Cancellation));
    }

    [Fact]
    public async Task Two_limits_at_once_leave_the_newer()
    {
        var pool = Guid.CreateVersion7();

        async Task SetAsync(long sequence)
        {
            await using var scope = database.CreateScope();
            await scope.ServiceProvider.GetRequiredService<SetPoolLimitHandler>().HandleAsync(new SetPoolLimit(pool, sequence, 0, sequence), Cancellation);
        }

        await Task.WhenAll(Enumerable.Range(1, 10).Select(sequence => SetAsync(sequence)));

        await using var check = database.CreateScope();
        Assert.Equal(10, (await check.ServiceProvider.GetRequiredService<IPoolState>().GetAsync(pool, Cancellation)).LimitBytes);
    }
}

/// <summary>SD6: E103 delivered twice: the free limit applies once.</summary>
public sealed class TrimOrderedHandlerIdempotencyTests(LimitsDatabase database)
{
    private static CancellationToken Cancellation => TestContext.Current.CancellationToken;

    [Fact]
    public async Task The_pool_is_back_on_the_free_limit()
    {
        var pool = Guid.CreateVersion7();
        const long free = 2L * 1024 * 1024 * 1024;
        await using var scope = database.CreateScope();
        var policy = scope.ServiceProvider.GetRequiredService<SetLimitFromPlan>();
        await policy.HandleAsync(new PlanReadOnlyEvent(Guid.CreateVersion7(), DateTimeOffset.UtcNow, pool, true, 0, 0, 2), Cancellation);
        var trim = new TrimOrderedEvent(Guid.CreateVersion7(), DateTimeOffset.UtcNow, pool, true, free, 0, [], 9);

        await policy.HandleAsync(trim, Cancellation);
        await policy.HandleAsync(trim, Cancellation);

        Assert.Equal(free, (await scope.ServiceProvider.GetRequiredService<IPoolState>().GetAsync(pool, Cancellation)).LimitBytes);
    }
}
