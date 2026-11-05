using Memorio.Modules.Album.Domain.Albums;
using Microsoft.Extensions.DependencyInjection;

namespace Memorio.Modules.Album.Tests;

/// <summary>D-267 on a real database.</summary>
public sealed class InvitationLimitTests(AlbumDatabase database)
{
    private static CancellationToken Cancellation => TestContext.Current.CancellationToken;

    private static string NewAddress() => $"{Guid.NewGuid()}@example.invalid";

    [Fact]
    public async Task Ten_a_day_in_all_albums_together_and_the_eleventh_leaves_no_invitation()
    {
        var owner = database.Neighbours.NewAccount();
        var first = await database.NewAlbumAsync(owner);
        var second = await database.NewAlbumAsync(owner);
        for (var number = 0; number < InvitationQuota.DailyLimit; number++)
        {
            var album = number % 2 == 0 ? first : second;
            Assert.True((await database.InviteAsync(album, owner, NewAddress())).IsSuccess);
        }

        Assert.Equal(AlbumErrors.InvitationLimitReached, (await database.InviteAsync(first, owner, NewAddress())).Error);

        Assert.Equal(5, (await database.SeatsAsync(first, owner)).Invited.Count);
    }

    [Fact]
    public async Task Inviting_the_same_address_again_counts_because_it_sends_again()
    {
        var owner = database.Neighbours.NewAccount();
        var album = await database.NewAlbumAsync(owner);
        var address = NewAddress();
        for (var number = 0; number < InvitationQuota.DailyLimit; number++)
        {
            Assert.True((await database.InviteAsync(album, owner, address)).IsSuccess);
        }

        Assert.Equal(AlbumErrors.InvitationLimitReached, (await database.InviteAsync(album, owner, address)).Error);
    }

    [Fact]
    public async Task A_refused_invitation_does_not_count()
    {
        var owner = database.Neighbours.NewAccount();
        var album = await database.NewAlbumAsync(owner);
        database.Neighbours.SeatsOf(album.Value, 1);
        for (var number = 0; number < InvitationQuota.DailyLimit + 2; number++)
        {
            Assert.Equal(AlbumErrors.NoFreeSeats, (await database.InviteAsync(album, owner, NewAddress())).Error);
        }

        database.Neighbours.SeatsOf(album.Value, 10);

        Assert.True((await database.InviteAsync(album, owner, NewAddress())).IsSuccess);
    }

    [Fact]
    public async Task Parallel_invitations_never_pass_the_limit()
    {
        var owner = database.Neighbours.NewAccount();
        var albums = await Task.WhenAll(Enumerable.Range(0, 15).Select(_ => database.NewAlbumAsync(owner)));

        var results = await Task.WhenAll(albums.Select(album => database.InviteAsync(album, owner, NewAddress())));

        Assert.Equal(InvitationQuota.DailyLimit, results.Count(result => result.IsSuccess));
        Assert.All(results.Where(result => result.IsFailure), result => Assert.Equal(AlbumErrors.InvitationLimitReached, result.Error));
    }

    [Fact]
    public async Task A_new_day_starts_again_and_another_account_has_its_own_count()
    {
        var (person, other) = (Guid.CreateVersion7(), Guid.CreateVersion7());
        var today = new DateOnly(2026, 11, 5);
        await using var scope = database.CreateScope();
        var albums = scope.ServiceProvider.GetRequiredService<IAlbums>();
        for (var number = 0; number < InvitationQuota.DailyLimit; number++)
        {
            Assert.True(await albums.TryConsumeInvitationAsync(person, today, Cancellation));
        }

        Assert.False(await albums.TryConsumeInvitationAsync(person, today, Cancellation));
        Assert.True(await albums.TryConsumeInvitationAsync(other, today, Cancellation));
        Assert.True(await albums.TryConsumeInvitationAsync(person, today.AddDays(1), Cancellation));
    }
}
