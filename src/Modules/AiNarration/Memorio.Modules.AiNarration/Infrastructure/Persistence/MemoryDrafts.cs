using Memorio.Modules.AiNarration.Domain.Drafts;
using Microsoft.EntityFrameworkCore;
using Npgsql;

namespace Memorio.Modules.AiNarration.Infrastructure.Persistence;

internal sealed class MemoryDrafts(AiNarrationDbContext database) : IMemoryDrafts
{
    public Task<MemoryDraft?> FindAsync(Guid id, CancellationToken cancellationToken) =>
        database.MemoryDrafts.FirstOrDefaultAsync(draft => draft.Id == id, cancellationToken);

    public async Task<MemoryDraft> AddOrFindAsync(MemoryDraft draft, CancellationToken cancellationToken)
    {
        database.MemoryDrafts.Add(draft);
        try
        {
            await database.SaveChangesAsync(cancellationToken);
            return draft;
        }
        catch (DbUpdateException exception) when (exception.InnerException is PostgresException { SqlState: PostgresErrorCodes.UniqueViolation })
        {
            // The same request twice at once.
            database.ChangeTracker.Clear();
            return await FindAsync(draft.Id, cancellationToken) ?? throw new InvalidOperationException("The draft that won the race is gone.");
        }
    }
}
