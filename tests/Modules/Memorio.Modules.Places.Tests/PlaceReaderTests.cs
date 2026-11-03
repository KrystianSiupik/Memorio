using Memorio.Modules.Places.Application.Commands.AddOwnPlace;
using Memorio.Modules.Places.Contracts;
using Microsoft.Extensions.DependencyInjection;

namespace Memorio.Modules.Places.Tests;

/// <summary>MEM-E2-MIEJSCA-04: the port as Archive sees it.</summary>
public sealed class PlaceReaderTests(PlacesDatabase database)
{
    private static CancellationToken Cancellation => TestContext.Current.CancellationToken;

    [Fact]
    public async Task An_osm_place_exists_for_everybody()
    {
        var osmId = Random.Shared.NextInt64(1, long.MaxValue);
        await database.AddOsmPlaceAsync(osmId, "Rysy", latitude: 49.18, longitude: 20.09);

        var place = await FindAsync(Guid.CreateVersion7(), $"osm:N:{osmId}");

        Assert.Equal("Rysy", place?.Name);
        Assert.Equal(49.18, place!.Latitude, 6);
    }

    [Fact]
    public async Task An_own_place_exists_only_for_its_owner()
    {
        var id = Guid.CreateVersion7();
        var owner = Guid.CreateVersion7();
        await using (var scope = database.CreateScope())
        {
            await scope.ServiceProvider.GetRequiredService<AddOwnPlaceHandler>().HandleAsync(new AddOwnPlace(id, owner, "Chata", 49, 20), Cancellation);
        }

        Assert.Equal("Chata", (await FindAsync(owner, $"own:{id}"))?.Name);
        Assert.Null(await FindAsync(Guid.CreateVersion7(), $"own:{id}"));
    }

    [Theory]
    [InlineData("osm:N:1")]
    [InlineData("nonsense")]
    public async Task What_does_not_exist_is_null(string reference)
    {
        Assert.Null(await FindAsync(Guid.CreateVersion7(), reference));
    }

    private async Task<PlaceInfo?> FindAsync(Guid account, string reference)
    {
        await using var scope = database.CreateScope();
        return await scope.ServiceProvider.GetRequiredService<IPlaceReader>().FindAsync(account, reference, Cancellation);
    }
}
