using Memorio.Modules.AiNarration.Domain.Summaries;
using Memorio.Modules.AiNarration.Infrastructure.Messaging;
using Memorio.Modules.Provenance.Contracts;

namespace Memorio.Modules.AiNarration.Tests;

/// <summary>MEM-E6-NARRACJA-AI-04, E61 (SD6): a saved summary whose source is gone loses its text for good, once.</summary>
public sealed class ArtifactInvalidatedHandlerIdempotencyTests(AiNarrationDatabase database)
{
    private static CancellationToken Cancellation => TestContext.Current.CancellationToken;

    [Fact]
    public async Task The_text_goes_once_and_other_kinds_of_artifacts_are_not_summaries()
    {
        var (draft, owner, _, _) = await ApproveSummaryTests.DraftAsync(database);
        var (other, otherOwner, _, _) = await ApproveSummaryTests.DraftAsync(database);
        await ApproveSummaryTests.ApproveAsync(database, draft.Id, owner);
        var at = DateTimeOffset.UtcNow.AddMinutes(-1);
        var invalidated = new ArtifactInvalidatedEvent(Guid.CreateVersion7(), at, draft.Id, ArtifactKinds.PeriodSummary, owner, "AiConsentRevoked", 2);
        var notASummary = new ArtifactInvalidatedEvent(Guid.CreateVersion7(), at, other.Id, "SomethingElse", otherOwner, "ContentDeleted", 2);

        for (var delivery = 0; delivery < 2; delivery++)
        {
            await using var scope = database.CreateScope();
            await ArtifactInvalidatedHandler.Handle(invalidated, scope.ServiceProvider, Cancellation);
            await ArtifactInvalidatedHandler.Handle(notASummary, scope.ServiceProvider, Cancellation);
        }

        var summary = await ApproveSummaryTests.ReadAsync(database, draft.Id);
        Assert.Equal(SummaryState.Invalidated, summary.State);
        Assert.Empty(summary.Sentences);
        Assert.Equal("", summary.Text);
        Assert.Equal("AiConsentRevoked", summary.InvalidReason);
        Assert.Equal(at.ToUnixTimeMilliseconds(), summary.InvalidatedAt!.Value.ToUnixTimeMilliseconds());
        Assert.Equal(SummaryState.Draft, (await ApproveSummaryTests.ReadAsync(database, other.Id)).State);
    }
}
