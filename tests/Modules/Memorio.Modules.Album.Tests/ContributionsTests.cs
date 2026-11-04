using Memorio.Modules.Album.Application.Commands.Contributions;
using Memorio.Modules.Album.Application.Queries;
using Memorio.Modules.Album.Domain.Albums;
using Memorio.Modules.Archive.Contracts;
using Memorio.Modules.Limits.Contracts;
using Microsoft.Extensions.DependencyInjection;

namespace Memorio.Modules.Album.Tests;

/// <summary>MEM-E3-ALBUM-05 to -15, -17, -21 to -23, -25, -26 on a real database.</summary>
public sealed class ContributionsTests(AlbumDatabase database)
{
    private static CancellationToken Cancellation => TestContext.Current.CancellationToken;

    [Fact]
    public async Task Bringing_a_memory_twice_counts_once()
    {
        var (album, _, person) = await database.AlbumWithParticipantAsync();
        var memory = database.Neighbours.NewMemory(person, sizeBytes: 70);

        Assert.True((await database.AddMemoryAsync(album, person, memory)).IsSuccess);
        Assert.True((await database.AddMemoryAsync(album, person, memory)).IsSuccess);

        Assert.Single((await database.ViewAsync(album)).Memories);
        Assert.Equal(70, database.Neighbours.Reserved[PoolKeys.For(album.Value, memory)]);
        Assert.True(database.Neighbours.IsLinked(memory, album.Value));
    }

    [Fact]
    public async Task Only_someone_in_the_album_brings_only_their_own_memory()
    {
        var (album, owner, person) = await database.AlbumWithParticipantAsync();

        Assert.Equal(AlbumErrors.NoAccess, (await database.AddMemoryAsync(album, person, database.Neighbours.NewMemory(owner))).Error);
        var stranger = database.Neighbours.NewAccount();
        Assert.Equal(AlbumErrors.NoAccess, (await database.AddMemoryAsync(album, stranger, database.Neighbours.NewMemory(stranger))).Error);
    }

    [Fact]
    public async Task A_full_pool_refuses_keeps_nothing_and_tells_the_owner_through_the_outbox()
    {
        var (album, owner, person) = await database.AlbumWithParticipantAsync();
        database.Neighbours.WithFullPool(album.Value);
        var memory = database.Neighbours.NewMemory(person);

        Assert.Equal(AlbumErrors.PoolFull, (await database.AddMemoryAsync(album, person, memory)).Error);

        Assert.Empty((await database.ViewAsync(album)).Memories);
        Assert.False(database.Neighbours.IsLinked(memory, album.Value));
        Assert.True(await AlbumDatabase.EventuallyAsync(async () => await database.QueuedEventsAsync("AlbumPoolExhaustedEvent", owner) == 1));
    }

    [Theory]
    [InlineData(AlbumPhotoOutcome.Added, true)]
    [InlineData(AlbumPhotoOutcome.AlreadyThere, true)]
    [InlineData(AlbumPhotoOutcome.PoolFull, false)]
    [InlineData(AlbumPhotoOutcome.UploadMissing, false)]
    public async Task A_photo_in_an_album_answers_as_Archive_decided(AlbumPhotoOutcome outcome, bool succeeds)
    {
        var (album, _, person) = await database.AlbumWithParticipantAsync();
        var memory = await database.BringMemoryAsync(album, person);
        database.Neighbours.NextPhotoIn(memory, outcome);
        await using var scope = database.CreateScope();

        var added = await scope.ServiceProvider.GetRequiredService<AddPhotoInAlbumHandler>().HandleAsync(new AddPhotoInAlbum(album, person, memory, Guid.CreateVersion7(), "uploads/x"), Cancellation);

        Assert.Equal(succeeds, added.IsSuccess);
    }

    [Fact]
    public async Task Only_the_author_adds_photos_to_a_memory_in_the_album()
    {
        var (album, owner, person) = await database.AlbumWithParticipantAsync();
        var memory = await database.BringMemoryAsync(album, person);
        await using var scope = database.CreateScope();

        var refused = await scope.ServiceProvider.GetRequiredService<AddPhotoInAlbumHandler>().HandleAsync(new AddPhotoInAlbum(album, owner, memory, Guid.CreateVersion7(), "uploads/x"), Cancellation);

        Assert.Equal(AlbumErrors.NoAccess, refused.Error);
    }

    [Fact]
    public async Task Leaving_with_keep_keeps_everything_and_can_be_revoked_later()
    {
        var (album, owner, person) = await database.AlbumWithParticipantAsync();
        var memory = await database.BringMemoryAsync(album, person);

        Assert.True((await LeaveAsync(album, person, keep: true)).IsSuccess);

        var view = await database.ViewAsync(album);
        Assert.DoesNotContain(view.Members, member => member.AccountId == person);
        Assert.Single(view.Memories);
        Assert.True(await AlbumDatabase.EventuallyAsync(async () => await database.QueuedEventsAsync("ParticipantLeftEvent", person) == 1));
        await using (var scope = database.CreateScope())
        {
            var kept = Assert.Single(await scope.ServiceProvider.GetRequiredService<IAlbumQueries>().KeptContributionsAsync(person, Cancellation));
            Assert.True(kept.ChosenByAuthor);
            Assert.Equal(1, kept.Memories);
        }

        Assert.True((await RevokeAsync(album, person)).IsSuccess);

        Assert.Empty((await database.ViewAsync(album)).Memories);
        Assert.False(database.Neighbours.IsLinked(memory, album.Value));
        Assert.False(database.Neighbours.Reserved.ContainsKey(PoolKeys.For(album.Value, memory)));
        Assert.True(await AlbumDatabase.EventuallyAsync(async () => await database.QueuedEventsAsync("ContributionsWithdrawnEvent", person) == 1));
        Assert.Equal(AlbumErrors.AlreadyDecided, (await RevokeAsync(album, person)).Error);
    }

    [Fact]
    public async Task Leaving_with_take_takes_it_along_at_once()
    {
        var (album, _, person) = await database.AlbumWithParticipantAsync();
        var memory = await database.BringMemoryAsync(album, person);

        Assert.True((await LeaveAsync(album, person, keep: false)).IsSuccess);

        Assert.Empty((await database.ViewAsync(album)).Memories);
        Assert.False(database.Neighbours.IsLinked(memory, album.Value));
    }

    [Fact]
    public async Task The_owner_cannot_leave()
    {
        var (album, owner, _) = await database.AlbumWithParticipantAsync();

        Assert.Equal(AlbumErrors.OwnerCannotLeave, (await LeaveAsync(album, owner, keep: true)).Error);
    }

    [Fact]
    public async Task A_removed_person_loses_the_seat_their_contributions_stay_until_they_choose()
    {
        var (album, owner, person) = await database.AlbumWithParticipantAsync();
        await database.BringMemoryAsync(album, person);
        await using var scope = database.CreateScope();

        var removed = await scope.ServiceProvider.GetRequiredService<RemoveParticipantHandler>().HandleAsync(new RemoveParticipant(album, owner, person), Cancellation);

        Assert.True(removed.IsSuccess);
        Assert.Single((await database.ViewAsync(album)).Memories);
        var kept = Assert.Single(await scope.ServiceProvider.GetRequiredService<IAlbumQueries>().KeptContributionsAsync(person, Cancellation));
        Assert.False(kept.ChosenByAuthor);
        Assert.True(await AlbumDatabase.EventuallyAsync(async () => await database.QueuedEventsAsync("ParticipantRemovedEvent", person) == 1));
        Assert.True((await RevokeAsync(album, person)).IsSuccess);
        Assert.Empty((await database.ViewAsync(album)).Memories);
    }

    [Fact]
    public async Task A_curator_orders_only_what_was_entrusted_and_only_while_it_is()
    {
        var (album, owner, person) = await database.AlbumWithParticipantAsync();
        var first = await database.BringMemoryAsync(album, person);
        var second = await database.BringMemoryAsync(album, person);
        var ownerMemory = await database.BringMemoryAsync(album, owner);

        await using (var scope = database.CreateScope())
        {
            var entrust = scope.ServiceProvider.GetRequiredService<EntrustToCuratorHandler>();
            Assert.Equal(AlbumErrors.CannotEntrustToYourself, (await entrust.HandleAsync(new EntrustToCurator(album, person, person), Cancellation)).Error);
            Assert.Equal(AlbumErrors.NotParticipant, (await entrust.HandleAsync(new EntrustToCurator(album, person, Guid.CreateVersion7()), Cancellation)).Error);
            Assert.True((await entrust.HandleAsync(new EntrustToCurator(album, person, owner), Cancellation)).IsSuccess);
        }

        Assert.Equal(AlbumErrors.NoAccess, (await ArrangeAsync(album, person, [first])).Error);
        Assert.Equal(AlbumErrors.InvalidArrangement, (await ArrangeAsync(album, owner, [first, first])).Error);
        Assert.Equal(AlbumErrors.InvalidArrangement, (await ArrangeAsync(album, owner, [])).Error);
        Assert.Equal(AlbumErrors.NoAccess, (await ArrangeAsync(album, owner, [ownerMemory])).Error);
        Assert.True((await ArrangeAsync(album, owner, [second, first])).IsSuccess);

        var ordered = (await database.ViewAsync(album)).Memories.Select(memory => memory.MemoryId).Take(2).ToList();
        Assert.Equal([second, first], ordered);

        await using (var scope = database.CreateScope())
        {
            var queries = scope.ServiceProvider.GetRequiredService<IAlbumQueries>();
            Assert.Equal(2, (await queries.EntrustedToAsync(owner, Cancellation)).Count(row => row.AlbumId == album.Value));
            var revoke = scope.ServiceProvider.GetRequiredService<RevokeEntrustmentHandler>();
            Assert.True((await revoke.HandleAsync(new RevokeEntrustment(album, person), Cancellation)).IsSuccess);
            Assert.True((await revoke.HandleAsync(new RevokeEntrustment(album, person), Cancellation)).IsSuccess);
            Assert.DoesNotContain(await queries.EntrustedToAsync(owner, Cancellation), row => row.AlbumId == album.Value);
        }

        Assert.Equal(AlbumErrors.NoAccess, (await ArrangeAsync(album, owner, [first])).Error);
    }

    [Fact]
    public async Task Tags_are_for_people_in_the_album_and_only_the_person_removes_their_own()
    {
        var (album, owner, person) = await database.AlbumWithParticipantAsync();
        var memory = await database.BringMemoryAsync(album, owner);
        await using var scope = database.CreateScope();
        var tag = scope.ServiceProvider.GetRequiredService<TagParticipantHandler>();
        var untag = scope.ServiceProvider.GetRequiredService<RemoveOwnTagHandler>();

        Assert.Equal(AlbumErrors.NotParticipant, (await tag.HandleAsync(new TagParticipant(album, owner, memory, Guid.CreateVersion7()), Cancellation)).Error);
        Assert.Equal(AlbumErrors.NoAccess, (await tag.HandleAsync(new TagParticipant(album, Guid.CreateVersion7(), memory, person), Cancellation)).Error);
        Assert.True((await tag.HandleAsync(new TagParticipant(album, owner, memory, person), Cancellation)).IsSuccess);
        Assert.True((await tag.HandleAsync(new TagParticipant(album, owner, memory, person), Cancellation)).IsSuccess);

        Assert.Equal([person], Assert.Single((await database.ViewAsync(album)).Memories).TaggedAccountIds);
        Assert.True(await AlbumDatabase.EventuallyAsync(async () => await database.QueuedEventsAsync("ParticipantTaggedEvent", person) == 1));

        Assert.True((await untag.HandleAsync(new RemoveOwnTag(album, owner, memory), Cancellation)).IsSuccess);
        Assert.Equal([person], Assert.Single((await database.ViewAsync(album)).Memories).TaggedAccountIds);

        Assert.True((await untag.HandleAsync(new RemoveOwnTag(album, person, memory), Cancellation)).IsSuccess);
        Assert.Empty(Assert.Single((await database.ViewAsync(album)).Memories).TaggedAccountIds);
    }

    private async Task<SharedKernel.Results.Result> LeaveAsync(AlbumId album, Guid accountId, bool keep)
    {
        await using var scope = database.CreateScope();
        return await scope.ServiceProvider.GetRequiredService<LeaveAlbumHandler>().HandleAsync(new LeaveAlbum(album, accountId, keep), Cancellation);
    }

    private async Task<SharedKernel.Results.Result> RevokeAsync(AlbumId album, Guid accountId)
    {
        await using var scope = database.CreateScope();
        return await scope.ServiceProvider.GetRequiredService<RevokeKeptContributionsHandler>().HandleAsync(new RevokeKeptContributions(album, accountId), Cancellation);
    }

    private async Task<SharedKernel.Results.Result> ArrangeAsync(AlbumId album, Guid accountId, IReadOnlyList<Guid> memories)
    {
        await using var scope = database.CreateScope();
        return await scope.ServiceProvider.GetRequiredService<ArrangeEntrustedHandler>().HandleAsync(new ArrangeEntrusted(album, accountId, memories), Cancellation);
    }
}
