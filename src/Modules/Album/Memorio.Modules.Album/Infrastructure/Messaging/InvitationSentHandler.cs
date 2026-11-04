using Memorio.Modules.Album.Contracts;
using Memorio.Modules.Album.Application.Policies;
using Microsoft.Extensions.DependencyInjection;

namespace Memorio.Modules.Album.Infrastructure.Messaging;

/// <summary>E42 in the worker: only resolves this module's policy and calls it (D-135, D-252).</summary>
public static class InvitationSentHandler
{
    public static Task Handle(InvitationSentEvent integrationEvent, IServiceProvider services, CancellationToken cancellationToken) =>
        services.GetRequiredService<InvitationReminderDeadline>().HandleAsync(integrationEvent, cancellationToken);
}
