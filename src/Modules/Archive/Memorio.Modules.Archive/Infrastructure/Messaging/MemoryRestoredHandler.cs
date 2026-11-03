using Memorio.Modules.Archive.Contracts;
using Memorio.SharedKernel.Messaging;
using Microsoft.Extensions.DependencyInjection;

namespace Memorio.Modules.Archive.Infrastructure.Messaging;

/// <summary>E24 in the worker: only resolves the policy and calls it (D-135).</summary>
public static class MemoryRestoredHandler
{
    public static Task Handle(MemoryRestoredEvent restored, IServiceProvider services, CancellationToken cancellationToken) =>
        services.GetRequiredService<IIntegrationEventPolicy<MemoryRestoredEvent>>().HandleAsync(restored, cancellationToken);
}
