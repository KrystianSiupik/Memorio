using Memorio.Modules.Access.Contracts;
using Memorio.Modules.Provenance.Domain.Artifacts;
using Memorio.Modules.Provenance.Infrastructure.Messaging;

namespace Memorio.Modules.Provenance.Tests;

/// <summary>MEM-E6-POCHODZENIE-06, P10 (SD6): the person who lost the album loses what was made for them from it, once.</summary>
public sealed class AccessRevokedHandlerIdempotencyTests(ProvenanceTestDatabase database)
{
    private static CancellationToken Cancellation => TestContext.Current.CancellationToken;

    [Fact]
    public async Task Artifacts_made_for_the_person_from_the_album_are_invalidated_once()
    {
        var album = Guid.CreateVersion7();
        var jan = Guid.CreateVersion7();
        var ola = Guid.CreateVersion7();
        var jans = await database.RegisterAsync(jan, album, new ArtifactSource(Guid.CreateVersion7(), ola, album));
        var olas = await database.RegisterAsync(ola, album, new ArtifactSource(Guid.CreateVersion7(), ola, album));
        var jansElsewhere = await database.RegisterAsync(jan, Guid.CreateVersion7(), new ArtifactSource(Guid.CreateVersion7(), jan, null));
        var gone = new AccessRevokedEvent(Guid.CreateVersion7(), DateTimeOffset.UtcNow, album, jan, 2);

        await database.DeliverAsync(services => AccessRevokedHandler.Handle(gone, services, Cancellation));
        await database.DeliverAsync(services => AccessRevokedHandler.Handle(gone, services, Cancellation));

        Assert.Equal((ArtifactState.Invalidated, InvalidationReason.AccessRevoked, 2), await database.StateOfAsync(jans));
        Assert.Equal(ArtifactState.Valid, (await database.StateOfAsync(olas)).State);
        Assert.Equal(ArtifactState.Valid, (await database.StateOfAsync(jansElsewhere)).State);
        Assert.Equal(1, await database.FactsOfAsync(gone.EventId));
    }
}
