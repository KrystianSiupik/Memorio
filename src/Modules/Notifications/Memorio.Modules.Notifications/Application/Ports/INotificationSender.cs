namespace Memorio.Modules.Notifications.Application.Ports;

internal enum NotificationKind
{
    Invitation,
    InvitationReminder,
    PoolFull,
    ChooseKeepOrTake,
    Tagged,
}

/// <summary>Whatever delivers the message. Only the kind and the album: the text is built from them at the last moment (<see cref="NotificationTexts"/>).</summary>
internal interface INotificationSender
{
    /// <param name="invitedBy">The address of who invites, for messages about an invitation (D-268).</param>
    Task SendAsync(string email, NotificationKind kind, Guid albumId, string? invitedBy, CancellationToken cancellationToken);
}
