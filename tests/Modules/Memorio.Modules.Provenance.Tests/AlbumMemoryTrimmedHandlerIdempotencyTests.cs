using Memorio.Modules.Album.Contracts;
using Memorio.Modules.Provenance.Domain.Artifacts;
using Memorio.Modules.Provenance.Infrastructure.Messaging;

namespace Memorio.Modules.Provenance.Tests;

/// <summary>MEM-E6-POCHODZENIE-08, P34 for an album (SD6): the memory left that album only, once.</summary>
public sealed class AlbumMemoryTrimmedHandlerIdempotencyTests(ProvenanceTestDatabase database)
{
    private static CancellationToken Cancellation => TestContext.Current.CancellationToken;

    [Fact]
    public async Task Artifacts_using_the_memory_through_that_album_are_invalidated_once()
    {
        var album = Guid.CreateVersion7();
        var author = Guid.CreateVersion7();
        var memory = Guid.CreateVersion7();
        var throughAlbum = await database.RegisterAsync(Guid.CreateVersion7(), album, new ArtifactSource(memory, author, album));
        var authorsOwn = await database.RegisterAsync(author, null, new ArtifactSource(memory, author, null));
        var trimmed = new AlbumMemoryTrimmedEvent(Guid.CreateVersion7(), DateTimeOffset.UtcNow, album, memory, author);

        await database.DeliverAsync(services => AlbumMemoryTrimmedHandler.Handle(trimmed, services, Cancellation));
        await database.DeliverAsync(services => AlbumMemoryTrimmedHandler.Handle(trimmed, services, Cancellation));

        Assert.Equal((ArtifactState.Invalidated, InvalidationReason.ArchiveTrimmed, 2), await database.StateOfAsync(throughAlbum));
        Assert.Equal(ArtifactState.Valid, (await database.StateOfAsync(authorsOwn)).State);
        Assert.Equal(1, await database.FactsOfAsync(trimmed.EventId));
    }
}
