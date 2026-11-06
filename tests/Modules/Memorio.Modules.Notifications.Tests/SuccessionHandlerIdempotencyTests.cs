using Memorio.Modules.Notifications.Application.Policies;
using Memorio.Modules.Notifications.Application.Ports;
using Memorio.Modules.Notifications.Infrastructure.Scheduling;
using Memorio.Modules.Succession.Contracts;
using Memorio.SharedKernel.Scheduling;
using Microsoft.Extensions.DependencyInjection;

namespace Memorio.Modules.Notifications.Tests;

/// <summary>SD6 and P43: the offer delivered twice: one message and two reminders.</summary>
public sealed class TakeoverOfferedHandlerIdempotencyTests(NotificationsDatabase database)
{
    private static CancellationToken Cancellation => TestContext.Current.CancellationToken;

    [Fact]
    public async Task One_message_and_reminders_on_days_7_and_21()
    {
        var (caseId, successor) = (Guid.CreateVersion7(), database.Neighbours.NewAccount());
        var at = new DateTimeOffset(2030, 1, 1, 10, 0, 0, TimeSpan.Zero);
        var offered = new TakeoverOfferedEvent(Guid.CreateVersion7(), at, caseId, Guid.CreateVersion7(), successor, at.AddDays(30), 2);
        await using var scope = database.CreateScope();
        var policy = scope.ServiceProvider.GetRequiredService<NotifySuccessor>();

        await policy.HandleAsync(offered, Cancellation);
        await policy.HandleAsync(offered, Cancellation);

        Assert.Equal(1, database.Neighbours.SentTo(database.Neighbours.EmailOf(successor), nameof(NotificationKind.TakeoverOffered)));
        Assert.Equal(at.AddDays(7), database.Deadlines.Find(NotifySuccessor.JobName, $"{caseId}:7")?.ScheduledFor);
        Assert.Equal(at.AddDays(21), database.Deadlines.Find(NotifySuccessor.JobName, $"{caseId}:21")?.ScheduledFor);
    }
}

/// <summary>SD12: the same firing twice.</summary>
public sealed class SuccessorReminderJobIdempotencyTests(NotificationsDatabase database)
{
    private static CancellationToken Cancellation => TestContext.Current.CancellationToken;

    private async Task FireAsync(Guid caseId, int days)
    {
        await using var scope = database.CreateScope();
        await ActivatorUtilities.CreateInstance<SuccessorReminderJob>(scope.ServiceProvider).RunAsync(
            new ScheduledJobRun(NotifySuccessor.JobName, $"{caseId}:{days}", DateTimeOffset.UtcNow, new Dictionary<string, string> { [NotifySuccessor.DaysKey] = days.ToString(System.Globalization.CultureInfo.InvariantCulture) }, 1),
            Cancellation);
    }

    [Fact]
    public async Task Each_reminder_goes_once_and_none_after_the_answer()
    {
        var (caseId, successor) = (Guid.CreateVersion7(), database.Neighbours.NewAccount());
        var email = database.Neighbours.EmailOf(successor);
        const string kind = nameof(NotificationKind.SuccessorReminder);
        database.Neighbours.OfferOpen(caseId, new OpenTakeoverOffer(Guid.CreateVersion7(), successor, DateTimeOffset.UtcNow.AddDays(30)));

        await FireAsync(caseId, 7);
        await FireAsync(caseId, 7);
        Assert.Equal(1, database.Neighbours.SentTo(email, kind));

        database.Neighbours.OfferAnswered(caseId);
        await FireAsync(caseId, 21);
        Assert.Equal(1, database.Neighbours.SentTo(email, kind));
    }
}

/// <summary>SD6: E122 delivered twice.</summary>
public sealed class DeathReportRejectedHandlerIdempotencyTests(NotificationsDatabase database)
{
    [Fact]
    public async Task Whoever_reported_hears_once()
    {
        var person = database.Neighbours.NewAccount();
        var rejected = new DeathReportRejectedEvent(Guid.CreateVersion7(), DateTimeOffset.UtcNow, Guid.CreateVersion7(), Guid.CreateVersion7(), person, 2);
        await using var scope = database.CreateScope();
        var policy = scope.ServiceProvider.GetRequiredService<NotifyOfRejectedReport>();

        await policy.HandleAsync(rejected, TestContext.Current.CancellationToken);
        await policy.HandleAsync(rejected, TestContext.Current.CancellationToken);

        Assert.Equal(1, database.Neighbours.SentTo(database.Neighbours.EmailOf(person), nameof(NotificationKind.DeathReportRejected)));
    }
}

/// <summary>SD6: E120 delivered twice.</summary>
public sealed class DeathReportedHandlerIdempotencyTests(NotificationsDatabase database)
{
    [Fact]
    public async Task The_owner_hears_once()
    {
        var owner = database.Neighbours.NewAccount();
        var reported = new DeathReportedEvent(Guid.CreateVersion7(), DateTimeOffset.UtcNow, Guid.CreateVersion7(), Guid.CreateVersion7(), owner, 1);
        await using var scope = database.CreateScope();
        var policy = scope.ServiceProvider.GetRequiredService<NotifyOwnerOfReport>();

        await policy.HandleAsync(reported, TestContext.Current.CancellationToken);
        await policy.HandleAsync(reported, TestContext.Current.CancellationToken);

        Assert.Equal(1, database.Neighbours.SentTo(database.Neighbours.EmailOf(owner), nameof(NotificationKind.DeathReported)));
    }
}

/// <summary>SD6: E126 delivered twice.</summary>
public sealed class PrivateArchiveSharedHandlerIdempotencyTests(NotificationsDatabase database)
{
    [Fact]
    public async Task The_contact_hears_once_without_any_content()
    {
        var owner = Guid.CreateVersion7();
        var email = $"{owner}@example.invalid";
        database.Neighbours.ContactOf(owner, email);
        var shared = new PrivateArchiveSharedEvent(Guid.CreateVersion7(), DateTimeOffset.UtcNow, owner, 2);
        await using var scope = database.CreateScope();
        var policy = scope.ServiceProvider.GetRequiredService<NotifyLegacyContact>();

        await policy.HandleAsync(shared, TestContext.Current.CancellationToken);
        await policy.HandleAsync(shared, TestContext.Current.CancellationToken);

        Assert.Equal(1, database.Neighbours.SentTo(email, nameof(NotificationKind.PrivateArchiveShared)));
    }
}
