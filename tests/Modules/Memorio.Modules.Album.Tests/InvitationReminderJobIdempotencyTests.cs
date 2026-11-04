using Memorio.Modules.Album.Application.Policies;
using Microsoft.Extensions.DependencyInjection;

namespace Memorio.Modules.Album.Tests;

/// <summary>SD12, MEM-E3-ALBUM-20: the deadline fired twice, too early and after the answer, on a real database.</summary>
public sealed class InvitationReminderJobIdempotencyTests(AlbumDatabase database)
{
    [Fact]
    public async Task Fired_twice_gives_one_E45_and_a_refreshed_invitation_waits_again()
    {
        var owner = database.Neighbours.NewAccount();
        var album = await database.NewAlbumAsync(owner);
        var invitation = Guid.CreateVersion7();
        await database.InviteAsync(album, owner, "a@example.invalid", invitation);
        var due = DateTimeOffset.UtcNow + InvitationReminderDeadline.Wait;

        await FireAsync(invitation, due.AddHours(-1));
        await Task.Delay(300, TestContext.Current.CancellationToken);
        Assert.Equal(0, await database.QueuedEventsAsync("InvitationWeekPassedEvent", invitation));

        await FireAsync(invitation, due.AddMinutes(1));
        await FireAsync(invitation, due.AddMinutes(2));

        Assert.True(await AlbumDatabase.EventuallyAsync(async () => await database.QueuedEventsAsync("InvitationWeekPassedEvent", invitation) == 1));
        await Task.Delay(300, TestContext.Current.CancellationToken);
        Assert.Equal(1, await database.QueuedEventsAsync("InvitationWeekPassedEvent", invitation));
    }

    [Fact]
    public async Task An_answered_invitation_gets_no_reminder()
    {
        var person = database.Neighbours.NewAccount();
        var owner = database.Neighbours.NewAccount();
        var album = await database.NewAlbumAsync(owner);
        var invitation = Guid.CreateVersion7();
        await database.InviteAsync(album, owner, database.Neighbours.EmailOf(person), invitation);
        await database.AnswerAsync(invitation, person, accepted: true);

        await FireAsync(invitation, DateTimeOffset.UtcNow.AddDays(8));

        await Task.Delay(300, TestContext.Current.CancellationToken);
        Assert.Equal(0, await database.QueuedEventsAsync("InvitationWeekPassedEvent", invitation));
    }

    private async Task FireAsync(Guid invitation, DateTimeOffset at)
    {
        await using var scope = database.CreateScope();
        await scope.ServiceProvider.GetRequiredService<RecordInvitationWeekPassed>().HandleAsync(invitation, at, TestContext.Current.CancellationToken);
    }
}
