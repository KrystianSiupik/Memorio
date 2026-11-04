using Memorio.Modules.Access.Contracts;
using Memorio.Modules.Album.Contracts;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Memorio.Modules.Access.Tests;

/// <summary>The one rule so far: being in the album, as Album says at this moment (ADR 0011, D-250).</summary>
public sealed class AccessReaderTests
{
    private static CancellationToken Cancellation => TestContext.Current.CancellationToken;

    [Fact]
    public async Task Nobody_sees_anything_until_a_rule_grants_it()
    {
        var reader = ReaderOver(new Members());
        await using var scope = reader;

        Assert.False(await scope.Reader.CanSeeAlbumAsync(Guid.NewGuid(), Guid.NewGuid(), Cancellation));
        Assert.Empty(await scope.Reader.VisibleAlbumsAsync(Guid.NewGuid(), Cancellation));
    }

    [Fact]
    public async Task A_member_sees_the_album_and_loses_it_at_the_very_next_query()
    {
        var members = new Members();
        var person = Guid.NewGuid();
        var album = Guid.NewGuid();
        members.Albums.Add((person, album));
        var reader = ReaderOver(members);
        await using var scope = reader;

        Assert.True(await scope.Reader.CanSeeAlbumAsync(person, album, Cancellation));
        Assert.Equal([album], await scope.Reader.VisibleAlbumsAsync(person, Cancellation));
        Assert.False(await scope.Reader.CanSeeAlbumAsync(Guid.NewGuid(), album, Cancellation));

        members.Albums.Clear();

        Assert.False(await scope.Reader.CanSeeAlbumAsync(person, album, Cancellation));
    }

    private static ReaderScope ReaderOver(Members members)
    {
        var services = new ServiceCollection();
        new AccessModule().AddServices(services, new ConfigurationBuilder().Build());
        services.AddSingleton<IAlbumMembership>(members);
        var provider = services.BuildServiceProvider();
        var scope = provider.CreateAsyncScope();
        return new ReaderScope(provider, scope, scope.ServiceProvider.GetRequiredService<IAccessReader>());
    }

    private sealed record ReaderScope(ServiceProvider Provider, AsyncServiceScope Scope, IAccessReader Reader) : IAsyncDisposable
    {
        public async ValueTask DisposeAsync()
        {
            await Scope.DisposeAsync();
            await Provider.DisposeAsync();
        }
    }

    private sealed class Members : IAlbumMembership
    {
        public List<(Guid Person, Guid Album)> Albums { get; } = [];

        public Task<bool> IsMemberAsync(Guid accountId, Guid albumId, CancellationToken cancellationToken) =>
            Task.FromResult(Albums.Contains((accountId, albumId)));

        public Task<IReadOnlyCollection<Guid>> AlbumsOfAsync(Guid accountId, CancellationToken cancellationToken) =>
            Task.FromResult<IReadOnlyCollection<Guid>>([.. Albums.Where(item => item.Person == accountId).Select(item => item.Album)]);
    }
}
