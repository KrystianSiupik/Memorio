using Memorio.Modules.Moderation.Contracts;
using Memorio.Modules.Moderation.Domain.Reports;
using Memorio.SharedKernel.Domain;
using Microsoft.EntityFrameworkCore;
using Npgsql;
using Wolverine.EntityFrameworkCore;
using Wolverine.Runtime;

namespace Memorio.Modules.Moderation.Infrastructure.Persistence;

internal sealed class Reports(IDbContextOutbox<ModerationDbContext> outbox) : IReports
{
    private ModerationDbContext Database => outbox.DbContext;

    public Task<Report?> FindAsync(ReportId id, CancellationToken cancellationToken) =>
        Database.Reports.FirstOrDefaultAsync(found => found.Id == id, cancellationToken);

    public Task<Report?> FindOpenAsync(Report like, CancellationToken cancellationToken) =>
        Database.Reports.FirstOrDefaultAsync(
            found => found.State == ReportState.Open && found.ReporterKey == like.ReporterKey && found.TargetKey == like.TargetKey,
            cancellationToken);

    public void Add(Report report) => Database.Reports.Add(report);

    public async Task<IReadOnlyList<Guid>> FindPendingActionsAsync(CancellationToken cancellationToken) =>
        await Database.Actions.AsNoTracking().Where(action => action.AppliedAt == null).OrderBy(action => action.DecidedAt).Select(action => action.Id).ToListAsync(cancellationToken);

    public Task<OperatorAction?> FindActionAsync(Guid id, CancellationToken cancellationToken) =>
        Database.Actions.FirstOrDefaultAsync(action => action.Id == id, cancellationToken);

    public async Task<bool> SaveAsync(CancellationToken cancellationToken)
    {
        var raising = Database.ChangeTracker.Entries<Report>().SelectMany(entry => entry.Entity.DomainEvents).ToList();
        foreach (var domainEvent in raising)
        {
            await PublishAsync(domainEvent);
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
            if (Database.Database.CurrentTransaction is { } transaction)
            {
                await transaction.RollbackAsync(cancellationToken);
                await transaction.DisposeAsync();
            }

            Database.ChangeTracker.Clear();
            return false;
        }

        foreach (var entry in Database.ChangeTracker.Entries<Report>())
        {
            entry.Entity.ClearDomainEvents();
        }

        return true;
    }

    private ValueTask PublishAsync(IDomainEvent domainEvent) => domainEvent switch
    {
        ContentReported reported => outbox.PublishAsync(new ContentReportedEvent(
            reported.EventId, reported.OccurredAt, reported.ReportId.Value, reported.Category.ToString(), reported.AuthorAccountId, reported.HiddenNow, reported.Sequence)),
        ReportDecided decided => outbox.PublishAsync(new ReportDecidedEvent(
            decided.EventId, decided.OccurredAt, decided.ReportId.Value, decided.Verdict.ToString(), decided.AuthorAccountId, decided.Sequence)),
        ContentHidden hidden => outbox.PublishAsync(new ContentHiddenEvent(
            hidden.EventId, hidden.OccurredAt, hidden.ReportId.Value, hidden.Target.ToString(), hidden.ResourceId, hidden.AuthorAccountId, hidden.Sequence)),
        ContentRestored restored => outbox.PublishAsync(new ContentRestoredEvent(restored.EventId, restored.OccurredAt, restored.ReportId.Value, restored.ResourceId, restored.Sequence)),
        AppealDecided appeal => outbox.PublishAsync(new AppealDecidedEvent(appeal.EventId, appeal.OccurredAt, appeal.ReportId.Value, appeal.Reversed, appeal.AuthorAccountId, appeal.Sequence)),

        // The others stay inside the module.
        _ => ValueTask.CompletedTask,
    };
}
