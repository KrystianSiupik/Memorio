using Memorio.Modules.Plans.Application.Commands.RecordEntitlement;
using Memorio.Modules.Plans.Domain.Entitlements;

namespace Memorio.Modules.Plans.Application.Ports;

/// <summary>
/// A payment source asked about the current state of a resource (S2:C3: waiting for its notifications is not enough).
/// An adapter translates the answer into ledger lines (D-078); a line recorded before changes nothing.
/// </summary>
internal interface ISubscriptionSource
{
    Task<IReadOnlyList<RecordEntitlement>> FetchAsync(EntitledResource resource, CancellationToken cancellationToken);
}
