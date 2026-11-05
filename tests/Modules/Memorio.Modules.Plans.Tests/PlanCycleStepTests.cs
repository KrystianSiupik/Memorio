using Memorio.Modules.Plans.Application.Commands.Cycle;
using Memorio.Modules.Plans.Application.Commands.RecordEntitlement;
using Memorio.Modules.Plans.Application.Queries;
using Memorio.Modules.Plans.Contracts;
using Memorio.Modules.Plans.Domain.Cycle;
using Memorio.Modules.Plans.Domain.Entitlements;
using Microsoft.Extensions.DependencyInjection;

namespace Memorio.Modules.Plans.Tests;

/// <summary>The cycle on the real database: the ledger, the outbox and the version (D-018).</summary>
public sealed class PlanCycleStepTests(PlansDatabase database)
{
    private static CancellationToken Cancellation => TestContext.Current.CancellationToken;

    private async Task PayAsync(EntitledResource resource, DateTimeOffset until, DateTimeOffset at)
    {
        await using var scope = database.CreateScope();
        var recorded = await scope.ServiceProvider.GetRequiredService<RecordEntitlementHandler>().HandleAsync(
            new RecordEntitlement("manual", Guid.CreateVersion7().ToString(), resource, until, at, "test"),
            Cancellation);
        Assert.True(recorded.IsSuccess);
    }

    private async Task AdvanceAsync(EntitledResource resource, DateTimeOffset at)
    {
        await using var scope = database.CreateScope();
        Assert.True((await scope.ServiceProvider.GetRequiredService<AdvanceCycleHandler>().HandleAsync(resource, at, Cancellation)).IsSuccess);
    }

    private async Task<PlanCycle?> FindAsync(EntitledResource resource)
    {
        await using var scope = database.CreateScope();
        return await scope.ServiceProvider.GetRequiredService<IPlanCycles>().FindAsync(resource, Cancellation);
    }

    [Fact]
    public async Task A_resource_without_a_ledger_line_gets_no_cycle()
    {
        var resource = PlansDatabase.NewAccount();

        await AdvanceAsync(resource, DateTimeOffset.UtcNow);

        Assert.Null(await FindAsync(resource));
    }

    [Fact]
    public async Task A_payment_activates_and_the_event_reaches_the_queue()
    {
        var resource = PlansDatabase.NewAlbum();
        var now = DateTimeOffset.UtcNow;
        await PayAsync(resource, now.AddMonths(1), now);

        await AdvanceAsync(resource, now);

        Assert.Equal(PlanMode.Active, (await FindAsync(resource))!.Mode);
        Assert.True(await PlansDatabase.EventuallyAsync(async () => await database.QueuedEventsAsync(nameof(PlanActivatedEvent), resource.Id) == 1));
    }

    [Fact]
    public async Task A_second_step_with_nothing_new_changes_nothing()
    {
        var resource = PlansDatabase.NewAlbum();
        var now = DateTimeOffset.UtcNow;
        await PayAsync(resource, now.AddMonths(1), now);

        await AdvanceAsync(resource, now);
        var first = (await FindAsync(resource))!.Sequence;
        await AdvanceAsync(resource, now.AddHours(1));

        Assert.Equal(first, (await FindAsync(resource))!.Sequence);
        Assert.True(await PlansDatabase.EventuallyAsync(async () => await database.QueuedEventsAsync(nameof(PlanActivatedEvent), resource.Id) == 1));
    }

    [Fact]
    public async Task A_notification_arriving_late_does_not_undo_a_newer_one()
    {
        var resource = PlansDatabase.NewAccount();
        var now = DateTimeOffset.UtcNow;
        await PayAsync(resource, now.AddMonths(2), now);
        await AdvanceAsync(resource, now);

        // Happened earlier at the source, arrives later here, and says the plan ended.
        await PayAsync(resource, now.AddDays(-1), now.AddDays(-2));
        await AdvanceAsync(resource, now.AddHours(1));

        Assert.Equal(PlanMode.Active, (await FindAsync(resource))!.Mode);
    }

    [Fact]
    public async Task The_end_of_the_period_goes_read_only_and_stores_the_lists()
    {
        var resource = PlansDatabase.NewAlbum();
        var now = DateTimeOffset.UtcNow;
        await PayAsync(resource, now.AddDays(1), now);
        await AdvanceAsync(resource, now);

        await AdvanceAsync(resource, now.AddDays(2));
        var cycle = (await FindAsync(resource))!;
        Assert.Equal(PlanMode.ReadOnly, cycle.Mode);
        Assert.True(await PlansDatabase.EventuallyAsync(async () => await database.QueuedEventsAsync(nameof(PlanReadOnlyEvent), resource.Id) == 1));

        // The warnings as stored: read back, they stop a second issue.
        var warned = cycle.TrimAt!.Value.AddDays(-30);
        await AdvanceAsync(resource, warned);
        Assert.Equal([90, 30], (await FindAsync(resource))!.WarningsDue);
        await AdvanceAsync(resource, warned.AddHours(1));
        Assert.True(await PlansDatabase.EventuallyAsync(async () => await database.QueuedEventsAsync(nameof(TrimWarningDueEvent), resource.Id) == 2));
    }

    [Fact]
    public async Task The_choice_of_material_is_stored_and_read_back()
    {
        var resource = PlansDatabase.NewAlbum();
        var now = DateTimeOffset.UtcNow.AddMonths(-24).AddDays(-1);

        await PayAsync(resource, now.AddDays(1), now);
        await AdvanceAsync(resource, now);
        await AdvanceAsync(resource, now.AddDays(1).AddHours(1));
        var kept = Guid.CreateVersion7();

        await using var scope = database.CreateScope();
        var answered = await scope.ServiceProvider.GetRequiredService<KeepMaterialHandler>().HandleAsync(new KeepMaterial(resource, [kept]), Cancellation);

        Assert.True(answered.IsSuccess);
        var state = await scope.ServiceProvider.GetRequiredService<IPlanStateQuery>().GetAsync(resource, Cancellation);
        Assert.Equal("ReadOnly", state.Mode);
        Assert.Equal([kept], state.KeptMemoryIds);
        Assert.NotNull(state.TrimAt);
        Assert.Equal(state.TrimAt!.Value.AddDays(-30), state.ChoiceOpensAt);
    }

    [Fact]
    public async Task A_resource_that_never_paid_reads_as_free()
    {
        await using var scope = database.CreateScope();

        var state = await scope.ServiceProvider.GetRequiredService<IPlanStateQuery>().GetAsync(PlansDatabase.NewAccount(), Cancellation);

        Assert.Equal("Free", state.Mode);
        Assert.Null(state.TrimAt);
    }

    [Fact]
    public async Task Refreshing_finds_a_payment_at_once()
    {
        var resource = PlansDatabase.NewAccount();
        var now = DateTimeOffset.UtcNow;
        await PayAsync(resource, now.AddMonths(1), now);

        await using var scope = database.CreateScope();
        Assert.True((await scope.ServiceProvider.GetRequiredService<RefreshSubscriptionHandler>().HandleAsync(resource, Cancellation)).IsSuccess);

        Assert.Equal(PlanMode.Active, (await FindAsync(resource))!.Mode);
    }

    [Fact]
    public async Task Two_steps_at_once_leave_one_activation()
    {
        var resource = PlansDatabase.NewAccount();
        var now = DateTimeOffset.UtcNow;
        await PayAsync(resource, now.AddMonths(1), now);

        async Task<bool> StepAsync()
        {
            await using var scope = database.CreateScope();
            return (await scope.ServiceProvider.GetRequiredService<AdvanceCycleHandler>().HandleAsync(resource, now, Cancellation)).IsSuccess;
        }

        var results = await Task.WhenAll(Enumerable.Range(0, 6).Select(_ => StepAsync()));

        Assert.All(results, Assert.True);
        Assert.Equal(PlanMode.Active, (await FindAsync(resource))!.Mode);
    }

    [Fact]
    public async Task Paying_at_the_moment_of_the_trim_wins()
    {
        var resource = PlansDatabase.NewAlbum();
        var now = DateTimeOffset.UtcNow;
        await PayAsync(resource, now.AddDays(1), now);
        await AdvanceAsync(resource, now);
        await AdvanceAsync(resource, now.AddDays(1).AddHours(1));
        var trimAt = (await FindAsync(resource))!.TrimAt!.Value;

        await PayAsync(resource, trimAt.AddYears(1), trimAt);
        await AdvanceAsync(resource, trimAt);

        Assert.Equal(PlanMode.Active, (await FindAsync(resource))!.Mode);
        Assert.Equal(0, await database.QueuedEventsAsync(nameof(TrimOrderedEvent), resource.Id));
    }

    [Fact]
    public async Task An_ownerless_album_is_activated_after_the_takeover_only_when_paid()
    {
        var album = PlansDatabase.NewAlbum();
        var now = DateTimeOffset.UtcNow;
        await PayAsync(album, now.AddDays(1), now);
        await AdvanceAsync(album, now);

        await using var scope = database.CreateScope();
        var activate = scope.ServiceProvider.GetRequiredService<ActivateAfterTakeoverHandler>();
        Assert.True((await scope.ServiceProvider.GetRequiredService<MarkAlbumOwnerlessHandler>().HandleAsync(album.Id, Cancellation)).IsSuccess);
        Assert.True((await activate.HandleAsync(album.Id, Cancellation)).IsSuccess);
        Assert.False((await FindAsync(album))!.IsOwnerless);
        Assert.Equal(PlanCycleErrors.NotPaid, (await activate.HandleAsync(Guid.CreateVersion7(), Cancellation)).Error);
    }
}
