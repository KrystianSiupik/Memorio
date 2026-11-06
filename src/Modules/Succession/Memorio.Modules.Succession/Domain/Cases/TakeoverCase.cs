using Memorio.SharedKernel.Domain;
using Memorio.SharedKernel.Results;

namespace Memorio.Modules.Succession.Domain.Cases;

internal readonly record struct TakeoverCaseId(Guid Value);

internal enum CaseState
{
    /// <summary>Someone in the album reported the death; the service has not checked the certificate.</summary>
    Reported,

    /// <summary>The successor of the snapshot has 30 days to answer.</summary>
    Offered,
    Transferred,
    Refused,
    Rejected,
}

/// <summary>
/// AG7: one decision per stage of one album's way to a new owner (D-008). <see cref="Sequence"/> is the version (D-021).
/// The effect in Album follows the decision through the outbox, so it is never lost and may repeat.
/// </summary>
internal sealed class TakeoverCase : AggregateRoot<TakeoverCaseId>
{
    /// <summary>HS-021.</summary>
    public static readonly TimeSpan AnswerWindow = TimeSpan.FromDays(30);

    private TakeoverCase(TakeoverCaseId id, Guid albumId, Guid ownerAccountId, DateTimeOffset reportedAt)
        : base(id)
    {
        AlbumId = albumId;
        OwnerAccountId = ownerAccountId;
        ReportedAt = reportedAt;
    }

    public Guid AlbumId { get; private set; }

    public Guid OwnerAccountId { get; private set; }

    public Guid? ReportedByAccountId { get; private set; }

    /// <summary>In the private bucket; null once the retention ended (D-055), or when no certificate belongs to this case.</summary>
    public string? CertificateKey { get; private set; }

    public CaseState State { get; private set; }

    /// <summary>D-020: who was named at the moment of verification; a later change does not touch this case.</summary>
    public Guid? SuccessorAccountId { get; private set; }

    public DateTimeOffset ReportedAt { get; private set; }

    public DateTimeOffset? AnswerBy { get; private set; }

    public DateTimeOffset? ClosedAt { get; private set; }

    public int Sequence { get; private set; } = 1;

    /// <summary>C120. The owner hears about it: a living one can object before the service decides (D-304).</summary>
    public static TakeoverCase Report(TakeoverCaseId id, Guid albumId, Guid ownerAccountId, Guid reportedByAccountId, string certificateKey, DateTimeOffset now)
    {
        var reported = new TakeoverCase(id, albumId, ownerAccountId, now) { State = CaseState.Reported, ReportedByAccountId = reportedByAccountId, CertificateKey = certificateKey };
        reported.Raise(new DeathReported(Guid.CreateVersion7(), now, id, albumId, ownerAccountId, reported.Sequence));
        return reported;
    }

    /// <summary>The way for the other albums of someone whose death is confirmed, or who deleted the account: nothing more to verify.</summary>
    public static TakeoverCase Open(TakeoverCaseId id, Guid albumId, Guid ownerAccountId, Guid? successorAccountId, DateTimeOffset now)
    {
        var opened = new TakeoverCase(id, albumId, ownerAccountId, now) { State = CaseState.Reported };
        opened.Offer(successorAccountId, now);
        return opened;
    }

    /// <summary>C121, E121.</summary>
    public Result Verify(Guid? successorAccountId, DateTimeOffset now)
    {
        if (State != CaseState.Reported)
        {
            return CaseErrors.AlreadyDecided;
        }

        Sequence++;
        Raise(new DeathConfirmed(Guid.CreateVersion7(), now, OwnerAccountId, Sequence));
        Offer(successorAccountId, now);
        return Result.Success();
    }

    /// <summary>C121, E122.</summary>
    public Result Reject(DateTimeOffset now)
    {
        if (State != CaseState.Reported)
        {
            return CaseErrors.AlreadyDecided;
        }

        State = CaseState.Rejected;
        ClosedAt = now;
        Sequence++;
        Raise(new DeathReportRejected(Guid.CreateVersion7(), now, Id, AlbumId, ReportedByAccountId!.Value, Sequence));
        return Result.Success();
    }

    /// <summary>C122: only the successor of the snapshot, after verification, within the 30 days.</summary>
    public Result Accept(Guid accountId, DateTimeOffset now)
    {
        var allowed = Answering(accountId, now);
        if (allowed.IsFailure)
        {
            return allowed;
        }

        State = CaseState.Transferred;
        ClosedAt = now;
        Sequence++;
        Raise(new AlbumTransferred(Guid.CreateVersion7(), now, Id, AlbumId, OwnerAccountId, accountId, Sequence));
        return Result.Success();
    }

    /// <summary>C123.</summary>
    public Result Refuse(Guid accountId, DateTimeOffset now)
    {
        var allowed = Answering(accountId, now);
        if (allowed.IsFailure)
        {
            return allowed;
        }

        Close(now);
        return Result.Success();
    }

    /// <summary>C126: the same as a refusal, and only for an offer whose time really ended (HS-021, D-019).</summary>
    public Result ExpireIfDue(DateTimeOffset at)
    {
        if (State != CaseState.Offered)
        {
            return CaseErrors.AlreadyDecided;
        }

        if (at < AnswerBy)
        {
            return CaseErrors.StillOpen;
        }

        Close(at);
        return Result.Success();
    }

    /// <summary>The file is gone (D-055).</summary>
    public void ForgetCertificate()
    {
        CertificateKey = null;
        Sequence++;
    }

    private void Offer(Guid? successorAccountId, DateTimeOffset now)
    {
        SuccessorAccountId = successorAccountId;
        if (successorAccountId is not { } successor)
        {
            Close(now);
            return;
        }

        State = CaseState.Offered;
        AnswerBy = now + AnswerWindow;
        Sequence++;
        Raise(new TakeoverOffered(Guid.CreateVersion7(), now, Id, AlbumId, successor, AnswerBy.Value, Sequence));
    }

    private void Close(DateTimeOffset now)
    {
        State = CaseState.Refused;
        ClosedAt = now;
        Sequence++;
        Raise(new TakeoverRefused(Guid.CreateVersion7(), now, Id, AlbumId, OwnerAccountId, Sequence));
    }

    private Result Answering(Guid accountId, DateTimeOffset now)
    {
        if (State == CaseState.Reported)
        {
            return CaseErrors.NotVerified;
        }

        if (accountId != SuccessorAccountId)
        {
            return CaseErrors.NotSuccessor;
        }

        return State != CaseState.Offered || now >= AnswerBy ? CaseErrors.AlreadyDecided : Result.Success();
    }
}

/// <summary>E120.</summary>
internal sealed record DeathReported(Guid EventId, DateTimeOffset OccurredAt, TakeoverCaseId CaseId, Guid AlbumId, Guid OwnerAccountId, int Sequence) : IDomainEvent;

/// <summary>E121.</summary>
internal sealed record DeathConfirmed(Guid EventId, DateTimeOffset OccurredAt, Guid OwnerAccountId, int Sequence) : IDomainEvent;

/// <summary>E122.</summary>
internal sealed record DeathReportRejected(Guid EventId, DateTimeOffset OccurredAt, TakeoverCaseId CaseId, Guid AlbumId, Guid ReportedByAccountId, int Sequence) : IDomainEvent;

internal sealed record TakeoverOffered(Guid EventId, DateTimeOffset OccurredAt, TakeoverCaseId CaseId, Guid AlbumId, Guid SuccessorAccountId, DateTimeOffset AnswerBy, int Sequence) : IDomainEvent;

/// <summary>E123.</summary>
internal sealed record AlbumTransferred(Guid EventId, DateTimeOffset OccurredAt, TakeoverCaseId CaseId, Guid AlbumId, Guid FromAccountId, Guid ToAccountId, int Sequence) : IDomainEvent;

/// <summary>E124.</summary>
internal sealed record TakeoverRefused(Guid EventId, DateTimeOffset OccurredAt, TakeoverCaseId CaseId, Guid AlbumId, Guid OwnerAccountId, int Sequence) : IDomainEvent;

/// <summary>E126.</summary>
internal sealed record PrivateArchiveShared(Guid EventId, DateTimeOffset OccurredAt, Guid OwnerAccountId, int Sequence) : IDomainEvent;

internal static class CaseErrors
{
    public static readonly Error NoAccess = Error.Forbidden("Succession.NoAccess", "No access to this album.");

    public static readonly Error NotParticipant = Error.Validation("Succession.NotParticipant", "A successor has to be in the album.");

    public static readonly Error AlreadyDecided = Error.Conflict("Succession.AlreadyDecided", "This has already been decided.");

    public static readonly Error AlreadyReported = Error.Conflict("Succession.AlreadyReported", "A report about this album is already being handled.");

    public static readonly Error NotVerified = Error.Conflict("Succession.NotVerified", "The certificate has not been checked yet.");

    public static readonly Error NotSuccessor = Error.Forbidden("Succession.NotSuccessor", "Another person was named to take this album over.");

    public static readonly Error StillOpen = Error.Conflict("Succession.StillOpen", "The time to answer has not ended.");

    public static readonly Error NotFound = Error.NotFound("Succession.CaseNotFound", "There is no such case.");

    public static readonly Error InvalidUpload = Error.Validation("Succession.InvalidUpload", "This is not an uploaded file.");

    public static readonly Error UploadMissing = Error.Validation("Succession.UploadMissing", "The file has not arrived.");

    public static readonly Error InvalidContentType = Error.Validation("Succession.InvalidContentType", "A certificate is a photo or a PDF.");

    public static readonly Error InvalidEmail = Error.Validation("Succession.InvalidEmail", "This is not an e-mail address.");

    public static readonly Error CannotNameYourself = Error.Validation("Succession.CannotNameYourself", "A contact is somebody else.");
}
