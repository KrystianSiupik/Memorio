using Memorio.Modules.AiNarration.Contracts;
using Memorio.Modules.Provenance.Domain.Artifacts;
using Memorio.Modules.Provenance.Infrastructure.Messaging;

namespace Memorio.Modules.Provenance.Tests;

/// <summary>MEM-E6-POCHODZENIE-07, P12 (SD6): a consent taken back invalidates what was made from that author's content in the album, once.</summary>
public sealed class AiConsentRevokedHandlerIdempotencyTests(ProvenanceTestDatabase database)
{
    private static CancellationToken Cancellation => TestContext.Current.CancellationToken;

    [Fact]
    public async Task Artifacts_citing_the_author_in_the_album_are_invalidated_once()
    {
        var album = Guid.CreateVersion7();
        var ola = Guid.CreateVersion7();
        var jan = Guid.CreateVersion7();
        var citesOla = await database.RegisterAsync(jan, album, new ArtifactSource(Guid.CreateVersion7(), ola, album));
        var onlyJan = await database.RegisterAsync(jan, album, new ArtifactSource(Guid.CreateVersion7(), jan, album));
        var olaElsewhere = await database.RegisterAsync(ola, Guid.CreateVersion7(), new ArtifactSource(Guid.CreateVersion7(), ola, null));
        var revoked = new AiConsentRevokedEvent(Guid.CreateVersion7(), DateTimeOffset.UtcNow, ola, album, AiConsentRevocationReasons.LeftAlbum, 2);

        await database.DeliverAsync(services => AiConsentRevokedHandler.Handle(revoked, services, Cancellation));
        await database.DeliverAsync(services => AiConsentRevokedHandler.Handle(revoked, services, Cancellation));

        Assert.Equal((ArtifactState.Invalidated, InvalidationReason.AiConsentRevoked, 2), await database.StateOfAsync(citesOla));
        Assert.Equal(ArtifactState.Valid, (await database.StateOfAsync(onlyJan)).State);
        Assert.Equal(ArtifactState.Valid, (await database.StateOfAsync(olaElsewhere)).State);
        Assert.Equal(1, await database.FactsOfAsync(revoked.EventId));
    }
}
