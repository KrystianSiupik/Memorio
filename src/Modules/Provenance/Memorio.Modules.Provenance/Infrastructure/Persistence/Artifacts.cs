using Memorio.Modules.Provenance.Domain.Artifacts;
using Memorio.SharedKernel.Domain;
using Microsoft.EntityFrameworkCore;
using Npgsql;
using Wolverine.EntityFrameworkCore;
using Wolverine.Runtime;

namespace Memorio.Modules.Provenance.Infrastructure.Persistence;

internal sealed class Artifacts(IDbContextOutbox<ProvenanceDbContext> outbox) : IArtifacts
{
    private ProvenanceDbContext Database => outbox.DbContext;

    public Task<Artifact?> FindAsync(Guid id, CancellationToken cancellationToken) =>
        Database.Artifacts.FirstOrDefaultAsync(artifact => artifact.Id == id, cancellationToken);

    public void Add(Artifact artifact) => Database.Artifacts.Add(artifact);

    public async Task<bool> SaveAsync(CancellationToken cancellationToken)
    {
        var raising = Database.ChangeTracker.Entries<Artifact>().SelectMany(entry => entry.Entity.DomainEvents).ToList();
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

        foreach (var entry in Database.ChangeTracker.Entries<Artifact>())
        {
            entry.Entity.ClearDomainEvents();
        }

        return true;
    }

    // Nothing leaves the module yet; C31 publishes the invalidation.
    private static ValueTask PublishAsync(IDomainEvent domainEvent) => ValueTask.CompletedTask;
}
