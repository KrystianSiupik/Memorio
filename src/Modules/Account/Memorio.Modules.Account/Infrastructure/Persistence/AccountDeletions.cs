using Memorio.Modules.Account.Contracts;
using Memorio.Modules.Account.Domain.Accounts;
using Memorio.Modules.Account.Domain.Deletions;
using Memorio.SharedKernel.Domain;
using Microsoft.EntityFrameworkCore;
using Npgsql;
using Wolverine.EntityFrameworkCore;
using Wolverine.Runtime;

namespace Memorio.Modules.Account.Infrastructure.Persistence;

/// <summary>The aggregate and the events it raised go in one transaction, through the outbox (ADR 0006).</summary>
internal sealed class AccountDeletions(IDbContextOutbox<AccountDbContext> outbox) : IAccountDeletions
{
    private AccountDbContext Database => outbox.DbContext;

    public Task<AccountDeletion?> FindAsync(AccountId accountId, CancellationToken cancellationToken) =>
        Database.AccountDeletions.FirstOrDefaultAsync(deletion => deletion.Id == accountId, cancellationToken);

    public async Task<IReadOnlyList<OverdueDeletion>> FindOverdueAsync(DateTimeOffset at, CancellationToken cancellationToken) =>
        await Database.AccountDeletions
            .AsNoTracking()
            .Where(deletion => deletion.State == DeletionState.Pending && deletion.WindowEndsAt <= at)
            .OrderBy(deletion => deletion.WindowEndsAt)
            .Select(deletion => new OverdueDeletion(deletion.Id, deletion.Sequence))
            .ToListAsync(cancellationToken);

    public void Add(AccountDeletion deletion) => Database.AccountDeletions.Add(deletion);

    public async Task<bool> SaveAsync(AccountDeletion deletion, CancellationToken cancellationToken)
    {
        foreach (var domainEvent in deletion.DomainEvents)
        {
            await PublishAsync(domainEvent);
        }

        // Sign-in, password and email go with the completed deletion, in its transaction; logins and tokens cascade.
        if (deletion.State == DeletionState.Completed
            && await Database.Users.FirstOrDefaultAsync(user => user.Id == deletion.Id.Value, cancellationToken) is { } user)
        {
            Database.Users.Remove(user);
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

        deletion.ClearDomainEvents();
        return true;
    }

    private ValueTask PublishAsync(IDomainEvent domainEvent) => domainEvent switch
    {
        DeletionRequested requested => outbox.PublishAsync(new AccountDeletionRequested(
            requested.EventId, requested.OccurredAt, requested.AccountId.Value, requested.Sequence, requested.WindowEndsAt)),

        DeletionCancelled cancelled => outbox.PublishAsync(new AccountDeletionCancelled(
            cancelled.EventId, cancelled.OccurredAt, cancelled.AccountId.Value, cancelled.Sequence)),

        DeletionCompleted completed => outbox.PublishAsync(new AccountDeleted(
            completed.EventId, completed.OccurredAt, completed.AccountId.Value, completed.Sequence)),
        _ => throw new InvalidOperationException($"No integration event for {domainEvent.GetType().Name}."),
    };
}
