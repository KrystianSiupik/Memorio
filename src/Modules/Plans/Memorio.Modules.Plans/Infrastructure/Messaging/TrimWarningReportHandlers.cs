using Memorio.Modules.Notifications.Contracts;
using Memorio.Modules.Plans.Application.Policies;
using Microsoft.Extensions.DependencyInjection;

namespace Memorio.Modules.Plans.Infrastructure.Messaging;

/// <summary>E98 in the worker: only resolves this module's policy and calls it (D-135, D-252).</summary>
public static class TrimWarningDeliveredHandler
{
    public static Task Handle(TrimWarningDeliveredEvent integrationEvent, IServiceProvider services, CancellationToken cancellationToken) =>
        services.GetRequiredService<RecordWarningDelivered>().HandleAsync(integrationEvent, cancellationToken);
}

/// <summary>E99 in the worker.</summary>
public static class TrimWarningUndeliveredHandler
{
    public static Task Handle(TrimWarningUndeliveredEvent integrationEvent, IServiceProvider services, CancellationToken cancellationToken) =>
        services.GetRequiredService<HoldTrimWhenUndelivered>().HandleAsync(integrationEvent, cancellationToken);
}
