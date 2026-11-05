using Memorio.Modules.Notifications.Application.Policies;
using Memorio.Modules.Plans.Contracts;
using Microsoft.Extensions.DependencyInjection;

namespace Memorio.Modules.Notifications.Infrastructure.Messaging;

/// <summary>E97 in the worker: only resolves this module's policy and calls it (D-135, D-252).</summary>
public static class TrimWarningDueHandler
{
    public static Task Handle(TrimWarningDueEvent integrationEvent, IServiceProvider services, CancellationToken cancellationToken) =>
        services.GetRequiredService<WarnWhenDue>().HandleAsync(integrationEvent, cancellationToken);
}

/// <summary>E100 in the worker.</summary>
public static class TrimHeldHandler
{
    public static Task Handle(TrimHeldEvent integrationEvent, IServiceProvider services, CancellationToken cancellationToken) =>
        services.GetRequiredService<RetryWhileHeld>().HandleAsync(integrationEvent, cancellationToken);
}

/// <summary>E94 in the worker.</summary>
public static class PlanActivatedHandler
{
    public static Task Handle(PlanActivatedEvent integrationEvent, IServiceProvider services, CancellationToken cancellationToken) =>
        services.GetRequiredService<RetryWhileHeld>().HandleAsync(integrationEvent, cancellationToken);
}
