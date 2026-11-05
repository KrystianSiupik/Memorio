using Memorio.Modules.Notifications.Contracts;
using Memorio.Modules.Plans.Application.Commands.Cycle;
using Memorio.Modules.Plans.Application.Commands.RecordEntitlement;
using Memorio.Modules.Plans.Application.Policies;
using Memorio.Modules.Plans.Contracts;
using Memorio.Modules.Plans.Domain.Cycle;
using Memorio.Modules.Plans.Domain.Entitlements;
using Microsoft.Extensions.DependencyInjection;

namespace Memorio.Modules.Plans.Tests;

internal static class WarningSteps
{
    private static CancellationToken Cancellation => TestContext.Current.CancellationToken;

    /// <summary>A cycle in the last days before its trim, with every warning issued.</summary>
    public static async Task<PlanCycle> WarnedCycleAsync(this PlansDatabase database, EntitledResource resource)
    {
        var now = DateTimeOffset.UtcNow;
        await using var scope = database.CreateScope();
        var advance = scope.ServiceProvider.GetRequiredService<AdvanceCycleHandler>();
        await scope.ServiceProvider.GetRequiredService<RecordEntitlementHandler>().HandleAsync(new RecordEntitlement("manual", Guid.CreateVersion7().ToString(), resource, now.AddDays(1), now, "test"), Cancellation);
        await advance.HandleAsync(resource, now, Cancellation);
        await advance.HandleAsync(resource, now.AddDays(2), Cancellation);
        await using var read = database.CreateScope();
        var trimAt = (await read.ServiceProvider.GetRequiredService<IPlanCycles>().FindAsync(resource, Cancellation))!.TrimAt!.Value;
        await using var last = database.CreateScope();
        await last.ServiceProvider.GetRequiredService<AdvanceCycleHandler>().HandleAsync(resource, trimAt.AddHours(-1), Cancellation);
        return await database.CycleAsync(resource);
    }

    public static async Task<PlanCycle> CycleAsync(this PlansDatabase database, EntitledResource resource)
    {
        await using var scope = database.CreateScope();
        return (await scope.ServiceProvider.GetRequiredService<IPlanCycles>().FindAsync(resource, Cancellation))!;
    }
}

/// <summary>SD6: E98 delivered twice, on a real database.</summary>
public sealed class TrimWarningDeliveredHandlerIdempotencyTests(PlansDatabase database)
{
    private static CancellationToken Cancellation => TestContext.Current.CancellationToken;

    private async Task DeliverAsync(TrimWarningDeliveredEvent report)
    {
        await using var scope = database.CreateScope();
        await scope.ServiceProvider.GetRequiredService<RecordWarningDelivered>().HandleAsync(report, Cancellation);
    }

    [Fact]
    public async Task All_four_proofs_let_the_trim_be_ordered_and_a_repeat_counts_once()
    {
        var resource = PlansDatabase.NewAlbum();
        var cycle = await database.WarnedCycleAsync(resource);
        var trimAt = cycle.TrimAt!.Value;
        var first = new TrimWarningDeliveredEvent(Guid.CreateVersion7(), DateTimeOffset.UtcNow, resource.Id, true, 90, cycle.WarningRound);

        await DeliverAsync(first);
        var sequence = (await database.CycleAsync(resource)).Sequence;
        await DeliverAsync(first);
        Assert.Equal(sequence, (await database.CycleAsync(resource)).Sequence);

        foreach (var days in new[] { 30, 7, 1 })
        {
            await DeliverAsync(new TrimWarningDeliveredEvent(Guid.CreateVersion7(), DateTimeOffset.UtcNow, resource.Id, true, days, cycle.WarningRound));
        }

        await using var scope = database.CreateScope();
        await scope.ServiceProvider.GetRequiredService<AdvanceCycleHandler>().HandleAsync(resource, trimAt, Cancellation);

        Assert.Equal(PlanMode.Trimmed, (await database.CycleAsync(resource)).Mode);
        Assert.True(await PlansDatabase.EventuallyAsync(async () => await database.QueuedEventsAsync(nameof(TrimOrderedEvent), resource.Id) == 1));
    }

    [Fact]
    public async Task A_report_of_another_round_or_an_unknown_resource_changes_nothing()
    {
        var resource = PlansDatabase.NewAccount();
        var cycle = await database.WarnedCycleAsync(resource);

        await DeliverAsync(new TrimWarningDeliveredEvent(Guid.CreateVersion7(), DateTimeOffset.UtcNow, resource.Id, false, 90, cycle.WarningRound - 1));
        await DeliverAsync(new TrimWarningDeliveredEvent(Guid.CreateVersion7(), DateTimeOffset.UtcNow, Guid.CreateVersion7(), false, 90, 1));

        Assert.Empty((await database.CycleAsync(resource)).WarningsDelivered);
    }
}

/// <summary>SD6 and P16: E99 delivered twice holds once; a paid plan is not held.</summary>
public sealed class TrimWarningUndeliveredHandlerIdempotencyTests(PlansDatabase database)
{
    private static CancellationToken Cancellation => TestContext.Current.CancellationToken;

    private async Task HoldAsync(TrimWarningUndeliveredEvent report)
    {
        await using var scope = database.CreateScope();
        await scope.ServiceProvider.GetRequiredService<HoldTrimWhenUndelivered>().HandleAsync(report, Cancellation);
    }

    [Fact]
    public async Task The_trim_waits_however_late_it_gets_until_the_warning_arrives()
    {
        var resource = PlansDatabase.NewAlbum();
        var cycle = await database.WarnedCycleAsync(resource);
        var trimAt = cycle.TrimAt!.Value;
        var refused = new TrimWarningUndeliveredEvent(Guid.CreateVersion7(), DateTimeOffset.UtcNow, resource.Id, true, 30, cycle.WarningRound);

        await HoldAsync(refused);
        await HoldAsync(refused);
        Assert.True(await PlansDatabase.EventuallyAsync(async () => await database.QueuedEventsAsync(nameof(TrimHeldEvent), resource.Id) == 1));

        await using (var scope = database.CreateScope())
        {
            await scope.ServiceProvider.GetRequiredService<AdvanceCycleHandler>().HandleAsync(resource, trimAt.AddYears(3), Cancellation);
        }

        Assert.Equal(PlanMode.ReadOnly, (await database.CycleAsync(resource)).Mode);
        Assert.Equal(0, await database.QueuedEventsAsync(nameof(TrimOrderedEvent), resource.Id));
    }

    [Fact]
    public async Task Delivery_of_one_goes_on_with_the_next_that_did_not_arrive()
    {
        var resource = PlansDatabase.NewAlbum();
        var cycle = await database.WarnedCycleAsync(resource);
        await HoldAsync(new TrimWarningUndeliveredEvent(Guid.CreateVersion7(), DateTimeOffset.UtcNow, resource.Id, true, 30, cycle.WarningRound));

        await using var scope = database.CreateScope();
        await scope.ServiceProvider.GetRequiredService<RecordWarningDelivered>().HandleAsync(
            new TrimWarningDeliveredEvent(Guid.CreateVersion7(), DateTimeOffset.UtcNow, resource.Id, true, 30, cycle.WarningRound),
            Cancellation);

        Assert.True((await database.CycleAsync(resource)).IsHeld);
        Assert.True(await PlansDatabase.EventuallyAsync(async () => await database.QueuedEventsAsync(nameof(TrimHeldEvent), resource.Id) == 2));
    }
}
