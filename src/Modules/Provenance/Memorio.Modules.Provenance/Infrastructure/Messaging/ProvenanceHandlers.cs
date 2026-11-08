using Memorio.Modules.Archive.Contracts;
using Memorio.Modules.Provenance.Application.Policies;
using Microsoft.Extensions.DependencyInjection;

namespace Memorio.Modules.Provenance.Infrastructure.Messaging;

/// <summary>P4 in the worker: only resolves this module's policy and calls it (D-135).</summary>
public static class ContentDeletedHandler
{
    public static Task Handle(ContentDeleted integrationEvent, IServiceProvider services, CancellationToken cancellationToken) =>
        services.GetRequiredService<InvalidateWhenSourceGone>().HandleAsync(integrationEvent, cancellationToken);
}
