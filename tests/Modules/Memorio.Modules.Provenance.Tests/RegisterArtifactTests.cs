using Memorio.Modules.Provenance.Application.Commands.RegisterArtifact;
using Memorio.Modules.Provenance.Domain.Artifacts;
using Memorio.Modules.Provenance.Infrastructure.Persistence;
using Memorio.SharedKernel.Results;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Memorio.Modules.Provenance.Tests;

/// <summary>MEM-E6-POCHODZENIE-02 on a real database: C154.</summary>
public sealed class RegisterArtifactTests(ProvenanceTestDatabase database)
{
    private static CancellationToken Cancellation => TestContext.Current.CancellationToken;

    private async Task<Result> RegisterAsync(RegisterArtifact command)
    {
        await using var scope = database.CreateScope();
        return await scope.ServiceProvider.GetRequiredService<RegisterArtifactHandler>().HandleAsync(command, Cancellation);
    }

    private async Task<Artifact?> LoadAsync(Guid id)
    {
        await using var scope = database.CreateScope();
        return await scope.ServiceProvider.GetRequiredService<ProvenanceDbContext>().Artifacts.AsNoTracking().FirstOrDefaultAsync(artifact => artifact.Id == id, Cancellation);
    }

    [Fact]
    public async Task The_artifact_is_kept_with_what_it_was_made_from()
    {
        var album = Guid.CreateVersion7();
        var author = Guid.CreateVersion7();
        var memories = new[] { Guid.CreateVersion7(), Guid.CreateVersion7() };
        var command = new RegisterArtifact(Guid.CreateVersion7(), "PeriodSummary", Guid.CreateVersion7(), album, [.. memories.Select(memory => new ArtifactSource(memory, author, album))]);

        Assert.True((await RegisterAsync(command)).IsSuccess);

        var saved = await LoadAsync(command.Id);
        Assert.NotNull(saved);
        Assert.Equal(ArtifactState.Valid, saved.State);
        Assert.Equal(album, saved.AlbumId);
        Assert.Equal(memories.Order(), saved.Sources.Select(source => source.MemoryId).Order());
        Assert.All(saved.Sources, source => Assert.Equal(author, source.AuthorAccountId));
    }

    [Fact]
    public async Task The_same_call_again_changes_nothing_and_another_artifact_cannot_take_its_id()
    {
        var command = new RegisterArtifact(Guid.CreateVersion7(), "PeriodSummary", Guid.CreateVersion7(), null, [new ArtifactSource(Guid.CreateVersion7(), Guid.CreateVersion7(), null)]);

        Assert.True((await RegisterAsync(command)).IsSuccess);
        Assert.True((await RegisterAsync(command)).IsSuccess);
        Assert.Equal(ArtifactErrors.IdTaken, (await RegisterAsync(command with { OwnerAccountId = Guid.CreateVersion7() })).Error);

        Assert.Single((await LoadAsync(command.Id))!.Sources);
    }

    [Fact]
    public async Task Wrong_input_saves_nothing()
    {
        var command = new RegisterArtifact(Guid.CreateVersion7(), "PeriodSummary", Guid.CreateVersion7(), null, []);

        Assert.Equal(ArtifactErrors.InvalidSources, (await RegisterAsync(command)).Error);
        Assert.Null(await LoadAsync(command.Id));
    }
}
