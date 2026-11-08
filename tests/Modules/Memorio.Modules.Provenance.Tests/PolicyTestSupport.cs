using Memorio.Modules.Provenance.Application.Commands.RegisterArtifact;
using Memorio.Modules.Provenance.Domain.Artifacts;
using Memorio.Modules.Provenance.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Memorio.Modules.Provenance.Tests;

/// <summary>What the policy tests share: register an artifact, read its state, count the facts.</summary>
internal static class PolicyTestSupport
{
    private static CancellationToken Cancellation => TestContext.Current.CancellationToken;

    public static async Task<Guid> RegisterAsync(this ProvenanceTestDatabase database, Guid owner, Guid? album, params ArtifactSource[] sources)
    {
        var command = new RegisterArtifact(Guid.CreateVersion7(), "PeriodSummary", owner, album, sources);
        await using var scope = database.CreateScope();
        Assert.True((await scope.ServiceProvider.GetRequiredService<RegisterArtifactHandler>().HandleAsync(command, Cancellation)).IsSuccess);
        return command.Id;
    }

    public static async Task<(ArtifactState State, InvalidationReason? Reason, int Sequence)> StateOfAsync(this ProvenanceTestDatabase database, Guid artifactId)
    {
        await using var scope = database.CreateScope();
        var artifact = await scope.ServiceProvider.GetRequiredService<ProvenanceDbContext>().Artifacts.AsNoTracking().SingleAsync(found => found.Id == artifactId, Cancellation);
        return (artifact.State, artifact.Reason, artifact.Sequence);
    }

    public static async Task<int> FactsOfAsync(this ProvenanceTestDatabase database, Guid eventId)
    {
        await using var scope = database.CreateScope();
        return await scope.ServiceProvider.GetRequiredService<ProvenanceDbContext>().UnavailableSources.CountAsync(fact => fact.Id == eventId, Cancellation);
    }

    /// <summary>Delivers the event through the handler, as the worker would.</summary>
    public static async Task DeliverAsync(this ProvenanceTestDatabase database, Func<IServiceProvider, Task> handle)
    {
        await using var scope = database.CreateScope();
        await handle(scope.ServiceProvider);
    }
}
