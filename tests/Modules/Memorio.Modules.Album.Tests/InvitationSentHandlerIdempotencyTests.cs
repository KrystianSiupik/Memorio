using Memorio.Modules.Album.Application.Policies;
using Memorio.Modules.Album.Contracts;
using Microsoft.Extensions.DependencyInjection;

namespace Memorio.Modules.Album.Tests;

/// <summary>SD6: E42 delivered twice, and after the answer, on a real database.</summary>
public sealed class InvitationSentHandlerIdempotencyTests(AlbumDatabase database)
{
    [Fact]
    public async Task The_same_event_twice_leaves_one_deadline_seven_days_later()
    {
        var owner = database.Neighbours.NewAccount();
        var album = await database.NewAlbumAsync(owner);
        var invitation = Guid.CreateVersion7();
        await database.InviteAsync(album, owner, "a@example.invalid", invitation);
        var sent = new InvitationSentEvent(Guid.CreateVersion7(), DateTimeOffset.UtcNow, album.Value, invitation, 2);

        await DeliverAsync(sent);
        await DeliverAsync(sent);

        var deadline = database.Deadlines.Find(InvitationReminderDeadline.JobName, invitation.ToString());
        Assert.NotNull(deadline);
        Assert.InRange(deadline.ScheduledFor - DateTimeOffset.UtcNow, TimeSpan.FromDays(6.9), TimeSpan.FromDays(7.1));
    }

    [Fact]
    public async Task An_invitation_already_answered_gets_no_deadline()
    {
        var person = database.Neighbours.NewAccount();
        var owner = database.Neighbours.NewAccount();
        var album = await database.NewAlbumAsync(owner);
        var invitation = Guid.CreateVersion7();
        await database.InviteAsync(album, owner, database.Neighbours.EmailOf(person), invitation);
        await database.AnswerAsync(invitation, person, accepted: false);

        await DeliverAsync(new InvitationSentEvent(Guid.CreateVersion7(), DateTimeOffset.UtcNow, album.Value, invitation, 2));

        Assert.Null(database.Deadlines.Find(InvitationReminderDeadline.JobName, invitation.ToString()));
    }

    private async Task DeliverAsync(InvitationSentEvent sent)
    {
        await using var scope = database.CreateScope();
        await new InvitationReminderDeadline(
            scope.ServiceProvider.GetRequiredService<Domain.Albums.IAlbums>(),
            database.Deadlines).HandleAsync(sent, TestContext.Current.CancellationToken);
    }
}
