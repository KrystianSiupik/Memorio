using Memorio.Modules.Plans.Domain.Entitlements;
using Memorio.SharedKernel.Results;

namespace Memorio.Modules.Plans.Tests;

/// <summary>What the ledger says about one resource (D-078), without a database.</summary>
public sealed class EntitlementRulesTests
{
    private static readonly EntitledResource Album = new(EntitledResourceKind.Album, Guid.NewGuid());
    private static readonly DateTimeOffset Now = new(2026, 10, 27, 12, 0, 0, TimeSpan.Zero);

    [Fact]
    public void No_line_is_never_entitled()
    {
        Assert.Null(EntitlementRules.EntitledUntil([]));
        Assert.False(EntitlementRules.IsEntitled([], Now));
    }

    [Fact]
    public void A_notification_arriving_late_does_not_undo_a_newer_one()
    {
        var renewed = Line("store", "n2", validUntil: Now.AddMonths(2), occurredAt: Now.AddDays(-1), recordedAt: Now);
        var olderArrivingLater = Line("store", "n1", validUntil: Now.AddMonths(1), occurredAt: Now.AddDays(-30), recordedAt: Now.AddMinutes(5));

        Assert.Equal(Now.AddMonths(2), EntitlementRules.EntitledUntil([renewed, olderArrivingLater]));
    }

    [Fact]
    public void A_refund_ends_what_its_source_granted()
    {
        var paid = Line("store", "n1", validUntil: Now.AddYears(1), occurredAt: Now.AddDays(-10));
        var refunded = Line("store", "n2", validUntil: Now.AddDays(-2), occurredAt: Now.AddDays(-2));

        Assert.Equal(Now.AddDays(-2), EntitlementRules.EntitledUntil([paid, refunded]));
        Assert.False(EntitlementRules.IsEntitled([paid, refunded], Now));
    }

    [Fact]
    public void Sources_do_not_end_each_other_the_latest_end_counts()
    {
        var store = Line("store", "n1", validUntil: Now.AddMonths(1), occurredAt: Now.AddDays(-5));
        var manualEnded = Line(EntitlementSources.Manual, "m1", validUntil: Now.AddDays(-1), occurredAt: Now.AddDays(-1), note: "tester left");

        Assert.Equal(Now.AddMonths(1), EntitlementRules.EntitledUntil([store, manualEnded]));
        Assert.True(EntitlementRules.IsEntitled([store, manualEnded], Now));
    }

    [Fact]
    public void A_manual_line_needs_a_reason_and_every_line_its_source_and_resource()
    {
        Assert.Equal(EntitlementErrors.ReasonRequired, Create(EntitlementSources.Manual, "m1", Album, note: " ").Error);
        Assert.Equal(EntitlementErrors.SourceRequired, Create("store", "", Album).Error);
        Assert.Equal(EntitlementErrors.ResourceRequired, Create("store", "n1", new(EntitledResourceKind.Account, Guid.Empty)).Error);
        Assert.True(Create("store", "n1", Album).IsSuccess);
    }

    private static Result<EntitlementEntry> Create(string source, string reference, EntitledResource resource, string? note = null) =>
        EntitlementEntry.Create(source, reference, resource, Now.AddMonths(1), Now, Now, note);

    private static EntitlementEntry Line(
        string source,
        string reference,
        DateTimeOffset validUntil,
        DateTimeOffset occurredAt,
        DateTimeOffset? recordedAt = null,
        string? note = null) =>
        EntitlementEntry.Create(source, reference, Album, validUntil, occurredAt, recordedAt ?? occurredAt, note).Value;
}
