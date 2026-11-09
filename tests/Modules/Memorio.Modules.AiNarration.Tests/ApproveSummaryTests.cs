using Memorio.Modules.AiNarration.Application.Commands.Summaries;
using Memorio.Modules.AiNarration.Domain.Summaries;
using Memorio.Modules.AiNarration.Infrastructure.Messaging;
using Memorio.Modules.Provenance.Contracts;
using Memorio.SharedKernel.Results;
using Microsoft.Extensions.DependencyInjection;

namespace Memorio.Modules.AiNarration.Tests;

/// <summary>MEM-E6-NARRACJA-AI-04 on a real database: C83, E84, with Provenance in memory.</summary>
public sealed class ApproveSummaryTests(AiNarrationDatabase database)
{
    private static CancellationToken Cancellation => TestContext.Current.CancellationToken;

    internal static async Task<Result<PeriodSummary>> ApproveAsync(AiNarrationDatabase database, Guid id, Guid account)
    {
        await using var scope = database.CreateScope();
        return await scope.ServiceProvider.GetRequiredService<ApproveSummaryHandler>().HandleAsync(new ApproveSummary(id, account), Cancellation);
    }

    internal static async Task<PeriodSummary> ReadAsync(AiNarrationDatabase database, Guid id)
    {
        await using var scope = database.CreateScope();
        return (await scope.ServiceProvider.GetRequiredService<IPeriodSummaries>().FindAsync(id, Cancellation))!;
    }

    /// <summary>A draft of the owner's album, with the owner, the album and the memory Ola brought.</summary>
    internal static async Task<(PeriodSummary Draft, Guid Owner, Guid Album, Guid OlasMemory)> DraftAsync(AiNarrationDatabase database)
    {
        var (album, owner, _, olasMemory, _) = await RequestSummaryTests.AlbumAsync(database);
        var draft = (await RequestSummaryTests.RequestAsync(database, Guid.CreateVersion7(), owner, album)).Value;
        return (draft, owner, album, olasMemory);
    }

    [Fact]
    public async Task Approved_it_is_kept_and_Provenance_knows_every_memory_it_was_made_from_once()
    {
        var (draft, owner, album, _) = await DraftAsync(database);

        var saved = (await ApproveAsync(database, draft.Id, owner)).Value;
        var again = (await ApproveAsync(database, draft.Id, owner)).Value;

        Assert.Equal(SummaryState.Saved, saved.State);
        Assert.NotNull(saved.SavedAt);
        Assert.Equal(SummaryState.Saved, again.State);
        Assert.Equal(SummaryState.Saved, (await ReadAsync(database, draft.Id)).State);
        Assert.Equal(1, database.Artifacts.Calls[draft.Id]);

        var artifact = database.Artifacts.Registered[draft.Id];
        Assert.Equal(ArtifactKinds.PeriodSummary, artifact.Kind);
        Assert.Equal(owner, artifact.OwnerAccountId);
        Assert.Equal(album, artifact.AlbumId);
        Assert.Equal((await ReadAsync(database, draft.Id)).MaterialsReadAt, artifact.MaterialsReadAt);
        Assert.Equal(draft.Sources.Select(source => source.MemoryId), artifact.Sources.Select(source => source.MemoryId));
        Assert.All(artifact.Sources, source => Assert.Equal(album, source.AlbumId));
    }

    [Fact]
    public async Task Only_the_person_who_asked_sees_and_approves_it()
    {
        var (draft, _, _, _) = await DraftAsync(database);

        Assert.Equal(SummaryErrors.NotFound, (await ApproveAsync(database, draft.Id, Guid.CreateVersion7())).Error);
        Assert.Equal(SummaryErrors.NotFound, (await ApproveAsync(database, Guid.CreateVersion7(), draft.AccountId)).Error);
        Assert.Equal(SummaryState.Draft, (await ReadAsync(database, draft.Id)).State);
        Assert.False(database.Artifacts.Calls.ContainsKey(draft.Id));
    }

    [Fact]
    public async Task A_memory_it_tells_of_that_may_no_longer_be_used_means_asking_anew()
    {
        var (draft, owner, _, olasMemory) = await DraftAsync(database);
        database.Albums.Hide(olasMemory);

        Assert.Equal(SummaryErrors.MaterialsChanged, (await ApproveAsync(database, draft.Id, owner)).Error);
        Assert.Equal(SummaryState.Draft, (await ReadAsync(database, draft.Id)).State);
        Assert.False(database.Artifacts.Calls.ContainsKey(draft.Id));
    }

    [Fact]
    public async Task Not_kept_when_Provenance_refuses_it()
    {
        var (draft, owner, _, _) = await DraftAsync(database);
        database.Artifacts.Refuse[draft.Id] = true;

        Assert.Equal("Provenance.IdTaken", (await ApproveAsync(database, draft.Id, owner)).Error.Code);
        Assert.Equal(SummaryState.Draft, (await ReadAsync(database, draft.Id)).State);
    }

    [Fact]
    public async Task An_invalidated_summary_cannot_be_approved()
    {
        var (draft, owner, _, _) = await DraftAsync(database);
        await using (var scope = database.CreateScope())
        {
            await ArtifactInvalidatedHandler.Handle(
                new ArtifactInvalidatedEvent(Guid.CreateVersion7(), DateTimeOffset.UtcNow, draft.Id, ArtifactKinds.PeriodSummary, owner, "ContentDeleted", 2),
                scope.ServiceProvider,
                Cancellation);
        }

        Assert.Equal(SummaryErrors.Invalidated, (await ApproveAsync(database, draft.Id, owner)).Error);
    }
}
