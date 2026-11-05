using Memorio.Modules.Plans.Application.Commands.RecordEntitlement;
using Memorio.Modules.Plans.Contracts;
using Memorio.Modules.Plans.Domain.Cycle;
using Memorio.Modules.Plans.Domain.Entitlements;
using Memorio.Modules.Plans.Infrastructure.Scheduling;
using Memorio.SharedKernel.Scheduling;
using Microsoft.Extensions.DependencyInjection;

namespace Memorio.Modules.Plans.Tests;

/// <summary>A firing moves every cycle in the database, also 24 months ahead: not next to tests of other cycles.</summary>
[CollectionDefinition(nameof(HourlyStep), DisableParallelization = true)]
public sealed class HourlyStep;

/// <summary>SD12: the same firing twice, on a real database; also what the hourly step finds.</summary>
[Collection(nameof(HourlyStep))]
public sealed class PlanCycleJobIdempotencyTests(PlansDatabase database)
{
    private static CancellationToken Cancellation => TestContext.Current.CancellationToken;

    private async Task RunAsync(DateTimeOffset at)
    {
        await using var scope = database.CreateScope();
        await ActivatorUtilities.CreateInstance<PlanCycleJob>(scope.ServiceProvider).RunAsync(
            new ScheduledJobRun(PlanCycleJob.JobName, null, at, new Dictionary<string, string>(), 1),
            Cancellation);
    }

    private async Task RecordAsync(EntitledResource resource, DateTimeOffset until, DateTimeOffset at)
    {
        await using var scope = database.CreateScope();
        Assert.True((await scope.ServiceProvider.GetRequiredService<RecordEntitlementHandler>().HandleAsync(
            new RecordEntitlement("manual", Guid.CreateVersion7().ToString(), resource, until, at, "test"),
            Cancellation)).IsSuccess);
    }

    private async Task<PlanCycle> CycleAsync(EntitledResource resource)
    {
        await using var scope = database.CreateScope();
        return (await scope.ServiceProvider.GetRequiredService<IPlanCycles>().FindAsync(resource, Cancellation))!;
    }

    [Fact]
    public async Task A_line_written_by_hand_is_picked_up_and_the_same_firing_changes_nothing_more()
    {
        var resource = PlansDatabase.NewAccount();
        var now = DateTimeOffset.UtcNow;
        await RecordAsync(resource, now.AddMonths(1), now);

        await RunAsync(now.AddMinutes(1));
        var first = await CycleAsync(resource);
        await RunAsync(now.AddMinutes(1));

        Assert.Equal(PlanMode.Active, first.Mode);
        Assert.Equal(first.Sequence, (await CycleAsync(resource)).Sequence);
        Assert.True(await PlansDatabase.EventuallyAsync(async () => await database.QueuedEventsAsync(nameof(PlanActivatedEvent), resource.Id) == 1));
    }

    [Fact]
    public async Task An_ended_period_goes_read_only_once()
    {
        var resource = PlansDatabase.NewAlbum();
        var now = DateTimeOffset.UtcNow;
        await RecordAsync(resource, now.AddDays(1), now);
        await RunAsync(now.AddMinutes(1));

        var later = now.AddDays(2);
        await RunAsync(later);
        await RunAsync(later);

        Assert.Equal(PlanMode.ReadOnly, (await CycleAsync(resource)).Mode);
        Assert.True(await PlansDatabase.EventuallyAsync(async () => await database.QueuedEventsAsync(nameof(PlanReadOnlyEvent), resource.Id) == 1));
    }

    [Fact]
    public async Task Without_delivered_warnings_the_firing_at_the_trim_only_holds_and_only_once()
    {
        var resource = PlansDatabase.NewAlbum();
        var now = DateTimeOffset.UtcNow;
        await RecordAsync(resource, now.AddDays(1), now);
        await RunAsync(now.AddMinutes(1));
        await RunAsync(now.AddDays(2));
        var trimAt = (await CycleAsync(resource)).TrimAt!.Value;

        await RunAsync(trimAt);
        await RunAsync(trimAt);

        var cycle = await CycleAsync(resource);
        Assert.Equal(PlanMode.ReadOnly, cycle.Mode);
        Assert.True(cycle.IsHeld);
        Assert.True(await PlansDatabase.EventuallyAsync(async () => await database.QueuedEventsAsync(nameof(TrimHeldEvent), resource.Id) == 1));
        Assert.Equal(0, await database.QueuedEventsAsync(nameof(TrimOrderedEvent), resource.Id));
        Assert.True(await PlansDatabase.EventuallyAsync(async () => await database.QueuedEventsAsync(nameof(TrimWarningDueEvent), resource.Id) == 4));
    }

    [Fact]
    public async Task A_cycle_with_nothing_due_is_not_touched()
    {
        var resource = PlansDatabase.NewAlbum();
        var now = DateTimeOffset.UtcNow;
        await RecordAsync(resource, now.AddYears(1), now);
        await RunAsync(now.AddMinutes(1));

        await using var scope = database.CreateScope();
        var due = await scope.ServiceProvider.GetRequiredService<IPlanCycles>().FindDueAsync(now.AddDays(1), Cancellation);

        Assert.DoesNotContain(resource, due);
    }
}
