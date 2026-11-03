using Memorio.Modules.Archive.Domain.Memories;
using Microsoft.EntityFrameworkCore;

namespace Memorio.Modules.Archive.Infrastructure.Persistence;

internal sealed class Memories(ArchiveDbContext database) : IMemories
{
    public async Task<IReadOnlyList<Memory>> FindByAuthorAsync(Guid accountId, CancellationToken cancellationToken) =>
        await database.Memories.Where(memory => memory.AuthorAccountId == accountId).ToListAsync(cancellationToken);

    public void Remove(Memory memory) => database.Memories.Remove(memory);

    public Task SaveAsync(CancellationToken cancellationToken) => database.SaveChangesAsync(cancellationToken);
}
