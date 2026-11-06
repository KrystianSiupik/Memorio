using Memorio.Modules.Album.Application.Commands.Contributions;
using Memorio.Modules.Album.Application.Commands.Takeover;
using Memorio.Modules.Album.Contracts;
using Memorio.Modules.Album.Domain.Albums;
using Memorio.Modules.Limits.Contracts;
using Memorio.SharedKernel.Results;
using Microsoft.Extensions.DependencyInjection;

namespace Memorio.Modules.Album.Tests;

/// <summary>D-307: the living owner hands the album over (S2:B3), on a real database.</summary>
public sealed class HandoverTests(AlbumDatabase database)
{
    private static CancellationToken Cancellation => TestContext.Current.CancellationToken;

    private async Task<Result> OfferAsync(AlbumId album, Guid owner, Guid to, bool keep)
    {
        await using var scope = database.CreateScope();
        return await scope.ServiceProvider.GetRequiredService<HandoverHandler>().HandleAsync(new OfferHandover(album, owner, to, keep), Cancellation);
    }

    private async Task<Result> AnswerAsync(AlbumId album, Guid person, bool accepted)
    {
        await using var scope = database.CreateScope();
        return await scope.ServiceProvider.GetRequiredService<HandoverHandler>().HandleAsync(new AnswerHandover(album, person, accepted), Cancellation);
    }

    private async Task<SharedAlbum> FindAsync(AlbumId album)
    {
        await using var scope = database.CreateScope();
        return (await scope.ServiceProvider.GetRequiredService<IAlbums>().FindAsync(album, Cancellation))!;
    }

    [Fact]
    public async Task Accepted_the_person_owns_it_and_the_former_owner_stays_with_their_memories()
    {
        var (album, owner, person) = await database.AlbumWithParticipantAsync();
        var memory = await database.BringMemoryAsync(album, owner);

        Assert.True((await OfferAsync(album, owner, person, keep: true)).IsSuccess);
        Assert.True(await AlbumDatabase.EventuallyAsync(async () => await database.QueuedEventsAsync(nameof(HandoverOfferedEvent), album.Value) == 1));
        Assert.True((await AnswerAsync(album, person, accepted: true)).IsSuccess);

        var handed = await FindAsync(album);
        Assert.True(handed.IsOwner(person));
        Assert.Contains(handed.Participants, participant => participant.AccountId == owner);
        Assert.Null(handed.HandoverToAccountId);
        Assert.True(database.Neighbours.IsLinked(memory, album.Value));
        Assert.True(await AlbumDatabase.EventuallyAsync(async () => await database.QueuedEventsAsync(nameof(AlbumOwnerChangedEvent), album.Value) == 1));
        Assert.Equal(AlbumErrors.HandoverGone, (await AnswerAsync(album, person, accepted: true)).Error);
    }

    [Fact]
    public async Task Chosen_so_the_former_owners_contributions_leave_on_acceptance()
    {
        var (album, owner, person) = await database.AlbumWithParticipantAsync();
        var mine = await database.BringMemoryAsync(album, owner);
        var theirs = await database.BringMemoryAsync(album, person);

        await OfferAsync(album, owner, person, keep: false);
        Assert.True(database.Neighbours.IsLinked(mine, album.Value));
        await AnswerAsync(album, person, accepted: true);

        Assert.False(database.Neighbours.IsLinked(mine, album.Value));
        Assert.False(database.Neighbours.Reserved.ContainsKey(PoolKeys.For(album.Value, mine)));
        Assert.True(database.Neighbours.IsLinked(theirs, album.Value));
        Assert.Contains((await FindAsync(album)).Participants, participant => participant.AccountId == owner);
    }

    [Fact]
    public async Task Refused_withdrawn_or_with_the_person_gone_nothing_changes()
    {
        var (album, owner, person) = await database.AlbumWithParticipantAsync();
        Assert.Equal(AlbumErrors.NoAccess, (await OfferAsync(album, person, owner, keep: true)).Error);
        Assert.Equal(AlbumErrors.HandoverNotParticipant, (await OfferAsync(album, owner, Guid.CreateVersion7(), keep: true)).Error);

        await OfferAsync(album, owner, person, keep: true);
        Assert.True((await AnswerAsync(album, person, accepted: false)).IsSuccess);
        Assert.True((await FindAsync(album)).IsOwner(owner));

        await OfferAsync(album, owner, person, keep: true);
        await using (var scope = database.CreateScope())
        {
            Assert.True((await scope.ServiceProvider.GetRequiredService<HandoverHandler>().HandleAsync(new WithdrawHandover(album, owner), Cancellation)).IsSuccess);
        }

        Assert.Equal(AlbumErrors.HandoverGone, (await AnswerAsync(album, person, accepted: true)).Error);

        await OfferAsync(album, owner, person, keep: true);
        await using (var scope = database.CreateScope())
        {
            await scope.ServiceProvider.GetRequiredService<LeaveAlbumHandler>().HandleAsync(new LeaveAlbum(album, person, true), Cancellation);
        }

        Assert.Null((await FindAsync(album)).HandoverToAccountId);
        Assert.True((await FindAsync(album)).IsOwner(owner));
    }
}
