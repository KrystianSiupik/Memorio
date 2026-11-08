using Memorio.Modules.Album.Contracts;
using Microsoft.Extensions.DependencyInjection;

namespace Memorio.Modules.Album.Tests;

/// <summary>MEM-E6-NARRACJA-AI-06: the album tells which memories it holds and whose, for the summary materials.</summary>
public sealed class AlbumMemoriesReaderTests(AlbumDatabase database)
{
    private static CancellationToken Cancellation => TestContext.Current.CancellationToken;

    [Fact]
    public async Task The_memories_of_the_album_with_their_authors_and_none_of_another()
    {
        var (album, owner, participant) = await database.AlbumWithParticipantAsync();
        var mine = await database.BringMemoryAsync(album, owner);
        var theirs = await database.BringMemoryAsync(album, participant);
        var other = await database.NewAlbumAsync(owner);
        await database.BringMemoryAsync(other, owner);

        await using var scope = database.CreateScope();
        var memories = await scope.ServiceProvider.GetRequiredService<IAlbumMemories>().MemoriesOfAsync(album.Value, Cancellation);

        Assert.Equal(
            [new AlbumMemoryRef(mine, owner), new AlbumMemoryRef(theirs, participant)],
            memories.OrderBy(memory => memory.MemoryId == mine ? 0 : 1));
    }
}
