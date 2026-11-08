using Memorio.Modules.AiNarration.Contracts;
using Memorio.Modules.AiNarration.Domain.Consents;
using Memorio.SharedKernel.Domain;
using Microsoft.EntityFrameworkCore;
using Npgsql;
using Wolverine.EntityFrameworkCore;
using Wolverine.Runtime;

namespace Memorio.Modules.AiNarration.Infrastructure.Persistence;

internal sealed class AiConsents(IDbContextOutbox<AiNarrationDbContext> outbox) : IAiConsents
{
    private AiNarrationDbContext Database => outbox.DbContext;

    public Task<AiConsent?> FindAsync(Guid accountId, Guid albumId, CancellationToken cancellationToken) =>
        Database.AiConsents.FirstOrDefaultAsync(consent => consent.AccountId == accountId && consent.AlbumId == albumId, cancellationToken);

    public void Add(AiConsent consent) => Database.AiConsents.Add(consent);

    public async Task<bool> SaveAsync(CancellationToken cancellationToken)
    {
        var raising = Database.ChangeTracker.Entries<AiConsent>().SelectMany(entry => entry.Entity.DomainEvents).ToList();
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

        foreach (var entry in Database.ChangeTracker.Entries<AiConsent>())
        {
            entry.Entity.ClearDomainEvents();
        }

        return true;
    }

    private ValueTask PublishAsync(IDomainEvent domainEvent) => domainEvent switch
    {
        AiConsentGiven given => outbox.PublishAsync(new AiConsentGivenEvent(given.EventId, given.OccurredAt, given.AccountId, given.AlbumId, given.Sequence)),
        _ => ValueTask.CompletedTask,
    };
}
