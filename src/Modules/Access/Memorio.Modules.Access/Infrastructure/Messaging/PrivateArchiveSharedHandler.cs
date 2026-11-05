using Memorio.Modules.Access.Application.Policies;
using Memorio.Modules.Succession.Contracts;
using Microsoft.Extensions.DependencyInjection;

namespace Memorio.Modules.Access.Infrastructure.Messaging;

/// <summary>E126 in the worker: only resolves this module's policy and calls it (D-135, D-252).</summary>
public static class PrivateArchiveSharedHandler
{
    public static Task Handle(PrivateArchiveSharedEvent integrationEvent, IServiceProvider services, CancellationToken cancellationToken) =>
        services.GetRequiredService<GrantLegacyRead>().HandleAsync(integrationEvent, cancellationToken);
}
