using Memorio.Modules.Archive.Application.Queries.MemoryDetails;
using Microsoft.Extensions.DependencyInjection;

namespace Memorio.Modules.Archive.Tests;

/// <summary>MEM-E2-ARCHIWUM-09 on a real database: person x memory.</summary>
public sealed class MemoryDetailsTests(ArchiveDatabase database)
{
    [Fact]
    public async Task The_author_sees_everything_with_signed_photos()
    {
        var account = Guid.CreateVersion7();
        var memory = await database.AddMemoryAsync(account, photos: 2);

        var details = await GetAsync(memory.Id.Value, account);

        Assert.Equal("Giewont", details?.PlaceName);
        Assert.Equal("Day", details?.DatePrecision);
        Assert.Equal("2026-07-01", details?.DateFrom);
        Assert.Equal(account, details?.AuthorAccountId);
        Assert.False(details?.DescriptionIsAi);
        Assert.Equal(2, details?.Photos.Count);
        Assert.All(details!.Photos, photo => Assert.Contains("signed", photo.Url.Query, StringComparison.Ordinal));
    }

    [Fact]
    public async Task A_stranger_sees_nothing_the_same_way_as_for_a_memory_that_does_not_exist()
    {
        var memory = await database.AddMemoryAsync(Guid.CreateVersion7());
        var stranger = Guid.CreateVersion7();

        Assert.Null(await GetAsync(memory.Id.Value, stranger));
        Assert.Null(await GetAsync(Guid.CreateVersion7(), stranger));
    }

    private async Task<MemoryDetails?> GetAsync(Guid memory, Guid account)
    {
        await using var scope = database.CreateScope();
        return await scope.ServiceProvider.GetRequiredService<IMemoryDetailsQuery>().GetAsync(memory, account, TestContext.Current.CancellationToken);
    }
}
