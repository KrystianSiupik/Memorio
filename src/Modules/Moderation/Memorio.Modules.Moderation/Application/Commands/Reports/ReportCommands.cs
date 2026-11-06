using Memorio.Modules.Access.Contracts;
using Memorio.Modules.Album.Contracts;
using Memorio.Modules.Archive.Contracts;
using Memorio.Modules.Moderation.Domain.Reports;
using Memorio.SharedKernel.Results;

namespace Memorio.Modules.Moderation.Application.Commands.Reports;

/// <summary>C110 from the app (S27). The id comes from the client: the same request again, or another about the same thing while it waits, is that report.</summary>
internal sealed record SubmitReport(ReportId Id, Guid AccountId, string? Target, Guid TargetId, string? Reason, string? Explanation);

/// <summary>C110 without an account (S21).</summary>
internal sealed record SubmitPublicReport(ReportId Id, string? Email, string? Location, string? Reason, string? Explanation);

internal sealed class SubmitReportHandler(IReports reports, IReportedMemories memories, IAlbumMembership albums, IAccessReader access, TimeProvider clock)
{
    public async Task<Result<ReportId>> HandleAsync(SubmitReport command, CancellationToken cancellationToken)
    {
        if (command.Target is not (nameof(ReportTarget.Memory) or nameof(ReportTarget.Album)) || !Enum.TryParse<ReportTarget>(command.Target, ignoreCase: false, out var target))
        {
            return ReportErrors.InvalidReport;
        }

        var author = target == ReportTarget.Memory
            ? await memories.AuthorSeenByAsync(command.TargetId, command.AccountId, cancellationToken)
            : await access.CanSeeAlbumAsync(command.AccountId, command.TargetId, cancellationToken) ? await albums.OwnerOfAsync(command.TargetId, cancellationToken) : null;
        if (author is null)
        {
            return ReportErrors.NoAccess;
        }

        var report = Report.Submit(command.Id, command.AccountId, target, command.TargetId, author.Value, command.Reason, command.Explanation, clock.GetUtcNow());
        return report.IsFailure ? report.Error : await StoreAsync(reports, report.Value, cancellationToken);
    }

    /// <summary>The same id, or the open report of the same person about the same thing, is the answer; nothing new waits in the queue.</summary>
    public static async Task<Result<ReportId>> StoreAsync(IReports reports, Report report, CancellationToken cancellationToken)
    {
        // A suspended reporter hides nothing and grows no queue (D-315).
        if (await reports.IsSuspendedAsync(report.ReporterKey, report.ReportedAt, cancellationToken))
        {
            return ReportErrors.ReportingSuspended;
        }

        for (var attempt = 0; attempt < 2; attempt++)
        {
            if (await reports.FindAsync(report.Id, cancellationToken) is { } known)
            {
                return known.ReporterKey == report.ReporterKey ? known.Id : ReportErrors.NoAccess;
            }

            if (await reports.FindOpenAsync(report, cancellationToken) is { } open)
            {
                return open.Id;
            }

            reports.Add(report);
            if (await reports.SaveAsync(cancellationToken))
            {
                return report.Id;
            }
        }

        return ReportErrors.AlreadyDecided;
    }
}

internal sealed class SubmitPublicReportHandler(IReports reports, TimeProvider clock)
{
    public async Task<Result<ReportId>> HandleAsync(SubmitPublicReport command, CancellationToken cancellationToken)
    {
        var report = Report.SubmitPublic(command.Id, command.Email, command.Location, command.Reason, command.Explanation, clock.GetUtcNow());
        return report.IsFailure ? report.Error : await SubmitReportHandler.StoreAsync(reports, report.Value, cancellationToken);
    }
}

/// <summary>C111 and the decision on an appeal, as the service wrote them (D-309).</summary>
internal sealed record DecideReport(ReportId Id, Verdict Verdict, string? Statement, string? DecidedBy, bool Unfounded = false);

internal sealed record DecideAppeal(ReportId Id, bool Reversed, string? Statement, string? DecidedBy);

/// <summary>C112.</summary>
internal sealed record AppealDecision(ReportId Id, Guid AccountId, string? Text);

internal sealed class ReportDecisionHandler(IReports reports, TimeProvider clock)
{
    public Task<Result> HandleAsync(DecideReport command, CancellationToken cancellationToken) =>
        DecideAsync(command.Id, report => report.Decide(command.Verdict, command.Statement, command.DecidedBy, clock.GetUtcNow(), command.Unfounded), cancellationToken);

    public Task<Result> HandleAsync(DecideAppeal command, CancellationToken cancellationToken) =>
        DecideAsync(command.Id, report => report.DecideAppeal(command.Reversed, command.Statement, command.DecidedBy, clock.GetUtcNow()), cancellationToken);

    public Task<Result> HandleAsync(AppealDecision command, CancellationToken cancellationToken) =>
        DecideAsync(command.Id, report => report.Appeal(command.AccountId, command.Text, clock.GetUtcNow()), cancellationToken);

    /// <summary>A parallel decision is read again: the loser is refused only when the new state refuses it.</summary>
    private async Task<Result> DecideAsync(ReportId id, Func<Report, Result> decide, CancellationToken cancellationToken)
    {
        for (var attempt = 0; attempt < 3; attempt++)
        {
            if (await reports.FindAsync(id, cancellationToken) is not { } report)
            {
                return ReportErrors.NoAccess;
            }

            var decided = decide(report);
            if (decided.IsFailure)
            {
                return decided;
            }

            if (await reports.SaveAsync(cancellationToken))
            {
                return Result.Success();
            }
        }

        return ReportErrors.AlreadyDecided;
    }
}
