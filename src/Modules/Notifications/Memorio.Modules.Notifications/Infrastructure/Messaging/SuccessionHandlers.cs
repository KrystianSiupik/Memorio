using Memorio.Modules.Notifications.Application.Policies;
using Memorio.Modules.Succession.Contracts;
using Microsoft.Extensions.DependencyInjection;

namespace Memorio.Modules.Notifications.Infrastructure.Messaging;

/// <summary>The offer in the worker: only resolves this module's policy and calls it (D-135, D-252).</summary>
public static class TakeoverOfferedHandler
{
    public static Task Handle(TakeoverOfferedEvent integrationEvent, IServiceProvider services, CancellationToken cancellationToken) =>
        services.GetRequiredService<NotifySuccessor>().HandleAsync(integrationEvent, cancellationToken);
}

/// <summary>E122 in the worker.</summary>
public static class DeathReportRejectedHandler
{
    public static Task Handle(DeathReportRejectedEvent integrationEvent, IServiceProvider services, CancellationToken cancellationToken) =>
        services.GetRequiredService<NotifyOfRejectedReport>().HandleAsync(integrationEvent, cancellationToken);
}

/// <summary>E120 in the worker.</summary>
public static class DeathReportedHandler
{
    public static Task Handle(DeathReportedEvent integrationEvent, IServiceProvider services, CancellationToken cancellationToken) =>
        services.GetRequiredService<NotifyOwnerOfReport>().HandleAsync(integrationEvent, cancellationToken);
}

/// <summary>E126 in the worker.</summary>
public static class PrivateArchiveSharedHandler
{
    public static Task Handle(PrivateArchiveSharedEvent integrationEvent, IServiceProvider services, CancellationToken cancellationToken) =>
        services.GetRequiredService<NotifyLegacyContact>().HandleAsync(integrationEvent, cancellationToken);
}
