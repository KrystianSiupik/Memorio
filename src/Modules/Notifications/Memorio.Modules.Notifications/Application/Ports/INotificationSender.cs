using Memorio.SharedKernel.Messaging;

namespace Memorio.Modules.Notifications.Application.Ports;

internal enum NotificationKind
{
    Invitation,
    InvitationReminder,
    PoolFull,
    ChooseKeepOrTake,
    Tagged,
    TrimWarningAlbum,
    TrimWarningAccount,
    NewOwner,
    AlbumWithoutOwner,
    TakeoverOffered,
    SuccessorReminder,
    DeathReportRejected,
    DeathReported,
    PrivateArchiveShared,
}

/// <summary>What became of a message. Only <see cref="Delivered"/> is proof: the trim of an archive waits without it (S2:C10).</summary>
internal enum DeliveryOutcome
{
    /// <summary>The provider took it.</summary>
    Delivered,

    /// <summary>The address does not work, and another attempt now will not help.</summary>
    Refused,

    /// <summary>No provider is set up, so nobody was told.</summary>
    NotSent,
}

/// <summary>Whatever delivers the message. Only the kind and the album: the text is built from them at the last moment (<see cref="NotificationTexts"/>).</summary>
internal interface INotificationSender
{
    /// <param name="detail">Who invites, or when the trim comes; what the text of this kind names (D-268).</param>
    /// <returns>A provider that does not answer throws instead, so the event is retried.</returns>
    Task<DeliveryOutcome> SendAsync(string email, NotificationKind kind, Guid albumId, string? detail, CancellationToken cancellationToken);
}

/// <summary>E98 and E99 reach the outbox in their own transaction: the messages are out, whatever happens next.</summary>
internal interface ITrimWarningReports
{
    Task PublishAsync<TEvent>(TEvent report, CancellationToken cancellationToken)
        where TEvent : IIntegrationEvent;
}
