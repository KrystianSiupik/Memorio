using Memorio.Modules.Album.Application.Policies;
using Memorio.Modules.Archive.Contracts;
using Memorio.Modules.Limits.Contracts;
using Microsoft.Extensions.DependencyInjection;

namespace Memorio.Modules.Album.Tests;

/// <summary>SD6: E22 delivered twice, on a real database.</summary>
public sealed class ContentDeletedHandlerIdempotencyTests(AlbumDatabase database)
{
    [Fact]
    public async Task A_deleted_memory_leaves_its_albums_and_gives_the_space_back_once()
    {
        var (album, _, person) = await database.AlbumWithParticipantAsync();
        var memory = await database.BringMemoryAsync(album, person);
        var deleted = new ContentDeleted(Guid.CreateVersion7(), DateTimeOffset.UtcNow, memory, person, 3);

        await DeliverAsync(deleted);
        await DeliverAsync(deleted);

        Assert.Empty((await database.ViewAsync(album)).Memories);
        Assert.False(database.Neighbours.Reserved.ContainsKey(PoolKeys.For(album.Value, memory)));
    }

    private async Task DeliverAsync(ContentDeleted deleted)
    {
        await using var scope = database.CreateScope();
        await scope.ServiceProvider.GetRequiredService<ForgetContentWhenDeleted>().HandleAsync(deleted, TestContext.Current.CancellationToken);
    }
}
