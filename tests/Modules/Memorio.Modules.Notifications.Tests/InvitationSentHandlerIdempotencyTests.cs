using Memorio.Modules.Album.Contracts;
using Memorio.Modules.Notifications.Application.Policies;
using Microsoft.Extensions.DependencyInjection;

namespace Memorio.Modules.Notifications.Tests;

/// <summary>SD6, E42 delivered twice on a real database.</summary>
public sealed class InvitationSentHandlerIdempotencyTests(NotificationsDatabase database)
{
    private static CancellationToken Cancellation => TestContext.Current.CancellationToken;

    [Fact]
    public async Task The_invitation_goes_once_also_when_the_first_send_failed()
    {
        var invitation = database.Neighbours.NewInvitation();
        var email = database.Neighbours.AddressOf(invitation);
        var sent = new InvitationSentEvent(Guid.CreateVersion7(), DateTimeOffset.UtcNow, Guid.CreateVersion7(), invitation, 2);
        database.Neighbours.FailNextSendTo(email);

        await Assert.ThrowsAsync<InvalidOperationException>(() => DeliverAsync(sent));
        await DeliverAsync(sent);
        await DeliverAsync(sent);

        Assert.Equal(1, database.Neighbours.SentTo(email, "Invitation"));
    }

    [Fact]
    public async Task A_refresh_sends_again_and_a_late_older_event_does_not()
    {
        var invitation = database.Neighbours.NewInvitation();
        var email = database.Neighbours.AddressOf(invitation);
        var album = Guid.CreateVersion7();

        await DeliverAsync(new InvitationSentEvent(Guid.CreateVersion7(), DateTimeOffset.UtcNow, album, invitation, 5));
        await DeliverAsync(new InvitationSentEvent(Guid.CreateVersion7(), DateTimeOffset.UtcNow, album, invitation, 3));
        Assert.Equal(1, database.Neighbours.SentTo(email, "Invitation"));

        await DeliverAsync(new InvitationSentEvent(Guid.CreateVersion7(), DateTimeOffset.UtcNow, album, invitation, 7));
        Assert.Equal(2, database.Neighbours.SentTo(email, "Invitation"));
    }

    [Fact]
    public async Task An_answered_invitation_and_a_blocked_address_get_nothing()
    {
        var answered = database.Neighbours.NewInvitation();
        var answeredEmail = database.Neighbours.AddressOf(answered);
        database.Neighbours.Answered(answered);

        await DeliverAsync(new InvitationSentEvent(Guid.CreateVersion7(), DateTimeOffset.UtcNow, Guid.CreateVersion7(), answered, 2));

        Assert.Equal(0, database.Neighbours.SentTo(answeredEmail, "Invitation"));
    }

    private async Task DeliverAsync(InvitationSentEvent sent)
    {
        await using var scope = database.CreateScope();
        await scope.ServiceProvider.GetRequiredService<SendInvitation>().HandleAsync(sent, Cancellation);
    }
}
