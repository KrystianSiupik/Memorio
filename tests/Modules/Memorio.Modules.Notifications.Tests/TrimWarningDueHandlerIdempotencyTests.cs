using Memorio.Modules.Notifications.Application.Policies;
using Memorio.Modules.Notifications.Application.Ports;
using Memorio.Modules.Notifications.Contracts;
using Memorio.Modules.Notifications.Infrastructure.Scheduling;
using Memorio.Modules.Plans.Contracts;
using Memorio.SharedKernel.Scheduling;
using Microsoft.Extensions.DependencyInjection;

namespace Memorio.Modules.Notifications.Tests;

/// <summary>SD6: E97 delivered twice, on a real database; the proof of delivery for the trim (S2:B1, S2:C10).</summary>
public sealed class TrimWarningDueHandlerIdempotencyTests(NotificationsDatabase database)
{
    private const string Kind = nameof(NotificationKind.TrimWarningAlbum);

    private static CancellationToken Cancellation => TestContext.Current.CancellationToken;

    private (Guid Album, Guid Owner, Guid Participant) NewAlbum()
    {
        var (album, owner, participant) = (Guid.CreateVersion7(), database.Neighbours.NewAccount(), database.Neighbours.NewAccount());
        database.Neighbours.Members(album, owner, participant);
        return (album, owner, participant);
    }

    private async Task DeliverAsync(TrimWarningDueEvent due)
    {
        await using var scope = database.CreateScope();
        await scope.ServiceProvider.GetRequiredService<WarnWhenDue>().HandleAsync(due, Cancellation);
    }

    private static TrimWarningDueEvent Due(Guid resource, bool isAlbum = true, int days = 30, int round = 3) =>
        new(Guid.CreateVersion7(), DateTimeOffset.UtcNow, resource, isAlbum, days, DateTimeOffset.UtcNow.AddDays(days), round);

    [Fact]
    public async Task The_owner_and_each_participant_hear_once_and_the_proof_goes_out()
    {
        var (album, owner, participant) = NewAlbum();
        var due = Due(album);

        await DeliverAsync(due);
        await DeliverAsync(due);

        Assert.Equal(1, database.Neighbours.SentTo(database.Neighbours.EmailOf(owner), Kind));
        Assert.Equal(1, database.Neighbours.SentTo(database.Neighbours.EmailOf(participant), Kind));
        Assert.True(await NotificationsDatabase.EventuallyAsync(async () => await database.QueuedEventsAsync(nameof(TrimWarningDeliveredEvent), album) == 2));
        Assert.Equal(0, await database.QueuedEventsAsync(nameof(TrimWarningUndeliveredEvent), album));
    }

    [Fact]
    public async Task An_account_hears_for_itself()
    {
        var account = database.Neighbours.NewAccount();

        await DeliverAsync(Due(account, isAlbum: false));

        Assert.Equal(1, database.Neighbours.SentTo(database.Neighbours.EmailOf(account), nameof(NotificationKind.TrimWarningAccount)));
    }

    [Fact]
    public async Task One_refused_address_means_no_proof_and_the_others_still_hear()
    {
        var (album, owner, participant) = NewAlbum();
        database.Neighbours.Refuses(database.Neighbours.EmailOf(participant));

        await DeliverAsync(Due(album));

        Assert.Equal(1, database.Neighbours.SentTo(database.Neighbours.EmailOf(owner), Kind));
        Assert.True(await NotificationsDatabase.EventuallyAsync(async () => await database.QueuedEventsAsync(nameof(TrimWarningUndeliveredEvent), album) == 1));
        Assert.Equal(0, await database.QueuedEventsAsync(nameof(TrimWarningDeliveredEvent), album));
    }

    [Fact]
    public async Task Without_a_provider_nobody_was_told_so_there_is_no_proof()
    {
        var account = database.Neighbours.NewAccount();
        database.Neighbours.Refuses(database.Neighbours.EmailOf(account), DeliveryOutcome.NotSent);

        await DeliverAsync(Due(account, isAlbum: false));

        Assert.True(await NotificationsDatabase.EventuallyAsync(async () => await database.QueuedEventsAsync(nameof(TrimWarningUndeliveredEvent), account) == 1));
    }

    [Fact]
    public async Task A_provider_that_does_not_answer_fails_so_the_event_is_retried_and_finishes_it()
    {
        var (album, owner, participant) = NewAlbum();
        database.Neighbours.FailNextSendTo(database.Neighbours.EmailOf(participant));
        var due = Due(album);

        await Assert.ThrowsAsync<InvalidOperationException>(() => DeliverAsync(due));
        await DeliverAsync(due);

        Assert.Equal(1, database.Neighbours.SentTo(database.Neighbours.EmailOf(owner), Kind));
        Assert.Equal(1, database.Neighbours.SentTo(database.Neighbours.EmailOf(participant), Kind));
    }

    [Fact]
    public async Task Someone_whose_account_is_gone_needs_no_warning()
    {
        var (album, owner, participant) = NewAlbum();
        database.Neighbours.Gone(participant);

        await DeliverAsync(Due(album));

        Assert.Equal(1, database.Neighbours.SentTo(database.Neighbours.EmailOf(owner), Kind));
        Assert.True(await NotificationsDatabase.EventuallyAsync(async () => await database.QueuedEventsAsync(nameof(TrimWarningDeliveredEvent), album) == 1));
    }

    [Fact]
    public async Task A_new_round_is_a_new_message_and_each_warning_has_its_own()
    {
        var account = database.Neighbours.NewAccount();
        var email = database.Neighbours.EmailOf(account);
        const string kind = nameof(NotificationKind.TrimWarningAccount);

        await DeliverAsync(Due(account, isAlbum: false, days: 90, round: 3));
        await DeliverAsync(Due(account, isAlbum: false, days: 30, round: 3));
        Assert.Equal(2, database.Neighbours.SentTo(email, kind));

        await DeliverAsync(Due(account, isAlbum: false, days: 30, round: 7));
        Assert.Equal(3, database.Neighbours.SentTo(email, kind));
    }

    [Fact]
    public void The_message_says_when_and_what_can_be_done()
    {
        var (subject, body) = Memorio.Modules.Notifications.Application.NotificationTexts.Of(NotificationKind.TrimWarningAlbum, "Rodzina", null, "7 dni");

        Assert.Contains("7 dni", subject);
        Assert.Contains("album „Rodzina”", body);
        Assert.Contains("wyjęte z albumu", body);
        Assert.Contains("zachowają swoje wspomnienia", body);
        Assert.Contains("odnowić plan", body);
    }
}

/// <summary>SD6 and P26: E100 delivered twice leaves one monthly deadline.</summary>
public sealed class TrimHeldHandlerIdempotencyTests(NotificationsDatabase database)
{
    private static CancellationToken Cancellation => TestContext.Current.CancellationToken;

    [Fact]
    public async Task The_retry_is_a_month_later_under_one_key()
    {
        var resource = Guid.CreateVersion7();
        var at = new DateTimeOffset(2030, 1, 31, 10, 0, 0, TimeSpan.Zero);
        var held = new TrimHeldEvent(Guid.CreateVersion7(), at, resource, true, 30, 4, 9);
        await using var scope = database.CreateScope();
        var policy = scope.ServiceProvider.GetRequiredService<RetryWhileHeld>();

        await policy.HandleAsync(held, Cancellation);
        await policy.HandleAsync(held, Cancellation);

        Assert.Equal(at.AddMonths(1), database.Deadlines.Find(RetryWhileHeld.JobName, resource.ToString())?.ScheduledFor);
    }
}

/// <summary>SD6: E94 delivered twice: a paid plan ends the retries.</summary>
public sealed class PlanActivatedHandlerIdempotencyTests(NotificationsDatabase database)
{
    private static CancellationToken Cancellation => TestContext.Current.CancellationToken;

    [Fact]
    public async Task Paying_ends_the_retries()
    {
        var resource = Guid.CreateVersion7();
        await using var scope = database.CreateScope();
        var policy = scope.ServiceProvider.GetRequiredService<RetryWhileHeld>();
        await policy.HandleAsync(new TrimHeldEvent(Guid.CreateVersion7(), DateTimeOffset.UtcNow, resource, false, 30, 4, 9), Cancellation);
        var paid = new PlanActivatedEvent(Guid.CreateVersion7(), DateTimeOffset.UtcNow, resource, false, 10, 1, 10);

        await policy.HandleAsync(paid, Cancellation);
        await policy.HandleAsync(paid, Cancellation);

        Assert.Null(database.Deadlines.Find(RetryWhileHeld.JobName, resource.ToString())?.ScheduledFor);
    }
}

/// <summary>SD12: the same firing twice; whoever got it is not written to again, and a failure tries again in a month.</summary>
public sealed class TrimWarningRetryJobIdempotencyTests(NotificationsDatabase database)
{
    private static CancellationToken Cancellation => TestContext.Current.CancellationToken;

    private async Task FireAsync(ScheduledJobRun run)
    {
        await using var scope = database.CreateScope();
        await ActivatorUtilities.CreateInstance<TrimWarningRetryJob>(scope.ServiceProvider).RunAsync(run, Cancellation);
    }

    private static ScheduledJobRun Run(Guid resource, DateTimeOffset at, bool isAlbum = false) => new(
        RetryWhileHeld.JobName,
        resource.ToString(),
        at,
        new Dictionary<string, string> { [RetryWhileHeld.IsAlbumKey] = isAlbum ? "True" : "False", [RetryWhileHeld.DaysKey] = "30", [RetryWhileHeld.RoundKey] = "4" },
        1);

    [Fact]
    public async Task Still_refused_means_again_in_a_month_then_delivered_means_the_end()
    {
        var account = database.Neighbours.NewAccount();
        var email = database.Neighbours.EmailOf(account);
        const string kind = nameof(NotificationKind.TrimWarningAccount);
        var at = new DateTimeOffset(2030, 3, 1, 10, 0, 0, TimeSpan.Zero);
        database.Neighbours.Refuses(email);

        await FireAsync(Run(account, at));
        await FireAsync(Run(account, at));

        Assert.Equal(at.AddMonths(1), database.Deadlines.Find(RetryWhileHeld.JobName, account.ToString())?.ScheduledFor);
        Assert.Equal(0, database.Neighbours.SentTo(email, kind));

        database.Neighbours.Accepts(email);
        await database.Deadlines.CancelAsync(RetryWhileHeld.JobName, account.ToString(), Cancellation);
        await FireAsync(Run(account, at.AddMonths(1)));
        await FireAsync(Run(account, at.AddMonths(1)));

        Assert.Equal(1, database.Neighbours.SentTo(email, kind));
        Assert.Null(database.Deadlines.Find(RetryWhileHeld.JobName, account.ToString())?.ScheduledFor);
        Assert.True(await NotificationsDatabase.EventuallyAsync(async () => await database.QueuedEventsAsync(nameof(TrimWarningDeliveredEvent), account) == 2));
    }

    [Fact]
    public async Task Who_got_the_warning_is_not_written_to_again()
    {
        var album = Guid.CreateVersion7();
        var (owner, participant) = (database.Neighbours.NewAccount(), database.Neighbours.NewAccount());
        database.Neighbours.Members(album, owner, participant);
        database.Neighbours.Refuses(database.Neighbours.EmailOf(participant));
        const string kind = nameof(NotificationKind.TrimWarningAlbum);
        var at = new DateTimeOffset(2030, 3, 1, 10, 0, 0, TimeSpan.Zero);

        await FireAsync(Run(album, at, isAlbum: true));
        database.Neighbours.Accepts(database.Neighbours.EmailOf(participant));
        await FireAsync(Run(album, at.AddMonths(1), isAlbum: true));

        Assert.Equal(1, database.Neighbours.SentTo(database.Neighbours.EmailOf(owner), kind));
        Assert.Equal(1, database.Neighbours.SentTo(database.Neighbours.EmailOf(participant), kind));
    }
}
