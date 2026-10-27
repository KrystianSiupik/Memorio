namespace Memorio.Modules.Plans.Domain.Entitlements;

/// <summary>
/// Sources of the ledger (D-078). Each payment source is an adapter in Infrastructure/Sources/&lt;Source&gt;
/// that turns the source's notifications into <c>RecordEntitlement</c>; the stores (directly or through a
/// subscription service) come with the payment tickets of E7.
/// </summary>
internal static class EntitlementSources
{
    /// <summary>Granted or ended by an operator (deploy/plans/entitlements.sh): testers, goodwill, a complaint settled.</summary>
    public const string Manual = "manual";
}
