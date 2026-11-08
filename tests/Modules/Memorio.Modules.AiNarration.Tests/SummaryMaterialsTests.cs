using Memorio.Modules.AiNarration.Application.Queries.SummaryMaterials;
using Microsoft.Extensions.DependencyInjection;

namespace Memorio.Modules.AiNarration.Tests;

/// <summary>MEM-E6-NARRACJA-AI-06: R8, the materials allowed for a summary (S2:C5), user × resource.</summary>
public sealed class SummaryMaterialsTests(AiNarrationDatabase database)
{
    private static CancellationToken Cancellation => TestContext.Current.CancellationToken;

    private static readonly DateOnly July = new(2026, 7, 1);

    private async Task<SummaryMaterials> ForAsync(Guid account, Guid album, DateOnly from, DateOnly to)
    {
        await using var scope = database.CreateScope();
        return await scope.ServiceProvider.GetRequiredService<ISummaryMaterialsQuery>().ForAsync(account, album, from, to, Cancellation);
    }

    [Fact]
    public async Task Only_what_the_person_may_read_from_the_period_by_consenting_authors_and_the_rest_counted()
    {
        var album = Guid.CreateVersion7();
        var owner = Guid.CreateVersion7();
        var ola = Guid.CreateVersion7();
        var jan = Guid.CreateVersion7();
        var ownersJuly = database.Albums.Bring(album, owner, July);
        var olasJuly = database.Albums.Bring(album, ola, July.AddDays(3));
        var jansJuly = database.Albums.Bring(album, jan, July.AddDays(5));
        var ownersAugust = database.Albums.Bring(album, owner, July.AddMonths(1));
        var hidden = database.Albums.Bring(album, owner, July.AddDays(7));
        database.Albums.Hide(hidden.MemoryId);
        await RevokeAiConsentTests.GiveAsync(database, owner, album);
        await RevokeAiConsentTests.GiveAsync(database, ola, album);

        var materials = await ForAsync(owner, album, July, July.AddDays(30));

        Assert.Equal([ownersJuly.MemoryId, olasJuly.MemoryId], materials.Facts.Select(fact => fact.MemoryId));
        Assert.Equal(1, materials.LeftOutMemories);
        Assert.Equal(1, materials.LeftOutPeople);
        Assert.DoesNotContain(materials.Facts, fact => fact.MemoryId == ownersAugust.MemoryId || fact.MemoryId == jansJuly.MemoryId);
    }

    [Fact]
    public async Task Nothing_for_somebody_who_may_not_read_the_album_and_nothing_once_a_consent_is_taken_back()
    {
        var album = Guid.CreateVersion7();
        var owner = Guid.CreateVersion7();
        database.Albums.Bring(album, owner, July);
        await RevokeAiConsentTests.GiveAsync(database, owner, album);

        Assert.Empty((await ForAsync(Guid.CreateVersion7(), album, July, July)).Facts);

        await using (var scope = database.CreateScope())
        {
            await scope.ServiceProvider.GetRequiredService<Memorio.Modules.AiNarration.Application.Commands.Consents.RevokeAiConsentHandler>()
                .HandleAsync(new(owner, album), Cancellation);
        }

        var materials = await ForAsync(owner, album, July, July);
        Assert.Empty(materials.Facts);
        Assert.Equal(1, materials.LeftOutMemories);
    }
}
