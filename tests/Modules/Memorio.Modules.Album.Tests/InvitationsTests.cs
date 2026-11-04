using Memorio.Modules.Album.Application.Commands.CreateAlbum;
using Memorio.Modules.Album.Application.Commands.Invitations;
using Memorio.Modules.Album.Application.Queries;
using Memorio.Modules.Album.Domain.Albums;
using Microsoft.Extensions.DependencyInjection;

namespace Memorio.Modules.Album.Tests;

/// <summary>MEM-E3-ALBUM-01 to -04, -16, -18, -20, -24 on a real database.</summary>
public sealed class InvitationsTests(AlbumDatabase database)
{
    private static CancellationToken Cancellation => TestContext.Current.CancellationToken;

    [Fact]
    public async Task Creating_twice_is_one_album_and_someone_elses_id_is_refused()
    {
        var owner = database.Neighbours.NewAccount();
        var id = Guid.CreateVersion7();
        await using var scope = database.CreateScope();
        var handler = scope.ServiceProvider.GetRequiredService<CreateAlbumHandler>();

        Assert.True((await handler.HandleAsync(new CreateAlbum(id, owner, "Rodzina"), Cancellation)).IsSuccess);
        Assert.True((await handler.HandleAsync(new CreateAlbum(id, owner, "Rodzina"), Cancellation)).IsSuccess);
        Assert.Equal(AlbumErrors.IdTaken, (await handler.HandleAsync(new CreateAlbum(id, Guid.CreateVersion7(), "Inny"), Cancellation)).Error);
        Assert.Equal(AlbumErrors.InvalidName, (await handler.HandleAsync(new CreateAlbum(Guid.CreateVersion7(), owner, " "), Cancellation)).Error);
    }

    [Fact]
    public async Task Parallel_invitations_never_take_more_seats_than_the_plan_has()
    {
        var owner = database.Neighbours.NewAccount();
        var album = await database.NewAlbumAsync(owner);
        database.Neighbours.SeatsOf(album.Value, 3);

        var results = await Task.WhenAll(Enumerable.Range(0, 6).Select(number =>
            database.InviteAsync(album, owner, $"{Guid.NewGuid()}-{number}@example.invalid")));

        Assert.Equal(2, results.Count(result => result.IsSuccess));
        Assert.All(results.Where(result => result.IsFailure), result =>
            Assert.Contains(result.Error, new[] { AlbumErrors.NoFreeSeats, AlbumErrors.AlreadyDecided }));
        Assert.Equal(2, (await database.SeatsAsync(album, owner)).Invited.Count);
    }

    [Fact]
    public async Task Accepting_and_withdrawing_at_once_leaves_one_consistent_result()
    {
        var owner = database.Neighbours.NewAccount();
        var person = database.Neighbours.NewAccount();
        var album = await database.NewAlbumAsync(owner);
        var invitation = Guid.CreateVersion7();
        await database.InviteAsync(album, owner, database.Neighbours.EmailOf(person), invitation);

        async Task<bool> WithdrawAsync()
        {
            await using var scope = database.CreateScope();
            return (await scope.ServiceProvider.GetRequiredService<WithdrawInvitationHandler>().HandleAsync(new WithdrawInvitation(album, owner, invitation), Cancellation)).IsSuccess;
        }

        var accepting = database.AnswerAsync(invitation, person, accepted: true);
        var withdrawing = WithdrawAsync();
        await Task.WhenAll(accepting, withdrawing);

        var (participants, invited) = await database.SeatsAsync(album, owner);
        Assert.Empty(invited);
        Assert.NotEqual((await accepting).IsSuccess, await withdrawing);
        Assert.Equal((await accepting).IsSuccess ? 1 : 0, participants);
    }

    [Fact]
    public async Task A_blocked_address_is_refused_without_a_seat()
    {
        var owner = database.Neighbours.NewAccount();
        var album = await database.NewAlbumAsync(owner);
        var email = $"{Guid.NewGuid()}@example.invalid";
        database.Neighbours.Block(email);

        Assert.Equal(AlbumErrors.AddressBlocked, (await database.InviteAsync(album, owner, email)).Error);
        Assert.Empty((await database.SeatsAsync(album, owner)).Invited);
    }

    [Fact]
    public async Task Inviting_a_bad_address_yourself_or_someone_elses_album_is_refused()
    {
        var owner = database.Neighbours.NewAccount();
        var album = await database.NewAlbumAsync(owner);

        Assert.Equal(AlbumErrors.InvalidEmail, (await database.InviteAsync(album, owner, "nie-adres")).Error);
        Assert.Equal(AlbumErrors.CannotInviteYourself, (await database.InviteAsync(album, owner, database.Neighbours.EmailOf(owner).ToUpperInvariant())).Error);
        Assert.Equal(AlbumErrors.NoAccess, (await database.InviteAsync(album, database.Neighbours.NewAccount(), "a@example.invalid")).Error);
    }

    [Fact]
    public async Task Only_the_invited_address_answers_and_the_answer_comes_once()
    {
        var owner = database.Neighbours.NewAccount();
        var person = database.Neighbours.NewAccount();
        var album = await database.NewAlbumAsync(owner);
        var invitation = Guid.CreateVersion7();
        await database.InviteAsync(album, owner, database.Neighbours.EmailOf(person), invitation);

        Assert.Equal(AlbumErrors.InvitationGone, (await database.AnswerAsync(invitation, database.Neighbours.NewAccount(), accepted: true)).Error);
        var accepted = await database.AnswerAsync(invitation, person, accepted: true);
        Assert.Equal(album, accepted.Value);
        Assert.Equal(AlbumErrors.InvitationGone, (await database.AnswerAsync(invitation, person, accepted: false)).Error);
        Assert.Contains((await database.ViewAsync(album)).Members, member => member.AccountId == person && !member.IsOwner);
    }

    [Fact]
    public async Task The_invitation_shows_who_invites_and_the_album_and_nothing_to_anyone_else()
    {
        var owner = database.Neighbours.NewAccount();
        var person = database.Neighbours.NewAccount();
        var album = await database.NewAlbumAsync(owner);
        var invitation = Guid.CreateVersion7();
        await database.InviteAsync(album, owner, database.Neighbours.EmailOf(person), invitation);
        await using var scope = database.CreateScope();
        var queries = scope.ServiceProvider.GetRequiredService<IAlbumQueries>();

        var mine = Assert.Single(await queries.PendingInvitationsAsync(database.Neighbours.EmailOf(person), Cancellation));

        Assert.Equal(invitation, mine.Id);
        Assert.Equal("Rodzina", mine.AlbumName);
        Assert.Equal(owner, mine.InvitedByAccountId);
        Assert.Empty(await queries.PendingInvitationsAsync($"{Guid.NewGuid()}@example.invalid", Cancellation));
    }

    [Fact]
    public async Task Seats_are_for_the_owner_only()
    {
        var (album, owner, participant) = await database.AlbumWithParticipantAsync();
        await using var scope = database.CreateScope();
        var queries = scope.ServiceProvider.GetRequiredService<IAlbumQueries>();

        Assert.Null(await queries.SeatsAsync(album.Value, participant, Cancellation));
        Assert.Equal(1, (await queries.SeatsAsync(album.Value, owner, Cancellation))!.Value.Participants);
    }

    [Fact]
    public async Task Accepted_and_sent_go_out_through_the_outbox_with_the_change()
    {
        var owner = database.Neighbours.NewAccount();
        var person = database.Neighbours.NewAccount();
        var album = await database.NewAlbumAsync(owner);
        var invitation = Guid.CreateVersion7();

        await database.InviteAsync(album, owner, database.Neighbours.EmailOf(person), invitation);
        Assert.True(await AlbumDatabase.EventuallyAsync(async () => await database.QueuedEventsAsync("InvitationSentEvent", invitation) == 1));

        await database.AnswerAsync(invitation, person, accepted: true);
        Assert.True(await AlbumDatabase.EventuallyAsync(async () => await database.QueuedEventsAsync("InvitationAcceptedEvent", person) == 1));
    }

    [Fact]
    public async Task A_refused_invitation_leaves_no_event()
    {
        var owner = database.Neighbours.NewAccount();
        var album = await database.NewAlbumAsync(owner);
        database.Neighbours.SeatsOf(album.Value, 1);
        var invitation = Guid.CreateVersion7();

        Assert.Equal(AlbumErrors.NoFreeSeats, (await database.InviteAsync(album, owner, "a@example.invalid", invitation)).Error);

        await Task.Delay(500, Cancellation);
        Assert.Equal(0, await database.QueuedEventsAsync("InvitationSentEvent", invitation));
    }
}
