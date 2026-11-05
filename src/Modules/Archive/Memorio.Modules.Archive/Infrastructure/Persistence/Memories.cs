using Memorio.Modules.Archive.Contracts;
using Memorio.Modules.Archive.Domain.Memories;
using Memorio.SharedKernel.Domain;
using Microsoft.EntityFrameworkCore;
using Npgsql;
using Wolverine.EntityFrameworkCore;
using Wolverine.Runtime;

namespace Memorio.Modules.Archive.Infrastructure.Persistence;

internal sealed class Memories(IDbContextOutbox<ArchiveDbContext> outbox) : IMemories
{
    private ArchiveDbContext Database => outbox.DbContext;

    public async Task<IReadOnlyList<Memory>> FindByAuthorAsync(Guid accountId, CancellationToken cancellationToken) =>
        await Database.Memories.Where(memory => memory.AuthorAccountId == accountId).ToListAsync(cancellationToken);

    public async Task<Memory> AddOrFindAsync(Memory memory, CancellationToken cancellationToken)
    {
        if (await FindAsync(memory.Id, cancellationToken) is { } existing)
        {
            return existing;
        }

        Database.Memories.Add(memory);
        if (await SaveAsync(cancellationToken))
        {
            return memory;
        }

        // The same request twice at once.
        return await FindAsync(memory.Id, cancellationToken) ?? throw new InvalidOperationException("The memory that won the race is gone.");
    }

    public async Task<IReadOnlyList<OverdueMemory>> FindOverdueAsync(DateTimeOffset at, CancellationToken cancellationToken) =>
        await Database.Memories
            .AsNoTracking()
            .Where(memory => memory.State == MemoryState.Trashed && memory.WindowEndsAt <= at)
            .OrderBy(memory => memory.WindowEndsAt)
            .Select(memory => new OverdueMemory(memory.Id, memory.Sequence))
            .ToListAsync(cancellationToken);

    public void Remove(Memory memory) => Database.Memories.Remove(memory);

    public Task<AppliedAccountEvent?> FindAppliedAsync(Guid accountId, CancellationToken cancellationToken) =>
        Database.AppliedAccountEvents.FirstOrDefaultAsync(applied => applied.AccountId == accountId, cancellationToken);

    public void Add(AppliedAccountEvent applied) => Database.AppliedAccountEvents.Add(applied);

    public Task<AppliedPlanEvent?> FindAppliedPlanEventAsync(Guid resourceId, CancellationToken cancellationToken) =>
        Database.AppliedPlanEvents.FirstOrDefaultAsync(applied => applied.ResourceId == resourceId, cancellationToken);

    public void RecordApplied(AppliedPlanEvent? applied, Guid resourceId, int sequence)
    {
        if (applied is null)
        {
            Database.AppliedPlanEvents.Add(new AppliedPlanEvent(resourceId, sequence));
        }
        else
        {
            applied.MoveTo(sequence);
        }
    }

    public async Task<IReadOnlyList<Memory>> FindTrimmedForAsync(Guid resourceId, CancellationToken cancellationToken) =>
        await Database.Memories.Where(memory => memory.TrimmedFor == resourceId && memory.State == MemoryState.Trashed).ToListAsync(cancellationToken);

    public async Task AnnounceAsync(ArchiveTrimmed trimmed) =>
        await outbox.PublishAsync(new ArchiveTrimmedEvent(trimmed.EventId, trimmed.OccurredAt, trimmed.ResourceId, trimmed.IsAlbum, trimmed.MemoriesTrimmed, trimmed.Sequence));

    public async Task<bool> SaveAsync(CancellationToken cancellationToken)
    {
        foreach (var memory in Database.ChangeTracker.Entries<Memory>().Select(entry => entry.Entity).ToList())
        {
            foreach (var domainEvent in memory.DomainEvents)
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

        foreach (var entry in Database.ChangeTracker.Entries<Memory>())
        {
            entry.Entity.ClearDomainEvents();
        }

        return true;
    }

    private ValueTask PublishAsync(IDomainEvent domainEvent) => domainEvent switch
    {
        ContentDeletedDefinitively deleted => outbox.PublishAsync(new ContentDeleted(
            deleted.EventId, deleted.OccurredAt, deleted.MemoryId.Value, deleted.AuthorAccountId, deleted.Sequence)),

        MemoryMovedToTrash moved => outbox.PublishAsync(new MemoryMovedToTrashEvent(
            moved.EventId, moved.OccurredAt, moved.MemoryId.Value, moved.Sequence, moved.WindowEndsAt)),

        MemoryRestored restored => outbox.PublishAsync(new MemoryRestoredEvent(
            restored.EventId, restored.OccurredAt, restored.MemoryId.Value, restored.Sequence)),

        // The others stay inside the module.
        _ => ValueTask.CompletedTask,
    };

    public Task<Memory?> FindAsync(MemoryId id, CancellationToken cancellationToken) =>
        Database.Memories.FirstOrDefaultAsync(memory => memory.Id == id, cancellationToken);
}
