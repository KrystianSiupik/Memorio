using Memorio.Modules.Archive.Application.Commands.CreateMemory;
using Memorio.Modules.Archive.Application.Queries.MemoryMap;
using Microsoft.Extensions.DependencyInjection;

namespace Memorio.Modules.Archive.Tests;

/// <summary>MEM-E2-ARCHIWUM-08 on a real database: person x memory.</summary>
public sealed class MemoryMapTests(ArchiveDatabase database)
{
    private static CancellationToken Cancellation => TestContext.Current.CancellationToken;

    [Fact]
    public async Task Each_person_sees_their_own_points_and_none_from_the_trash()
    {
        var ola = Guid.CreateVersion7();
        var jan = Guid.CreateVersion7();
        var hers = await database.AddMemoryAsync(ola);
        var trashed = await database.AddMemoryAsync(ola);
        var his = await database.AddMemoryAsync(jan);
        await database.TrashAsync(trashed.Id, ola);

        var seenByOla = await GetAsync(ola);
        var seenByJan = await GetAsync(jan);

        Assert.Equal(hers.Id.Value, Assert.Single(seenByOla).Id);
        Assert.Equal(49.25, seenByOla[0].Latitude);
        Assert.Equal(his.Id.Value, Assert.Single(seenByJan).Id);
    }

    [Fact]
    public async Task Dates_filter_by_overlap()
    {
        var account = Guid.CreateVersion7();
        var trip = Guid.CreateVersion7();
        var year = Guid.CreateVersion7();
        await using (var scope = database.CreateScope())
        {
            var create = scope.ServiceProvider.GetRequiredService<CreateMemoryHandler>();
            await create.HandleAsync(new CreateMemory(trip, account, "osm:N:1", "Period", "2025-06-20", "2025-08-10"), Cancellation);
        }

        await using (var scope = database.CreateScope())
        {
            var create = scope.ServiceProvider.GetRequiredService<CreateMemoryHandler>();
            await create.HandleAsync(new CreateMemory(year, account, "osm:N:1", "Year", "2023-05-01", null), Cancellation);
        }

        Assert.Equal(trip, Assert.Single(await GetAsync(account, new DateOnly(2025, 7, 1), new DateOnly(2025, 7, 31))).Id);
        Assert.Equal(year, Assert.Single(await GetAsync(account, null, new DateOnly(2023, 1, 1))).Id);
        Assert.Equal(trip, Assert.Single(await GetAsync(account, new DateOnly(2025, 8, 10), null)).Id);
        Assert.Empty(await GetAsync(account, new DateOnly(2024, 1, 1), new DateOnly(2024, 12, 31)));
        Assert.Equal(2, (await GetAsync(account)).Count);
    }

    private async Task<IReadOnlyList<MemoryPoint>> GetAsync(Guid account, DateOnly? from = null, DateOnly? to = null)
    {
        await using var scope = database.CreateScope();
        return await scope.ServiceProvider.GetRequiredService<IMemoryMapQuery>().GetAsync(account, from, to, Cancellation);
    }
}
