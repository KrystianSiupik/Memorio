using Memorio.Modules.Album.Application.Policies;
using Memorio.Modules.Archive.Contracts;
using Microsoft.Extensions.DependencyInjection;

namespace Memorio.Modules.Album.Infrastructure.Messaging;

/// <summary>E22 in the worker: only resolves this module's policy and calls it (D-135, D-252).</summary>
public static class ContentDeletedHandler
{
    public static Task Handle(ContentDeleted integrationEvent, IServiceProvider services, CancellationToken cancellationToken) =>
        services.GetRequiredService<ForgetContentWhenDeleted>().HandleAsync(integrationEvent, cancellationToken);
}
