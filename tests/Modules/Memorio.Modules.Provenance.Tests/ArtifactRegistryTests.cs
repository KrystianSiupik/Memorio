using Memorio.Modules.Provenance.Contracts;
using Memorio.Modules.Provenance.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Memorio.Modules.Provenance.Tests;

/// <summary>MEM-E6-POCHODZENIE-03: another module registers through the port, as AiNarration will (C83 → C154).</summary>
public sealed class ArtifactRegistryTests(ProvenanceTestDatabase database)
{
    private static CancellationToken Cancellation => TestContext.Current.CancellationToken;

    [Fact]
    public async Task A_summary_registered_through_the_port_is_kept_with_its_sources()
    {
        var album = Guid.CreateVersion7();
        var author = Guid.CreateVersion7();
        var artifact = new NewArtifact(
            Guid.CreateVersion7(),
            ArtifactKinds.PeriodSummary,
            Guid.CreateVersion7(),
            album,
            DateTimeOffset.UtcNow,
            [new ArtifactSourceItem(Guid.CreateVersion7(), author, album), new ArtifactSourceItem(Guid.CreateVersion7(), author, album)]);

        await using (var scope = database.CreateScope())
        {
            var registry = scope.ServiceProvider.GetRequiredService<IArtifactRegistry>();
            Assert.True((await registry.RegisterAsync(artifact, Cancellation)).IsSuccess);
            Assert.True((await registry.RegisterAsync(artifact, Cancellation)).IsSuccess);
            Assert.True((await registry.RegisterAsync(artifact with { Sources = [] }, Cancellation)).IsSuccess);
            Assert.True((await registry.RegisterAsync(artifact with { Id = Guid.CreateVersion7(), Sources = [] }, Cancellation)).IsFailure);
        }

        await using var check = database.CreateScope();
        var saved = await check.ServiceProvider.GetRequiredService<ProvenanceDbContext>().Artifacts.AsNoTracking()
            .SingleAsync(found => found.Id == artifact.Id, Cancellation);
        Assert.Equal(ArtifactKinds.PeriodSummary, saved.Kind);
        Assert.Equal(2, saved.Sources.Count);
    }
}
