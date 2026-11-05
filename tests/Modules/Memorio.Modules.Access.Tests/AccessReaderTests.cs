using Memorio.Modules.Access.Application.Commands;
using Memorio.Modules.Access.Contracts;
using Microsoft.Extensions.DependencyInjection;

namespace Memorio.Modules.Access.Tests;

/// <summary>The rules on a real database: being in the album, a place on the list, and a block over both (ADR 0011).</summary>
public sealed class AccessReaderTests(AccessDatabase database)
{
    private static CancellationToken Cancellation => TestContext.Current.CancellationToken;

    [Fact]
    public async Task Nobody_sees_anything_until_a_rule_grants_it()
    {
        await using var scope = database.CreateScope();
        var reader = scope.ServiceProvider.GetRequiredService<IAccessReader>();
        var (album, _) = database.Neighbours.NewAlbum();

        Assert.False(await reader.CanSeeAlbumAsync(Guid.CreateVersion7(), album, Cancellation));
        Assert.Empty(await reader.VisibleAlbumsAsync(Guid.CreateVersion7(), Cancellation));
    }

    [Fact]
    public async Task A_participant_sees_the_album_and_loses_it_at_the_very_next_query()
    {
        var (album, _) = database.Neighbours.NewAlbum();
        var person = database.Neighbours.NewParticipant(album);
        await using var scope = database.CreateScope();
        var reader = scope.ServiceProvider.GetRequiredService<IAccessReader>();

        Assert.True(await reader.CanSeeAlbumAsync(person, album, Cancellation));
        Assert.Equal([album], await reader.VisibleAlbumsAsync(person, Cancellation));

        database.Neighbours.Leaves(album, person);

        Assert.False(await reader.CanSeeAlbumAsync(person, album, Cancellation));
    }

    [Fact]
    public async Task Someone_let_in_through_a_link_sees_it_and_a_block_takes_it_away_at_once()
    {
        var (album, owner) = database.Neighbours.NewAlbum();
        var person = Guid.CreateVersion7();
        await using var scope = database.CreateScope();
        var token = (await scope.ServiceProvider.GetRequiredService<ShareByLinkHandler>().HandleAsync(album, owner, Cancellation)).Value;
        await scope.ServiceProvider.GetRequiredService<EnterByLinkHandler>().HandleAsync(token, person, Cancellation);
        var reader = scope.ServiceProvider.GetRequiredService<IAccessReader>();
        Assert.True(await reader.CanSeeAlbumAsync(person, album, Cancellation));
        Assert.Contains(album, await reader.VisibleAlbumsAsync(person, Cancellation));

        await scope.ServiceProvider.GetRequiredService<BlockUserHandler>().HandleAsync(album, owner, person, Cancellation);

        Assert.False(await reader.CanSeeAlbumAsync(person, album, Cancellation));
        Assert.DoesNotContain(album, await reader.VisibleAlbumsAsync(person, Cancellation));
    }

    [Fact]
    public async Task The_map_filter_has_albums_the_person_is_in_without_those_read_through_a_link_or_blocked()
    {
        var (mine, _) = database.Neighbours.NewAlbum();
        var person = database.Neighbours.NewParticipant(mine);
        var (read, readOwner) = database.Neighbours.NewAlbum();
        var (blocked, blockedOwner) = database.Neighbours.NewAlbum();
        database.Neighbours.AddParticipant(blocked, person);
        await using var scope = database.CreateScope();
        var token = (await scope.ServiceProvider.GetRequiredService<ShareByLinkHandler>().HandleAsync(read, readOwner, Cancellation)).Value;
        await scope.ServiceProvider.GetRequiredService<EnterByLinkHandler>().HandleAsync(token, person, Cancellation);
        await scope.ServiceProvider.GetRequiredService<BlockUserHandler>().HandleAsync(blocked, blockedOwner, person, Cancellation);
        var reader = scope.ServiceProvider.GetRequiredService<IAccessReader>();

        Assert.Equal([mine], await reader.AlbumsTakingPartInAsync(person, Cancellation));
        Assert.Contains(read, await reader.VisibleAlbumsAsync(person, Cancellation));
    }

    [Fact]
    public async Task A_block_wins_over_being_in_the_album_before_Album_has_removed_the_person()
    {
        var (album, owner) = database.Neighbours.NewAlbum();
        var person = database.Neighbours.NewParticipant(album);
        await using var scope = database.CreateScope();

        await scope.ServiceProvider.GetRequiredService<BlockUserHandler>().HandleAsync(album, owner, person, Cancellation);

        var reader = scope.ServiceProvider.GetRequiredService<IAccessReader>();
        Assert.False(await reader.CanSeeAlbumAsync(person, album, Cancellation));
        Assert.DoesNotContain(album, await reader.VisibleAlbumsAsync(person, Cancellation));
        Assert.True(await scope.ServiceProvider.GetRequiredService<IBlockList>().IsBlockedAsync(album, person, Cancellation));
    }
}
