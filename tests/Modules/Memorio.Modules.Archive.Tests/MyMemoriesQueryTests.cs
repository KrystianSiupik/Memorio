using Memorio.Modules.Archive.Application.Queries.MyMemories;
using Microsoft.Extensions.DependencyInjection;

namespace Memorio.Modules.Archive.Tests;

public sealed class MyMemoriesQueryTests(ArchiveDatabase database)
{
    [Fact]
    public async Task Only_the_persons_own_active_memories()
    {
        var account = Guid.CreateVersion7();
        var active = await database.AddMemoryAsync(account);
        var trashed = await database.AddMemoryAsync(account);
        await database.AddMemoryAsync(Guid.CreateVersion7());
        await database.TrashAsync(trashed.Id, account);
        await using var scope = database.CreateScope();

        var mine = await scope.ServiceProvider.GetRequiredService<IMyMemoriesQuery>().GetAsync(account, TestContext.Current.CancellationToken);

        Assert.Equal(active.Id.Value, Assert.Single(mine).Id);
    }
}
