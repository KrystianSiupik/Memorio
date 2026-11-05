using Memorio.Modules.Access.Contracts;
using Memorio.Modules.Access.Domain.Audiences;
using Memorio.SharedKernel.Domain;
using Microsoft.EntityFrameworkCore;
using Npgsql;
using Wolverine.EntityFrameworkCore;
using Wolverine.Runtime;

namespace Memorio.Modules.Access.Infrastructure.Persistence;

internal sealed class Audiences(IDbContextOutbox<AccessDbContext> outbox) : IAudiences
{
    private AccessDbContext Database => outbox.DbContext;

    public Task<AlbumAudience?> FindAsync(Guid albumId, CancellationToken cancellationToken) =>
        Database.Audiences.FirstOrDefaultAsync(audience => audience.Id == albumId, cancellationToken);

    public Task<AlbumAudience?> FindByTokenAsync(string token, CancellationToken cancellationToken) =>
        Database.Audiences.FirstOrDefaultAsync(audience => audience.LinkToken == token, cancellationToken);

    public void Add(AlbumAudience audience) => Database.Audiences.Add(audience);

    public async Task ForgetAsync(Guid accountId, CancellationToken cancellationToken)
    {
        await Database.Database.ExecuteSqlAsync($"DELETE FROM access.album_viewers WHERE account_id = {accountId}", cancellationToken);
        await Database.Database.ExecuteSqlAsync($"DELETE FROM access.album_blocks WHERE account_id = {accountId}", cancellationToken);
    }

    public async Task<bool> SaveAsync(CancellationToken cancellationToken)
    {
        foreach (var audience in Database.ChangeTracker.Entries<AlbumAudience>().Select(entry => entry.Entity).ToList())
        {
            foreach (var domainEvent in audience.DomainEvents)
            {
                await PublishAsync(domainEvent);
            }
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

        foreach (var entry in Database.ChangeTracker.Entries<AlbumAudience>())
        {
            entry.Entity.ClearDomainEvents();
        }

        return true;
    }

    private ValueTask PublishAsync(IDomainEvent domainEvent) => domainEvent switch
    {
        AccessRevoked revoked => outbox.PublishAsync(new AccessRevokedEvent(revoked.EventId, revoked.OccurredAt, revoked.AlbumId, revoked.AccountId, revoked.Sequence)),
        UserBlocked blocked => outbox.PublishAsync(new UserBlockedEvent(blocked.EventId, blocked.OccurredAt, blocked.AlbumId, blocked.AccountId, blocked.Sequence)),

        // The others stay inside the module.
        _ => ValueTask.CompletedTask,
    };
}
