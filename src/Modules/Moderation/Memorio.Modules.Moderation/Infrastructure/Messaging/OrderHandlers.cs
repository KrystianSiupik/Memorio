using Memorio.Modules.Moderation.Application.Policies;
using Memorio.Modules.Moderation.Contracts;
using Microsoft.Extensions.DependencyInjection;

namespace Memorio.Modules.Moderation.Infrastructure.Messaging;

/// <summary>E115 in the worker: only resolves this module's policy and calls it (D-135, D-252).</summary>
public static class RemovalOrderReceivedHandler
{
    public static Task Handle(RemovalOrderReceivedEvent integrationEvent, IServiceProvider services, CancellationToken cancellationToken) =>
        services.GetRequiredService<HideWhenRemovalOrdered>().HandleAsync(integrationEvent, cancellationToken);
}

/// <summary>E116 in the worker.</summary>
public static class DataOrderReceivedHandler
{
    public static Task Handle(DataOrderReceivedEvent integrationEvent, IServiceProvider services, CancellationToken cancellationToken) =>
        services.GetRequiredService<SecureWhenDataOrdered>().HandleAsync(integrationEvent, cancellationToken);
}
