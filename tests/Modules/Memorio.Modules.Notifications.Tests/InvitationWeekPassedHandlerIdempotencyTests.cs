using Memorio.Modules.Album.Contracts;
using Memorio.Modules.Notifications.Application.Policies;
using Microsoft.Extensions.DependencyInjection;

namespace Memorio.Modules.Notifications.Tests;

/// <summary>SD6, MEM-E3-POWIADOMIENIA-01 and -07: E45 delivered twice on a real database.</summary>
public sealed class InvitationWeekPassedHandlerIdempotencyTests(NotificationsDatabase database)
{
    private static CancellationToken Cancellation => TestContext.Current.CancellationToken;

    [Fact]
    public async Task One_reminder_only()
    {
        var invitation = database.Neighbours.NewInvitation();
        var email = database.Neighbours.AddressOf(invitation);
        var passed = new InvitationWeekPassedEvent(Guid.CreateVersion7(), DateTimeOffset.UtcNow, Guid.CreateVersion7(), invitation, 3);

        await DeliverAsync(passed);
        await DeliverAsync(passed);

        Assert.Equal(1, database.Neighbours.SentTo(email, "InvitationReminder"));
    }

    [Fact]
    public async Task An_answered_invitation_gets_no_reminder()
    {
        var invitation = database.Neighbours.NewInvitation();
        var email = database.Neighbours.AddressOf(invitation);
        database.Neighbours.Answered(invitation);

        await DeliverAsync(new InvitationWeekPassedEvent(Guid.CreateVersion7(), DateTimeOffset.UtcNow, Guid.CreateVersion7(), invitation, 3));

        Assert.Equal(0, database.Neighbours.SentTo(email, "InvitationReminder"));
    }

    private async Task DeliverAsync(InvitationWeekPassedEvent passed)
    {
        await using var scope = database.CreateScope();
        await scope.ServiceProvider.GetRequiredService<SendInvitationReminder>().HandleAsync(passed, Cancellation);
    }
}
