using Memorio.Modules.Album.Contracts;
using Memorio.Modules.Plans.Application.Commands.Cycle;
using Memorio.Modules.Plans.Application.Commands.RecordEntitlement;
using Memorio.Modules.Plans.Application.Policies;
using Memorio.Modules.Plans.Contracts;
using Memorio.Modules.Plans.Domain.Cycle;
using Memorio.Modules.Plans.Domain.Entitlements;
using Microsoft.Extensions.DependencyInjection;

namespace Memorio.Modules.Plans.Tests;

/// <summary>SD6 and P36: E162 delivered twice, on a real database.</summary>
public sealed class AlbumWithoutOwnerHandlerIdempotencyTests(PlansDatabase database)
{
    private static CancellationToken Cancellation => TestContext.Current.CancellationToken;

    [Fact]
    public async Task An_album_without_a_plan_goes_read_only_once_and_says_so()
    {
        var album = Guid.CreateVersion7();
        var lost = new AlbumWithoutOwnerEvent(Guid.CreateVersion7(), DateTimeOffset.UtcNow, album, 3);

        for (var delivery = 0; delivery < 2; delivery++)
        {
            await using var scope = database.CreateScope();
            await scope.ServiceProvider.GetRequiredService<MarkOwnerlessWhenAlbumLostOwner>().HandleAsync(lost, Cancellation);
        }

        var cycle = await database.CycleAsync(new EntitledResource(EntitledResourceKind.Album, album));
        Assert.True(cycle.IsOwnerless);
        Assert.Equal(PlanMode.ReadOnly, cycle.Mode);
        Assert.True(await PlansDatabase.EventuallyAsync(async () => await database.QueuedEventsAsync(nameof(PlanReadOnlyEvent), album) == 1));
    }
}

/// <summary>SD6 and P41: E167 delivered twice: paid by the person who took it over, the plan is active.</summary>
public sealed class AlbumTakenOverHandlerIdempotencyTests(PlansDatabase database)
{
    private static CancellationToken Cancellation => TestContext.Current.CancellationToken;

    [Fact]
    public async Task The_whole_way_from_a_reservation_paid_to_an_active_plan()
    {
        var album = PlansDatabase.NewAlbum();
        var now = DateTimeOffset.UtcNow;
        await using (var scope = database.CreateScope())
        {
            await scope.ServiceProvider.GetRequiredService<MarkOwnerlessWhenAlbumLostOwner>().HandleAsync(new AlbumWithoutOwnerEvent(Guid.CreateVersion7(), now, album.Id, 3), Cancellation);
            await scope.ServiceProvider.GetRequiredService<RecordEntitlementHandler>().HandleAsync(new RecordEntitlement("manual", Guid.CreateVersion7().ToString(), album, now.AddMonths(1), now, "test"), Cancellation);
        }

        await using (var scope = database.CreateScope())
        {
            await scope.ServiceProvider.GetRequiredService<AdvanceCycleHandler>().HandleAsync(album, now, Cancellation);
        }

        Assert.Equal(PlanMode.ReadOnly, (await database.CycleAsync(album)).Mode);
        Assert.True(await PlansDatabase.EventuallyAsync(async () => await database.QueuedEventsAsync(nameof(PaidWhileOwnerlessEvent), album.Id) == 1));

        var taken = new AlbumTakenOverEvent(Guid.CreateVersion7(), now, album.Id, Guid.CreateVersion7(), 4);
        for (var delivery = 0; delivery < 2; delivery++)
        {
            await using var scope = database.CreateScope();
            await scope.ServiceProvider.GetRequiredService<ActivateWhenAlbumTakenOver>().HandleAsync(taken, Cancellation);
        }

        var cycle = await database.CycleAsync(album);
        Assert.Equal(PlanMode.Active, cycle.Mode);
        Assert.False(cycle.IsOwnerless);
        Assert.True(await PlansDatabase.EventuallyAsync(async () => await database.QueuedEventsAsync(nameof(PlanActivatedEvent), album.Id) == 1));
    }

    [Fact]
    public async Task Without_a_payment_nothing_happens_and_nothing_fails()
    {
        await using var scope = database.CreateScope();
        await scope.ServiceProvider.GetRequiredService<ActivateWhenAlbumTakenOver>().HandleAsync(new AlbumTakenOverEvent(Guid.CreateVersion7(), DateTimeOffset.UtcNow, Guid.CreateVersion7(), Guid.CreateVersion7(), 4), Cancellation);
    }
}
