using Memorio.Modules.Plans.Application.Commands.Cycle;
using Memorio.Modules.Plans.Application.Commands.RecordEntitlement;
using Memorio.Modules.Plans.Contracts;
using Microsoft.Extensions.DependencyInjection;

namespace Memorio.Modules.Plans.Tests;

/// <summary>MEM-E3-PLANY-01, D-261, D-080: what an album's plan allows at each stage.</summary>
public sealed class AlbumSeatLimitTests(PlansDatabase database)
{
    private static CancellationToken Cancellation => TestContext.Current.CancellationToken;

    [Fact]
    public async Task An_album_without_a_plan_has_five_seats_and_takes_new_things()
    {
        await using var scope = database.CreateScope();
        var limit = scope.ServiceProvider.GetRequiredService<IAlbumSeatLimit>();
        var album = Guid.CreateVersion7();

        Assert.Equal(5, await limit.SeatsOfAsync(album, Cancellation));
        Assert.False(await limit.IsReadOnlyAsync(album, Cancellation));
    }

    [Fact]
    public async Task A_paid_album_has_ten_and_a_read_only_one_no_new_seat()
    {
        var album = PlansDatabase.NewAlbum();
        var now = DateTimeOffset.UtcNow;
        await using var scope = database.CreateScope();
        var services = scope.ServiceProvider;
        var limit = services.GetRequiredService<IAlbumSeatLimit>();
        var advance = services.GetRequiredService<AdvanceCycleHandler>();
        await services.GetRequiredService<RecordEntitlementHandler>().HandleAsync(new RecordEntitlement("manual", "paid", album, now.AddDays(1), now, "test"), Cancellation);

        await advance.HandleAsync(album, now, Cancellation);
        Assert.Equal(10, await limit.SeatsOfAsync(album.Id, Cancellation));
        Assert.False(await limit.IsReadOnlyAsync(album.Id, Cancellation));

        await advance.HandleAsync(album, now.AddDays(2), Cancellation);
        Assert.Equal(0, await limit.SeatsOfAsync(album.Id, Cancellation));
        Assert.True(await limit.IsReadOnlyAsync(album.Id, Cancellation));
    }
}
