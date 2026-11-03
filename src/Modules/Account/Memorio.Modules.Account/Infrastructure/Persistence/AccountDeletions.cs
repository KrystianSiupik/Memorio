using Memorio.Modules.Account.Contracts;
using Memorio.Modules.Account.Domain.Accounts;
using Memorio.Modules.Account.Domain.Deletions;
using Memorio.SharedKernel.Domain;
using Microsoft.EntityFrameworkCore;
using Npgsql;
using Wolverine.EntityFrameworkCore;

namespace Memorio.Modules.Account.Infrastructure.Persistence;

/// <summary>The aggregate and the events it raised go in one transaction, through the outbox (ADR 0006).</summary>
internal sealed class AccountDeletions(IDbContextOutbox<AccountDbContext> outbox) : IAccountDeletions
{
    private AccountDbContext Database => outbox.DbContext;

    public Task<AccountDeletion?> FindAsync(AccountId accountId, CancellationToken cancellationToken) =>
        Database.AccountDeletions.FirstOrDefaultAsync(deletion => deletion.Id == accountId, cancellationToken);

    public void Add(AccountDeletion deletion) => Database.AccountDeletions.Add(deletion);

    public async Task<bool> SaveAsync(AccountDeletion deletion, CancellationToken cancellationToken)
    {
        foreach (var domainEvent in deletion.DomainEvents)
        {
            await PublishAsync(domainEvent);
        }

        try
        {
            await outbox.SaveChangesAndFlushMessagesAsync(cancellationToken);
        }
        catch (DbUpdateException exception) when (exception is DbUpdateConcurrencyException
            || exception.InnerException is PostgresException { SqlState: PostgresErrorCodes.UniqueViolation })
        {
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
        _ => throw new InvalidOperationException($"No integration event for {domainEvent.GetType().Name}."),
    };
}
