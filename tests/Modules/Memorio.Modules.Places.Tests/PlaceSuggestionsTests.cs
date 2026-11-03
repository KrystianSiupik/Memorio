using Memorio.Modules.Places.Application.Queries.PlaceSuggestions;
using Microsoft.Extensions.DependencyInjection;

namespace Memorio.Modules.Places.Tests;

/// <summary>MEM-E2-MIEJSCA-03 on a real database: person x resource. OpenStreetMap is for all, own places for their owner.</summary>
public sealed class PlaceSuggestionsTests(PlacesDatabase database)
{
    [Fact]
    public async Task Own_places_are_seen_by_their_owner_only_and_come_first()
    {
        var name = UniqueName();
        var ola = Guid.CreateVersion7();
        var jan = Guid.CreateVersion7();
        await database.AddOsmPlaceAsync(NewOsmId(), name + " city");
        await database.AddOwnPlaceAsync(ola, name + " Olas cabin in the woods");
        await database.AddOwnPlaceAsync(jan, name + " Jans cabin");

        var seenByOla = await FindAsync(ola, name);
        var seenByJan = await FindAsync(jan, name);
        var seenByNobody = await FindAsync(Guid.CreateVersion7(), name);

        Assert.Equal([name + " Olas cabin in the woods", name + " city"], seenByOla.Select(place => place.Name));
        Assert.True(seenByOla[0].IsOwn);
        Assert.StartsWith("own:", seenByOla[0].PlaceReference, StringComparison.Ordinal);
        Assert.Equal([name + " Jans cabin", name + " city"], seenByJan.Select(place => place.Name));
        Assert.Equal([name + " city"], seenByNobody.Select(place => place.Name));
    }

    [Fact]
    public async Task An_osm_place_comes_with_its_reference_and_position_ignoring_case()
    {
        var name = UniqueName();
        var osmId = NewOsmId();
        await database.AddOsmPlaceAsync(osmId, name, latitude: 49.3, longitude: 19.95);

        var place = Assert.Single(await FindAsync(Guid.CreateVersion7(), name.ToUpperInvariant()));

        Assert.Equal($"osm:N:{osmId}", place.PlaceReference);
        Assert.Equal(49.3, place.Latitude, 6);
        Assert.Equal(19.95, place.Longitude, 6);
        Assert.False(place.IsOwn);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData(" k ")]
    public async Task Too_little_text_finds_nothing(string? text)
    {
        Assert.Empty(await FindAsync(Guid.CreateVersion7(), text));
    }

    [Theory]
    [InlineData("%%")]
    [InlineData("__")]
    public async Task Wildcards_are_plain_text(string text)
    {
        await database.AddOsmPlaceAsync(NewOsmId(), UniqueName());

        Assert.Empty(await FindAsync(Guid.CreateVersion7(), text));
    }

    [Fact]
    public async Task The_limit_holds()
    {
        var name = UniqueName();
        for (var index = 0; index < 3; index++)
        {
            await database.AddOsmPlaceAsync(NewOsmId(), name + index);
        }

        await using var scope = database.CreateScope();
        var found = await scope.ServiceProvider.GetRequiredService<IPlaceSuggestionsQuery>()
            .FindAsync(Guid.CreateVersion7(), name, 2, TestContext.Current.CancellationToken);

        Assert.Equal(2, found.Count);
    }

    private static string UniqueName() => "p" + Guid.NewGuid().ToString("N");

    private static long NewOsmId() => Random.Shared.NextInt64(1, long.MaxValue);

    private async Task<IReadOnlyList<PlaceSuggestion>> FindAsync(Guid account, string? text)
    {
        await using var scope = database.CreateScope();
        return await scope.ServiceProvider.GetRequiredService<IPlaceSuggestionsQuery>()
            .FindAsync(account, text, 10, TestContext.Current.CancellationToken);
    }
}
