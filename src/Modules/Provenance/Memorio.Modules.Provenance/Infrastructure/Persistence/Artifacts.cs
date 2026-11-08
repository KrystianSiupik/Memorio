using Memorio.Modules.Provenance.Contracts;
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

    public async Task AddFactAsync(UnavailableSource fact, CancellationToken cancellationToken)
    {
        if (await Database.UnavailableSources.AnyAsync(found => found.Id == fact.Id, cancellationToken))
        {
            return;
        }

        Database.UnavailableSources.Add(fact);
        try
        {
            await Database.SaveChangesAsync(cancellationToken);
        }
        catch (DbUpdateException exception) when (exception.InnerException is PostgresException { SqlState: PostgresErrorCodes.UniqueViolation })
        {
            // The same event delivered twice at once: the other delivery recorded it.
            Database.ChangeTracker.Clear();
        }
    }

    public async Task<IReadOnlyList<Guid>> FindValidHitByAsync(UnavailableSource fact, CancellationToken cancellationToken)
    {
        var valid = Database.Artifacts.AsNoTracking().Where(artifact => artifact.State == ArtifactState.Valid);
        var hit = fact.Kind switch
        {
            UnavailableKind.Memory => valid.Where(artifact => artifact.Sources.Any(source => source.MemoryId == fact.MemoryId)),
            UnavailableKind.MemoryInAlbum => valid.Where(artifact => artifact.Sources.Any(source => source.MemoryId == fact.MemoryId && source.AlbumId == fact.AlbumId)),
            UnavailableKind.AuthorInAlbum => valid.Where(artifact => artifact.Sources.Any(source => source.AlbumId == fact.AlbumId && source.AuthorAccountId == fact.AccountId)),
            UnavailableKind.ReaderOfAlbum => valid.Where(artifact => artifact.AlbumId == fact.AlbumId && artifact.OwnerAccountId == fact.AccountId),
            _ => valid.Where(_ => false),
        };
        return await hit.Select(artifact => artifact.Id).ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<UnavailableSource>> FindFactsSinceAsync(DateTimeOffset since, CancellationToken cancellationToken) =>
        await Database.UnavailableSources.AsNoTracking().Where(fact => fact.At >= since).ToListAsync(cancellationToken);

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

    private ValueTask PublishAsync(IDomainEvent domainEvent) => domainEvent switch
    {
        ArtifactInvalidated invalidated => outbox.PublishAsync(new ArtifactInvalidatedEvent(
            invalidated.EventId,
            invalidated.OccurredAt,
            invalidated.ArtifactId,
            invalidated.Kind,
            invalidated.OwnerAccountId,
            invalidated.Reason.ToString(),
            invalidated.Sequence)),

        // The others stay inside the module.
        _ => ValueTask.CompletedTask,
    };
}
