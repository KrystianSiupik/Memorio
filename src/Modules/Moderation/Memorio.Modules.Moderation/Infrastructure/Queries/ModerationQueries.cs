using Dapper;
using Memorio.Modules.Moderation.Application.Queries;
using Memorio.Modules.Moderation.Contracts;
using Memorio.Modules.Moderation.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Memorio.Modules.Moderation.Infrastructure.Queries;

/// <summary>Dapper on the module's tables (D-126). Dapper reads timestamptz as DateTime, hence the rows with setters.</summary>
internal sealed class ModerationQueries(ModerationDbContext database) : IModerationQueries, IReportNotices, ILegalHolds
{
    private const string HeldSql = """
        SELECT EXISTS (SELECT 1 FROM moderation.orders WHERE account_id = @accountId AND secured_at IS NOT NULL AND closed_at IS NULL)
        """;

    public async Task<bool> IsHeldAsync(Guid accountId, CancellationToken cancellationToken) =>
        await database.Database.GetDbConnection().ExecuteScalarAsync<bool>(new CommandDefinition(HeldSql, new { accountId }, cancellationToken: cancellationToken));

    private const string MineSql = """
        SELECT id AS "Id", target AS "Target", target_id AS "TargetId", reason AS "Reason", state AS "State", verdict AS "Verdict",
               statement AS "Statement", reported_at AS "ReportedAt", decided_at AS "DecidedAt",
               CASE WHEN verdict = 'Kept' THEN appeal_until END AS "AppealUntil",
               CASE WHEN appeal_by_reporter THEN appeal_reversed END AS "AppealReversed",
               CASE WHEN appeal_by_reporter THEN appeal_statement END AS "AppealStatement"
        FROM moderation.reports WHERE reporter_account_id = @accountId ORDER BY reported_at DESC
        """;

    private const string AboutSql = """
        SELECT id AS "Id", target AS "Target", target_id AS "TargetId", reason AS "Reason", category AS "Category", state AS "State",
               hidden AS "Hidden", verdict AS "Verdict", statement AS "Statement", reported_at AS "ReportedAt", decided_at AS "DecidedAt",
               CASE WHEN verdict = 'Removed' THEN appeal_until END AS "AppealUntil", appeal_reversed AS "AppealReversed", appeal_statement AS "AppealStatement"
        FROM moderation.reports WHERE author_account_id = @accountId ORDER BY reported_at DESC
        """;

    private const string NoticeSql = """
        SELECT reporter_account_id AS "ReporterAccountId", reporter_email AS "ReporterEmail", reason AS "Reason",
               statement AS "Statement", appeal_statement AS "AppealStatement"
        FROM moderation.reports WHERE id = @reportId
        """;

    public async Task<IReadOnlyList<MyReport>> ReportsByAsync(Guid accountId, CancellationToken cancellationToken) =>
        [.. (await database.Database.GetDbConnection().QueryAsync<MineRow>(new CommandDefinition(MineSql, new { accountId }, cancellationToken: cancellationToken)))
            .Select(row => new MyReport(
                row.Id, row.Target, row.TargetId, row.Reason, row.State, row.Verdict, row.Statement, Utc(row.ReportedAt), UtcOrNull(row.DecidedAt),
                UtcOrNull(row.AppealUntil), row.AppealReversed, row.AppealStatement))];

    public async Task<IReadOnlyList<ReportAboutMine>> ReportsAboutAsync(Guid accountId, CancellationToken cancellationToken) =>
        [.. (await database.Database.GetDbConnection().QueryAsync<AboutRow>(new CommandDefinition(AboutSql, new { accountId }, cancellationToken: cancellationToken)))
            .Select(row => new ReportAboutMine(
                row.Id, row.Target, row.TargetId, row.Reason, row.Category, row.State, row.Hidden, row.Verdict, row.Statement,
                Utc(row.ReportedAt), UtcOrNull(row.DecidedAt), UtcOrNull(row.AppealUntil), row.AppealReversed, row.AppealStatement))];

    public async Task<ReportNotice?> NoticeOfAsync(Guid reportId, CancellationToken cancellationToken) =>
        await database.Database.GetDbConnection().QuerySingleOrDefaultAsync<ReportNotice>(new CommandDefinition(NoticeSql, new { reportId }, cancellationToken: cancellationToken));

    private static DateTimeOffset Utc(DateTime value) => new(DateTime.SpecifyKind(value, DateTimeKind.Utc));

    private static DateTimeOffset? UtcOrNull(DateTime? value) => value is { } at ? Utc(at) : null;

    private sealed class MineRow
    {
        public Guid Id { get; set; }

        public string Target { get; set; } = "";

        public Guid? TargetId { get; set; }

        public string Reason { get; set; } = "";

        public string State { get; set; } = "";

        public string? Verdict { get; set; }

        public string? Statement { get; set; }

        public DateTime ReportedAt { get; set; }

        public DateTime? DecidedAt { get; set; }

        public DateTime? AppealUntil { get; set; }

        public bool? AppealReversed { get; set; }

        public string? AppealStatement { get; set; }
    }

    private sealed class AboutRow
    {
        public Guid Id { get; set; }

        public string Target { get; set; } = "";

        public Guid TargetId { get; set; }

        public string Reason { get; set; } = "";

        public string Category { get; set; } = "";

        public string State { get; set; } = "";

        public bool Hidden { get; set; }

        public string? Verdict { get; set; }

        public string? Statement { get; set; }

        public DateTime ReportedAt { get; set; }

        public DateTime? DecidedAt { get; set; }

        public DateTime? AppealUntil { get; set; }

        public bool? AppealReversed { get; set; }

        public string? AppealStatement { get; set; }
    }
}
