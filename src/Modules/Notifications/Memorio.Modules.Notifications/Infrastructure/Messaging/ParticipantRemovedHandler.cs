using Memorio.Modules.Album.Contracts;
using Memorio.Modules.Notifications.Application.Policies;
using Microsoft.Extensions.DependencyInjection;

namespace Memorio.Modules.Notifications.Infrastructure.Messaging;

/// <summary>E59 in the worker: only resolves this module's policy and calls it (D-135, D-252).</summary>
public static class ParticipantRemovedHandler
{
    public static Task Handle(ParticipantRemovedEvent integrationEvent, IServiceProvider services, CancellationToken cancellationToken) =>
        services.GetRequiredService<AskToChooseKeepOrTake>().HandleAsync(integrationEvent, cancellationToken);
}
