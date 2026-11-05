using Memorio.Modules.Plans.Domain.Cycle;
using Memorio.Modules.Plans.Domain.Entitlements;

namespace Memorio.Modules.Plans.Tests;

/// <summary>The rules of AG8 without a database.</summary>
public sealed class PlanCycleTests
{
    private static readonly DateTimeOffset Paid = new(2026, 1, 1, 12, 0, 0, TimeSpan.Zero);
    private static readonly DateTimeOffset End = Paid.AddMonths(1);
    private static readonly EntitledResource Album = new(EntitledResourceKind.Album, Guid.CreateVersion7());
    private static readonly EntitledResource Account = new(EntitledResourceKind.Account, Guid.CreateVersion7());

    private static PlanCycle ReadOnlyCycle(EntitledResource? resource = null)
    {
        var cycle = PlanCycle.Start(resource ?? Album);
        cycle.ApplyEntitlement(End, Paid);
        cycle.ApplyEntitlement(End, End);
        cycle.ClearDomainEvents();
        return cycle;
    }

    /// <summary>Every warning issued and reported as delivered, as Notifications would.</summary>
    private static void DeliverAllWarnings(PlanCycle cycle)
    {
        var trimAt = cycle.TrimAt!.Value;
        cycle.IssueDueWarnings(trimAt);
        foreach (var days in PlanCycle.WarningDays)
        {
            cycle.RecordWarningDelivered(days, cycle.WarningRound, trimAt);
        }

        cycle.ClearDomainEvents();
    }

    [Fact]
    public void Paying_activates_with_the_limits_of_the_plan()
    {
        var cycle = PlanCycle.Start(Album);

        cycle.ApplyEntitlement(End, Paid);

        var activated = Assert.IsType<PlanActivated>(Assert.Single(cycle.DomainEvents));
        Assert.Equal(PlanMode.Active, cycle.Mode);
        Assert.Equal(150L * 1024 * 1024 * 1024, activated.Terms.LimitBytes);
        Assert.Equal(cycle.Sequence, activated.Sequence);
    }

    [Fact]
    public void A_personal_plan_has_its_own_limits()
    {
        var cycle = PlanCycle.Start(Account);

        cycle.ApplyEntitlement(End, Paid);

        Assert.Equal(50L * 1024 * 1024 * 1024, Assert.IsType<PlanActivated>(Assert.Single(cycle.DomainEvents)).Terms.LimitBytes);
    }

    [Fact]
    public void A_renewal_only_moves_the_date()
    {
        var cycle = PlanCycle.Start(Album);
        cycle.ApplyEntitlement(End, Paid);
        cycle.ClearDomainEvents();

        cycle.ApplyEntitlement(End.AddMonths(1), Paid.AddDays(29));

        Assert.Empty(cycle.DomainEvents);
        Assert.Equal(End.AddMonths(1), cycle.EntitledUntil);
    }

    [Fact]
    public void A_line_saying_nothing_is_paid_on_a_never_paid_resource_changes_nothing()
    {
        var cycle = PlanCycle.Start(Album);

        cycle.ApplyEntitlement(Paid.AddDays(-1), Paid);

        Assert.Equal(PlanMode.Free, cycle.Mode);
        Assert.Empty(cycle.DomainEvents);
    }

    [Fact]
    public void The_end_of_the_paid_period_means_read_only_for_24_months()
    {
        var cycle = PlanCycle.Start(Album);
        cycle.ApplyEntitlement(End, Paid);
        cycle.ClearDomainEvents();

        cycle.ApplyEntitlement(End, End.AddHours(1));

        var readOnly = Assert.IsType<PlanWentReadOnly>(Assert.Single(cycle.DomainEvents));
        Assert.Equal(0, readOnly.Terms.LimitBytes);
        Assert.Equal(PlanMode.ReadOnly, cycle.Mode);
        Assert.Equal(End.AddHours(1).AddMonths(24), cycle.TrimAt);
    }

    [Fact]
    public void An_account_going_read_only_keeps_the_free_limit()
    {
        var cycle = PlanCycle.Start(Account);
        cycle.ApplyEntitlement(End, Paid);
        cycle.ClearDomainEvents();

        cycle.ApplyEntitlement(End, End);

        var readOnly = Assert.IsType<PlanWentReadOnly>(Assert.Single(cycle.DomainEvents));
        Assert.Equal(PlanTerms.Free, readOnly.Terms);
    }

    [Fact]
    public void Paying_again_brings_back_everything_and_forgets_the_choice()
    {
        var cycle = ReadOnlyCycle();
        cycle.KeepMaterial([Guid.CreateVersion7()], cycle.TrimAt!.Value.AddDays(-1));
        cycle.ClearDomainEvents();

        cycle.ApplyEntitlement(End.AddYears(1), End.AddMonths(2));

        Assert.IsType<PlanActivated>(Assert.Single(cycle.DomainEvents));
        Assert.Null(cycle.TrimAt);
        Assert.Empty(cycle.KeptMemoryIds);
    }

    [Theory]
    [InlineData(91, 0)]
    [InlineData(90, 1)]
    [InlineData(30, 2)]
    [InlineData(7, 3)]
    [InlineData(1, 4)]
    public void Warnings_come_90_30_7_and_1_days_before(int daysBefore, int warnings)
    {
        var cycle = ReadOnlyCycle();

        cycle.IssueDueWarnings(cycle.TrimAt!.Value.AddDays(-daysBefore));

        Assert.Equal(warnings, cycle.DomainEvents.OfType<TrimWarningDue>().Count());
    }

    [Fact]
    public void A_warning_is_issued_once()
    {
        var cycle = ReadOnlyCycle();
        var at = cycle.TrimAt!.Value.AddDays(-90);

        cycle.IssueDueWarnings(at);
        cycle.ClearDomainEvents();
        cycle.IssueDueWarnings(at.AddHours(1));

        Assert.Empty(cycle.DomainEvents);
    }

    [Fact]
    public void Before_24_months_nothing_is_trimmed()
    {
        var cycle = ReadOnlyCycle();
        DeliverAllWarnings(cycle);

        cycle.OrderTrimWhenDue(cycle.TrimAt!.Value.AddSeconds(-1));

        Assert.Equal(PlanMode.ReadOnly, cycle.Mode);
        Assert.Empty(cycle.DomainEvents);
    }

    [Fact]
    public void After_24_months_with_every_warning_delivered_the_trim_is_ordered_with_the_choice()
    {
        var cycle = ReadOnlyCycle();
        var kept = Guid.CreateVersion7();
        cycle.KeepMaterial([kept, kept], cycle.TrimAt!.Value.AddDays(-1));
        DeliverAllWarnings(cycle);

        cycle.OrderTrimWhenDue(cycle.TrimAt!.Value);

        var trim = Assert.IsType<TrimOrdered>(Assert.Single(cycle.DomainEvents));
        Assert.Equal(PlanMode.Trimmed, cycle.Mode);
        Assert.Equal([kept], trim.KeptMemoryIds);
        Assert.Equal(PlanTerms.Free, trim.Terms);
    }

    [Fact]
    public void Without_proof_of_delivery_the_trim_is_held_instead()
    {
        var cycle = ReadOnlyCycle();
        var trimAt = cycle.TrimAt!.Value;
        cycle.IssueDueWarnings(trimAt);
        foreach (var days in new[] { 90, 30, 7 })
        {
            cycle.RecordWarningDelivered(days, cycle.WarningRound, trimAt);
        }

        cycle.ClearDomainEvents();

        cycle.OrderTrimWhenDue(trimAt);

        var held = Assert.IsType<TrimHeld>(Assert.Single(cycle.DomainEvents));
        Assert.Equal(1, held.DaysBefore);
        Assert.Equal(PlanMode.ReadOnly, cycle.Mode);
    }

    [Fact]
    public void A_held_trim_is_not_ordered_however_late_it_gets()
    {
        var cycle = ReadOnlyCycle();
        DeliverAllWarnings(cycle);
        var trimAt = cycle.TrimAt!.Value;
        cycle.HoldTrim(1, cycle.WarningRound, trimAt.AddDays(-1));
        cycle.ClearDomainEvents();

        cycle.OrderTrimWhenDue(trimAt.AddYears(5));

        Assert.Equal(PlanMode.ReadOnly, cycle.Mode);
        Assert.Empty(cycle.DomainEvents);
    }

    [Fact]
    public void A_second_failure_while_held_changes_nothing()
    {
        var cycle = ReadOnlyCycle();
        var at = cycle.TrimAt!.Value.AddDays(-90);
        cycle.HoldTrim(90, cycle.WarningRound, at);
        cycle.ClearDomainEvents();
        var sequence = cycle.Sequence;

        cycle.HoldTrim(30, cycle.WarningRound, at.AddDays(60));

        Assert.Empty(cycle.DomainEvents);
        Assert.Equal(sequence, cycle.Sequence);
    }

    [Fact]
    public void A_report_about_an_earlier_round_counts_for_nothing()
    {
        var cycle = ReadOnlyCycle();
        var round = cycle.WarningRound;
        var at = cycle.TrimAt!.Value.AddDays(-90);
        cycle.IssueDueWarnings(at);
        cycle.ClearDomainEvents();

        cycle.RecordWarningDelivered(90, round - 1, at);
        cycle.HoldTrim(90, round - 1, at);

        Assert.Empty(cycle.WarningsDelivered);
        Assert.False(cycle.IsHeld);
    }

    [Fact]
    public void Delivery_ends_the_hold_and_gives_30_more_days_with_new_warnings()
    {
        var cycle = ReadOnlyCycle();
        var trimAt = cycle.TrimAt!.Value;
        cycle.IssueDueWarnings(trimAt);
        foreach (var days in new[] { 90, 30, 7 })
        {
            cycle.RecordWarningDelivered(days, cycle.WarningRound, trimAt);
        }

        cycle.OrderTrimWhenDue(trimAt);
        Assert.True(cycle.IsHeld);
        var round = cycle.WarningRound;
        var later = trimAt.AddMonths(3);
        cycle.ClearDomainEvents();

        cycle.RecordWarningDelivered(1, round, later);

        Assert.False(cycle.IsHeld);
        Assert.Equal(later.AddDays(30), cycle.TrimAt);
        Assert.NotEqual(round, cycle.WarningRound);
        Assert.Equal([90], cycle.WarningsDue);
        Assert.Equal([90], cycle.WarningsDelivered);

        // The trim waits for the three that were sent again.
        cycle.OrderTrimWhenDue(later.AddDays(30));
        Assert.Equal(PlanMode.ReadOnly, cycle.Mode);
        Assert.True(cycle.IsHeld);
    }

    [Fact]
    public void Material_is_chosen_only_in_the_last_30_days()
    {
        var cycle = ReadOnlyCycle();
        var trimAt = cycle.TrimAt!.Value;

        Assert.Equal(PlanCycleErrors.ChoiceClosed, cycle.KeepMaterial([Guid.CreateVersion7()], trimAt.AddDays(-31)).Error);
        Assert.True(cycle.KeepMaterial([Guid.CreateVersion7()], trimAt.AddDays(-30)).IsSuccess);
        Assert.Equal(PlanCycleErrors.ChoiceClosed, cycle.KeepMaterial([Guid.CreateVersion7()], trimAt).Error);
    }

    [Fact]
    public void A_new_choice_replaces_the_earlier_one()
    {
        var cycle = ReadOnlyCycle();
        var at = cycle.TrimAt!.Value.AddDays(-1);
        var second = Guid.CreateVersion7();

        cycle.KeepMaterial([Guid.CreateVersion7()], at);
        cycle.KeepMaterial([second], at);

        Assert.Equal([second], cycle.KeptMemoryIds);
    }

    [Fact]
    public void While_held_the_window_stays_open()
    {
        var cycle = ReadOnlyCycle();
        var trimAt = cycle.TrimAt!.Value;
        cycle.HoldTrim(90, cycle.WarningRound, trimAt.AddDays(-90));

        Assert.True(cycle.KeepMaterial([Guid.CreateVersion7()], trimAt.AddYears(1)).IsSuccess);
    }

    [Fact]
    public void Nothing_is_chosen_on_a_plan_that_is_active()
    {
        var cycle = PlanCycle.Start(Album);
        cycle.ApplyEntitlement(End, Paid);

        Assert.Equal(PlanCycleErrors.ChoiceClosed, cycle.KeepMaterial([Guid.CreateVersion7()], Paid).Error);
    }

    [Fact]
    public void An_album_without_an_owner_runs_until_the_end_of_the_paid_period()
    {
        var cycle = PlanCycle.Start(Album);
        cycle.ApplyEntitlement(End, Paid);
        cycle.ClearDomainEvents();

        cycle.MarkOwnerless(1, End);
        cycle.ApplyEntitlement(End, End.AddDays(-1));
        Assert.Equal(PlanMode.Active, cycle.Mode);

        cycle.ApplyEntitlement(End, End);
        Assert.Equal(PlanMode.ReadOnly, cycle.Mode);
    }

    [Fact]
    public void Paying_for_an_album_without_an_owner_does_not_activate_it()
    {
        var cycle = ReadOnlyCycle();
        cycle.MarkOwnerless(1, End);

        cycle.ApplyEntitlement(End.AddYears(1), End.AddMonths(2));

        Assert.IsType<PaidWhileOwnerless>(Assert.Single(cycle.DomainEvents));
        Assert.Equal(PlanMode.ReadOnly, cycle.Mode);

        // The same line again says nothing new.
        cycle.ClearDomainEvents();
        cycle.ApplyEntitlement(End.AddYears(1), End.AddMonths(2));
        Assert.Empty(cycle.DomainEvents);
    }

    [Fact]
    public void A_paid_trim_is_never_ordered()
    {
        var cycle = ReadOnlyCycle();
        DeliverAllWarnings(cycle);
        cycle.MarkOwnerless(1, End);
        var trimAt = cycle.TrimAt!.Value;
        cycle.ApplyEntitlement(trimAt.AddYears(1), trimAt);
        cycle.ClearDomainEvents();

        cycle.OrderTrimWhenDue(trimAt);

        Assert.Equal(PlanMode.ReadOnly, cycle.Mode);
        Assert.Empty(cycle.DomainEvents);
    }

    [Fact]
    public void The_takeover_activates_only_a_paid_plan()
    {
        var cycle = ReadOnlyCycle();
        cycle.MarkOwnerless(1, End);
        var now = End.AddMonths(2);

        Assert.Equal(PlanCycleErrors.NotPaid, cycle.ActivateAfterTakeover(2, now).Error);

        cycle.ApplyEntitlement(now.AddYears(1), now);
        cycle.ClearDomainEvents();

        Assert.True(cycle.ActivateAfterTakeover(2, now).IsSuccess);
        Assert.IsType<PlanActivated>(Assert.Single(cycle.DomainEvents));
        Assert.False(cycle.IsOwnerless);
    }

    [Fact]
    public void An_album_that_never_paid_goes_read_only_when_it_loses_its_owner()
    {
        var cycle = PlanCycle.Start(Album);

        cycle.MarkOwnerless(1, Paid);

        Assert.IsType<PlanWentReadOnly>(Assert.Single(cycle.DomainEvents));
        Assert.Equal(Paid.AddMonths(24), cycle.TrimAt);
    }

    [Fact]
    public void Paying_while_the_period_still_runs_is_reported_too()
    {
        var cycle = PlanCycle.Start(Album);
        cycle.ApplyEntitlement(End, Paid);
        cycle.MarkOwnerless(1, Paid);
        cycle.ClearDomainEvents();

        cycle.ApplyEntitlement(End.AddMonths(1), Paid.AddDays(1));

        Assert.IsType<PaidWhileOwnerless>(Assert.Single(cycle.DomainEvents));
    }

    [Fact]
    public void A_late_event_about_the_album_changes_nothing()
    {
        var cycle = ReadOnlyCycle();
        cycle.MarkOwnerless(5, End);
        var now = End.AddMonths(2);
        cycle.ApplyEntitlement(now.AddYears(1), now);
        Assert.True(cycle.ActivateAfterTakeover(6, now).IsSuccess);

        cycle.MarkOwnerless(5, now);

        Assert.False(cycle.IsOwnerless);
    }

    [Fact]
    public void A_closed_cycle_ignores_the_ledger()
    {
        var cycle = PlanCycle.Start(Account);
        cycle.Close();

        cycle.ApplyEntitlement(End, Paid);

        Assert.Equal(PlanMode.Closed, cycle.Mode);
        Assert.Empty(cycle.DomainEvents);
    }
}
