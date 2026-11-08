using Memorio.Modules.Archive.Contracts;
using Memorio.Modules.Provenance.Domain.Artifacts;
using Memorio.Modules.Provenance.Infrastructure.Messaging;

namespace Memorio.Modules.Provenance.Tests;

/// <summary>MEM-E6-POCHODZENIE-08, P34 (SD6): an account's trimmed memories take what cites them, once.</summary>
public sealed class ArchiveTrimmedHandlerIdempotencyTests(ProvenanceTestDatabase database)
{
    private static CancellationToken Cancellation => TestContext.Current.CancellationToken;

    [Fact]
    public async Task Artifacts_citing_a_trimmed_memory_are_invalidated_once()
    {
        var author = Guid.CreateVersion7();
        var first = Guid.CreateVersion7();
        var second = Guid.CreateVersion7();
        var citesFirst = await database.RegisterAsync(author, null, new ArtifactSource(first, author, null));
        var citesSecond = await database.RegisterAsync(author, null, new ArtifactSource(second, author, null));
        var kept = await database.RegisterAsync(author, null, new ArtifactSource(Guid.CreateVersion7(), author, null));
        var trimmed = new ArchiveTrimmedEvent(Guid.CreateVersion7(), DateTimeOffset.UtcNow, author, false, 2, 3, [first, second]);
        var before = new ArchiveTrimmedEvent(Guid.CreateVersion7(), DateTimeOffset.UtcNow, author, false, 1, 2);

        await database.DeliverAsync(services => ArchiveTrimmedHandler.Handle(trimmed, services, Cancellation));
        await database.DeliverAsync(services => ArchiveTrimmedHandler.Handle(trimmed, services, Cancellation));
        await database.DeliverAsync(services => ArchiveTrimmedHandler.Handle(before, services, Cancellation));

        Assert.Equal((ArtifactState.Invalidated, InvalidationReason.ArchiveTrimmed, 2), await database.StateOfAsync(citesFirst));
        Assert.Equal((ArtifactState.Invalidated, InvalidationReason.ArchiveTrimmed, 2), await database.StateOfAsync(citesSecond));
        Assert.Equal(ArtifactState.Valid, (await database.StateOfAsync(kept)).State);
    }
}
