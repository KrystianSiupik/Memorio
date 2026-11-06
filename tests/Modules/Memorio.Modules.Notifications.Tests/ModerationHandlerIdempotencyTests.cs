using Memorio.Modules.Moderation.Contracts;
using Memorio.Modules.Notifications.Application.Policies;
using Memorio.Modules.Notifications.Application.Ports;
using Microsoft.Extensions.DependencyInjection;

namespace Memorio.Modules.Notifications.Tests;

/// <summary>SD6: E110 delivered twice. The reporter gets the receipt, the author the reason, each once.</summary>
public sealed class ContentReportedHandlerIdempotencyTests(NotificationsDatabase database)
{
    [Fact]
    public async Task Each_side_hears_once()
    {
        var (report, reporter, author) = (Guid.CreateVersion7(), database.Neighbours.NewAccount(), database.Neighbours.NewAccount());
        database.Neighbours.Notice(report, new ReportNotice(reporter, null, "Threat", null, null));
        var reported = new ContentReportedEvent(Guid.CreateVersion7(), DateTimeOffset.UtcNow, report, "Illegal", author, true, 1);
        await using var scope = database.CreateScope();
        var policy = scope.ServiceProvider.GetRequiredService<NotifyOfModeration>();

        await policy.HandleAsync(reported, TestContext.Current.CancellationToken);
        await policy.HandleAsync(reported, TestContext.Current.CancellationToken);

        Assert.Equal(1, database.Neighbours.SentTo(database.Neighbours.EmailOf(reporter), nameof(NotificationKind.ReportReceived)));
        Assert.Equal(1, database.Neighbours.SentTo(database.Neighbours.EmailOf(author), nameof(NotificationKind.ContentHiddenAfterReport)));
    }
}

/// <summary>SD6: E112 delivered twice; a report without an account hears on its address.</summary>
public sealed class ReportDecidedHandlerIdempotencyTests(NotificationsDatabase database)
{
    [Fact]
    public async Task Both_sides_hear_the_reasons_once()
    {
        var (report, author) = (Guid.CreateVersion7(), database.Neighbours.NewAccount());
        var email = $"{report}@example.invalid";
        database.Neighbours.Notice(report, new ReportNotice(null, email, "SomeonesImage", "Wizerunek bez zgody.", null));
        var decided = new ReportDecidedEvent(Guid.CreateVersion7(), DateTimeOffset.UtcNow, report, "Removed", author, 2);
        await using var scope = database.CreateScope();
        var policy = scope.ServiceProvider.GetRequiredService<NotifyOfModeration>();

        await policy.HandleAsync(decided, TestContext.Current.CancellationToken);
        await policy.HandleAsync(decided, TestContext.Current.CancellationToken);

        Assert.Equal(1, database.Neighbours.SentTo(email, nameof(NotificationKind.ReportActedOn)));
        Assert.Equal(1, database.Neighbours.SentTo(database.Neighbours.EmailOf(author), nameof(NotificationKind.ContentRemoved)));
    }
}

/// <summary>SD6: the decision on an appeal delivered twice.</summary>
public sealed class AppealDecidedHandlerIdempotencyTests(NotificationsDatabase database)
{
    [Fact]
    public async Task The_author_hears_once()
    {
        var (report, author) = (Guid.CreateVersion7(), database.Neighbours.NewAccount());
        database.Neighbours.Notice(report, new ReportNotice(Guid.CreateVersion7(), null, "Other", "x", "Autor ma zgodę."));
        var decided = new AppealDecidedEvent(Guid.CreateVersion7(), DateTimeOffset.UtcNow, report, true, author, 4);
        await using var scope = database.CreateScope();
        var policy = scope.ServiceProvider.GetRequiredService<NotifyOfModeration>();

        await policy.HandleAsync(decided, TestContext.Current.CancellationToken);
        await policy.HandleAsync(decided, TestContext.Current.CancellationToken);

        Assert.Equal(1, database.Neighbours.SentTo(database.Neighbours.EmailOf(author), nameof(NotificationKind.AppealReversed)));
    }
}

/// <summary>SD6: an executed order delivered twice; a confidential one tells nobody.</summary>
public sealed class RemovalOrderExecutedHandlerIdempotencyTests(NotificationsDatabase database)
{
    [Fact]
    public async Task The_author_hears_once_unless_it_is_confidential()
    {
        var (author, quiet) = (database.Neighbours.NewAccount(), database.Neighbours.NewAccount());
        await using var scope = database.CreateScope();
        var policy = scope.ServiceProvider.GetRequiredService<NotifyOfOrder>();
        var executed = new RemovalOrderExecutedEvent(Guid.CreateVersion7(), DateTimeOffset.UtcNow, Guid.CreateVersion7(), author, false, 2);

        await policy.HandleAsync(executed, TestContext.Current.CancellationToken);
        await policy.HandleAsync(executed, TestContext.Current.CancellationToken);
        await policy.HandleAsync(new RemovalOrderExecutedEvent(Guid.CreateVersion7(), DateTimeOffset.UtcNow, Guid.CreateVersion7(), quiet, true, 2), TestContext.Current.CancellationToken);

        Assert.Equal(1, database.Neighbours.SentTo(database.Neighbours.EmailOf(author), nameof(NotificationKind.ContentHiddenByOrder)));
        Assert.Equal(0, database.Neighbours.SentTo(database.Neighbours.EmailOf(quiet), nameof(NotificationKind.ContentHiddenByOrder)));
    }
}
