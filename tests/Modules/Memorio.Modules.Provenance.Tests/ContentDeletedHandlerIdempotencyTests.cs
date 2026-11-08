using Memorio.Modules.Archive.Contracts;
using Memorio.Modules.Provenance.Domain.Artifacts;
using Memorio.Modules.Provenance.Infrastructure.Messaging;

namespace Memorio.Modules.Provenance.Tests;

/// <summary>MEM-E6-POCHODZENIE-04, P4 (SD6): a memory deleted for good takes what cites it, and twice is once.</summary>
public sealed class ContentDeletedHandlerIdempotencyTests(ProvenanceTestDatabase database)
{
    private static CancellationToken Cancellation => TestContext.Current.CancellationToken;

    [Fact]
    public async Task Artifacts_citing_the_memory_are_invalidated_once_and_others_stay()
    {
        var memory = Guid.CreateVersion7();
        var author = Guid.CreateVersion7();
        var album = Guid.CreateVersion7();
        var inAlbum = await database.RegisterAsync(Guid.CreateVersion7(), album, new ArtifactSource(memory, author, album));
        var privateOne = await database.RegisterAsync(author, null, new ArtifactSource(memory, author, null));
        var other = await database.RegisterAsync(author, null, new ArtifactSource(Guid.CreateVersion7(), author, null));
        var deleted = new ContentDeleted(Guid.CreateVersion7(), DateTimeOffset.UtcNow, memory, author, 3);

        await database.DeliverAsync(services => ContentDeletedHandler.Handle(deleted, services, Cancellation));
        await database.DeliverAsync(services => ContentDeletedHandler.Handle(deleted, services, Cancellation));

        Assert.Equal((ArtifactState.Invalidated, InvalidationReason.ContentDeleted, 2), await database.StateOfAsync(inAlbum));
        Assert.Equal((ArtifactState.Invalidated, InvalidationReason.ContentDeleted, 2), await database.StateOfAsync(privateOne));
        Assert.Equal(ArtifactState.Valid, (await database.StateOfAsync(other)).State);
        Assert.Equal(1, await database.FactsOfAsync(deleted.EventId));
        Assert.Equal(1, await database.QueuedEventsAsync("ArtifactInvalidatedEvent", inAlbum));
    }
}
