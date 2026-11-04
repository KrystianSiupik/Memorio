using Memorio.Modules.Plans.Infrastructure;

namespace Memorio.Modules.Plans.Tests;

/// <summary>MEM-E3-PLANY-01, D-261.</summary>
public sealed class AlbumSeatLimitTests
{
    [Fact]
    public async Task An_album_has_five_seats() =>
        Assert.Equal(5, await new AlbumSeatLimit().SeatsOfAsync(Guid.CreateVersion7(), TestContext.Current.CancellationToken));
}
