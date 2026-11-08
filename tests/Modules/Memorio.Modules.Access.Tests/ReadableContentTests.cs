using Memorio.Modules.Access.Contracts;
using Memorio.Modules.Access.Infrastructure;

namespace Memorio.Modules.Access.Tests;

/// <summary>MEM-E6-DOSTEP-01: the read port for AiNarration answers only what the access reader answers.</summary>
public sealed class ReadableContentTests
{
    private static CancellationToken Cancellation => TestContext.Current.CancellationToken;

    private static readonly Guid Person = Guid.CreateVersion7();
    private static readonly Guid Album = Guid.CreateVersion7();

    [Fact]
    public async Task Of_a_visible_album_everything_but_what_Moderation_hid()
    {
        var shown = Guid.CreateVersion7();
        var hidden = Guid.CreateVersion7();
        var port = new ReadableContent(new Reader(sees: true, hidden: [hidden]));

        var readable = await port.ReadableAsync(Person, Album, [shown, hidden, shown], Cancellation);

        Assert.Equal([shown], readable);
    }

    [Fact]
    public async Task Nothing_of_an_album_the_person_cannot_see_or_that_is_hidden()
    {
        var memory = Guid.CreateVersion7();

        Assert.Empty(await new ReadableContent(new Reader(sees: false, hidden: [])).ReadableAsync(Person, Album, [memory], Cancellation));
        Assert.Empty(await new ReadableContent(new Reader(sees: true, hidden: [Album])).ReadableAsync(Person, Album, [memory], Cancellation));
        Assert.Empty(await new ReadableContent(new Reader(sees: true, hidden: [])).ReadableAsync(Person, Album, [], Cancellation));
    }

    private sealed class Reader(bool sees, IReadOnlyCollection<Guid> hidden) : IAccessReader
    {
        public Task<bool> CanSeeAlbumAsync(Guid personId, Guid albumId, CancellationToken cancellationToken) => Task.FromResult(sees && personId == Person && albumId == Album);

        public Task<IReadOnlyCollection<Guid>> VisibleAlbumsAsync(Guid personId, CancellationToken cancellationToken) => Task.FromResult<IReadOnlyCollection<Guid>>(sees ? [Album] : []);

        public Task<IReadOnlyCollection<Guid>> AlbumsTakingPartInAsync(Guid personId, CancellationToken cancellationToken) => Task.FromResult<IReadOnlyCollection<Guid>>([]);

        public Task<IReadOnlyCollection<Guid>> LegacyArchivesAsync(Guid personId, CancellationToken cancellationToken) => Task.FromResult<IReadOnlyCollection<Guid>>([]);

        public Task<IReadOnlyCollection<Guid>> HiddenFromAsync(Guid personId, IReadOnlyCollection<Guid> resourceIds, CancellationToken cancellationToken) =>
            Task.FromResult<IReadOnlyCollection<Guid>>([.. resourceIds.Where(hidden.Contains)]);
    }
}
