using Memorio.Modules.Account.Contracts;
using Memorio.Modules.Album.Application.Policies;
using Memorio.Modules.Limits.Contracts;
using Microsoft.Extensions.DependencyInjection;

namespace Memorio.Modules.Album.Tests;

/// <summary>SD6, MEM-E3-ALBUM-12 and -19: E7 delivered twice, and late, on a real database.</summary>
public sealed class AccountDeletedHandlerIdempotencyTests(AlbumDatabase database)
{
    private static CancellationToken Cancellation => TestContext.Current.CancellationToken;

    [Fact]
    public async Task The_person_leaves_other_peoples_albums_once_and_their_own_album_stays()
    {
        var (album, owner, person) = await database.AlbumWithParticipantAsync();
        var memory = await database.BringMemoryAsync(album, person);
        var own = await database.NewAlbumAsync(person);
        var deleted = new AccountDeleted(Guid.CreateVersion7(), DateTimeOffset.UtcNow, person, 2);

        await DeliverAsync(deleted);
        await DeliverAsync(deleted);

        var view = await database.ViewAsync(album);
        Assert.DoesNotContain(view.Members, member => member.AccountId == person);
        Assert.Empty(view.Memories);
        Assert.False(database.Neighbours.IsLinked(memory, album.Value));
        Assert.False(database.Neighbours.Reserved.ContainsKey(PoolKeys.For(album.Value, memory)));
        Assert.NotNull(await database.ViewAsync(own));
        Assert.True(await AlbumDatabase.EventuallyAsync(async () => await database.QueuedEventsAsync("ContributionsWithdrawnEvent", person) == 1));
        Assert.True(await AlbumDatabase.EventuallyAsync(async () => await database.QueuedEventsAsync("ParticipantLeftEvent", person) == 1));
    }

    [Fact]
    public async Task An_event_older_than_the_applied_one_changes_nothing()
    {
        var (album, _, person) = await database.AlbumWithParticipantAsync();
        await DeliverAsync(new AccountDeleted(Guid.CreateVersion7(), DateTimeOffset.UtcNow, Guid.CreateVersion7(), 5));
        var newer = new AccountDeleted(Guid.CreateVersion7(), DateTimeOffset.UtcNow, person, 5);
        await DeliverAsync(newer);

        var invitation = Guid.CreateVersion7();
        var owner = (await database.ViewAsync(album)).OwnerAccountId;
        await database.InviteAsync(album, owner, database.Neighbours.EmailOf(person), invitation);
        await database.AnswerAsync(invitation, person, accepted: true);

        await DeliverAsync(new AccountDeleted(Guid.CreateVersion7(), DateTimeOffset.UtcNow, person, 4));

        Assert.Contains((await database.ViewAsync(album)).Members, member => member.AccountId == person);
    }

    private async Task DeliverAsync(AccountDeleted deleted)
    {
        await using var scope = database.CreateScope();
        await scope.ServiceProvider.GetRequiredService<WithdrawContributionsWhenAccountDeleted>().HandleAsync(deleted, Cancellation);
    }
}
