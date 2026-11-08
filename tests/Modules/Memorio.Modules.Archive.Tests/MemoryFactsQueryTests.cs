using Memorio.Modules.Archive.Contracts;
using Microsoft.Extensions.DependencyInjection;

namespace Memorio.Modules.Archive.Tests;

/// <summary>MEM-E6-NARRACJA-AI-06: what a summary may tell of memories, in order, without the trashed ones.</summary>
public sealed class MemoryFactsQueryTests(ArchiveDatabase database)
{
    private static CancellationToken Cancellation => TestContext.Current.CancellationToken;

    [Fact]
    public async Task Facts_of_the_memories_in_use_in_date_order()
    {
        var author = Guid.CreateVersion7();
        var later = await database.AddMemoryAsync(author, dateFrom: new DateOnly(2026, 7, 2), time: "18:30");
        var earlier = await database.AddMemoryAsync(author, dateFrom: new DateOnly(2026, 7, 1));
        var trashed = await database.AddMemoryAsync(author);
        await database.TrashAsync(trashed.Id, author);

        await using var scope = database.CreateScope();
        var facts = await scope.ServiceProvider.GetRequiredService<IMemoryFacts>()
            .FactsAsync([later.Id.Value, earlier.Id.Value, trashed.Id.Value, Guid.CreateVersion7()], Cancellation);

        Assert.Equal([earlier.Id.Value, later.Id.Value], facts.Select(fact => fact.MemoryId));
        var last = facts[1];
        Assert.Equal(author, last.AuthorAccountId);
        Assert.Equal(new DateOnly(2026, 7, 2), last.DateFrom);
        Assert.Equal("18:30", last.Time);
        Assert.Equal(later.PlaceName, last.PlaceName);
        Assert.Empty(await scope.ServiceProvider.GetRequiredService<IMemoryFacts>().FactsAsync([], Cancellation));
    }
}
