using Memorio.Modules.Archive.Contracts;
using Memorio.SharedKernel.Messaging;
using Microsoft.Extensions.DependencyInjection;

namespace Memorio.Modules.Archive.Infrastructure.Messaging;

/// <summary>E20 in the worker: only resolves the policy and calls it (D-135).</summary>
public static class MemoryMovedToTrashHandler
{
    public static Task Handle(MemoryMovedToTrashEvent moved, IServiceProvider services, CancellationToken cancellationToken) =>
        services.GetRequiredService<IIntegrationEventPolicy<MemoryMovedToTrashEvent>>().HandleAsync(moved, cancellationToken);
}
