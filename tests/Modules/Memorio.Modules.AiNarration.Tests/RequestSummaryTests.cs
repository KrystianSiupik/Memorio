using Memorio.Modules.AiNarration.Application.Commands.Summaries;
using Memorio.Modules.AiNarration.Domain.Summaries;
using Memorio.SharedKernel.Results;
using Microsoft.Extensions.DependencyInjection;

namespace Memorio.Modules.AiNarration.Tests;

/// <summary>MEM-E6-NARRACJA-AI-03 on a real database: C82, E82 and E83, with the provider and the allowance in memory.</summary>
public sealed class RequestSummaryTests(AiNarrationDatabase database)
{
    private static CancellationToken Cancellation => TestContext.Current.CancellationToken;

    private static readonly DateOnly July = new(2026, 7, 1);

    internal static async Task<Result<PeriodSummary>> RequestAsync(AiNarrationDatabase database, Guid id, Guid account, Guid album, DateOnly? from = null, DateOnly? to = null)
    {
        await using var scope = database.CreateScope();
        return await scope.ServiceProvider.GetRequiredService<RequestSummaryHandler>()
            .HandleAsync(new RequestSummary(id, account, album, from ?? July, to ?? July.AddDays(30)), Cancellation);
    }

    /// <summary>An album whose owner and Ola consented and whose Jan did not, each with a July memory.</summary>
    internal static async Task<(Guid Album, Guid Owner, Guid OwnersMemory, Guid OlasMemory, Guid Jan)> AlbumAsync(AiNarrationDatabase database, string ownersDescription = "W górach.")
    {
        var album = Guid.CreateVersion7();
        var owner = Guid.CreateVersion7();
        var ola = Guid.CreateVersion7();
        var jan = Guid.CreateVersion7();
        var owners = database.Albums.Bring(album, owner, July, place: "Giewont", description: ownersDescription);
        var olas = database.Albums.Bring(album, ola, July.AddDays(2), place: "Morskie Oko");
        database.Albums.Bring(album, jan, July.AddDays(4));
        await RevokeAiConsentTests.GiveAsync(database, owner, album);
        await RevokeAiConsentTests.GiveAsync(database, ola, album);
        return (album, owner, owners.MemoryId, olas.MemoryId, jan);
    }

    [Fact]
    public async Task The_story_keeps_only_traceable_sentences_says_what_was_left_out_and_names_no_one_to_the_provider()
    {
        var (album, owner, ownersMemory, olasMemory, _) = await AlbumAsync(database);
        var id = Guid.CreateVersion7();

        var summary = (await RequestAsync(database, id, owner, album)).Value;

        Assert.Equal(SummaryState.Draft, summary.State);
        Assert.Equal("W lipcu byliście w Giewont.", Assert.Single(summary.Sentences).Text);
        Assert.Equal([ownersMemory], summary.Sentences[0].MemoryIds);
        Assert.Equal(2, summary.DroppedSentences);
        Assert.Equal(1, summary.LeftOutMemories);
        Assert.Equal(1, summary.LeftOutPeople);
        Assert.Equal([ownersMemory, olasMemory], summary.Sources.Select(source => source.MemoryId));
        Assert.Equal("test-model", summary.Model);
        Assert.Equal("period-summary@2", summary.PromptVersion);

        var sent = database.Provider.Materials.Where(material => material.Contains("Recorded by person B", StringComparison.Ordinal)).ToList();
        Assert.NotEmpty(sent);
        Assert.All(sent, material =>
        {
            Assert.DoesNotContain("Giewont", material, StringComparison.Ordinal);
            Assert.DoesNotContain("Morskie Oko", material, StringComparison.Ordinal);
            Assert.DoesNotContain(owner.ToString(), material, StringComparison.Ordinal);
        });
    }

    [Fact]
    public async Task Asking_again_with_the_same_id_gets_the_same_summary_for_one_use_of_the_allowance()
    {
        var (album, owner, _, _, _) = await AlbumAsync(database);
        var id = Guid.CreateVersion7();

        var first = (await RequestAsync(database, id, owner, album)).Value;
        var again = (await RequestAsync(database, id, owner, album)).Value;

        Assert.Equal(first.Id, again.Id);
        Assert.Equal(first.Text, again.Text);
        Assert.Equal(1, database.Allowance.Consumed[id]);
        Assert.Equal(SummaryErrors.NotYours, (await RequestAsync(database, id, Guid.CreateVersion7(), album)).Error);
    }

    [Fact]
    public async Task Refused_without_a_record_outside_the_album_without_consent_without_material_or_allowance()
    {
        var (album, owner, _, _, jan) = await AlbumAsync(database);

        Assert.Equal(SummaryErrors.NotMember, (await RequestAsync(database, Guid.CreateVersion7(), Guid.CreateVersion7(), album)).Error);
        Assert.Equal(SummaryErrors.ConsentRequired, (await RequestAsync(database, Guid.CreateVersion7(), jan, album)).Error);
        Assert.Equal(SummaryErrors.NoMaterials, (await RequestAsync(database, Guid.CreateVersion7(), owner, album, July.AddYears(-1), July.AddYears(-1))).Error);
        Assert.Equal(SummaryErrors.InvalidPeriod, (await RequestAsync(database, Guid.CreateVersion7(), owner, album, July, July.AddDays(-1))).Error);

        database.Allowance.UsedUp[owner] = true;
        var id = Guid.CreateVersion7();
        Assert.Equal("AiNarration.LimitReached", (await RequestAsync(database, id, owner, album)).Error.Code);
        database.Allowance.UsedUp.TryRemove(owner, out _);

        await using var scope = database.CreateScope();
        Assert.Null(await scope.ServiceProvider.GetRequiredService<IPeriodSummaries>().FindAsync(id, Cancellation));
    }

    [Fact]
    public async Task When_the_AI_supports_nothing_there_is_no_summary_and_the_retry_costs_nothing_more()
    {
        var marker = "Dolina" + Guid.CreateVersion7().ToString("N")[..8];
        var (album, owner, _, _, _) = await AlbumAsync(database, ownersDescription: marker);
        var id = Guid.CreateVersion7();
        database.Provider.SummaryAnswerWhen[marker] = "Było pięknie. Wszyscy się cieszyli.";

        Assert.Equal(SummaryErrors.NothingTold, (await RequestAsync(database, id, owner, album)).Error);

        database.Provider.SummaryAnswerWhen.TryRemove(marker, out _);

        Assert.True((await RequestAsync(database, id, owner, album)).IsSuccess);
        Assert.Equal(1, database.Allowance.Consumed[id]);
    }
}
