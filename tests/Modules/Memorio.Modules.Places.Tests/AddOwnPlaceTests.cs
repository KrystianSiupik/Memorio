using Memorio.Modules.Places.Application.Commands.AddOwnPlace;
using Memorio.Modules.Places.Application.Queries.PlaceSuggestions;
using Memorio.Modules.Places.Domain.Places;
using Microsoft.Extensions.DependencyInjection;

namespace Memorio.Modules.Places.Tests;

/// <summary>MEM-E2-MIEJSCA-02 on a real database.</summary>
public sealed class AddOwnPlaceTests(PlacesDatabase database)
{
    [Fact]
    public async Task A_new_place_gives_the_event_and_shows_in_the_owners_suggestions()
    {
        var account = Guid.CreateVersion7();
        var name = "p" + Guid.NewGuid().ToString("N");

        var added = await AddAsync(Guid.CreateVersion7(), account, "  " + name + "  ");

        Assert.Equal(name, added.Value.Place.Name);
        await using var scope = database.CreateScope();
        var found = await scope.ServiceProvider.GetRequiredService<IPlaceSuggestionsQuery>().FindAsync(account, name, 5, TestContext.Current.CancellationToken);
        Assert.Equal(name, Assert.Single(found).Name);
    }

    [Theory]
    [InlineData(null, 50, 19)]
    [InlineData("   ", 50, 19)]
    [InlineData("name", 91, 19)]
    [InlineData("name", -91, 19)]
    [InlineData("name", 50, 181)]
    [InlineData("name", 50, -181)]
    public async Task Bad_input_is_refused(string? name, double latitude, double longitude)
    {
        var refused = await AddAsync(Guid.CreateVersion7(), Guid.CreateVersion7(), name, latitude, longitude);

        Assert.True(refused.IsFailure);
    }

    [Fact]
    public async Task A_name_that_is_too_long_is_refused()
    {
        var refused = await AddAsync(Guid.CreateVersion7(), Guid.CreateVersion7(), new string('a', OwnPlace.MaximumNameLength + 1));

        Assert.Equal(PlaceErrors.InvalidName, refused.Error);
    }

    [Fact]
    public async Task The_same_id_twice_or_at_once_is_one_place()
    {
        var id = Guid.CreateVersion7();
        var account = Guid.CreateVersion7();

        var results = await Task.WhenAll(Enumerable.Range(0, 5).Select(_ => AddAsync(id, account, "Chata")));

        Assert.All(results, result => Assert.Equal(id, result.Value.Place.Id));
    }

    [Fact]
    public async Task An_id_of_someone_elses_place_is_refused()
    {
        var id = Guid.CreateVersion7();
        await AddAsync(id, Guid.CreateVersion7(), "Chata");

        var refused = await AddAsync(id, Guid.CreateVersion7(), "Moja chata");

        Assert.Equal(PlaceErrors.IdTaken, refused.Error);
    }

    private async Task<SharedKernel.Results.Result<OwnPlaceAdded>> AddAsync(Guid id, Guid account, string? name, double latitude = 50, double longitude = 19)
    {
        await using var scope = database.CreateScope();
        return await scope.ServiceProvider.GetRequiredService<AddOwnPlaceHandler>()
            .HandleAsync(new AddOwnPlace(id, account, name, latitude, longitude), TestContext.Current.CancellationToken);
    }
}
