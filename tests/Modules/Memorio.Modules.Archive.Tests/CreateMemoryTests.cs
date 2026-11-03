using Memorio.Modules.Archive.Application.Commands.CreateMemory;
using Memorio.Modules.Archive.Domain.Memories;
using Microsoft.Extensions.DependencyInjection;

namespace Memorio.Modules.Archive.Tests;

/// <summary>MEM-E2-ARCHIWUM-01 on a real database.</summary>
public sealed class CreateMemoryTests(ArchiveDatabase database)
{
    private static string Today => DateOnly.FromDateTime(DateTime.UtcNow).ToString("yyyy-MM-dd", System.Globalization.CultureInfo.InvariantCulture);

    [Fact]
    public async Task A_memory_keeps_its_author_place_and_date()
    {
        var account = Guid.CreateVersion7();
        var id = Guid.CreateVersion7();

        var created = await CreateAsync(id, account);

        var memory = created.Value;
        Assert.Equal(new MemoryId(id), memory.Id);
        Assert.Equal(account, memory.AuthorAccountId);
        Assert.Equal("Giewont", memory.PlaceName);
        Assert.Equal(49.25, memory.Latitude);
        Assert.Equal(DatePrecision.Day, memory.DatePrecision);
    }

    [Theory]
    [InlineData("Day", "2026-07-15", null, "2026-07-15", "2026-07-15")]
    [InlineData("Month", "2026-02-15", null, "2026-02-01", "2026-02-28")]
    [InlineData("Year", "2024-07-15", null, "2024-01-01", "2024-12-31")]
    [InlineData("Period", "2026-07-01", "2026-07-20", "2026-07-01", "2026-07-20")]
    public void A_date_stands_for_a_range(string precision, string from, string? to, string first, string last)
    {
        var date = MemoryDate.Create(precision, from, to, new DateOnly(2026, 11, 3));

        Assert.Equal(DateOnly.Parse(first, System.Globalization.CultureInfo.InvariantCulture), date.Value.From);
        Assert.Equal(DateOnly.Parse(last, System.Globalization.CultureInfo.InvariantCulture), date.Value.To);
    }

    [Theory]
    [InlineData(null, "2026-07-15", null)]
    [InlineData("Sometime", "2026-07-15", null)]
    [InlineData("7", "2026-07-15", null)]
    [InlineData("Day", null, null)]
    [InlineData("Day", "15.07.2026", null)]
    [InlineData("Day", "2026-13-01", null)]
    [InlineData("Period", "2026-07-15", null)]
    [InlineData("Period", "2026-07-15", "2026-07-14")]
    public void What_is_not_a_date_is_refused(string? precision, string? from, string? to)
    {
        Assert.Equal(MemoryErrors.InvalidDate, MemoryDate.Create(precision, from, to, new DateOnly(2026, 11, 3)).Error);
    }

    [Fact]
    public void A_date_in_the_future_is_refused_but_the_devices_day_may_run_ahead()
    {
        var today = new DateOnly(2026, 11, 3);

        Assert.True(MemoryDate.Create("Day", "2026-11-04", null, today).IsSuccess);
        Assert.Equal(MemoryErrors.DateInFuture, MemoryDate.Create("Day", "2026-11-05", null, today).Error);
        Assert.Equal(MemoryErrors.DateInFuture, MemoryDate.Create("Year", "2027-01-01", null, today).Error);
    }

    [Fact]
    public async Task A_future_date_stores_nothing()
    {
        var id = Guid.CreateVersion7();

        var refused = await CreateAsync(id, Guid.CreateVersion7(), from: "2999-01-01");
        var later = await CreateAsync(id, Guid.CreateVersion7());

        Assert.Equal(MemoryErrors.DateInFuture, refused.Error);
        Assert.True(later.IsSuccess);
    }

    [Fact]
    public async Task Terms_come_first()
    {
        var account = Guid.CreateVersion7();
        database.Neighbours.WithoutTerms(account);

        var refused = await CreateAsync(Guid.CreateVersion7(), account);

        Assert.Equal(MemoryErrors.TermsNotAccepted, refused.Error);
    }

    [Fact]
    public async Task A_place_that_does_not_exist_is_refused()
    {
        var refused = await CreateAsync(Guid.CreateVersion7(), Guid.CreateVersion7(), place: FakeNeighbours.MissingPlace);

        Assert.Equal(MemoryErrors.PlaceNotFound, refused.Error);
    }

    [Fact]
    public async Task The_same_id_twice_or_at_once_is_one_memory()
    {
        var id = Guid.CreateVersion7();
        var account = Guid.CreateVersion7();

        var results = await Task.WhenAll(Enumerable.Range(0, 5).Select(_ => CreateAsync(id, account)));

        Assert.All(results, result => Assert.Equal(new MemoryId(id), result.Value.Id));
    }

    [Fact]
    public async Task An_id_of_someone_elses_memory_is_refused()
    {
        var id = Guid.CreateVersion7();
        await CreateAsync(id, Guid.CreateVersion7());

        var refused = await CreateAsync(id, Guid.CreateVersion7());

        Assert.Equal(MemoryErrors.IdTaken, refused.Error);
    }

    private async Task<SharedKernel.Results.Result<Memory>> CreateAsync(Guid id, Guid account, string place = "osm:N:1", string? from = null)
    {
        await using var scope = database.CreateScope();
        return await scope.ServiceProvider.GetRequiredService<CreateMemoryHandler>()
            .HandleAsync(new CreateMemory(id, account, place, "Day", from ?? Today, null), TestContext.Current.CancellationToken);
    }
}
