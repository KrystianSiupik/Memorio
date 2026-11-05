using Memorio.Modules.Account.Contracts;
using Memorio.Modules.Succession.Application.Commands.Cases;
using Memorio.Modules.Succession.Application.Policies;
using Memorio.Modules.Succession.Application.Queries;
using Memorio.Modules.Succession.Contracts;
using Memorio.Modules.Succession.Domain.Cases;
using Memorio.Modules.Succession.Infrastructure.Scheduling;
using Memorio.SharedKernel.Results;
using Memorio.SharedKernel.Scheduling;
using Microsoft.Extensions.DependencyInjection;
using Npgsql;

namespace Memorio.Modules.Succession.Tests;

internal static class CaseSteps
{
    private static CancellationToken Cancellation => TestContext.Current.CancellationToken;

    public static async Task<Result> ReportAsync(this SuccessionTestDatabase database, Guid caseId, Guid album, Guid person, string? upload)
    {
        await using var scope = database.CreateScope();
        return await scope.ServiceProvider.GetRequiredService<ReportDeathHandler>().HandleAsync(new ReportDeath(new TakeoverCaseId(caseId), album, person, upload), Cancellation);
    }

    public static async Task<Guid> ReportedAsync(this SuccessionTestDatabase database, Guid album, Guid person)
    {
        var caseId = Guid.CreateVersion7();
        Assert.True((await database.ReportAsync(caseId, album, person, database.Neighbours.AddUpload())).IsSuccess);
        return caseId;
    }

    public static async Task<Result> DecideAsync(this SuccessionTestDatabase database, Guid caseId, bool confirmed)
    {
        await using var scope = database.CreateScope();
        return await scope.ServiceProvider.GetRequiredService<DecideReportHandler>().HandleAsync(new DecideReport(new TakeoverCaseId(caseId), confirmed), Cancellation);
    }

    public static async Task<Result> AnswerAsync(this SuccessionTestDatabase database, Guid caseId, Guid person, bool accepted)
    {
        await using var scope = database.CreateScope();
        return await scope.ServiceProvider.GetRequiredService<AnswerOfferHandler>().HandleAsync(new AnswerOffer(new TakeoverCaseId(caseId), person, accepted), Cancellation);
    }

    public static async Task<TakeoverCase> CaseAsync(this SuccessionTestDatabase database, Guid caseId)
    {
        await using var scope = database.CreateScope();
        return (await scope.ServiceProvider.GetRequiredService<ITakeoverCases>().FindAsync(new TakeoverCaseId(caseId), Cancellation))!;
    }
}

/// <summary>E9 on a real database: from a report to an answer.</summary>
public sealed class TakeoverFlowTests(SuccessionTestDatabase database)
{
    private static CancellationToken Cancellation => TestContext.Current.CancellationToken;

    [Fact]
    public async Task Only_someone_in_the_album_reports_and_not_the_owner()
    {
        var (album, owner, _) = database.Neighbours.NewAlbum();

        Assert.Equal(CaseErrors.NoAccess, (await database.ReportAsync(Guid.CreateVersion7(), album, database.Neighbours.NewAccount(), database.Neighbours.AddUpload())).Error);
        Assert.Equal(CaseErrors.NoAccess, (await database.ReportAsync(Guid.CreateVersion7(), album, owner, database.Neighbours.AddUpload())).Error);
    }

    [Fact]
    public async Task A_report_needs_a_file_that_arrived()
    {
        var (album, _, member) = database.Neighbours.NewAlbum();

        Assert.Equal(CaseErrors.InvalidUpload, (await database.ReportAsync(Guid.CreateVersion7(), album, member, null)).Error);
        Assert.Equal(CaseErrors.InvalidUpload, (await database.ReportAsync(Guid.CreateVersion7(), album, member, "photos/not-an-upload")).Error);
        Assert.Equal(CaseErrors.UploadMissing, (await database.ReportAsync(Guid.CreateVersion7(), album, member, "uploads/never-sent")).Error);
    }

    [Fact]
    public async Task The_same_report_again_changes_nothing_and_another_waits_for_the_first()
    {
        var (album, _, member) = database.Neighbours.NewAlbum();
        var caseId = await database.ReportedAsync(album, member);

        Assert.True((await database.ReportAsync(caseId, album, member, null)).IsSuccess);
        Assert.Equal(CaseErrors.AlreadyReported, (await database.ReportAsync(Guid.CreateVersion7(), album, member, database.Neighbours.AddUpload())).Error);
    }

    [Fact]
    public async Task Six_reports_at_once_leave_one_case_and_the_other_files_go()
    {
        var (album, _, member) = database.Neighbours.NewAlbum();

        var results = await Task.WhenAll(Enumerable.Range(0, 6).Select(_ => database.ReportAsync(Guid.CreateVersion7(), album, member, database.Neighbours.AddUpload())));

        Assert.Single(results, result => result.IsSuccess);
    }

    [Fact]
    public async Task Confirming_stores_the_successor_of_that_moment()
    {
        var (album, _, member) = database.Neighbours.NewAlbum();
        var caseId = await database.ReportedAsync(album, member);

        Assert.True((await database.DecideAsync(caseId, confirmed: true)).IsSuccess);
        database.Neighbours.ChangeSuccessor(album, Guid.CreateVersion7());

        var found = await database.CaseAsync(caseId);
        Assert.Equal(member, found.SuccessorAccountId);
        Assert.True((await database.AnswerAsync(caseId, member, accepted: true)).IsSuccess);
        Assert.True(await SuccessionTestDatabase.EventuallyAsync(async () => await database.QueuedEventsAsync(nameof(AlbumTransferredEvent), album) == 1));
    }

    [Fact]
    public async Task A_second_decision_is_refused_and_the_events_are_in_the_queue()
    {
        var (album, owner, member) = database.Neighbours.NewAlbum();
        var caseId = await database.ReportedAsync(album, member);

        Assert.True((await database.DecideAsync(caseId, confirmed: true)).IsSuccess);
        Assert.Equal(CaseErrors.AlreadyDecided, (await database.DecideAsync(caseId, confirmed: false)).Error);

        Assert.True(await SuccessionTestDatabase.EventuallyAsync(async () => await database.QueuedEventsAsync(nameof(DeathConfirmedEvent), owner) == 1));
        Assert.True(await SuccessionTestDatabase.EventuallyAsync(async () => await database.QueuedEventsAsync(nameof(TakeoverOfferedEvent), album) == 1));
    }

    [Fact]
    public async Task Two_answers_at_once_leave_one()
    {
        var (album, _, member) = database.Neighbours.NewAlbum();
        var caseId = await database.ReportedAsync(album, member);
        await database.DecideAsync(caseId, confirmed: true);

        var answers = await Task.WhenAll(database.AnswerAsync(caseId, member, true), database.AnswerAsync(caseId, member, false));

        Assert.Single(answers, answer => answer.IsSuccess);
        Assert.Contains((await database.CaseAsync(caseId)).State, new[] { CaseState.Transferred, CaseState.Refused });
    }

    [Fact]
    public async Task After_a_rejected_report_a_new_one_can_come()
    {
        var (album, _, member) = database.Neighbours.NewAlbum();
        var caseId = await database.ReportedAsync(album, member);
        await database.DecideAsync(caseId, confirmed: false);

        await database.ReportedAsync(album, member);

        Assert.Equal(CaseState.Rejected, (await database.CaseAsync(caseId)).State);
    }

    [Fact]
    public async Task The_list_of_offers_shows_only_ones_own_while_they_wait()
    {
        var (album, _, member) = database.Neighbours.NewAlbum();
        var caseId = await database.ReportedAsync(album, member);
        await using var scope = database.CreateScope();
        var queries = scope.ServiceProvider.GetRequiredService<ITakeoverQueries>();
        var now = DateTimeOffset.UtcNow;

        Assert.Empty(await queries.OpenOffersOfAsync(member, now, Cancellation));

        await database.DecideAsync(caseId, confirmed: true);

        Assert.Equal(caseId, Assert.Single(await queries.OpenOffersOfAsync(member, now, Cancellation)).Id);
        Assert.Empty(await queries.OpenOffersOfAsync(database.Neighbours.NewAccount(), now, Cancellation));
        Assert.Empty(await queries.OpenOffersOfAsync(member, now.AddDays(31), Cancellation));

        var offers = scope.ServiceProvider.GetRequiredService<ITakeoverOffers>();
        Assert.Equal(member, (await offers.FindOpenAsync(caseId, Cancellation))!.SuccessorAccountId);
        await database.AnswerAsync(caseId, member, accepted: false);
        Assert.Null(await offers.FindOpenAsync(caseId, Cancellation));
    }

    [Fact]
    public async Task A_legacy_contact_is_named_changed_and_removed()
    {
        var owner = database.Neighbours.NewAccount();
        await using var scope = database.CreateScope();
        var handler = scope.ServiceProvider.GetRequiredService<NameLegacyContactHandler>();
        var contacts = scope.ServiceProvider.GetRequiredService<ILegacyContacts>();

        Assert.Equal(CaseErrors.InvalidEmail, (await handler.HandleAsync(owner, "nie adres", Cancellation)).Error);
        Assert.Equal(CaseErrors.CannotNameYourself, (await handler.HandleAsync(owner, database.Neighbours.EmailOf(owner).ToUpperInvariant(), Cancellation)).Error);
        Assert.True((await handler.HandleAsync(owner, " Ania@Example.invalid ", Cancellation)).IsSuccess);
        Assert.Equal("ania@example.invalid", await contacts.ContactOfAsync(owner, Cancellation));
        Assert.True((await handler.HandleAsync(owner, "kasia@example.invalid", Cancellation)).IsSuccess);
        Assert.Equal("kasia@example.invalid", await contacts.ContactOfAsync(owner, Cancellation));

        Assert.True((await handler.RemoveAsync(owner, Cancellation)).IsSuccess);
        Assert.True((await handler.RemoveAsync(owner, Cancellation)).IsSuccess);
        Assert.Null(await contacts.ContactOfAsync(owner, Cancellation));
    }
}

/// <summary>SD6, P21 and P23: E121 delivered twice.</summary>
public sealed class DeathConfirmedHandlerIdempotencyTests(SuccessionTestDatabase database)
{
    private static CancellationToken Cancellation => TestContext.Current.CancellationToken;

    [Fact]
    public async Task Every_album_gets_one_case_and_the_contact_one_share()
    {
        var owner = database.Neighbours.NewAccount();
        var (reported, _, member) = database.Neighbours.NewAlbum(owner: owner);
        var (other, _, _) = database.Neighbours.NewAlbum(owner: owner);
        var (without, _, _) = database.Neighbours.NewAlbum(withSuccessor: false, owner: owner);
        await using (var scope = database.CreateScope())
        {
            await scope.ServiceProvider.GetRequiredService<NameLegacyContactHandler>().HandleAsync(owner, "ania@example.invalid", Cancellation);
        }

        var caseId = await database.ReportedAsync(reported, member);
        await database.DecideAsync(caseId, confirmed: true);
        var confirmed = new DeathConfirmedEvent(Guid.CreateVersion7(), DateTimeOffset.UtcNow, owner, 2);

        for (var delivery = 0; delivery < 2; delivery++)
        {
            await using var scope = database.CreateScope();
            await scope.ServiceProvider.GetRequiredService<ActOnConfirmedDeath>().HandleAsync(confirmed, Cancellation);
        }

        Assert.True(await SuccessionTestDatabase.EventuallyAsync(async () => await database.QueuedEventsAsync(nameof(TakeoverOfferedEvent), other) == 1));
        Assert.True(await SuccessionTestDatabase.EventuallyAsync(async () => await database.QueuedEventsAsync(nameof(TakeoverRefusedEvent), without) == 1));
        Assert.Equal(1, await database.QueuedEventsAsync(nameof(TakeoverOfferedEvent), reported));
        Assert.True(await SuccessionTestDatabase.EventuallyAsync(async () => await database.QueuedEventsAsync(nameof(PrivateArchiveSharedEvent), owner) == 1));
    }

    [Fact]
    public async Task Without_a_contact_nobody_gets_anything()
    {
        var owner = database.Neighbours.NewAccount();
        await using var scope = database.CreateScope();

        await scope.ServiceProvider.GetRequiredService<ActOnConfirmedDeath>().HandleAsync(new DeathConfirmedEvent(Guid.CreateVersion7(), DateTimeOffset.UtcNow, owner, 2), Cancellation);

        await Task.Delay(300, Cancellation);
        Assert.Equal(0, await database.QueuedEventsAsync(nameof(PrivateArchiveSharedEvent), owner));
    }
}

/// <summary>SD6: E7 delivered twice.</summary>
public sealed class AccountDeletedHandlerIdempotencyTests(SuccessionTestDatabase database)
{
    private static CancellationToken Cancellation => TestContext.Current.CancellationToken;

    [Fact]
    public async Task An_album_with_a_successor_gets_one_offer_one_without_none_and_the_contact_goes()
    {
        var owner = database.Neighbours.NewAccount();
        var (with, _, _) = database.Neighbours.NewAlbum(owner: owner);
        var (without, _, _) = database.Neighbours.NewAlbum(withSuccessor: false, owner: owner);
        await using (var scope = database.CreateScope())
        {
            await scope.ServiceProvider.GetRequiredService<NameLegacyContactHandler>().HandleAsync(owner, "ania@example.invalid", Cancellation);
        }

        var deleted = new AccountDeleted(Guid.CreateVersion7(), DateTimeOffset.UtcNow, owner, 2);
        for (var delivery = 0; delivery < 2; delivery++)
        {
            await using var scope = database.CreateScope();
            await scope.ServiceProvider.GetRequiredService<OfferAlbumsWhenAccountDeleted>().HandleAsync(deleted, Cancellation);
        }

        Assert.True(await SuccessionTestDatabase.EventuallyAsync(async () => await database.QueuedEventsAsync(nameof(TakeoverOfferedEvent), with) == 1));
        Assert.Equal(0, await database.QueuedEventsAsync(nameof(TakeoverRefusedEvent), without));
        await using var read = database.CreateScope();
        Assert.Null(await read.ServiceProvider.GetRequiredService<ILegacyContacts>().ContactOfAsync(owner, Cancellation));
    }
}

/// <summary>SD6: the offer delivered twice leaves one deadline.</summary>
public sealed class TakeoverOfferedHandlerIdempotencyTests(SuccessionTestDatabase database)
{
    [Fact]
    public async Task One_deadline_at_the_end_of_the_30_days()
    {
        var caseId = Guid.CreateVersion7();
        var by = new DateTimeOffset(2030, 2, 1, 10, 0, 0, TimeSpan.Zero);
        var offered = new TakeoverOfferedEvent(Guid.CreateVersion7(), by.AddDays(-30), caseId, Guid.CreateVersion7(), Guid.CreateVersion7(), by, 2);
        await using var scope = database.CreateScope();
        var policy = scope.ServiceProvider.GetRequiredService<AnswerDeadline>();

        await policy.HandleAsync(offered, TestContext.Current.CancellationToken);
        await policy.HandleAsync(offered, TestContext.Current.CancellationToken);

        Assert.Equal(by, database.Neighbours.DeadlineOf(AnswerDeadline.JobName, caseId.ToString()));
    }
}

/// <summary>SD6: E123 delivered twice reaches Album twice, and Album finds it done (its tests show that).</summary>
public sealed class AlbumTransferredHandlerIdempotencyTests(SuccessionTestDatabase database)
{
    [Fact]
    public async Task The_effect_is_the_transfer()
    {
        var (album, to) = (Guid.CreateVersion7(), Guid.CreateVersion7());
        await using var scope = database.CreateScope();
        var transferred = new AlbumTransferredEvent(Guid.CreateVersion7(), DateTimeOffset.UtcNow, Guid.CreateVersion7(), album, Guid.CreateVersion7(), to, 3);

        await scope.ServiceProvider.GetRequiredService<ApplyCaseToAlbum>().HandleAsync(transferred, TestContext.Current.CancellationToken);
        await scope.ServiceProvider.GetRequiredService<ApplyCaseToAlbum>().HandleAsync(transferred, TestContext.Current.CancellationToken);

        Assert.Equal(2, database.Neighbours.Effects.Count(effect => effect == $"transfer:{album}:{to}"));
    }
}

/// <summary>SD6: E124 delivered twice.</summary>
public sealed class TakeoverRefusedHandlerIdempotencyTests(SuccessionTestDatabase database)
{
    [Fact]
    public async Task The_effect_is_the_release()
    {
        var album = Guid.CreateVersion7();
        await using var scope = database.CreateScope();
        var refused = new TakeoverRefusedEvent(Guid.CreateVersion7(), DateTimeOffset.UtcNow, Guid.CreateVersion7(), album, Guid.CreateVersion7(), 3);

        await scope.ServiceProvider.GetRequiredService<ApplyCaseToAlbum>().HandleAsync(refused, TestContext.Current.CancellationToken);

        Assert.Contains($"release:{album}", database.Neighbours.Effects);
    }
}

/// <summary>SD12 and P38.</summary>
public sealed class AnswerDeadlineJobIdempotencyTests(SuccessionTestDatabase database)
{
    [Fact]
    public async Task At_the_deadline_it_is_a_refusal_once()
    {
        var (album, _, member) = database.Neighbours.NewAlbum();
        var caseId = await database.ReportedAsync(album, member);
        await database.DecideAsync(caseId, confirmed: true);
        var by = (await database.CaseAsync(caseId)).AnswerBy!.Value;

        for (var firing = 0; firing < 2; firing++)
        {
            await using var scope = database.CreateScope();
            await ActivatorUtilities.CreateInstance<AnswerDeadlineJob>(scope.ServiceProvider).RunAsync(
                new ScheduledJobRun(AnswerDeadline.JobName, caseId.ToString(), by, new Dictionary<string, string>(), 1),
                TestContext.Current.CancellationToken);
        }

        Assert.Equal(CaseState.Refused, (await database.CaseAsync(caseId)).State);
        Assert.True(await SuccessionTestDatabase.EventuallyAsync(async () => await database.QueuedEventsAsync(nameof(TakeoverRefusedEvent), album) == 1));
    }
}

/// <summary>A firing goes through every row, so never next to tests that expect their rows to stay pending.</summary>
[CollectionDefinition(nameof(OperatorJobs), DisableParallelization = true)]
public sealed class OperatorJobs;

/// <summary>SD12: what deploy/succession/cases.sh writes, carried out once.</summary>
[Collection(nameof(OperatorJobs))]
public sealed class OperatorDecisionsJobIdempotencyTests(SuccessionTestDatabase database)
{
    private static CancellationToken Cancellation => TestContext.Current.CancellationToken;

    private async Task WriteDecisionAsync(Guid caseId, bool confirmed)
    {
        await using var connection = await database.OpenAsAsync(SuccessionTestDatabase.Role);
        await using var command = new NpgsqlCommand("INSERT INTO succession.operator_decisions (id, case_id, confirmed, decided_by, decided_at) VALUES (gen_random_uuid(), @caseId, @confirmed, 'operator', now())", connection);
        command.Parameters.AddWithValue("caseId", caseId);
        command.Parameters.AddWithValue("confirmed", confirmed);
        await command.ExecuteNonQueryAsync(Cancellation);
    }

    private async Task FireAsync()
    {
        await using var scope = database.CreateScope();
        await ActivatorUtilities.CreateInstance<OperatorDecisionsJob>(scope.ServiceProvider).RunAsync(
            new ScheduledJobRun(OperatorDecisionsJob.JobName, null, DateTimeOffset.UtcNow, new Dictionary<string, string>(), 1),
            Cancellation);
    }

    [Fact]
    public async Task A_decision_is_applied_once_and_a_second_one_only_says_it_was_too_late()
    {
        var (album, _, member) = database.Neighbours.NewAlbum();
        var caseId = await database.ReportedAsync(album, member);

        await WriteDecisionAsync(caseId, confirmed: true);
        await WriteDecisionAsync(caseId, confirmed: false);
        await FireAsync();
        await FireAsync();

        Assert.Equal(CaseState.Offered, (await database.CaseAsync(caseId)).State);
        await using var connection = await database.OpenAsAsync(SuccessionTestDatabase.Role);
        await using var command = new NpgsqlCommand("SELECT count(*) FROM succession.operator_decisions WHERE case_id = @caseId AND applied_at IS NOT NULL", connection);
        command.Parameters.AddWithValue("caseId", caseId);
        Assert.Equal(2L, await command.ExecuteScalarAsync(Cancellation));
    }

    [Fact]
    public async Task A_rejection_closes_the_report()
    {
        var (album, _, member) = database.Neighbours.NewAlbum();
        var caseId = await database.ReportedAsync(album, member);

        await WriteDecisionAsync(caseId, confirmed: false);
        await FireAsync();

        Assert.Equal(CaseState.Rejected, (await database.CaseAsync(caseId)).State);
        Assert.True(await SuccessionTestDatabase.EventuallyAsync(async () => await database.QueuedEventsAsync(nameof(DeathReportRejectedEvent), caseId) == 1));
    }
}

/// <summary>SD12: a firing moves everything due, so with the same restriction.</summary>
[Collection(nameof(OperatorJobs))]
public sealed class SuccessionSweepJobIdempotencyTests(SuccessionTestDatabase database)
{
    private async Task FireAsync(DateTimeOffset at)
    {
        await using var scope = database.CreateScope();
        await ActivatorUtilities.CreateInstance<SuccessionSweepJob>(scope.ServiceProvider).RunAsync(
            new ScheduledJobRun(SuccessionSweepJob.JobName, null, at, new Dictionary<string, string>(), 1),
            TestContext.Current.CancellationToken);
    }

    [Fact]
    public async Task An_overdue_offer_ends_and_a_year_later_the_certificate_goes_once()
    {
        var (album, _, member) = database.Neighbours.NewAlbum();
        var caseId = await database.ReportedAsync(album, member);
        await database.DecideAsync(caseId, confirmed: true);
        var by = (await database.CaseAsync(caseId)).AnswerBy!.Value;

        await FireAsync(by);
        await FireAsync(by);
        var closed = await database.CaseAsync(caseId);
        Assert.Equal(CaseState.Refused, closed.State);
        var key = closed.CertificateKey!;

        await FireAsync(closed.ClosedAt!.Value.AddMonths(11));
        Assert.DoesNotContain(key, database.Neighbours.Deleted);

        await FireAsync(closed.ClosedAt!.Value.AddMonths(12));
        await FireAsync(closed.ClosedAt!.Value.AddMonths(12));

        Assert.Single(database.Neighbours.Deleted, deleted => deleted == key);
        Assert.Null((await database.CaseAsync(caseId)).CertificateKey);
    }
}
