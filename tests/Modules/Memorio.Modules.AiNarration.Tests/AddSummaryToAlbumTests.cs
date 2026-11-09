using Memorio.Modules.AiNarration.Application.Commands.Summaries;
using Memorio.Modules.AiNarration.Application.Queries.AlbumSummaries;
using Memorio.Modules.AiNarration.Domain.Summaries;
using Memorio.Modules.AiNarration.Infrastructure.Messaging;
using Memorio.Modules.Provenance.Contracts;
using Memorio.SharedKernel.Results;
using Microsoft.Extensions.DependencyInjection;

namespace Memorio.Modules.AiNarration.Tests;

/// <summary>MEM-E6-NARRACJA-AI-05 on a real database: C84, E85, and what the album's participants see (HS-009).</summary>
public sealed class AddSummaryToAlbumTests(AiNarrationDatabase database)
{
    private static CancellationToken Cancellation => TestContext.Current.CancellationToken;

    private async Task<Result<PeriodSummary>> AddAsync(Guid id, Guid account)
    {
        await using var scope = database.CreateScope();
        return await scope.ServiceProvider.GetRequiredService<AddSummaryToAlbumHandler>().HandleAsync(new AddSummaryToAlbum(id, account), Cancellation);
    }

    private async Task<Result<IReadOnlyList<PeriodSummary>>> InAlbumAsync(Guid account, Guid album)
    {
        await using var scope = database.CreateScope();
        return await scope.ServiceProvider.GetRequiredService<AlbumSummariesQuery>().ForAsync(account, album, Cancellation);
    }

    /// <summary>A saved summary of the owner, with Jan, another participant of the album.</summary>
    private async Task<(PeriodSummary Saved, Guid Owner, Guid Album, Guid OwnersMemory, Guid Jan)> SavedAsync()
    {
        var (album, owner, ownersMemory, _, jan) = await RequestSummaryTests.AlbumAsync(database);
        var draft = (await RequestSummaryTests.RequestAsync(database, Guid.CreateVersion7(), owner, album)).Value;
        var saved = (await ApproveSummaryTests.ApproveAsync(database, draft.Id, owner)).Value;
        return (saved, owner, album, ownersMemory, jan);
    }

    [Fact]
    public async Task Added_once_it_is_seen_by_the_participants_and_by_no_one_else()
    {
        var (saved, owner, album, _, jan) = await SavedAsync();

        var added = (await AddAsync(saved.Id, owner)).Value;
        var again = (await AddAsync(saved.Id, owner)).Value;

        Assert.NotNull(added.AddedToAlbumAt);
        Assert.Equal(added.AddedToAlbumAt!.Value.ToUnixTimeMilliseconds(), again.AddedToAlbumAt!.Value.ToUnixTimeMilliseconds());
        Assert.Equal([saved.Id], (await InAlbumAsync(jan, album)).Value.Select(summary => summary.Id));
        Assert.Equal([saved.Id], (await InAlbumAsync(owner, album)).Value.Select(summary => summary.Id));
        Assert.Equal(SummaryErrors.NotMember, (await InAlbumAsync(Guid.CreateVersion7(), album)).Error);
    }

    [Fact]
    public async Task Only_its_participating_author_adds_a_saved_summary()
    {
        var (album, owner, _, _, _) = await RequestSummaryTests.AlbumAsync(database);
        var draft = (await RequestSummaryTests.RequestAsync(database, Guid.CreateVersion7(), owner, album)).Value;

        Assert.Equal(SummaryErrors.NotSaved, (await AddAsync(draft.Id, owner)).Error);
        Assert.Equal(SummaryErrors.NotFound, (await AddAsync(draft.Id, Guid.CreateVersion7())).Error);

        var (saved, savedOwner, savedAlbum, _, jan) = await SavedAsync();
        database.Albums.Leave(savedOwner, savedAlbum);
        Assert.Equal(SummaryErrors.NotMember, (await AddAsync(saved.Id, savedOwner)).Error);
        Assert.Empty((await InAlbumAsync(jan, savedAlbum)).Value);
    }

    [Fact]
    public async Task A_summary_of_a_memory_hidden_from_the_reader_or_invalidated_is_not_shown()
    {
        var (saved, owner, album, ownersMemory, jan) = await SavedAsync();
        await AddAsync(saved.Id, owner);

        database.Albums.Hide(ownersMemory);
        Assert.Empty((await InAlbumAsync(jan, album)).Value);

        var (other, otherOwner, otherAlbum, _, otherJan) = await SavedAsync();
        await AddAsync(other.Id, otherOwner);
        await using (var scope = database.CreateScope())
        {
            await ArtifactInvalidatedHandler.Handle(
                new ArtifactInvalidatedEvent(Guid.CreateVersion7(), DateTimeOffset.UtcNow, other.Id, ArtifactKinds.PeriodSummary, otherOwner, "AccessRevoked", 3),
                scope.ServiceProvider,
                Cancellation);
        }

        Assert.Empty((await InAlbumAsync(otherJan, otherAlbum)).Value);
        Assert.Equal(SummaryErrors.Invalidated, (await AddAsync(other.Id, otherOwner)).Error);
    }
}
