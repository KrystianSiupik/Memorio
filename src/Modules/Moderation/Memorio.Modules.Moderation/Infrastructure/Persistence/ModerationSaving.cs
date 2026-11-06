using Memorio.Modules.Moderation.Contracts;
using Memorio.Modules.Moderation.Domain.Orders;
using Memorio.Modules.Moderation.Domain.Reports;
using Memorio.SharedKernel.Domain;
using Microsoft.EntityFrameworkCore;
using Npgsql;
using Wolverine.EntityFrameworkCore;
using Wolverine.Runtime;

namespace Memorio.Modules.Moderation.Infrastructure.Persistence;

/// <summary>Reports and orders share one context: a save takes every change of the scope with all its events.</summary>
internal static class ModerationSaving
{
    public static async Task<bool> SaveAsync(IDbContextOutbox<ModerationDbContext> outbox, CancellationToken cancellationToken)
    {
        var database = outbox.DbContext;
        var raising = database.ChangeTracker.Entries<Report>().SelectMany(entry => entry.Entity.DomainEvents)
            .Concat(database.ChangeTracker.Entries<AuthorityOrder>().SelectMany(entry => entry.Entity.DomainEvents))
            .ToList();
        foreach (var domainEvent in raising)
        {
            await PublishAsync(outbox, domainEvent);
        }

        try
        {
            // Every save flushes (D-278): by default what a second save in a scope publishes is dropped.
            await outbox.SaveChangesAndFlushMessagesAsync(MultiFlushMode.AllowMultiples, cancellationToken);
        }
        catch (DbUpdateException exception) when (exception is DbUpdateConcurrencyException
            || exception.InnerException is PostgresException { SqlState: PostgresErrorCodes.UniqueViolation })
        {
            // The refused transaction must not stay open: PostgreSQL would refuse everything after a failed statement.
            if (database.Database.CurrentTransaction is { } transaction)
            {
                await transaction.RollbackAsync(cancellationToken);
                await transaction.DisposeAsync();
            }

            database.ChangeTracker.Clear();
            return false;
        }

        foreach (var entry in database.ChangeTracker.Entries<Report>())
        {
            entry.Entity.ClearDomainEvents();
        }

        foreach (var entry in database.ChangeTracker.Entries<AuthorityOrder>())
        {
            entry.Entity.ClearDomainEvents();
        }

        return true;
    }

    private static ValueTask PublishAsync(IDbContextOutbox<ModerationDbContext> outbox, IDomainEvent domainEvent) => domainEvent switch
    {
        ContentReported reported => outbox.PublishAsync(new ContentReportedEvent(
            reported.EventId, reported.OccurredAt, reported.ReportId.Value, reported.Category.ToString(), reported.AuthorAccountId, reported.HiddenNow, reported.Sequence)),
        ReportDecided decided => outbox.PublishAsync(new ReportDecidedEvent(
            decided.EventId, decided.OccurredAt, decided.ReportId.Value, decided.Verdict.ToString(), decided.AuthorAccountId, decided.Sequence)),
        ContentHidden hidden => outbox.PublishAsync(new ContentHiddenEvent(
            hidden.EventId, hidden.OccurredAt, hidden.ReportId.Value, hidden.Target.ToString(), hidden.ResourceId, hidden.AuthorAccountId, hidden.Sequence)),
        ContentRestored restored => outbox.PublishAsync(new ContentRestoredEvent(restored.EventId, restored.OccurredAt, restored.ReportId.Value, restored.ResourceId, restored.Sequence)),
        AppealDecided appeal => outbox.PublishAsync(new AppealDecidedEvent(appeal.EventId, appeal.OccurredAt, appeal.ReportId.Value, appeal.Reversed, appeal.AuthorAccountId, appeal.Sequence)),

        RemovalOrderReceived removal => outbox.PublishAsync(new RemovalOrderReceivedEvent(removal.EventId, removal.OccurredAt, removal.OrderId.Value, removal.Sequence)),
        DataOrderReceived data => outbox.PublishAsync(new DataOrderReceivedEvent(data.EventId, data.OccurredAt, data.OrderId.Value, data.Sequence)),
        AccountDataSecured secured => outbox.PublishAsync(new AccountDataSecuredEvent(secured.EventId, secured.OccurredAt, secured.OrderId.Value, secured.AccountId, secured.Sequence)),
        ContentHiddenByOrder byOrder => PublishHiddenByOrderAsync(outbox, byOrder),

        // The others stay inside the module.
        _ => ValueTask.CompletedTask,
    };

    /// <summary>The hold of the order in Access as for a report (P32 makes it a no-op after the port), and the author's notice.</summary>
    private static async ValueTask PublishHiddenByOrderAsync(IDbContextOutbox<ModerationDbContext> outbox, ContentHiddenByOrder hidden)
    {
        await outbox.PublishAsync(new ContentHiddenEvent(hidden.EventId, hidden.OccurredAt, hidden.OrderId.Value, hidden.Target.ToString(), hidden.ResourceId, hidden.AuthorAccountId, hidden.Sequence));
        await outbox.PublishAsync(new RemovalOrderExecutedEvent(Guid.CreateVersion7(), hidden.OccurredAt, hidden.OrderId.Value, hidden.AuthorAccountId, hidden.Confidential, hidden.Sequence));
    }
}
