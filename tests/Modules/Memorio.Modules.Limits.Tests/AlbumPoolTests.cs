using Memorio.Modules.Limits.Contracts;
using Microsoft.Extensions.DependencyInjection;

namespace Memorio.Modules.Limits.Tests;

/// <summary>MEM-E3-LIMITY-02 and -03 on a real database.</summary>
public sealed class AlbumPoolTests(LimitsDatabase database)
{
    private static CancellationToken Cancellation => TestContext.Current.CancellationToken;

    [Fact]
    public async Task A_pool_not_used_yet_is_empty_with_the_default_limit()
    {
        await using var scope = database.CreateScope();
        var pool = scope.ServiceProvider.GetRequiredService<IAlbumPool>();

        var state = await pool.GetStateAsync(Guid.CreateVersion7(), Cancellation);

        Assert.Equal(0, state.UsedBytes);
        Assert.Equal(state.LimitBytes, state.FreeBytes);
    }

    [Fact]
    public async Task A_reservation_counts_once_and_comes_back_once()
    {
        var album = Guid.CreateVersion7();
        var key = Guid.CreateVersion7();
        await using var scope = database.CreateScope();
        var pool = scope.ServiceProvider.GetRequiredService<IAlbumPool>();

        Assert.True(await pool.TryReserveAsync(album, key, 100, Cancellation));
        Assert.True(await pool.TryReserveAsync(album, key, 100, Cancellation));
        Assert.Equal(100, (await pool.GetStateAsync(album, Cancellation)).UsedBytes);

        await pool.ReleaseAsync(key, Cancellation);
        await pool.ReleaseAsync(key, Cancellation);
        Assert.Equal(0, (await pool.GetStateAsync(album, Cancellation)).UsedBytes);
    }

    [Fact]
    public async Task What_does_not_fit_is_refused_and_free_space_is_never_negative()
    {
        var album = await database.AccountWithLimitAsync(100);
        await using var scope = database.CreateScope();
        var pool = scope.ServiceProvider.GetRequiredService<IAlbumPool>();

        Assert.False(await pool.TryReserveAsync(album, Guid.CreateVersion7(), 101, Cancellation));

        Assert.Equal(100, (await pool.GetStateAsync(album, Cancellation)).FreeBytes);
        Assert.Equal(0, new AlbumPoolState(150, 100).FreeBytes);
    }
}
