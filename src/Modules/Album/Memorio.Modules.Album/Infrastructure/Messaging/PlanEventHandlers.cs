using Memorio.Modules.Album.Application.Policies;
using Memorio.Modules.Plans.Contracts;
using Microsoft.Extensions.DependencyInjection;

namespace Memorio.Modules.Album.Infrastructure.Messaging;

/// <summary>E103 in the worker: only resolves this module's policy and calls it (D-135, D-252).</summary>
public static class TrimOrderedHandler
{
    public static Task Handle(TrimOrderedEvent integrationEvent, IServiceProvider services, CancellationToken cancellationToken) =>
        services.GetRequiredService<TrimAlbumWhenOrdered>().HandleAsync(integrationEvent, cancellationToken);
}

/// <summary>E94 in the worker.</summary>
public static class PlanActivatedHandler
{
    public static Task Handle(PlanActivatedEvent integrationEvent, IServiceProvider services, CancellationToken cancellationToken) =>
        services.GetRequiredService<RestoreTrimmedWhenPlanActive>().HandleAsync(integrationEvent, cancellationToken);
}
