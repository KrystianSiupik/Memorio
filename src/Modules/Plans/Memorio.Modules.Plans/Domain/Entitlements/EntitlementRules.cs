namespace Memorio.Modules.Plans.Domain.Entitlements;

/// <summary>What the ledger says about one resource (D-078).</summary>
internal static class EntitlementRules
{
    /// <summary>
    /// Until when the resource is entitled, from its ledger lines only. Of each source only the line that
    /// happened last at the source counts, so a notification arriving late does not undo a newer one (S2:C3)
    /// and a refund or cancellation ends what that source granted; the resource is entitled until the
    /// latest of those. Null: no line, never entitled.
    /// </summary>
    public static DateTimeOffset? EntitledUntil(IEnumerable<EntitlementEntry> entries)
    {
        var latestOfEachSource = entries
            .GroupBy(entry => entry.Source, StringComparer.Ordinal)
            .Select(lines => lines.OrderBy(line => line.OccurredAt).ThenBy(line => line.RecordedAt).Last().ValidUntil)
            .ToList();

        return latestOfEachSource.Count == 0 ? null : latestOfEachSource.Max();
    }

    public static bool IsEntitled(IEnumerable<EntitlementEntry> entries, DateTimeOffset at) => EntitledUntil(entries) > at;
}
