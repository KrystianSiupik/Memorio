using Memorio.Modules.Account.Contracts;
using Memorio.Modules.Album.Contracts;
using Memorio.Modules.Notifications.Application.Ports;
using Memorio.SharedKernel.Messaging;

namespace Memorio.Modules.Notifications.Application.Policies;

/// <summary>P42 and C166: everyone but the new owner hears it, and may leave with the choice to keep or take (D-004, S2:C10).</summary>
internal sealed class NotifyOfNewOwner(IAlbumMessaging albums, IAccountDirectory directory, SendOnce send)
    : IIntegrationEventPolicy<AlbumTakenOverEvent>, IIntegrationEventPolicy<AlbumOwnerChangedEvent>, IIntegrationEventPolicy<AlbumWithoutOwnerEvent>
{
    public Task HandleAsync(AlbumTakenOverEvent integrationEvent, CancellationToken cancellationToken) =>
        TellAsync(integrationEvent.AlbumId, integrationEvent.OwnerAccountId, integrationEvent.Sequence, NotificationKind.NewOwner, cancellationToken);

    public Task HandleAsync(AlbumOwnerChangedEvent integrationEvent, CancellationToken cancellationToken) =>
        TellAsync(integrationEvent.AlbumId, integrationEvent.OwnerAccountId, integrationEvent.Sequence, NotificationKind.NewOwner, cancellationToken);

    public Task HandleAsync(AlbumWithoutOwnerEvent integrationEvent, CancellationToken cancellationToken) =>
        TellAsync(integrationEvent.AlbumId, null, integrationEvent.Sequence, NotificationKind.AlbumWithoutOwner, cancellationToken);

    private async Task TellAsync(Guid albumId, Guid? except, int sequence, NotificationKind kind, CancellationToken cancellationToken)
    {
        foreach (var person in await albums.MembersOfAsync(albumId, cancellationToken))
        {
            if (person != except && await directory.EmailOfAsync(person, cancellationToken) is { } email)
            {
                await send.SendAsync($"{kind}:{albumId}:{sequence}:{person}", sequence, email, kind, albumId, cancellationToken);
            }
        }
    }
}
