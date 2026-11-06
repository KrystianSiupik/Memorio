using Memorio.Modules.Album.Application.Commands.CreateAlbum;
using Memorio.Modules.Album.Domain.Albums;
using Memorio.SharedKernel.Results;
using Microsoft.Extensions.DependencyInjection;

namespace Memorio.Modules.Album.Tests;

/// <summary>C71 on a real database.</summary>
public sealed class CreateAlbumFromPeriodTests(AlbumDatabase database)
{
    private static readonly DateOnly From = new(2026, 1, 1);
    private static readonly DateOnly To = new(2026, 12, 31);

    private async Task<Result<PeriodAlbum>> CreateAsync(Guid id, Guid account)
    {
        await using var scope = database.CreateScope();
        return await scope.ServiceProvider.GetRequiredService<CreateAlbumFromPeriodHandler>().HandleAsync(
            new CreateAlbumFromPeriod(id, account, "Rok 2026", From, To),
            TestContext.Current.CancellationToken);
    }

    [Fact]
    public async Task The_own_memories_of_the_period_go_in_and_again_changes_nothing()
    {
        var account = database.Neighbours.NewAccount();
        var (first, second) = (database.Neighbours.NewMemory(account), database.Neighbours.NewMemory(account));
        database.Neighbours.InPeriod(account, first, second);
        var id = Guid.CreateVersion7();

        var created = await CreateAsync(id, account);
        var again = await CreateAsync(id, account);

        Assert.Equal(new PeriodAlbum(2, 0), created.Value);
        Assert.Equal(new PeriodAlbum(2, 0), again.Value);
        var view = await database.ViewAsync(new AlbumId(id));
        Assert.Equal("Rok 2026", view.Name);
        Assert.Equal(2, view.Memories.Count);
    }

    [Fact]
    public async Task What_does_not_fit_is_counted_and_the_album_is_still_there()
    {
        var account = database.Neighbours.NewAccount();
        var memory = database.Neighbours.NewMemory(account);
        database.Neighbours.InPeriod(account, memory);
        var id = Guid.CreateVersion7();
        database.Neighbours.WithFullPool(id);

        var created = await CreateAsync(id, account);

        Assert.Equal(new PeriodAlbum(0, 1), created.Value);
        Assert.Empty((await database.ViewAsync(new AlbumId(id))).Memories);
    }

    [Fact]
    public async Task An_empty_period_is_an_empty_album()
    {
        var account = database.Neighbours.NewAccount();

        Assert.Equal(new PeriodAlbum(0, 0), (await CreateAsync(Guid.CreateVersion7(), account)).Value);
    }

    [Fact]
    public async Task Someone_elses_album_id_is_refused()
    {
        var owner = database.Neighbours.NewAccount();
        var id = (await database.NewAlbumAsync(owner)).Value;

        Assert.Equal(AlbumErrors.IdTaken, (await CreateAsync(id, database.Neighbours.NewAccount())).Error);
    }
}
