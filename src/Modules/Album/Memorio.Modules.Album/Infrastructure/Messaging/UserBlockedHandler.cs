using Memorio.Modules.Access.Contracts;
using Memorio.Modules.Album.Application.Policies;
using Microsoft.Extensions.DependencyInjection;

namespace Memorio.Modules.Album.Infrastructure.Messaging;

/// <summary>E67 in the worker: only resolves this module's policy and calls it (D-135, D-252).</summary>
public static class UserBlockedHandler
{
    public static Task Handle(UserBlockedEvent integrationEvent, IServiceProvider services, CancellationToken cancellationToken) =>
        services.GetRequiredService<RemoveBlockedParticipant>().HandleAsync(integrationEvent, cancellationToken);
}
