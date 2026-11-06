using Memorio.Modules.Succession.Contracts;
using Memorio.Modules.Succession.Domain.Cases;
using Memorio.SharedKernel.Domain;
using Microsoft.EntityFrameworkCore;
using Npgsql;
using Wolverine.EntityFrameworkCore;
using Wolverine.Runtime;

namespace Memorio.Modules.Succession.Infrastructure.Persistence;

internal sealed class TakeoverCases(IDbContextOutbox<SuccessionDbContext> outbox) : ITakeoverCases
{
    private SuccessionDbContext Database => outbox.DbContext;

    public Task<TakeoverCase?> FindAsync(TakeoverCaseId id, CancellationToken cancellationToken) =>
        Database.Cases.FirstOrDefaultAsync(found => found.Id == id, cancellationToken);

    public Task<TakeoverCase?> FindOpenAsync(Guid albumId, CancellationToken cancellationToken) =>
        Database.Cases.FirstOrDefaultAsync(found => found.AlbumId == albumId && (found.State == CaseState.Reported || found.State == CaseState.Offered), cancellationToken);

    public Task<bool> ExistsAsync(Guid albumId, Guid ownerAccountId, CancellationToken cancellationToken) =>
        Database.Cases.AnyAsync(found => found.AlbumId == albumId && found.OwnerAccountId == ownerAccountId && found.State != CaseState.Rejected, cancellationToken);

    public async Task<IReadOnlyList<TakeoverCaseId>> FindOverdueAsync(DateTimeOffset at, CancellationToken cancellationToken) =>
        await Database.Cases.AsNoTracking().Where(found => found.State == CaseState.Offered && found.AnswerBy <= at).Select(found => found.Id).ToListAsync(cancellationToken);

    public async Task<IReadOnlyList<TakeoverCaseId>> FindExpiredCertificatesAsync(DateTimeOffset closedBefore, CancellationToken cancellationToken) =>
        await Database.Cases.AsNoTracking().Where(found => found.CertificateKey != null && found.ClosedAt <= closedBefore).Select(found => found.Id).ToListAsync(cancellationToken);

    public void Add(TakeoverCase takeoverCase) => Database.Cases.Add(takeoverCase);

    public Task<LegacyContact?> FindContactAsync(Guid ownerAccountId, CancellationToken cancellationToken) =>
        Database.Contacts.FirstOrDefaultAsync(contact => contact.Id == ownerAccountId, cancellationToken);

    public void Add(LegacyContact contact) => Database.Contacts.Add(contact);

    public void Remove(LegacyContact contact) => Database.Contacts.Remove(contact);

    public async Task<IReadOnlyList<Guid>> FindPendingDecisionsAsync(CancellationToken cancellationToken) =>
        await Database.Decisions.AsNoTracking().Where(decision => decision.AppliedAt == null).OrderBy(decision => decision.DecidedAt).Select(decision => decision.Id).ToListAsync(cancellationToken);

    public Task<OperatorDecision?> FindDecisionAsync(Guid id, CancellationToken cancellationToken) =>
        Database.Decisions.FirstOrDefaultAsync(decision => decision.Id == id, cancellationToken);

    public async Task<bool> SaveAsync(CancellationToken cancellationToken)
    {
        var raising = Database.ChangeTracker.Entries<TakeoverCase>().Select(entry => (IEnumerable<IDomainEvent>)entry.Entity.DomainEvents)
            .Concat(Database.ChangeTracker.Entries<LegacyContact>().Select(entry => (IEnumerable<IDomainEvent>)entry.Entity.DomainEvents))
            .SelectMany(events => events)
            .ToList();
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

        foreach (var entry in Database.ChangeTracker.Entries<TakeoverCase>())
        {
            entry.Entity.ClearDomainEvents();
        }

        foreach (var entry in Database.ChangeTracker.Entries<LegacyContact>())
        {
            entry.Entity.ClearDomainEvents();
        }

        return true;
    }

    private ValueTask PublishAsync(IDomainEvent domainEvent) => domainEvent switch
    {
        DeathReported reported => outbox.PublishAsync(new DeathReportedEvent(reported.EventId, reported.OccurredAt, reported.CaseId.Value, reported.AlbumId, reported.OwnerAccountId, reported.Sequence)),
        DeathConfirmed confirmed => outbox.PublishAsync(new DeathConfirmedEvent(confirmed.EventId, confirmed.OccurredAt, confirmed.OwnerAccountId, confirmed.Sequence)),
        DeathReportRejected rejected => outbox.PublishAsync(new DeathReportRejectedEvent(rejected.EventId, rejected.OccurredAt, rejected.CaseId.Value, rejected.AlbumId, rejected.ReportedByAccountId, rejected.Sequence)),
        TakeoverOffered offered => outbox.PublishAsync(new TakeoverOfferedEvent(offered.EventId, offered.OccurredAt, offered.CaseId.Value, offered.AlbumId, offered.SuccessorAccountId, offered.AnswerBy, offered.Sequence)),
        AlbumTransferred transferred => outbox.PublishAsync(new AlbumTransferredEvent(transferred.EventId, transferred.OccurredAt, transferred.CaseId.Value, transferred.AlbumId, transferred.FromAccountId, transferred.ToAccountId, transferred.Sequence)),
        TakeoverRefused refused => outbox.PublishAsync(new TakeoverRefusedEvent(refused.EventId, refused.OccurredAt, refused.CaseId.Value, refused.AlbumId, refused.OwnerAccountId, refused.Sequence)),
        PrivateArchiveShared shared => outbox.PublishAsync(new PrivateArchiveSharedEvent(shared.EventId, shared.OccurredAt, shared.OwnerAccountId, shared.Sequence)),

        // The others stay inside the module.
        _ => ValueTask.CompletedTask,
    };
}
