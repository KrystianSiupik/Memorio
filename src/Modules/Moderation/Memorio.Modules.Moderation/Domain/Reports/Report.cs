using System.Net.Mail;
using Memorio.SharedKernel.Domain;
using Memorio.SharedKernel.Results;

namespace Memorio.Modules.Moderation.Domain.Reports;

internal readonly record struct ReportId(Guid Value);

/// <summary>What the person says is wrong; it decides the category (S2:C2).</summary>
internal enum ReportReason
{
    ChildAbuse,
    Terrorism,
    Threat,
    Incitement,
    OtherIllegal,
    SomeonesImage,
    PersonalData,
    Defamation,
    Copyright,
    Other,
}

/// <summary>A: illegal, hidden at once and decided within 24 h. B: disputed, stays seen and decided within 7 days (S2:C2).</summary>
internal enum ReportCategory
{
    Illegal,
    Disputed,
}

/// <summary>Elsewhere: a report through the public form, about a place described in words (S21).</summary>
internal enum ReportTarget
{
    Memory,
    Album,
    Elsewhere,
}

internal enum ReportState
{
    Open,
    Decided,
    Appealed,
    Closed,
}

internal enum Verdict
{
    Removed,
    Kept,
}

/// <summary>
/// AG4: one report and at most one decision on it, then at most one appeal with its own decision (S2:C2, DSA art. 16,
/// 17, 20). Its id is the hold on the content in Access (D-308): hidden while the report says so, and nothing is ever
/// deleted here. <see cref="Sequence"/> is the version and orders its events (D-013).
/// </summary>
internal sealed class Report : AggregateRoot<ReportId>
{
    public const int MaximumExplanationLength = 4_000;

    public const int MaximumLocationLength = 1_000;

    /// <summary>D-315: three manifestly unfounded reports within this time suspend reporting...</summary>
    public static readonly TimeSpan UnfoundedWindow = TimeSpan.FromDays(90);

    /// <summary>...for this long (DSA art. 23, after the warning each unfounded decision gives).</summary>
    public static readonly TimeSpan Suspension = TimeSpan.FromDays(183);

    public const int UnfoundedToSuspend = 3;

    /// <summary>DSA art. 20: at least six months to appeal.</summary>
    public static readonly TimeSpan AppealWindow = TimeSpan.FromDays(183);

    private Report(ReportId id)
        : base(id)
    {
    }

    public Guid? ReporterAccountId { get; private set; }

    /// <summary>Only for a report without an account: where the decision goes.</summary>
    public string? ReporterEmail { get; private set; }

    public ReportTarget Target { get; private set; }

    public Guid? TargetId { get; private set; }

    /// <summary>Only for <see cref="ReportTarget.Elsewhere"/>: an address or a description of where it is.</summary>
    public string? Location { get; private set; }

    /// <summary>The author of the memory or the owner of the album: the one who keeps seeing it, and may appeal.</summary>
    public Guid? AuthorAccountId { get; private set; }

    public ReportReason Reason { get; private set; }

    public ReportCategory Category { get; private set; }

    public string Explanation { get; private set; } = "";

    public ReportState State { get; private set; }

    public DateTimeOffset ReportedAt { get; private set; }

    public DateTimeOffset DueBy { get; private set; }

    /// <summary>Whether this report holds the content hidden now.</summary>
    public bool Hidden { get; private set; }

    public Verdict? Verdict { get; private set; }

    public string? Statement { get; private set; }

    public string? DecidedBy { get; private set; }

    /// <summary>Kept, and the report was manifestly unfounded: it counts towards suspending the reporter (D-315).</summary>
    public bool Unfounded { get; private set; }

    public DateTimeOffset? DecidedAt { get; private set; }

    public DateTimeOffset? AppealUntil { get; private set; }

    public string? AppealText { get; private set; }

    public DateTimeOffset? AppealedAt { get; private set; }

    /// <summary>The appeal came from the reporter, against keeping the content (D-314); otherwise from the author.</summary>
    public bool? AppealByReporter { get; private set; }

    public bool? AppealReversed { get; private set; }

    public string? AppealStatement { get; private set; }

    public DateTimeOffset? AppealDecidedAt { get; private set; }

    public int Sequence { get; private set; } = 1;

    /// <summary>The key of "the same person about the same thing": a repeated report does not grow the queue (S2:C2).</summary>
    public string ReporterKey { get; private set; } = "";

    /// <summary>The thing reported: its id, or the place described.</summary>
    public string TargetKey { get; private set; } = "";

    public static ReportCategory CategoryOf(ReportReason reason) =>
        reason is ReportReason.ChildAbuse or ReportReason.Terrorism or ReportReason.Threat or ReportReason.Incitement or ReportReason.OtherIllegal
            ? ReportCategory.Illegal
            : ReportCategory.Disputed;

    /// <summary>C110 from the app: about a memory or an album the person sees; illegal content is hidden at once.</summary>
    public static Result<Report> Submit(ReportId id, Guid reporterAccountId, ReportTarget target, Guid targetId, Guid authorAccountId, string? reason, string? explanation, DateTimeOffset now)
    {
        if (target == ReportTarget.Elsewhere || targetId == Guid.Empty)
        {
            return ReportErrors.InvalidReport;
        }

        if (authorAccountId == reporterAccountId)
        {
            return ReportErrors.OwnContent;
        }

        var report = Create(id, reason, explanation, now);
        if (report.IsFailure)
        {
            return report;
        }

        var created = report.Value;
        created.ReporterAccountId = reporterAccountId;
        created.Target = target;
        created.TargetId = targetId;
        created.AuthorAccountId = authorAccountId;
        created.ReporterKey = reporterAccountId.ToString();
        created.TargetKey = targetId.ToString();
        created.Hidden = created.Category == ReportCategory.Illegal;
        created.Raise(new ContentReported(Guid.CreateVersion7(), now, id, created.Category, authorAccountId, created.Hidden, created.Sequence));
        if (created.Hidden)
        {
            created.Raise(new ContentHidden(Guid.CreateVersion7(), now, id, target, targetId, authorAccountId, created.Sequence));
        }

        return created;
    }

    /// <summary>C110 without an account (S21): anyone, about something described in words. The service finds it.</summary>
    public static Result<Report> SubmitPublic(ReportId id, string? email, string? location, string? reason, string? explanation, DateTimeOffset now)
    {
        var address = email?.Trim();
        var where = location?.Trim();
        if (string.IsNullOrEmpty(address) || address.Length > 320 || !MailAddress.TryCreate(address, out _)
            || string.IsNullOrEmpty(where) || where.Length > MaximumLocationLength)
        {
            return ReportErrors.InvalidReport;
        }

        var report = Create(id, reason, explanation, now);
        if (report.IsFailure)
        {
            return report;
        }

        var created = report.Value;
        created.ReporterEmail = address.ToLowerInvariant();
        created.Target = ReportTarget.Elsewhere;
        created.Location = where;
        created.ReporterKey = created.ReporterEmail;
        created.TargetKey = where;
        created.Raise(new ContentReported(Guid.CreateVersion7(), now, id, created.Category, null, false, created.Sequence));
        return created;
    }

    /// <summary>C111: one decision, with its reasons for both sides (DSA art. 17). Removed hides the content, kept shows it again.</summary>
    public Result Decide(Verdict verdict, string? statement, string? decidedBy, DateTimeOffset now, bool unfounded = false)
    {
        if (State != ReportState.Open)
        {
            return ReportErrors.AlreadyDecided;
        }

        if (unfounded && verdict != Reports.Verdict.Kept)
        {
            return ReportErrors.InvalidDecision;
        }

        var reasons = statement?.Trim();
        if (string.IsNullOrEmpty(reasons) || reasons.Length > MaximumExplanationLength || string.IsNullOrWhiteSpace(decidedBy))
        {
            return ReportErrors.InvalidDecision;
        }

        State = ReportState.Decided;
        Verdict = verdict;
        Statement = reasons;
        DecidedBy = decidedBy.Trim();
        DecidedAt = now;
        Unfounded = unfounded;
        // The side the decision went against may appeal: the author a removal, the reporter with an account keeping it (D-314).
        AppealUntil = (verdict == Reports.Verdict.Removed ? AuthorAccountId : ReporterAccountId) is not null ? now + AppealWindow : null;
        Sequence++;
        Raise(new ReportDecided(Guid.CreateVersion7(), now, Id, verdict, unfounded, AuthorAccountId, Sequence));
        ShowOrHide(verdict == Reports.Verdict.Removed, now);
        return Result.Success();
    }

    /// <summary>C112: the side the decision went against, within the window, once (DSA art. 20, D-314).</summary>
    public Result Appeal(Guid accountId, string? text, DateTimeOffset now)
    {
        var byReporter = accountId == ReporterAccountId;
        if (accountId != AuthorAccountId && !byReporter)
        {
            return ReportErrors.NoAccess;
        }

        var against = byReporter ? Reports.Verdict.Kept : Reports.Verdict.Removed;
        if (State != ReportState.Decided || Verdict != against || !(now < AppealUntil))
        {
            return ReportErrors.AlreadyDecided;
        }

        var reasons = text?.Trim();
        if (string.IsNullOrEmpty(reasons) || reasons.Length > MaximumExplanationLength)
        {
            return ReportErrors.InvalidAppeal;
        }

        State = ReportState.Appealed;
        AppealByReporter = byReporter;
        AppealText = reasons;
        AppealedAt = now;
        Sequence++;
        Raise(new AppealFiled(Guid.CreateVersion7(), now, Id, Sequence));
        return Result.Success();
    }

    /// <summary>The decision on the appeal, with its own reasons. Reversed turns the first decision around: a removal shows the content again, keeping it hides it.</summary>
    public Result DecideAppeal(bool reversed, string? statement, string? decidedBy, DateTimeOffset now)
    {
        if (State != ReportState.Appealed)
        {
            return ReportErrors.AlreadyDecided;
        }

        var reasons = statement?.Trim();
        if (string.IsNullOrEmpty(reasons) || reasons.Length > MaximumExplanationLength || string.IsNullOrWhiteSpace(decidedBy))
        {
            return ReportErrors.InvalidDecision;
        }

        // D-316: an appeal is decided by somebody else than the first decision (HS-007).
        if (string.Equals(decidedBy.Trim(), DecidedBy, StringComparison.OrdinalIgnoreCase))
        {
            return ReportErrors.SameDecider;
        }

        State = ReportState.Closed;
        AppealReversed = reversed;
        AppealStatement = reasons;
        AppealDecidedAt = now;
        Sequence++;
        Raise(new AppealDecided(Guid.CreateVersion7(), now, Id, reversed, AppealByReporter == true, AuthorAccountId, Sequence));
        ShowOrHide(Verdict == Reports.Verdict.Removed ? !reversed : reversed, now);
        return Result.Success();
    }

    private static Result<Report> Create(ReportId id, string? reason, string? explanation, DateTimeOffset now)
    {
        var text = explanation?.Trim();
        if (id.Value == Guid.Empty || reason is null || !Enum.GetNames<ReportReason>().Contains(reason) || !Enum.TryParse<ReportReason>(reason, ignoreCase: false, out var parsed)
            || string.IsNullOrEmpty(text) || text.Length > MaximumExplanationLength)
        {
            return ReportErrors.InvalidReport;
        }

        var category = CategoryOf(parsed);
        return new Report(id)
        {
            Reason = parsed,
            Category = category,
            Explanation = text,
            State = ReportState.Open,
            ReportedAt = now,
            DueBy = now + (category == ReportCategory.Illegal ? TimeSpan.FromHours(24) : TimeSpan.FromDays(7)),
        };
    }

    private void ShowOrHide(bool hide, DateTimeOffset now)
    {
        if (TargetId is not { } resource || hide == Hidden)
        {
            return;
        }

        Hidden = hide;
        if (hide)
        {
            Raise(new ContentHidden(Guid.CreateVersion7(), now, Id, Target, resource, AuthorAccountId, Sequence));
        }
        else
        {
            Raise(new ContentRestored(Guid.CreateVersion7(), now, Id, resource, Sequence));
        }
    }
}

/// <summary>E110.</summary>
internal sealed record ContentReported(Guid EventId, DateTimeOffset OccurredAt, ReportId ReportId, ReportCategory Category, Guid? AuthorAccountId, bool HiddenNow, int Sequence) : IDomainEvent;

/// <summary>E112.</summary>
internal sealed record ReportDecided(Guid EventId, DateTimeOffset OccurredAt, ReportId ReportId, Verdict Verdict, bool Unfounded, Guid? AuthorAccountId, int Sequence) : IDomainEvent;

/// <summary>E113.</summary>
internal sealed record ContentHidden(Guid EventId, DateTimeOffset OccurredAt, ReportId ReportId, ReportTarget Target, Guid ResourceId, Guid? AuthorAccountId, int Sequence) : IDomainEvent;

/// <summary>The hold of this report ended.</summary>
internal sealed record ContentRestored(Guid EventId, DateTimeOffset OccurredAt, ReportId ReportId, Guid ResourceId, int Sequence) : IDomainEvent;

/// <summary>E114: the service sees it in the queue.</summary>
internal sealed record AppealFiled(Guid EventId, DateTimeOffset OccurredAt, ReportId ReportId, int Sequence) : IDomainEvent;

internal sealed record AppealDecided(Guid EventId, DateTimeOffset OccurredAt, ReportId ReportId, bool Reversed, bool ByReporter, Guid? AuthorAccountId, int Sequence) : IDomainEvent;

internal static class ReportErrors
{
    public static readonly Error InvalidReport = Error.Validation(
        "Moderation.InvalidReport",
        "A report needs a reason from the list and an explanation up to 4000 characters; without an account also an e-mail address and where it is.");

    public static readonly Error InvalidDecision = Error.Validation("Moderation.InvalidDecision", "A decision needs its reasons, up to 4000 characters, and who made it.");

    public static readonly Error InvalidAppeal = Error.Validation("Moderation.InvalidAppeal", "An appeal needs its reasons, up to 4000 characters.");

    public static readonly Error OwnContent = Error.Validation("Moderation.OwnContent", "Your own content you can change or delete yourself.");

    public static readonly Error AlreadyDecided = Error.Conflict("Moderation.AlreadyDecided", "This has already been decided.");

    public static readonly Error SameDecider = Error.Conflict("Moderation.SameDecider", "An appeal is decided by somebody else than the first decision.");

    public static readonly Error ReportingSuspended = Error.Forbidden(
        "Moderation.ReportingSuspended",
        "Your reports are not accepted for now: three of them were manifestly unfounded within 90 days.");

    /// <summary>The same answer for content that does not exist and content the person does not see.</summary>
    public static readonly Error NoAccess = Error.Forbidden("Moderation.NoAccess", "No access to this content.");
}

/// <summary>What the service decided through deploy/moderation, applied by the worker within a minute (D-295, D-309).</summary>
internal sealed class OperatorAction
{
    public const string Decide = "Decide";

    public const string DecideAppeal = "DecideAppeal";

    private OperatorAction()
    {
    }

    public Guid Id { get; private set; }

    public string Kind { get; private set; } = "";

    public Guid ReportId { get; private set; }

    /// <summary>For a decision: Removed, Kept or Unfounded (kept, and counted against the reporter); for an appeal: Reversed or Upheld.</summary>
    public string Outcome { get; private set; } = "";

    public string Statement { get; private set; } = "";

    public string DecidedBy { get; private set; } = "";

    public DateTimeOffset DecidedAt { get; private set; }

    public DateTimeOffset? AppliedAt { get; private set; }

    public string? Result { get; private set; }

    public void MarkApplied(string result, DateTimeOffset now)
    {
        Result = result;
        AppliedAt = now;
    }
}

internal interface IReports
{
    Task<Report?> FindAsync(ReportId id, CancellationToken cancellationToken);

    /// <summary>
    /// D-315: three unfounded reports of this reporter within 90 days, the last of them less than the suspension ago.
    /// </summary>
    Task<bool> IsSuspendedAsync(string reporterKey, DateTimeOffset now, CancellationToken cancellationToken);

    /// <summary>The open report of the same person about the same thing, if any.</summary>
    Task<Report?> FindOpenAsync(Report like, CancellationToken cancellationToken);

    void Add(Report report);

    Task<IReadOnlyList<Guid>> FindPendingActionsAsync(CancellationToken cancellationToken);

    Task<OperatorAction?> FindActionAsync(Guid id, CancellationToken cancellationToken);

    /// <summary>Stores the changes with their events, in one transaction. False when another decision got there first.</summary>
    Task<bool> SaveAsync(CancellationToken cancellationToken);
}
