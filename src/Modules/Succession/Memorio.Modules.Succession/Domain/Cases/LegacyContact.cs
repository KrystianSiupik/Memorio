using Memorio.SharedKernel.Domain;

namespace Memorio.Modules.Succession.Domain.Cases;

/// <summary>
/// C124: who may read what the owner kept private after the death (S2:C8). An address, because the person may have no
/// account yet. Nobody else ever gets it, not even heirs with a document.
/// </summary>
internal sealed class LegacyContact : AggregateRoot<Guid>
{
    private LegacyContact(Guid id, string email)
        : base(id)
    {
        Email = email;
    }

    public string Email { get; private set; }

    public DateTimeOffset? SharedAt { get; private set; }

    public int Sequence { get; private set; } = 1;

    public static LegacyContact Name(Guid ownerAccountId, string email) => new(ownerAccountId, email);

    public void ChangeTo(string email)
    {
        Email = email;
        Sequence++;
    }

    /// <summary>C125: once.</summary>
    public void Share(DateTimeOffset now)
    {
        if (SharedAt is not null)
        {
            return;
        }

        SharedAt = now;
        Sequence++;
        Raise(new PrivateArchiveShared(Guid.CreateVersion7(), now, Id, Sequence));
    }
}

/// <summary>What the service decided, written next to the database by deploy/succession/cases.sh; the worker carries it out (D-295).</summary>
internal sealed class OperatorDecision
{
    private OperatorDecision()
    {
    }

    public Guid Id { get; private set; }

    public Guid CaseId { get; private set; }

    public bool Confirmed { get; private set; }

    public string DecidedBy { get; private set; } = "";

    public DateTimeOffset DecidedAt { get; private set; }

    public DateTimeOffset? AppliedAt { get; private set; }

    public string? Outcome { get; private set; }

    public void MarkApplied(string outcome, DateTimeOffset now)
    {
        Outcome = outcome;
        AppliedAt = now;
    }
}

internal interface ITakeoverCases
{
    Task<TakeoverCase?> FindAsync(TakeoverCaseId id, CancellationToken cancellationToken);

    /// <summary>A case still waiting for a decision about this album.</summary>
    Task<TakeoverCase?> FindOpenAsync(Guid albumId, CancellationToken cancellationToken);

    /// <summary>Anything but a rejected report: this album of this owner is already on its way.</summary>
    Task<bool> ExistsAsync(Guid albumId, Guid ownerAccountId, CancellationToken cancellationToken);

    Task<IReadOnlyList<TakeoverCaseId>> FindOverdueAsync(DateTimeOffset at, CancellationToken cancellationToken);

    /// <summary>Closed for longer than the retention, with the file still there (D-055).</summary>
    Task<IReadOnlyList<TakeoverCaseId>> FindExpiredCertificatesAsync(DateTimeOffset closedBefore, CancellationToken cancellationToken);

    void Add(TakeoverCase takeoverCase);

    Task<LegacyContact?> FindContactAsync(Guid ownerAccountId, CancellationToken cancellationToken);

    void Add(LegacyContact contact);

    void Remove(LegacyContact contact);

    Task<IReadOnlyList<Guid>> FindPendingDecisionsAsync(CancellationToken cancellationToken);

    Task<OperatorDecision?> FindDecisionAsync(Guid id, CancellationToken cancellationToken);

    /// <summary>Stores the changes with the events, in one transaction. False when another decision got there first: nothing was stored.</summary>
    Task<bool> SaveAsync(CancellationToken cancellationToken);
}
