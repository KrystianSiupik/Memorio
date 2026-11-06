namespace Memorio.Modules.Moderation.Contracts;

/// <summary>
/// Read port for Notifications: who reported (an account, or an address for a report without one) and the reasons of
/// the decisions, so no address and no reasons travel in events.
/// </summary>
public interface IReportNotices
{
    Task<ReportNotice?> NoticeOfAsync(Guid reportId, CancellationToken cancellationToken);
}

/// <param name="Reason">What the report says is wrong, e.g. PersonalData.</param>
/// <param name="Statement">The reasons of the decision, once decided.</param>
/// <param name="AppealStatement">The reasons of the decision on the appeal, once decided.</param>
public sealed record ReportNotice(Guid? ReporterAccountId, string? ReporterEmail, string Reason, string? Statement, string? AppealStatement);
