using Memorio.Modules.Plans.Domain.Entitlements;

namespace Memorio.Modules.Plans.Application.Entitlements;

/// <summary>
/// How the plan cycle (AG8, E7: active, read-only, trimming) learns whether a resource is paid for: from the
/// ledger only, never from a payment source (D-078). A new source (web, invoices, one-off purchases, an album
/// taken over, D-004) is a new adapter writing to the ledger and changes nothing here or in the cycle.
/// </summary>
internal sealed class PlanEntitlements(IEntitlementLedger ledger)
{
    public async Task<DateTimeOffset?> EntitledUntilAsync(EntitledResource resource, CancellationToken cancellationToken) =>
        EntitlementRules.EntitledUntil(await ledger.EntriesOfAsync(resource, cancellationToken));
}
