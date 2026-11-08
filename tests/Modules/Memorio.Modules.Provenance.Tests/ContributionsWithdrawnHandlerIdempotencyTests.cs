using Memorio.Modules.Album.Contracts;
using Memorio.Modules.Provenance.Domain.Artifacts;
using Memorio.Modules.Provenance.Infrastructure.Messaging;

namespace Memorio.Modules.Provenance.Tests;

/// <summary>MEM-E6-POCHODZENIE-05, P9 (SD6): withdrawn contributions take what was made from them in that album only.</summary>
public sealed class ContributionsWithdrawnHandlerIdempotencyTests(ProvenanceTestDatabase database)
{
    private static CancellationToken Cancellation => TestContext.Current.CancellationToken;

    [Fact]
    public async Task Artifacts_citing_the_author_in_the_album_are_invalidated_once()
    {
        var album = Guid.CreateVersion7();
        var otherAlbum = Guid.CreateVersion7();
        var ola = Guid.CreateVersion7();
        var jan = Guid.CreateVersion7();
        var citesOla = await database.RegisterAsync(jan, album, new ArtifactSource(Guid.CreateVersion7(), ola, album), new ArtifactSource(Guid.CreateVersion7(), jan, album));
        var olaElsewhere = await database.RegisterAsync(jan, otherAlbum, new ArtifactSource(Guid.CreateVersion7(), ola, otherAlbum));
        var onlyJan = await database.RegisterAsync(jan, album, new ArtifactSource(Guid.CreateVersion7(), jan, album));
        var withdrawn = new ContributionsWithdrawnEvent(Guid.CreateVersion7(), DateTimeOffset.UtcNow, album, ola, 4);

        await database.DeliverAsync(services => ContributionsWithdrawnHandler.Handle(withdrawn, services, Cancellation));
        await database.DeliverAsync(services => ContributionsWithdrawnHandler.Handle(withdrawn, services, Cancellation));

        Assert.Equal((ArtifactState.Invalidated, InvalidationReason.ContributionsWithdrawn, 2), await database.StateOfAsync(citesOla));
        Assert.Equal(ArtifactState.Valid, (await database.StateOfAsync(olaElsewhere)).State);
        Assert.Equal(ArtifactState.Valid, (await database.StateOfAsync(onlyJan)).State);
        Assert.Equal(1, await database.FactsOfAsync(withdrawn.EventId));
    }
}
