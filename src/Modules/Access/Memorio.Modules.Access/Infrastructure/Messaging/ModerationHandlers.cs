using Memorio.Modules.Access.Application.Policies;
using Memorio.Modules.Moderation.Contracts;
using Microsoft.Extensions.DependencyInjection;

namespace Memorio.Modules.Access.Infrastructure.Messaging;

/// <summary>E113 in the worker: only resolves this module's policy and calls it (D-135, D-252).</summary>
public static class ContentHiddenHandler
{
    public static Task Handle(ContentHiddenEvent integrationEvent, IServiceProvider services, CancellationToken cancellationToken) =>
        services.GetRequiredService<HideWhenContentHidden>().HandleAsync(integrationEvent, cancellationToken);
}

/// <summary>The end of a hold in the worker.</summary>
public static class ContentRestoredHandler
{
    public static Task Handle(ContentRestoredEvent integrationEvent, IServiceProvider services, CancellationToken cancellationToken) =>
        services.GetRequiredService<HideWhenContentHidden>().HandleAsync(integrationEvent, cancellationToken);
}
