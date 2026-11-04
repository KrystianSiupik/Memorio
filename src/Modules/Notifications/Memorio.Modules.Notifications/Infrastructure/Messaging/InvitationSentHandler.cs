using Memorio.Modules.Album.Contracts;
using Memorio.Modules.Notifications.Application.Policies;
using Microsoft.Extensions.DependencyInjection;

namespace Memorio.Modules.Notifications.Infrastructure.Messaging;

/// <summary>E42 in the worker: only resolves this module's policy and calls it (D-135, D-252).</summary>
public static class InvitationSentHandler
{
    public static Task Handle(InvitationSentEvent integrationEvent, IServiceProvider services, CancellationToken cancellationToken) =>
        services.GetRequiredService<SendInvitation>().HandleAsync(integrationEvent, cancellationToken);
}
