using Memorio.Modules.Places.Application.Commands.ChoosePlace;
using Memorio.Modules.Places.Domain.Places;
using Microsoft.Extensions.DependencyInjection;

namespace Memorio.Modules.Places.Tests;

/// <summary>MEM-E2-MIEJSCA-01 on a real database.</summary>
public sealed class ChoosePlaceTests(PlacesDatabase database)
{
    [Fact]
    public async Task Choosing_an_existing_place_gives_the_event_with_the_place()
    {
        var osmId = Random.Shared.NextInt64(1, long.MaxValue);
        await database.AddOsmPlaceAsync(osmId, "Giewont");
        var account = Guid.CreateVersion7();

        var chosen = await ChooseAsync(account, $"osm:N:{osmId}");

        Assert.Equal("Giewont", chosen.Value.Place.Name);
        Assert.Equal(account, chosen.Value.AccountId);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("osm:N")]
    [InlineData("osm:X:1")]
    [InlineData("osm:N:-1")]
    [InlineData("own:not-a-guid")]
    [InlineData("somewhere")]
    public async Task What_is_not_a_reference_is_refused(string? reference)
    {
        var refused = await ChooseAsync(Guid.CreateVersion7(), reference);

        Assert.Equal(PlaceErrors.InvalidReference, refused.Error);
    }

    [Fact]
    public async Task A_place_that_does_not_exist_is_refused()
    {
        var refused = await ChooseAsync(Guid.CreateVersion7(), "osm:N:1");

        Assert.Equal(PlaceErrors.NotFound, refused.Error);
    }

    private async Task<SharedKernel.Results.Result<PlaceChosen>> ChooseAsync(Guid account, string? reference)
    {
        await using var scope = database.CreateScope();
        return await scope.ServiceProvider.GetRequiredService<ChoosePlaceHandler>()
            .HandleAsync(new ChoosePlace(account, reference), TestContext.Current.CancellationToken);
    }
}
