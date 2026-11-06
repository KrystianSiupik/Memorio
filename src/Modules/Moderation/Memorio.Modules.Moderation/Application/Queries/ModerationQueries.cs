namespace Memorio.Modules.Moderation.Application.Queries;

/// <summary>R18: what the person reported, and every decision about their own content, with the reasons. Never who reported it.</summary>
internal interface IModerationQueries
{
    Task<IReadOnlyList<MyReport>> ReportsByAsync(Guid accountId, CancellationToken cancellationToken);

    Task<IReadOnlyList<ReportAboutMine>> ReportsAboutAsync(Guid accountId, CancellationToken cancellationToken);
}

/// <param name="Target">Memory or Album.</param>
/// <param name="State">Open, Decided, Appealed or Closed.</param>
/// <param name="Verdict">Removed or Kept, once decided.</param>
internal sealed record MyReport(Guid Id, string Target, Guid? TargetId, string Reason, string State, string? Verdict, string? Statement, DateTimeOffset ReportedAt, DateTimeOffset? DecidedAt);

/// <param name="Hidden">Nobody else sees it because of this report now.</param>
/// <param name="AppealUntil">Set while an appeal is possible.</param>
/// <param name="AppealReversed">Set once the appeal is decided.</param>
internal sealed record ReportAboutMine(
    Guid Id,
    string Target,
    Guid TargetId,
    string Reason,
    string Category,
    string State,
    bool Hidden,
    string? Verdict,
    string? Statement,
    DateTimeOffset ReportedAt,
    DateTimeOffset? DecidedAt,
    DateTimeOffset? AppealUntil,
    bool? AppealReversed,
    string? AppealStatement);
