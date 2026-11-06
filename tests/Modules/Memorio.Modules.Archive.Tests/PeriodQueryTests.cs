using Memorio.Modules.Archive.Application.Queries.Period;
using Memorio.Modules.Archive.Contracts;
using Microsoft.Extensions.DependencyInjection;

namespace Memorio.Modules.Archive.Tests;

/// <summary>R7 on a real database: the overlap of S2:C1, the order, and who sees what (ADR 0011).</summary>
public sealed class PeriodQueryTests(ArchiveDatabase database)
{
    private static CancellationToken Cancellation => TestContext.Current.CancellationToken;

    private static readonly DateOnly From = new(2026, 1, 1);
    private static readonly DateOnly To = new(2026, 12, 31);

    private async Task<IReadOnlyList<PeriodMemory>> ReadAsync(Guid account, DateOnly? from = null, DateOnly? to = null)
    {
        await using var scope = database.CreateScope();
        return await scope.ServiceProvider.GetRequiredService<IPeriodQuery>().GetAsync(account, from ?? From, to ?? To, Cancellation);
    }

    [Fact]
    public async Task What_touches_the_period_is_in_it_in_order_of_dates_then_of_adding()
    {
        var account = Guid.CreateVersion7();
        var spring = await database.AddMemoryAsync(account, dateFrom: new DateOnly(2026, 4, 1));
        var newYear = await database.AddRangeMemoryAsync(account, new DateOnly(2025, 12, 31), new DateOnly(2026, 1, 1));
        var sameDayFirst = await database.AddMemoryAsync(account, dateFrom: new DateOnly(2026, 6, 1));
        var sameDaySecond = await database.AddMemoryAsync(account, dateFrom: new DateOnly(2026, 6, 1));
        await database.AddMemoryAsync(account, dateFrom: new DateOnly(2025, 6, 1));

        var period = await ReadAsync(account);

        Assert.Equal([newYear.Id.Value, spring.Id.Value, sameDayFirst.Id.Value, sameDaySecond.Id.Value], period.Select(memory => memory.Id));
        Assert.Equal("2025-12-31", period[0].DateFrom);
        Assert.Equal("2026-01-01", period[0].DateTo);
        Assert.Contains(newYear.Id.Value, (await ReadAsync(account, new DateOnly(2025, 1, 1), new DateOnly(2025, 12, 31))).Select(memory => memory.Id));
    }

    [Fact]
    public async Task Only_ones_own_and_albums_one_takes_part_in_and_never_the_trash()
    {
        var (author, reader, viewer) = (Guid.CreateVersion7(), Guid.CreateVersion7(), Guid.CreateVersion7());
        var album = Guid.CreateVersion7();
        var shared = await database.AddMemoryAsync(author, dateFrom: new DateOnly(2026, 3, 1));
        var mine = await database.AddMemoryAsync(author, dateFrom: new DateOnly(2026, 3, 2));
        var trashed = await database.AddMemoryAsync(author, dateFrom: new DateOnly(2026, 3, 3));
        await database.TrashAsync(trashed.Id, author);
        await using (var scope = database.CreateScope())
        {
            await scope.ServiceProvider.GetRequiredService<IAlbumContent>().LinkAsync(shared.Id.Value, album, Cancellation);
        }

        database.Neighbours.TakesPart(reader, album);
        database.Neighbours.Sees(viewer, album);

        Assert.Equal([shared.Id.Value, mine.Id.Value], (await ReadAsync(author)).Select(memory => memory.Id));
        var read = Assert.Single(await ReadAsync(reader));
        Assert.Equal(shared.Id.Value, read.Id);
        Assert.False(read.IsMine);
        Assert.Empty(await ReadAsync(viewer));
        Assert.Empty(await ReadAsync(Guid.CreateVersion7()));
    }

    [Fact]
    public async Task The_first_three_photos_come_signed()
    {
        var account = Guid.CreateVersion7();
        var memory = await database.AddMemoryAsync(account, photos: 4, dateFrom: new DateOnly(2026, 5, 1));

        var read = Assert.Single(await ReadAsync(account));

        Assert.Equal(memory.Id.Value, read.Id);
        Assert.Equal(4, read.Photos);
        Assert.Equal(3, read.PhotoUrls.Count);
    }

    [Fact]
    public async Task For_an_album_only_what_is_ones_own()
    {
        var (author, other) = (Guid.CreateVersion7(), Guid.CreateVersion7());
        var mine = await database.AddMemoryAsync(author, dateFrom: new DateOnly(2026, 2, 1));
        var album = Guid.CreateVersion7();
        var theirs = await database.AddMemoryAsync(other, dateFrom: new DateOnly(2026, 2, 2));
        await using var scope = database.CreateScope();
        await scope.ServiceProvider.GetRequiredService<IAlbumContent>().LinkAsync(theirs.Id.Value, album, Cancellation);
        database.Neighbours.TakesPart(author, album);

        Assert.Equal([mine.Id.Value], await scope.ServiceProvider.GetRequiredService<IAlbumContent>().OwnInPeriodAsync(author, From, To, Cancellation));
    }
}
