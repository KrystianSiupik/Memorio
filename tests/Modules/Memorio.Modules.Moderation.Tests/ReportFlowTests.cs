using Memorio.Modules.Moderation.Application.Commands.Reports;
using Memorio.Modules.Moderation.Application.Queries;
using Memorio.Modules.Moderation.Contracts;
using Memorio.Modules.Moderation.Domain.Reports;
using Memorio.Modules.Moderation.Infrastructure.Scheduling;
using Memorio.SharedKernel.Results;
using Memorio.SharedKernel.Scheduling;
using Microsoft.Extensions.DependencyInjection;
using Npgsql;

namespace Memorio.Modules.Moderation.Tests;

/// <summary>Reports, decisions and appeals on a real database, with the outbox as in the API.</summary>
[Collection(nameof(OperatorJobs))]
public sealed class ReportFlowTests(ModerationTestDatabase database)
{
    private static CancellationToken Cancellation => TestContext.Current.CancellationToken;

    private async Task<Result<ReportId>> ReportAsync(Guid id, Guid reporter, string target, Guid targetId, string reason = nameof(ReportReason.PersonalData))
    {
        await using var scope = database.CreateScope();
        return await scope.ServiceProvider.GetRequiredService<SubmitReportHandler>().HandleAsync(
            new SubmitReport(new ReportId(id), reporter, target, targetId, reason, "To moje dane."), Cancellation);
    }

    private async Task<Report> FindAsync(Guid id)
    {
        await using var scope = database.CreateScope();
        return (await scope.ServiceProvider.GetRequiredService<IReports>().FindAsync(new ReportId(id), Cancellation))!;
    }

    internal static async Task WriteActionAsync(ModerationTestDatabase database, Guid reportId, string kind, string outcome, string statement = "Uzasadnienie.")
    {
        await using var connection = await database.OpenAsAsync(ModerationTestDatabase.Role);
        await using var command = new NpgsqlCommand(
            "INSERT INTO moderation.operator_actions (id, kind, report_id, outcome, statement, decided_by, decided_at) VALUES (gen_random_uuid(), @kind, @report, @outcome, @statement, 'test', now())",
            connection);
        command.Parameters.AddWithValue("kind", kind);
        command.Parameters.AddWithValue("report", reportId);
        command.Parameters.AddWithValue("outcome", outcome);
        command.Parameters.AddWithValue("statement", statement);
        await command.ExecuteNonQueryAsync(Cancellation);
    }

    internal static async Task FireAsync(ModerationTestDatabase database)
    {
        await using var scope = database.CreateScope();
        await ActivatorUtilities.CreateInstance<OperatorActionsJob>(scope.ServiceProvider).RunAsync(
            new ScheduledJobRun(OperatorActionsJob.JobName, null, DateTimeOffset.UtcNow, new Dictionary<string, string>(), 1),
            Cancellation);
    }

    [Fact]
    public async Task Illegal_content_someone_sees_is_hidden_at_once_and_the_same_report_again_is_that_report()
    {
        var (memory, author, reader) = database.Neighbours.NewMemory();
        var id = Guid.CreateVersion7();

        Assert.Equal(id, (await ReportAsync(id, reader, "Memory", memory, nameof(ReportReason.Threat))).Value.Value);
        Assert.Equal(id, (await ReportAsync(Guid.CreateVersion7(), reader, "Memory", memory)).Value.Value);
        Assert.Equal(id, (await ReportAsync(id, reader, "Memory", memory)).Value.Value);

        var report = await FindAsync(id);
        Assert.True(report.Hidden);
        Assert.Equal(author, report.AuthorAccountId);
        Assert.True(await ModerationTestDatabase.EventuallyAsync(async () => await database.QueuedEventsAsync(nameof(ContentHiddenEvent), id) == 1));
        Assert.True(await ModerationTestDatabase.EventuallyAsync(async () => await database.QueuedEventsAsync(nameof(ContentReportedEvent), id) == 1));
        await using var connection = await database.OpenAsAsync(ModerationTestDatabase.Role);
        await using var command = new NpgsqlCommand("SELECT count(*) FROM moderation.reports WHERE target_id = @memory", connection);
        command.Parameters.AddWithValue("memory", memory);
        Assert.Equal(1L, await command.ExecuteScalarAsync(Cancellation));
    }

    [Fact]
    public async Task What_the_person_does_not_see_or_owns_cannot_be_reported()
    {
        var (memory, author, _) = database.Neighbours.NewMemory();
        var (album, owner, member) = database.Neighbours.NewAlbum();

        Assert.Equal(ReportErrors.NoAccess, (await ReportAsync(Guid.CreateVersion7(), Guid.CreateVersion7(), "Memory", memory)).Error);
        Assert.Equal(ReportErrors.NoAccess, (await ReportAsync(Guid.CreateVersion7(), Guid.CreateVersion7(), "Album", album)).Error);
        Assert.Equal(ReportErrors.OwnContent, (await ReportAsync(Guid.CreateVersion7(), author, "Memory", memory)).Error);
        Assert.Equal(ReportErrors.InvalidReport, (await ReportAsync(Guid.CreateVersion7(), member, "Photo", album)).Error);
        Assert.True((await ReportAsync(Guid.CreateVersion7(), member, "Album", album)).IsSuccess);
        Assert.NotEqual(owner, member);
    }

    [Fact]
    public async Task Six_reports_at_once_of_one_person_about_one_thing_leave_one()
    {
        var (memory, _, reader) = database.Neighbours.NewMemory();

        var results = await Task.WhenAll(Enumerable.Range(0, 6).Select(_ => ReportAsync(Guid.CreateVersion7(), reader, "Memory", memory)));

        Assert.All(results, result => Assert.True(result.IsSuccess));
        Assert.Single(results.Select(result => result.Value).Distinct());
    }

    [Fact]
    public async Task A_report_without_an_account_waits_for_the_service()
    {
        var id = Guid.CreateVersion7();
        await using var scope = database.CreateScope();
        var reported = await scope.ServiceProvider.GetRequiredService<SubmitPublicReportHandler>().HandleAsync(
            new SubmitPublicReport(new ReportId(id), "Osoba@Example.invalid", "Album „Wakacje” z linku od znajomego", nameof(ReportReason.SomeonesImage), "To ja na zdjęciu."),
            Cancellation);

        Assert.True(reported.IsSuccess);
        var report = await FindAsync(id);
        Assert.Equal("osoba@example.invalid", report.ReporterEmail);
        Assert.False(report.Hidden);
        var notice = await scope.ServiceProvider.GetRequiredService<IReportNotices>().NoticeOfAsync(id, Cancellation);
        Assert.Equal("osoba@example.invalid", notice!.ReporterEmail);
    }

    [Fact]
    public async Task The_service_decides_through_the_job_once_and_the_author_appeals()
    {
        var (memory, author, reader) = database.Neighbours.NewMemory();
        var id = Guid.CreateVersion7();
        await ReportAsync(id, reader, "Memory", memory);

        await WriteActionAsync(database, id, OperatorAction.Decide, "Removed", "Dane osobowe bez zgody.");
        await WriteActionAsync(database, id, OperatorAction.Decide, "Kept");
        await FireAsync(database);

        var decided = await FindAsync(id);
        Assert.Equal(Verdict.Removed, decided.Verdict);
        Assert.Equal("Dane osobowe bez zgody.", decided.Statement);
        Assert.True(decided.Hidden);
        Assert.True(await ModerationTestDatabase.EventuallyAsync(async () => await database.QueuedEventsAsync(nameof(ReportDecidedEvent), id) == 1));
        Assert.True(await ModerationTestDatabase.EventuallyAsync(async () => await database.QueuedEventsAsync(nameof(ContentHiddenEvent), id) == 1));

        await using (var scope = database.CreateScope())
        {
            var mine = await scope.ServiceProvider.GetRequiredService<IModerationQueries>().ReportsAboutAsync(author, Cancellation);
            var about = Assert.Single(mine);
            Assert.Equal("Removed", about.Verdict);
            Assert.NotNull(about.AppealUntil);
            Assert.Equal(id, Assert.Single(await scope.ServiceProvider.GetRequiredService<IModerationQueries>().ReportsByAsync(reader, Cancellation)).Id);
            Assert.True((await scope.ServiceProvider.GetRequiredService<ReportDecisionHandler>().HandleAsync(new AppealDecision(new ReportId(id), author, "To moje zdjęcie."), Cancellation)).IsSuccess);
        }

        await WriteActionAsync(database, id, OperatorAction.DecideAppeal, "Reversed", "Autor ma zgodę.");
        await FireAsync(database);

        var closed = await FindAsync(id);
        Assert.Equal(ReportState.Closed, closed.State);
        Assert.False(closed.Hidden);
        Assert.True(await ModerationTestDatabase.EventuallyAsync(async () => await database.QueuedEventsAsync(nameof(ContentRestoredEvent), id) == 1));
        Assert.True(await ModerationTestDatabase.EventuallyAsync(async () => await database.QueuedEventsAsync(nameof(AppealDecidedEvent), id) == 1));
    }

    [Fact]
    public async Task Two_decisions_at_once_leave_one()
    {
        var (memory, _, reader) = database.Neighbours.NewMemory();
        var id = Guid.CreateVersion7();
        await ReportAsync(id, reader, "Memory", memory);

        var decisions = await Task.WhenAll(
            Enumerable.Range(0, 2).Select(async index =>
            {
                await using var scope = database.CreateScope();
                return await scope.ServiceProvider.GetRequiredService<ReportDecisionHandler>().HandleAsync(
                    new DecideReport(new ReportId(id), index == 0 ? Verdict.Removed : Verdict.Kept, "Powód.", "test"), Cancellation);
            }));

        Assert.Single(decisions, decision => decision.IsSuccess);
        Assert.Single(decisions, decision => decision.IsFailure && decision.Error == ReportErrors.AlreadyDecided);
    }
}

/// <summary>The job takes every pending action, so tests that write them run one at a time.</summary>
[CollectionDefinition(nameof(OperatorJobs), DisableParallelization = true)]
public sealed class OperatorJobs;

/// <summary>SD12: a firing applies every pending action once, whatever happens to run twice.</summary>
[Collection(nameof(OperatorJobs))]
public sealed class OperatorActionsJobIdempotencyTests(ModerationTestDatabase database)
{
    [Fact]
    public async Task Twice_is_once()
    {
        var (memory, _, reader) = database.Neighbours.NewMemory();
        var id = Guid.CreateVersion7();
        await using (var scope = database.CreateScope())
        {
            await scope.ServiceProvider.GetRequiredService<SubmitReportHandler>().HandleAsync(
                new SubmitReport(new ReportId(id), reader, "Memory", memory, nameof(ReportReason.Copyright), "Moje zdjęcie."), TestContext.Current.CancellationToken);
        }

        await ReportFlowTests.WriteActionAsync(database, id, OperatorAction.Decide, "Removed");
        await ReportFlowTests.FireAsync(database);
        await ReportFlowTests.FireAsync(database);

        Assert.True(await ModerationTestDatabase.EventuallyAsync(async () => await database.QueuedEventsAsync(nameof(ReportDecidedEvent), id) == 1));
        await using var connection = await database.OpenAsAsync(ModerationTestDatabase.Role);
        await using var command = new NpgsqlCommand("SELECT result FROM moderation.operator_actions WHERE report_id = @id", connection);
        command.Parameters.AddWithValue("id", id);
        Assert.Equal("Applied", await command.ExecuteScalarAsync(TestContext.Current.CancellationToken));
    }
}
