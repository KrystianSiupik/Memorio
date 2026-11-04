using Memorio.Modules.Album.Contracts;
using Memorio.Modules.Notifications.Application.Policies;
using Microsoft.Extensions.DependencyInjection;

namespace Memorio.Modules.Notifications.Tests;

/// <summary>SD6, MEM-E3-POWIADOMIENIA-03 and -08: E51 delivered twice on a real database.</summary>
public sealed class AlbumPoolExhaustedHandlerIdempotencyTests(NotificationsDatabase database)
{
    private static CancellationToken Cancellation => TestContext.Current.CancellationToken;

    [Fact]
    public async Task The_owner_hears_once_a_day_however_often_it_does_not_fit()
    {
        var owner = database.Neighbours.NewAccount();
        var album = Guid.CreateVersion7();
        var today = new DateTimeOffset(2026, 11, 4, 12, 0, 0, TimeSpan.Zero);

        await DeliverAsync(new AlbumPoolExhaustedEvent(Guid.CreateVersion7(), today, album, owner, 3));
        await DeliverAsync(new AlbumPoolExhaustedEvent(Guid.CreateVersion7(), today.AddHours(1), album, owner, 3));
        Assert.Equal(1, database.Neighbours.SentTo(database.Neighbours.EmailOf(owner), "PoolFull"));

        await DeliverAsync(new AlbumPoolExhaustedEvent(Guid.CreateVersion7(), today.AddDays(1), album, owner, 3));
        Assert.Equal(2, database.Neighbours.SentTo(database.Neighbours.EmailOf(owner), "PoolFull"));
    }

    private async Task DeliverAsync(AlbumPoolExhaustedEvent full)
    {
        await using var scope = database.CreateScope();
        await scope.ServiceProvider.GetRequiredService<NotifyOwnerOfFullPool>().HandleAsync(full, Cancellation);
    }
}
