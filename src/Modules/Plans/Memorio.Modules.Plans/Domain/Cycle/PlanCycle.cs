using Memorio.Modules.Plans.Domain.Entitlements;
using Memorio.SharedKernel.Domain;
using Memorio.SharedKernel.Results;

namespace Memorio.Modules.Plans.Domain.Cycle;

internal enum PlanMode
{
    /// <summary>Nothing paid and nothing waiting: what never paid, or what the cycle brought back here.</summary>
    Free,
    Active,

    /// <summary>The 24 months of full reading (S2:B1). For an account this is also "nothing new over the limit" (S2:C7).</summary>
    ReadOnly,

    /// <summary>The trim was ordered: the excess is in the trash, the free limit applies.</summary>
    Trimmed,

    /// <summary>The account is gone; nothing happens here any more.</summary>
    Closed,
}

/// <summary>
/// AG8: payment, the hold, the choice of material and the trim decide over one state (D-006), because the trim is
/// the one irreversible step of a plan. <see cref="Sequence"/> is its version (D-018) and orders its events (D-013).
/// </summary>
internal sealed class PlanCycle : AggregateRoot<EntitledResource>
{
    public const int RetentionMonths = 24;

    /// <summary>The owner chooses during the last 30 days before the trim (S2:B1, D-279).</summary>
    public static readonly TimeSpan ChoiceWindow = TimeSpan.FromDays(30);

    /// <summary>Days before the trim, in the order they come (S2:B1).</summary>
    public static readonly IReadOnlyList<int> WarningDays = [90, 30, 7, 1];

    private readonly List<int> _warningsDue = [];
    private readonly List<int> _warningsDelivered = [];
    private readonly List<Guid> _keptMemoryIds = [];

    private PlanCycle(EntitledResource id)
        : base(id)
    {
        ResourceKind = id.Kind;
        ResourceId = id.Id;
    }

    // For EF Core.
    private PlanCycle(EntitledResourceKind resourceKind, Guid resourceId)
        : this(new EntitledResource(resourceKind, resourceId))
    {
    }

    public EntitledResourceKind ResourceKind { get; private set; }

    public Guid ResourceId { get; private set; }

    public PlanMode Mode { get; private set; } = PlanMode.Free;

    public int Sequence { get; private set; } = 1;

    public DateTimeOffset? EntitledUntil { get; private set; }

    public DateTimeOffset? ReadOnlySince { get; private set; }

    public DateTimeOffset? TrimAt { get; private set; }

    public DateTimeOffset? HeldSince { get; private set; }

    public DateTimeOffset? TrimmedAt { get; private set; }

    /// <summary>When this cycle last looked at the ledger; lines recorded later are still to be applied.</summary>
    public DateTimeOffset? LedgerCheckedAt { get; private set; }

    /// <summary>The album lost its owner (D-001): the plan runs until the paid period ends and does not come back by payment alone (D-004).</summary>
    public bool IsOwnerless { get; private set; }

    /// <summary>Names the current series of warnings. A later hold released starts a new one.</summary>
    public int WarningRound { get; private set; }

    public IReadOnlyList<int> WarningsDue => _warningsDue.AsReadOnly();

    public IReadOnlyList<int> WarningsDelivered => _warningsDelivered.AsReadOnly();

    public IReadOnlyList<Guid> KeptMemoryIds => _keptMemoryIds.AsReadOnly();

    public bool IsHeld => HeldSince is not null;

    public static PlanCycle Start(EntitledResource resource) => new(resource);

    public bool IsChoiceOpen(DateTimeOffset now) =>
        Mode == PlanMode.ReadOnly && now >= TrimAt - ChoiceWindow && (now < TrimAt || IsHeld);

    /// <summary>
    /// What the ledger says now; the ledger already ignored what its source said too late (S2:C3). A renewal only
    /// moves the date. The plan ends only here, so an album without an owner runs to the end of what is paid (HS-020).
    /// </summary>
    public void ApplyEntitlement(DateTimeOffset? entitledUntil, DateTimeOffset now)
    {
        LedgerCheckedAt = now;
        if (Mode == PlanMode.Closed)
        {
            return;
        }

        var changed = entitledUntil != EntitledUntil;
        EntitledUntil = entitledUntil;
        if (entitledUntil > now)
        {
            if (IsOwnerless && Mode != PlanMode.Active)
            {
                if (changed)
                {
                    Sequence++;
                    Raise(new PaidWhileOwnerless(Guid.CreateVersion7(), now, Id, Sequence));
                }

                return;
            }

            Activate(now);
            return;
        }

        if (Mode == PlanMode.Active)
        {
            GoReadOnly(now);
        }
    }

    /// <summary>C96: the source was asked, whatever it answered.</summary>
    public void RecordSynchronized(DateTimeOffset now) => Raise(new SubscriptionSynchronized(Guid.CreateVersion7(), now, Id));

    /// <summary>C91 and, for an account, C100 (D-280). Counted from when it happened here: a late check never shortens the 24 months.</summary>
    private void GoReadOnly(DateTimeOffset now)
    {
        Mode = PlanMode.ReadOnly;
        ReadOnlySince = now;
        TrimAt = now.AddMonths(RetentionMonths);
        HeldSince = null;
        _warningsDue.Clear();
        _warningsDelivered.Clear();
        _keptMemoryIds.Clear();
        Sequence++;
        WarningRound = Sequence;
        Raise(new PlanWentReadOnly(Guid.CreateVersion7(), now, Id, PlanTerms.ReadOnly(Id.Kind), Sequence));
    }

    /// <summary>C90.</summary>
    private void Activate(DateTimeOffset now)
    {
        if (Mode == PlanMode.Active)
        {
            return;
        }

        Mode = PlanMode.Active;
        ReadOnlySince = null;
        TrimAt = null;
        HeldSince = null;
        TrimmedAt = null;
        _warningsDue.Clear();
        _warningsDelivered.Clear();
        _keptMemoryIds.Clear();
        Sequence++;
        Raise(new PlanActivated(Guid.CreateVersion7(), now, Id, PlanTerms.Paid(Id.Kind), Sequence));
    }

    /// <summary>E97 for every warning whose day has come; once per round. Late ones all go: each is part of the proof.</summary>
    public void IssueDueWarnings(DateTimeOffset now)
    {
        if (Mode != PlanMode.ReadOnly)
        {
            return;
        }

        foreach (var days in WarningDays.Where(days => !_warningsDue.Contains(days) && now >= TrimAt!.Value.AddDays(-days)))
        {
            _warningsDue.Add(days);
            Sequence++;
            Raise(new TrimWarningDue(Guid.CreateVersion7(), now, Id, days, TrimAt!.Value, WarningRound));
        }
    }

    /// <summary>E98: the warning reached everyone. When it also ends a hold, the trim gets a new date with new warnings.</summary>
    public void RecordWarningDelivered(int days, int round, DateTimeOffset now)
    {
        if (Mode != PlanMode.ReadOnly || round != WarningRound || _warningsDelivered.Contains(days))
        {
            return;
        }

        _warningsDelivered.Add(days);
        Sequence++;
        if (IsHeld && _warningsDue.All(_warningsDelivered.Contains))
        {
            ReleaseHold(now);
        }
    }

    private void ReleaseHold(DateTimeOffset now)
    {
        HeldSince = null;
        var earliest = now + ChoiceWindow;
        if (TrimAt < earliest)
        {
            TrimAt = earliest;
        }

        var trimAt = TrimAt!.Value;
        _warningsDue.RemoveAll(days => trimAt.AddDays(-days) >= now);
        _warningsDelivered.RemoveAll(days => trimAt.AddDays(-days) >= now);
        Sequence++;
        WarningRound = Sequence;
    }

    /// <summary>C93: E99 stops the only irreversible step (S2:C10). Already held: nothing new.</summary>
    public void HoldTrim(int days, int round, DateTimeOffset now)
    {
        if (Mode != PlanMode.ReadOnly || round != WarningRound || IsHeld)
        {
            return;
        }

        HeldSince = now;
        Sequence++;
        Raise(new TrimHeld(Guid.CreateVersion7(), now, Id, days, WarningRound, Sequence));
    }

    /// <summary>C94: only in the window; a new choice replaces the earlier one. The archive reads it, so ids are all it takes.</summary>
    public Result KeepMaterial(IReadOnlyCollection<Guid> memoryIds, DateTimeOffset now)
    {
        if (!IsChoiceOpen(now))
        {
            return PlanCycleErrors.ChoiceClosed;
        }

        _keptMemoryIds.Clear();
        _keptMemoryIds.AddRange(memoryIds.Distinct());
        Sequence++;
        Raise(new MaterialChosen(Guid.CreateVersion7(), now, Id));
        return Result.Success();
    }

    /// <summary>
    /// C95, the invariant of AG8: only after 24 months in this mode and the end of the window, not held and not paid.
    /// The caller applied the ledger first. A warning without proof of delivery holds the trim instead (S2:B1).
    /// </summary>
    public void OrderTrimWhenDue(DateTimeOffset now)
    {
        if (Mode != PlanMode.ReadOnly || IsHeld || now < TrimAt || now < ReadOnlySince!.Value.AddMonths(RetentionMonths))
        {
            return;
        }

        if (EntitledUntil > now)
        {
            return;
        }

        foreach (var days in WarningDays)
        {
            if (!_warningsDelivered.Contains(days))
            {
                HoldTrim(days, WarningRound, now);
                return;
            }
        }

        Mode = PlanMode.Trimmed;
        TrimmedAt = now;
        Sequence++;
        Raise(new TrimOrdered(Guid.CreateVersion7(), now, Id, PlanTerms.Free, [.. _keptMemoryIds], Sequence));
    }

    /// <summary>E162 (D-001): nothing changes until the paid period ends.</summary>
    public void MarkOwnerless()
    {
        if (!IsOwnerless)
        {
            IsOwnerless = true;
            Sequence++;
        }
    }

    /// <summary>C165: back to active only after a takeover and with the plan paid (D-004).</summary>
    public Result ActivateAfterTakeover(DateTimeOffset now)
    {
        if (!(EntitledUntil > now))
        {
            return PlanCycleErrors.NotPaid;
        }

        IsOwnerless = false;
        Activate(now);
        return Result.Success();
    }

    public void Close()
    {
        Mode = PlanMode.Closed;
        Sequence++;
    }
}
