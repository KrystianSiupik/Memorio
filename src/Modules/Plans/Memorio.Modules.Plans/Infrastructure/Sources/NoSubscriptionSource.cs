using Memorio.Modules.Plans.Application.Commands.RecordEntitlement;
using Memorio.Modules.Plans.Application.Ports;
using Memorio.Modules.Plans.Domain.Entitlements;

namespace Memorio.Modules.Plans.Infrastructure.Sources;

/// <summary>No payment source is connected yet (D-281): asking finds nothing, and the ledger holds only manual lines.</summary>
internal sealed class NoSubscriptionSource : ISubscriptionSource
{
    public Task<IReadOnlyList<RecordEntitlement>> FetchAsync(EntitledResource resource, CancellationToken cancellationToken) =>
        Task.FromResult<IReadOnlyList<RecordEntitlement>>([]);
}
