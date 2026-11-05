using Memorio.Modules.Album.Application.Commands.CreateAlbum;
using Memorio.Modules.Album.Domain.Albums;
using Microsoft.Extensions.DependencyInjection;

namespace Memorio.Modules.Album.Tests;

/// <summary>D-278: what a save publishes reaches the queue, also the second one in a scope and after a lost race.</summary>
public sealed class OutboxPerSaveTests(AlbumDatabase database)
{
    private static CancellationToken Cancellation => TestContext.Current.CancellationToken;

    private static string NewAddress() => $"{Guid.NewGuid()}@example.invalid";

    /// <summary>The failed attempt's event goes out too, as a duplicate: handlers take it (SD6), and it is never lost.</summary>
    [Fact]
    public async Task A_retry_after_a_lost_race_still_publishes_its_event()
    {
        var owner = database.Neighbours.NewAccount();
        var id = await database.NewAlbumAsync(owner);
        var invitation = Guid.CreateVersion7();
        await using var scope = database.CreateScope();
        var albums = scope.ServiceProvider.GetRequiredService<IAlbums>();
        var album = (await albums.FindAsync(id, Cancellation))!;

        await database.InviteAsync(id, owner, NewAddress());

        album.Invite(invitation, owner, NewAddress(), 10, DateTimeOffset.UtcNow);
        Assert.False(await albums.SaveAsync(Cancellation));

        var fresh = (await albums.FindAsync(id, Cancellation))!;
        fresh.Invite(invitation, owner, NewAddress(), 10, DateTimeOffset.UtcNow);
        Assert.True(await albums.SaveAsync(Cancellation));

        Assert.True(await AlbumDatabase.EventuallyAsync(async () => await database.QueuedEventsAsync("InvitationSentEvent", invitation) >= 1));
    }

    [Fact]
    public async Task A_failed_save_alone_publishes_nothing()
    {
        var owner = database.Neighbours.NewAccount();
        var id = await database.NewAlbumAsync(owner);
        var invitation = Guid.CreateVersion7();
        await using (var scope = database.CreateScope())
        {
            var albums = scope.ServiceProvider.GetRequiredService<IAlbums>();
            var album = (await albums.FindAsync(id, Cancellation))!;
            await database.InviteAsync(id, owner, NewAddress());

            album.Invite(invitation, owner, NewAddress(), 10, DateTimeOffset.UtcNow);
            Assert.False(await albums.SaveAsync(Cancellation));
        }

        await Task.Delay(500, Cancellation);
        Assert.Equal(0, await database.QueuedEventsAsync("InvitationSentEvent", invitation));
    }

    [Fact]
    public async Task A_second_save_in_a_scope_publishes_its_event_too()
    {
        var owner = database.Neighbours.NewAccount();
        var id = await database.NewAlbumAsync(owner);
        var (first, second) = (Guid.CreateVersion7(), Guid.CreateVersion7());
        await using var scope = database.CreateScope();
        var albums = scope.ServiceProvider.GetRequiredService<IAlbums>();
        var album = (await albums.FindAsync(id, Cancellation))!;

        album.Invite(first, owner, NewAddress(), 10, DateTimeOffset.UtcNow);
        Assert.True(await albums.SaveAsync(Cancellation));
        album.Invite(second, owner, NewAddress(), 10, DateTimeOffset.UtcNow);
        Assert.True(await albums.SaveAsync(Cancellation));

        Assert.True(await AlbumDatabase.EventuallyAsync(async () => await database.QueuedEventsAsync("InvitationSentEvent", second) == 1));
        Assert.Equal(1, await database.QueuedEventsAsync("InvitationSentEvent", first));
    }

    [Fact]
    public async Task The_same_album_created_twice_at_once_is_one_album_for_both()
    {
        var owner = database.Neighbours.NewAccount();
        var id = Guid.CreateVersion7();

        async Task<bool> CreateAsync()
        {
            await using var scope = database.CreateScope();
            return (await scope.ServiceProvider.GetRequiredService<CreateAlbumHandler>().HandleAsync(new CreateAlbum(id, owner, "Rodzina"), Cancellation)).IsSuccess;
        }

        var results = await Task.WhenAll(Enumerable.Range(0, 6).Select(_ => CreateAsync()));

        Assert.All(results, Assert.True);
    }
}
