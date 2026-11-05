using Memorio.Modules.Account.Contracts;
using Memorio.Modules.Album.Contracts;
using Memorio.Modules.Notifications.Application.Ports;
using Memorio.Modules.Notifications.Contracts;
using Memorio.SharedKernel.Messaging;

namespace Memorio.Modules.Notifications.Application.Policies;

/// <summary>E42: the invitation itself. The block is checked again at sending, the cost of the race is one message (D-012).</summary>
internal sealed class SendInvitation(IAlbumMessaging albums, IInvitationBlocks blocks, IAccountDirectory directory, SendOnce send) : IIntegrationEventPolicy<InvitationSentEvent>
{
    public async Task HandleAsync(InvitationSentEvent integrationEvent, CancellationToken cancellationToken)
    {
        var invitation = await albums.FindPendingInvitationAsync(integrationEvent.InvitationId, cancellationToken);
        if (invitation is null || await blocks.IsBlockedAsync(invitation.Email, cancellationToken))
        {
            return;
        }

        await send.SendAsync($"invitation:{integrationEvent.InvitationId}", integrationEvent.Sequence, invitation.Email, NotificationKind.Invitation, integrationEvent.AlbumId, cancellationToken, await directory.EmailOfAsync(invitation.InvitedByAccountId, cancellationToken));
    }
}

/// <summary>P6 and C43: one reminder after 7 days without an answer.</summary>
internal sealed class SendInvitationReminder(IAlbumMessaging albums, IInvitationBlocks blocks, IAccountDirectory directory, SendOnce send) : IIntegrationEventPolicy<InvitationWeekPassedEvent>
{
    public async Task HandleAsync(InvitationWeekPassedEvent integrationEvent, CancellationToken cancellationToken)
    {
        var invitation = await albums.FindPendingInvitationAsync(integrationEvent.InvitationId, cancellationToken);
        if (invitation is null || await blocks.IsBlockedAsync(invitation.Email, cancellationToken))
        {
            return;
        }

        await send.SendAsync($"reminder:{integrationEvent.InvitationId}", integrationEvent.Sequence, invitation.Email, NotificationKind.InvitationReminder, integrationEvent.AlbumId, cancellationToken, await directory.EmailOfAsync(invitation.InvitedByAccountId, cancellationToken));
    }
}

/// <summary>P7 and C48: the owner hears a full pool once per album and day, however often something does not fit.</summary>
internal sealed class NotifyOwnerOfFullPool(IAccountDirectory directory, SendOnce send) : IIntegrationEventPolicy<AlbumPoolExhaustedEvent>
{
    public async Task HandleAsync(AlbumPoolExhaustedEvent integrationEvent, CancellationToken cancellationToken)
    {
        if (await directory.EmailOfAsync(integrationEvent.OwnerAccountId, cancellationToken) is not { } email)
        {
            return;
        }

        await send.SendAsync(
            $"pool-full:{integrationEvent.AlbumId}:{integrationEvent.OccurredAt.UtcDateTime:yyyyMMdd}",
            integrationEvent.Sequence,
            email,
            NotificationKind.PoolFull,
            integrationEvent.AlbumId,
            cancellationToken);
    }
}

/// <summary>P8 and C54: whoever was removed chooses what happens to what they brought.</summary>
internal sealed class AskToChooseKeepOrTake(IAccountDirectory directory, SendOnce send) : IIntegrationEventPolicy<ParticipantRemovedEvent>
{
    public async Task HandleAsync(ParticipantRemovedEvent integrationEvent, CancellationToken cancellationToken)
    {
        if (await directory.EmailOfAsync(integrationEvent.AccountId, cancellationToken) is not { } email)
        {
            return;
        }

        await send.SendAsync($"choose:{integrationEvent.AlbumId}:{integrationEvent.AccountId}", integrationEvent.Sequence, email, NotificationKind.ChooseKeepOrTake, integrationEvent.AlbumId, cancellationToken);
    }
}

/// <summary>P45 and C171: the tagged person hears it and can remove the tag.</summary>
internal sealed class NotifyTagged(IAccountDirectory directory, SendOnce send) : IIntegrationEventPolicy<ParticipantTaggedEvent>
{
    public async Task HandleAsync(ParticipantTaggedEvent integrationEvent, CancellationToken cancellationToken)
    {
        if (await directory.EmailOfAsync(integrationEvent.AccountId, cancellationToken) is not { } email)
        {
            return;
        }

        await send.SendAsync($"tagged:{integrationEvent.MemoryId}:{integrationEvent.AccountId}", integrationEvent.Sequence, email, NotificationKind.Tagged, integrationEvent.AlbumId, cancellationToken);
    }
}
