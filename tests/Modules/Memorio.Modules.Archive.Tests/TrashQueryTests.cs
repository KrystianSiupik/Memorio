using Memorio.Modules.Archive.Application.Commands.TrashMemory;
using Memorio.Modules.Archive.Application.Queries.Trash;
using Memorio.Modules.Archive.Domain.Memories;
using Microsoft.Extensions.DependencyInjection;

namespace Memorio.Modules.Archive.Tests;

/// <summary>MEM-E2-ARCHIWUM-10 on a real database: person x content.</summary>
public sealed class TrashQueryTests(ArchiveDatabase database)
{
    private static CancellationToken Cancellation => TestContext.Current.CancellationToken;

    [Fact]
    public async Task Each_person_sees_only_their_own_trash()
    {
        var ola = Guid.CreateVersion7();
        var jan = Guid.CreateVersion7();
        var hers = await database.AddMemoryAsync(ola, photos: 2);
        await database.AddMemoryAsync(ola);
        var his = await database.AddMemoryAsync(jan);
        await TrashAsync(hers.Id, ola);
        await TrashAsync(his.Id, jan);

        var seen = Assert.Single(await GetAsync(ola));

        Assert.Equal(hers.Id.Value, seen.Id);
        Assert.Equal("Giewont", seen.PlaceName);
        Assert.Equal(2, seen.Photos);
        Assert.Equal(30, seen.DaysLeft);
    }

    [Fact]
    public async Task After_its_window_it_is_no_longer_there()
    {
        var account = Guid.CreateVersion7();
        var memory = await database.AddMemoryAsync(account);
        await TrashAsync(memory.Id, account);
        await using (var connection = await database.OpenAsAsync(ArchiveDatabase.Role))
        await using (var update = new Npgsql.NpgsqlCommand("UPDATE archive.memories SET window_ends_at = now() - interval '1 minute' WHERE id = @id", connection))
        {
            update.Parameters.AddWithValue("id", memory.Id.Value);
            await update.ExecuteNonQueryAsync(Cancellation);
        }

        Assert.Empty(await GetAsync(account));
    }

    private async Task TrashAsync(MemoryId memory, Guid account)
    {
        await using var scope = database.CreateScope();
        await scope.ServiceProvider.GetRequiredService<TrashMemoryHandler>().HandleAsync(new TrashMemory(memory, account), Cancellation);
    }

    private async Task<IReadOnlyList<TrashedMemory>> GetAsync(Guid account)
    {
        await using var scope = database.CreateScope();
        return await scope.ServiceProvider.GetRequiredService<ITrashQuery>().GetAsync(account, Cancellation);
    }
}
