using Memorio.Modules.Account.Contracts;
using Memorio.Modules.Album.Application.Commands.Contributions;
using Memorio.Modules.Album.Application.Commands.Takeover;
using Memorio.Modules.Album.Application.Policies;
using Memorio.Modules.Album.Application.Queries;
using Memorio.Modules.Album.Contracts;
using Memorio.Modules.Album.Domain.Albums;
using Memorio.Modules.Album.Infrastructure.Scheduling;
using Memorio.Modules.Plans.Contracts;
using Memorio.SharedKernel.Results;
using Memorio.SharedKernel.Scheduling;
using Microsoft.Extensions.DependencyInjection;

namespace Memorio.Modules.Album.Tests;

/// <summary>E9 on a real database: an album without an owner, its successor and a takeover (D-001, D-004).</summary>
public sealed class AlbumWithoutOwnerTests(AlbumDatabase database)
{
    private static CancellationToken Cancellation => TestContext.Current.CancellationToken;

    private async Task<(AlbumId Album, Guid Person)> OwnerlessAsync()
    {
        var (album, owner, person) = await database.AlbumWithParticipantAsync();
        await using var scope = database.CreateScope();
        await scope.ServiceProvider.GetRequiredService<IAlbumSuccession>().ReleaseOwnerAsync(album.Value, owner, Cancellation);
        return (album, person);
    }

    internal static async Task<Result> ReserveAsync(AlbumDatabase database, AlbumId album, Guid person)
    {
        await using var scope = database.CreateScope();
        return await scope.ServiceProvider.GetRequiredService<ReserveTakeoverHandler>().HandleAsync(new ReserveTakeover(album, person), Cancellation);
    }

    private async Task<SharedAlbum> FindAsync(AlbumId album)
    {
        await using var scope = database.CreateScope();
        return (await scope.ServiceProvider.GetRequiredService<IAlbums>().FindAsync(album, Cancellation))!;
    }

    [Fact]
    public async Task Releasing_twice_leaves_one_fact_and_participants_still_read()
    {
        var (album, owner, person) = await database.AlbumWithParticipantAsync();
        await using var scope = database.CreateScope();
        var port = scope.ServiceProvider.GetRequiredService<IAlbumSuccession>();

        await port.ReleaseOwnerAsync(album.Value, owner, Cancellation);
        await port.ReleaseOwnerAsync(album.Value, owner, Cancellation);

        Assert.True(await AlbumDatabase.EventuallyAsync(async () => await database.QueuedEventsAsync(nameof(AlbumWithoutOwnerEvent), album.Value) == 1));
        await using var read = database.CreateScope();
        var membership = read.ServiceProvider.GetRequiredService<IAlbumMembership>();
        Assert.Null(await membership.OwnerOfAsync(album.Value, Cancellation));
        Assert.False(await membership.IsMemberAsync(owner, album.Value, Cancellation));
        Assert.True(await membership.IsMemberAsync(person, album.Value, Cancellation));
        Assert.Equal([person], await read.ServiceProvider.GetRequiredService<IAlbumMessaging>().MembersOfAsync(album.Value, Cancellation));
        var view = await database.ViewAsync(album);
        Assert.Equal(person, Assert.Single(view.Members).AccountId);
        var listed = Assert.Single(await read.ServiceProvider.GetRequiredService<IAlbumQueries>().MyAlbumsAsync(person, Cancellation));
        Assert.Equal(1, listed.Members);
    }

    [Fact]
    public async Task Someone_who_is_not_the_owner_any_more_releases_nothing()
    {
        var (album, _, _) = await database.AlbumWithParticipantAsync();
        await using var scope = database.CreateScope();

        await scope.ServiceProvider.GetRequiredService<IAlbumSuccession>().ReleaseOwnerAsync(album.Value, Guid.CreateVersion7(), Cancellation);

        Assert.False((await FindAsync(album)).IsOwnerless);
    }

    [Fact]
    public async Task The_successor_takes_it_with_the_contributions_of_the_founder()
    {
        var (album, owner, person) = await database.AlbumWithParticipantAsync();
        await database.BringMemoryAsync(album, owner);
        await using var scope = database.CreateScope();
        var port = scope.ServiceProvider.GetRequiredService<IAlbumSuccession>();
        Assert.Equal(1, await port.MemoriesOfAsync(album.Value, owner, Cancellation));
        Assert.Equal(album.Value, Assert.Single(await port.OwnedByAsync(owner, Cancellation)).AlbumId);

        await port.TransferAsync(album.Value, owner, person, Cancellation);
        await port.TransferAsync(album.Value, owner, person, Cancellation);

        var moved = await FindAsync(album);
        Assert.True(moved.IsOwner(person));
        Assert.Empty(moved.Participants);
        Assert.Single((await database.ViewAsync(album)).Memories);
        Assert.True(await AlbumDatabase.EventuallyAsync(async () => await database.QueuedEventsAsync(nameof(AlbumOwnerChangedEvent), album.Value) == 1));
    }

    [Fact]
    public async Task A_successor_who_left_cannot_take_it_and_it_is_as_after_a_refusal()
    {
        var (album, owner, person) = await database.AlbumWithParticipantAsync();
        await using var scope = database.CreateScope();
        Assert.True((await scope.ServiceProvider.GetRequiredService<LeaveAlbumHandler>().HandleAsync(new LeaveAlbum(album, person, true), Cancellation)).IsSuccess);

        await using var next = database.CreateScope();
        await next.ServiceProvider.GetRequiredService<IAlbumSuccession>().TransferAsync(album.Value, owner, person, Cancellation);

        Assert.True((await FindAsync(album)).IsOwnerless);
    }

    [Fact]
    public async Task Only_someone_in_an_ownerless_album_reserves_it()
    {
        var (withOwner, _, person) = await database.AlbumWithParticipantAsync();
        Assert.Equal(AlbumErrors.HasOwner, (await ReserveAsync(database, withOwner, person)).Error);

        var (album, _) = await OwnerlessAsync();
        Assert.Equal(AlbumErrors.NoAccess, (await ReserveAsync(database, album, Guid.CreateVersion7())).Error);
    }

    [Fact]
    public async Task The_first_one_holds_it_and_the_same_person_again_keeps_the_time()
    {
        var (album, first) = await OwnerlessAsync();
        var second = database.Neighbours.NewAccount();
        await JoinAsync(album, second);

        Assert.True((await ReserveAsync(database, album, first)).IsSuccess);
        var until = (await FindAsync(album)).ReservedUntil;
        Assert.True((await ReserveAsync(database, album, first)).IsSuccess);
        Assert.Equal(until, (await FindAsync(album)).ReservedUntil);

        Assert.Equal(AlbumErrors.TakeoverReserved, (await ReserveAsync(database, album, second)).Error);
    }

    [Fact]
    public async Task Six_at_once_and_one_holds_it()
    {
        var (album, first) = await OwnerlessAsync();
        var others = Enumerable.Range(0, 3).Select(_ => database.Neighbours.NewAccount()).ToList();
        foreach (var other in others)
        {
            await JoinAsync(album, other);
        }

        var results = await Task.WhenAll(others.Append(first).Select(person => ReserveAsync(database, album, person)));

        Assert.Single(results, result => result.IsSuccess);
    }

    [Fact]
    public async Task Paying_with_the_reservation_makes_an_owner_once()
    {
        var (album, person) = await OwnerlessAsync();
        await ReserveAsync(database, album, person);
        var paid = new PaidWhileOwnerlessEvent(Guid.CreateVersion7(), DateTimeOffset.UtcNow, album.Value, 3);

        for (var delivery = 0; delivery < 2; delivery++)
        {
            await using var scope = database.CreateScope();
            await scope.ServiceProvider.GetRequiredService<TakeOverWhenPaid>().HandleAsync(paid, Cancellation);
        }

        var taken = await FindAsync(album);
        Assert.True(taken.IsOwner(person));
        Assert.False(taken.IsOwnerless);
        Assert.Null(taken.ReservedByAccountId);
        Assert.True(await AlbumDatabase.EventuallyAsync(async () => await database.QueuedEventsAsync(nameof(AlbumTakenOverEvent), album.Value) == 1));
    }

    [Fact]
    public async Task Paying_without_a_reservation_changes_nothing()
    {
        var (album, _) = await OwnerlessAsync();

        await using var scope = database.CreateScope();
        await scope.ServiceProvider.GetRequiredService<TakeOverWhenPaid>().HandleAsync(new PaidWhileOwnerlessEvent(Guid.CreateVersion7(), DateTimeOffset.UtcNow, album.Value, 3), Cancellation);

        Assert.True((await FindAsync(album)).IsOwnerless);
    }

    [Fact]
    public async Task Leaving_ends_ones_own_reservation()
    {
        var (album, person) = await OwnerlessAsync();
        await ReserveAsync(database, album, person);

        await using var scope = database.CreateScope();
        Assert.True((await scope.ServiceProvider.GetRequiredService<LeaveAlbumHandler>().HandleAsync(new LeaveAlbum(album, person, true), Cancellation)).IsSuccess);

        Assert.Null((await FindAsync(album)).ReservedUntil);
    }

    [Fact]
    public async Task A_deleted_owner_without_a_successor_leaves_it_without_an_owner_and_with_one_it_waits()
    {
        var (without, withoutOwner, _) = await database.AlbumWithParticipantAsync();
        var (with, withOwner, successor) = await database.AlbumWithParticipantAsync();
        await using (var scope = database.CreateScope())
        {
            await scope.ServiceProvider.GetRequiredService<IAlbumSuccession>().NameAsync(with.Value, withOwner, successor, Cancellation);
        }

        foreach (var deleted in new[] { withoutOwner, withOwner })
        {
            await using var scope = database.CreateScope();
            await scope.ServiceProvider.GetRequiredService<WithdrawContributionsWhenAccountDeleted>().HandleAsync(new AccountDeleted(Guid.CreateVersion7(), DateTimeOffset.UtcNow, deleted, 2), Cancellation);
        }

        Assert.True((await FindAsync(without)).IsOwnerless);
        Assert.True((await FindAsync(with)).IsOwner(withOwner));
    }

    private async Task JoinAsync(AlbumId album, Guid person)
    {
        await using var scope = database.CreateScope();
        var albums = scope.ServiceProvider.GetRequiredService<IAlbums>();
        (await albums.FindAsync(album, Cancellation))!.AddParticipant(person);
        Assert.True(await albums.SaveAsync(Cancellation));
    }
}

/// <summary>SD6: E165 delivered twice leaves one deadline.</summary>
public sealed class TakeoverReservedHandlerIdempotencyTests(AlbumDatabase database)
{
    [Fact]
    public async Task One_deadline_at_the_end_of_the_30_minutes()
    {
        var album = Guid.CreateVersion7();
        var until = new DateTimeOffset(2030, 1, 1, 10, 30, 0, TimeSpan.Zero);
        var reserved = new TakeoverReservedEvent(Guid.CreateVersion7(), until.AddMinutes(-30), album, until, 4);
        await using var scope = database.CreateScope();
        var policy = scope.ServiceProvider.GetRequiredService<TakeoverReservationDeadline>();

        await policy.HandleAsync(reserved, TestContext.Current.CancellationToken);
        await policy.HandleAsync(reserved, TestContext.Current.CancellationToken);

        Assert.Equal(until, database.Deadlines.Find(TakeoverReservationDeadline.JobName, album.ToString())?.ScheduledFor);
    }
}

/// <summary>SD6: E171 delivered twice is covered by paying with the reservation above; this names the handler.</summary>
public sealed class PaidWhileOwnerlessHandlerIdempotencyTests(AlbumDatabase database)
{
    [Fact]
    public async Task A_payment_about_an_album_with_an_owner_changes_nothing()
    {
        var (album, owner, _) = await database.AlbumWithParticipantAsync();
        await using var scope = database.CreateScope();

        await scope.ServiceProvider.GetRequiredService<TakeOverWhenPaid>().HandleAsync(new PaidWhileOwnerlessEvent(Guid.CreateVersion7(), DateTimeOffset.UtcNow, album.Value, 3), TestContext.Current.CancellationToken);

        var found = await scope.ServiceProvider.GetRequiredService<IAlbums>().FindAsync(album, TestContext.Current.CancellationToken);
        Assert.True(found!.IsOwner(owner));
    }
}

/// <summary>SD12: the same firing twice; only a reservation whose time ended goes.</summary>
public sealed class TakeoverReservationJobIdempotencyTests(AlbumDatabase database)
{
    private static CancellationToken Cancellation => TestContext.Current.CancellationToken;

    private async Task FireAsync(AlbumId album, DateTimeOffset at)
    {
        await using var scope = database.CreateScope();
        await ActivatorUtilities.CreateInstance<TakeoverReservationJob>(scope.ServiceProvider).RunAsync(
            new ScheduledJobRun(TakeoverReservationDeadline.JobName, album.Value.ToString(), at, new Dictionary<string, string>(), 1),
            Cancellation);
    }

    [Fact]
    public async Task The_time_ended_and_someone_else_can_hold_it()
    {
        var (album, owner, person) = await database.AlbumWithParticipantAsync();
        await using (var scope = database.CreateScope())
        {
            await scope.ServiceProvider.GetRequiredService<IAlbumSuccession>().ReleaseOwnerAsync(album.Value, owner, Cancellation);
        }

        Assert.True((await AlbumWithoutOwnerTests.ReserveAsync(database, album, person)).IsSuccess);

        await using (var read = database.CreateScope())
        {
            var until = (await read.ServiceProvider.GetRequiredService<IAlbums>().FindAsync(album, Cancellation))!.ReservedUntil!.Value;
            await FireAsync(album, until.AddMinutes(-1));
            await using var check = database.CreateScope();
            Assert.NotNull((await check.ServiceProvider.GetRequiredService<IAlbums>().FindAsync(album, Cancellation))!.ReservedUntil);

            await FireAsync(album, until);
            await FireAsync(album, until);
        }

        await using var after = database.CreateScope();
        Assert.Null((await after.ServiceProvider.GetRequiredService<IAlbums>().FindAsync(album, Cancellation))!.ReservedUntil);
    }
}
