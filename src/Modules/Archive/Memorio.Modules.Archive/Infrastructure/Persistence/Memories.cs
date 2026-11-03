using Memorio.Modules.Archive.Domain.Memories;
using Microsoft.EntityFrameworkCore;
using Npgsql;
using Wolverine.EntityFrameworkCore;

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

    public void Remove(Memory memory) => Database.Memories.Remove(memory);

    public Task<AppliedAccountEvent?> FindAppliedAsync(Guid accountId, CancellationToken cancellationToken) =>
        Database.AppliedAccountEvents.FirstOrDefaultAsync(applied => applied.AccountId == accountId, cancellationToken);

    public void Add(AppliedAccountEvent applied) => Database.AppliedAccountEvents.Add(applied);

    public async Task<bool> SaveAsync(CancellationToken cancellationToken)
    {
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

        foreach (var entry in Database.ChangeTracker.Entries<Memory>())
        {
            entry.Entity.ClearDomainEvents();
        }

        return true;
    }

    private Task<Memory?> FindAsync(MemoryId id, CancellationToken cancellationToken) =>
        Database.Memories.FirstOrDefaultAsync(memory => memory.Id == id, cancellationToken);
}
