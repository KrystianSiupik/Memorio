using Memorio.Modules.Provenance.Application.Commands.InvalidateArtifacts;
using Memorio.Modules.Provenance.Application.Commands.RegisterArtifact;
using Memorio.Modules.Provenance.Domain.Artifacts;
using Memorio.Modules.Provenance.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Memorio.Modules.Provenance.Tests;

/// <summary>MEM-E6-POCHODZENIE-01 on a real database: C31, AG9 and D-023.</summary>
public sealed class InvalidateArtifactsTests(ProvenanceTestDatabase database)
{
    private static CancellationToken Cancellation => TestContext.Current.CancellationToken;

    private static readonly DateTimeOffset Now = DateTimeOffset.UtcNow;

    private async Task<Guid> RegisterAsync(Guid owner, Guid? album, IReadOnlyCollection<ArtifactSource> sources, DateTimeOffset? readAt = null)
    {
        var command = new RegisterArtifact(Guid.CreateVersion7(), "PeriodSummary", owner, album, sources) { MaterialsReadAt = readAt };
        await using var scope = database.CreateScope();
        Assert.True((await scope.ServiceProvider.GetRequiredService<RegisterArtifactHandler>().HandleAsync(command, Cancellation)).IsSuccess);
        return command.Id;
    }

    private async Task<int> InvalidateAsync(UnavailableSource fact)
    {
        await using var scope = database.CreateScope();
        return await scope.ServiceProvider.GetRequiredService<InvalidateArtifactsHandler>().HandleAsync(fact, Cancellation);
    }

    private async Task<Artifact> LoadAsync(Guid id)
    {
        await using var scope = database.CreateScope();
        return await scope.ServiceProvider.GetRequiredService<ProvenanceDbContext>().Artifacts.AsNoTracking().SingleAsync(found => found.Id == id, Cancellation);
    }

    [Fact]
    public async Task Each_kind_of_fact_hits_only_the_artifacts_made_from_what_it_takes_away()
    {
        var album = Guid.CreateVersion7();
        var otherAlbum = Guid.CreateVersion7();
        var ola = Guid.CreateVersion7();
        var jan = Guid.CreateVersion7();
        var shared = Guid.CreateVersion7();
        var olas = Guid.CreateVersion7();

        var fromShared = await RegisterAsync(jan, album, [new(shared, ola, album)]);
        var sameMemoryElsewhere = await RegisterAsync(jan, otherAlbum, [new(shared, ola, otherAlbum)]);
        var janFromAlbum = await RegisterAsync(jan, album, [new(Guid.CreateVersion7(), jan, album)]);
        var olaInAlbum = await RegisterAsync(ola, album, [new(olas, ola, album)]);
        var untouched = await RegisterAsync(ola, otherAlbum, [new(Guid.CreateVersion7(), jan, otherAlbum)]);

        // P34: the memory left one album only.
        Assert.Equal(1, await InvalidateAsync(UnavailableSource.MemoryInAlbum(Guid.CreateVersion7(), shared, album, InvalidationReason.ArchiveTrimmed, Now)));
        Assert.Equal(ArtifactState.Invalidated, (await LoadAsync(fromShared)).State);
        Assert.Equal(ArtifactState.Valid, (await LoadAsync(sameMemoryElsewhere)).State);

        // P4: gone for good, everywhere.
        Assert.Equal(1, await InvalidateAsync(UnavailableSource.Memory(Guid.CreateVersion7(), shared, InvalidationReason.ContentDeleted, Now)));
        Assert.Equal(ArtifactState.Invalidated, (await LoadAsync(sameMemoryElsewhere)).State);

        // P10: Jan lost the album, with what was made for him from it.
        Assert.Equal(1, await InvalidateAsync(UnavailableSource.ReaderOfAlbum(Guid.CreateVersion7(), album, jan, InvalidationReason.AccessRevoked, Now)));
        Assert.Equal(ArtifactState.Invalidated, (await LoadAsync(janFromAlbum)).State);

        // P9 / P12: Ola's contributions to the album.
        Assert.Equal(1, await InvalidateAsync(UnavailableSource.AuthorInAlbum(Guid.CreateVersion7(), album, ola, InvalidationReason.AiConsentRevoked, Now)));
        var olaArtifact = await LoadAsync(olaInAlbum);
        Assert.Equal(ArtifactState.Invalidated, olaArtifact.State);
        Assert.Equal(InvalidationReason.AiConsentRevoked, olaArtifact.Reason);

        Assert.Equal(ArtifactState.Valid, (await LoadAsync(untouched)).State);
        Assert.Equal(1, await database.QueuedEventsAsync("ArtifactInvalidatedEvent", olaInAlbum));
    }

    [Fact]
    public async Task The_same_fact_twice_records_and_invalidates_once_and_an_invalidated_artifact_never_comes_back()
    {
        var memory = Guid.CreateVersion7();
        var artifact = await RegisterAsync(Guid.CreateVersion7(), null, [new(memory, Guid.CreateVersion7(), null)]);
        var fact = UnavailableSource.Memory(Guid.CreateVersion7(), memory, InvalidationReason.ContentDeleted, Now);

        Assert.Equal(1, await InvalidateAsync(fact));
        Assert.Equal(0, await InvalidateAsync(fact));
        Assert.Equal(0, await InvalidateAsync(UnavailableSource.Memory(Guid.CreateVersion7(), memory, InvalidationReason.ArchiveTrimmed, Now)));

        var saved = await LoadAsync(artifact);
        Assert.Equal(InvalidationReason.ContentDeleted, saved.Reason);
        Assert.Equal(2, saved.Sequence);
        Assert.Equal(1, await database.QueuedEventsAsync("ArtifactInvalidatedEvent", artifact));
    }

    [Fact]
    public async Task Withdrawn_after_the_materials_were_read_the_new_artifact_is_invalid_at_once_and_before_it_stays_valid()
    {
        var album = Guid.CreateVersion7();
        var author = Guid.CreateVersion7();
        var readAt = Now;
        await InvalidateAsync(UnavailableSource.AuthorInAlbum(Guid.CreateVersion7(), album, author, InvalidationReason.ContributionsWithdrawn, readAt.AddSeconds(1)));

        var late = await RegisterAsync(Guid.CreateVersion7(), album, [new(Guid.CreateVersion7(), author, album)], readAt);
        var fresh = await RegisterAsync(Guid.CreateVersion7(), album, [new(Guid.CreateVersion7(), author, album)], readAt.AddSeconds(2));

        Assert.Equal(ArtifactState.Invalidated, (await LoadAsync(late)).State);
        Assert.Equal(InvalidationReason.ContributionsWithdrawn, (await LoadAsync(late)).Reason);
        Assert.Equal(ArtifactState.Valid, (await LoadAsync(fresh)).State);
    }

    [Fact]
    public async Task A_registration_and_a_withdrawal_at_the_same_time_never_leave_a_live_artifact()
    {
        for (var round = 0; round < 10; round++)
        {
            var memory = Guid.CreateVersion7();
            var readAt = DateTimeOffset.UtcNow;
            var command = new RegisterArtifact(Guid.CreateVersion7(), "PeriodSummary", Guid.CreateVersion7(), null, [new(memory, Guid.CreateVersion7(), null)])
            {
                MaterialsReadAt = readAt,
            };
            var fact = UnavailableSource.Memory(Guid.CreateVersion7(), memory, InvalidationReason.ContentDeleted, readAt.AddMilliseconds(1));

            await Task.WhenAll(
                Task.Run(async () =>
                {
                    await using var scope = database.CreateScope();
                    await scope.ServiceProvider.GetRequiredService<RegisterArtifactHandler>().HandleAsync(command, Cancellation);
                }, Cancellation),
                Task.Run(() => InvalidateAsync(fact), Cancellation));

            Assert.Equal(ArtifactState.Invalidated, (await LoadAsync(command.Id)).State);
        }
    }
}
