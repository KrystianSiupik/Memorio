using Memorio.Modules.Moderation.Contracts;
using Memorio.Modules.Notifications.Application.Policies;
using Microsoft.Extensions.DependencyInjection;

namespace Memorio.Modules.Notifications.Infrastructure.Messaging;

/// <summary>E110 in the worker: only resolves this module's policy and calls it (D-135, D-252).</summary>
public static class ContentReportedHandler
{
    public static Task Handle(ContentReportedEvent integrationEvent, IServiceProvider services, CancellationToken cancellationToken) =>
        services.GetRequiredService<NotifyOfModeration>().HandleAsync(integrationEvent, cancellationToken);
}

/// <summary>E112 in the worker.</summary>
public static class ReportDecidedHandler
{
    public static Task Handle(ReportDecidedEvent integrationEvent, IServiceProvider services, CancellationToken cancellationToken) =>
        services.GetRequiredService<NotifyOfModeration>().HandleAsync(integrationEvent, cancellationToken);
}

/// <summary>An order executed, in the worker.</summary>
public static class RemovalOrderExecutedHandler
{
    public static Task Handle(RemovalOrderExecutedEvent integrationEvent, IServiceProvider services, CancellationToken cancellationToken) =>
        services.GetRequiredService<NotifyOfOrder>().HandleAsync(integrationEvent, cancellationToken);
}

/// <summary>The decision on an appeal in the worker.</summary>
public static class AppealDecidedHandler
{
    public static Task Handle(AppealDecidedEvent integrationEvent, IServiceProvider services, CancellationToken cancellationToken) =>
        services.GetRequiredService<NotifyOfModeration>().HandleAsync(integrationEvent, cancellationToken);
}
