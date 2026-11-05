using Memorio.Modules.Album.Contracts;
using Memorio.Modules.Plans.Application.Policies;
using Microsoft.Extensions.DependencyInjection;

namespace Memorio.Modules.Plans.Infrastructure.Messaging;

/// <summary>E162 in the worker: only resolves this module's policy and calls it (D-135, D-252).</summary>
public static class AlbumWithoutOwnerHandler
{
    public static Task Handle(AlbumWithoutOwnerEvent integrationEvent, IServiceProvider services, CancellationToken cancellationToken) =>
        services.GetRequiredService<MarkOwnerlessWhenAlbumLostOwner>().HandleAsync(integrationEvent, cancellationToken);
}

/// <summary>E167 in the worker.</summary>
public static class AlbumTakenOverHandler
{
    public static Task Handle(AlbumTakenOverEvent integrationEvent, IServiceProvider services, CancellationToken cancellationToken) =>
        services.GetRequiredService<ActivateWhenAlbumTakenOver>().HandleAsync(integrationEvent, cancellationToken);
}
