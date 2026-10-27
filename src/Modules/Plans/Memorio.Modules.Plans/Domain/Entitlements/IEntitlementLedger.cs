namespace Memorio.Modules.Plans.Domain.Entitlements;

/// <summary>The entitlement ledger: lines are only added, never changed or deleted (D-078).</summary>
internal interface IEntitlementLedger
{
    /// <summary>Adds the line; false when this notification of the source is in the ledger already.</summary>
    Task<bool> AppendAsync(EntitlementEntry entry, CancellationToken cancellationToken);

    Task<IReadOnlyList<EntitlementEntry>> EntriesOfAsync(EntitledResource resource, CancellationToken cancellationToken);
}
