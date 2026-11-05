using Memorio.Modules.Access.Contracts;
using Memorio.Modules.Album.Application.Policies;
using Memorio.Modules.Album.Domain.Albums;
using Microsoft.Extensions.DependencyInjection;

namespace Memorio.Modules.Album.Tests;

/// <summary>SD6, D-270: E67 delivered twice, on a real database.</summary>
public sealed class UserBlockedHandlerIdempotencyTests(AlbumDatabase database)
{
    [Fact]
    public async Task A_blocked_participant_loses_the_seat_once_and_what_they_brought_stays()
    {
        var (album, _, person) = await database.AlbumWithParticipantAsync();
        await database.BringMemoryAsync(album, person);
        var blocked = new UserBlockedEvent(Guid.CreateVersion7(), DateTimeOffset.UtcNow, album.Value, person, 2);

        await DeliverAsync(blocked);
        await DeliverAsync(blocked);

        var view = await database.ViewAsync(album);
        Assert.DoesNotContain(view.Members, member => member.AccountId == person);
        Assert.Single(view.Memories);
        Assert.True(await AlbumDatabase.EventuallyAsync(async () => await database.QueuedEventsAsync("ParticipantRemovedEvent", person) == 1));
    }

    [Fact]
    public async Task Someone_not_in_the_album_and_an_unknown_album_change_nothing()
    {
        var (album, owner, person) = await database.AlbumWithParticipantAsync();

        await DeliverAsync(new UserBlockedEvent(Guid.CreateVersion7(), DateTimeOffset.UtcNow, album.Value, Guid.CreateVersion7(), 2));
        await DeliverAsync(new UserBlockedEvent(Guid.CreateVersion7(), DateTimeOffset.UtcNow, Guid.CreateVersion7(), person, 2));
        await DeliverAsync(new UserBlockedEvent(Guid.CreateVersion7(), DateTimeOffset.UtcNow, album.Value, owner, 2));

        Assert.Equal(2, (await database.ViewAsync(album)).Members.Count);
    }

    [Fact]
    public async Task A_blocked_person_cannot_accept_an_invitation_that_still_waits()
    {
        var owner = database.Neighbours.NewAccount();
        var person = database.Neighbours.NewAccount();
        var album = await database.NewAlbumAsync(owner);
        var invitation = Guid.CreateVersion7();
        await database.InviteAsync(album, owner, database.Neighbours.EmailOf(person), invitation);
        database.Neighbours.BlockedIn(album.Value, person);

        Assert.Equal(AlbumErrors.NoAccess, (await database.AnswerAsync(invitation, person, accepted: true)).Error);
        Assert.True((await database.AnswerAsync(invitation, person, accepted: false)).IsSuccess);
    }

    private async Task DeliverAsync(UserBlockedEvent blocked)
    {
        await using var scope = database.CreateScope();
        await scope.ServiceProvider.GetRequiredService<RemoveBlockedParticipant>().HandleAsync(blocked, TestContext.Current.CancellationToken);
    }
}
