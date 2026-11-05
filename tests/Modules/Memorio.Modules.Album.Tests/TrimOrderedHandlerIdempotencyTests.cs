using Memorio.Modules.Album.Application.Commands.Contributions;
using Memorio.Modules.Album.Application.Policies;
using Memorio.Modules.Album.Domain.Albums;
using Memorio.Modules.Limits.Contracts;
using Memorio.Modules.Plans.Contracts;
using Microsoft.Extensions.DependencyInjection;

namespace Memorio.Modules.Album.Tests;

/// <summary>SD6 and D-290: E103 delivered twice, on a real database. A memory leaves the album and stays its author's.</summary>
public sealed class TrimOrderedHandlerIdempotencyTests(AlbumDatabase database)
{
    private static CancellationToken Cancellation => TestContext.Current.CancellationToken;

    internal static TrimOrderedEvent Order(AlbumId album, int sequence = 5) =>
        new(Guid.CreateVersion7(), DateTimeOffset.UtcNow, album.Value, true, 100, 0, [], sequence);

    internal static async Task DeliverAsync(AlbumDatabase database, TrimOrderedEvent order)
    {
        await using var scope = database.CreateScope();
        await scope.ServiceProvider.GetRequiredService<TrimAlbumWhenOrdered>().HandleAsync(order, Cancellation);
    }

    [Fact]
    public async Task What_is_chosen_to_go_leaves_the_album_once_and_the_rest_stays()
    {
        var (album, _, person) = await database.AlbumWithParticipantAsync();
        var (stays, leaves) = (await database.BringMemoryAsync(album, person), await database.BringMemoryAsync(album, person));
        database.Neighbours.TrimsAway(album.Value, leaves);
        var order = Order(album);

        await DeliverAsync(database, order);
        await DeliverAsync(database, order);

        Assert.Equal(stays, Assert.Single((await database.ViewAsync(album)).Memories).MemoryId);
        Assert.False(database.Neighbours.IsLinked(leaves, album.Value));
        Assert.True(database.Neighbours.IsLinked(stays, album.Value));
        Assert.False(database.Neighbours.Reserved.ContainsKey(PoolKeys.For(album.Value, leaves)));
    }

    [Fact]
    public async Task A_retry_finishes_the_same_list_after_the_choice_changed()
    {
        var (album, _, person) = await database.AlbumWithParticipantAsync();
        var leaves = await database.BringMemoryAsync(album, person);
        database.Neighbours.TrimsAway(album.Value, leaves);
        database.Neighbours.FailNextUnlink(leaves);
        var order = Order(album);

        await Assert.ThrowsAsync<InvalidOperationException>(() => DeliverAsync(database, order));
        database.Neighbours.TrimsAway(album.Value);
        await DeliverAsync(database, order);

        Assert.Empty((await database.ViewAsync(album)).Memories);
    }

    [Fact]
    public async Task An_order_about_an_account_is_not_for_this_module()
    {
        var (album, _, person) = await database.AlbumWithParticipantAsync();
        var memory = await database.BringMemoryAsync(album, person);
        database.Neighbours.TrimsAway(album.Value, memory);

        await DeliverAsync(database, Order(album) with { IsAlbum = false });

        Assert.Single((await database.ViewAsync(album)).Memories);
    }
}

/// <summary>SD6 and P44: E94 delivered twice: what left comes back within 30 days.</summary>
public sealed class PlanActivatedHandlerIdempotencyTests(AlbumDatabase database)
{
    private static CancellationToken Cancellation => TestContext.Current.CancellationToken;

    private async Task PayAsync(PlanActivatedEvent paid)
    {
        await using var scope = database.CreateScope();
        await scope.ServiceProvider.GetRequiredService<RestoreTrimmedWhenPlanActive>().HandleAsync(paid, Cancellation);
    }

    private static PlanActivatedEvent Paid(AlbumId album, int sequence = 6) =>
        new(Guid.CreateVersion7(), DateTimeOffset.UtcNow, album.Value, true, 1000, 1, sequence);

    private async Task<(AlbumId Album, Guid Person, Guid Memory)> TrimmedAsync()
    {
        var (album, _, person) = await database.AlbumWithParticipantAsync();
        var memory = await database.BringMemoryAsync(album, person);
        database.Neighbours.TrimsAway(album.Value, memory);
        await TrimOrderedHandlerIdempotencyTests.DeliverAsync(database, TrimOrderedHandlerIdempotencyTests.Order(album));
        return (album, person, memory);
    }

    [Fact]
    public async Task Paying_again_brings_it_back_once_even_above_the_limit()
    {
        var (album, _, memory) = await TrimmedAsync();
        database.Neighbours.PoolFull(album.Value);
        var paid = Paid(album);

        await PayAsync(paid);
        await PayAsync(paid);

        Assert.Equal(memory, Assert.Single((await database.ViewAsync(album)).Memories).MemoryId);
        Assert.True(database.Neighbours.IsLinked(memory, album.Value));
        Assert.True(database.Neighbours.Reserved.ContainsKey(PoolKeys.For(album.Value, memory)));
    }

    [Fact]
    public async Task A_late_activation_brings_nothing_back()
    {
        var (album, _, _) = await TrimmedAsync();

        await PayAsync(Paid(album, sequence: 4));

        Assert.Empty((await database.ViewAsync(album)).Memories);
    }

    [Fact]
    public async Task After_30_days_it_stays_with_its_author()
    {
        var (album, _, _) = await TrimmedAsync();
        await using (var connection = await database.OpenAsAsync(AlbumDatabase.Role))
        {
            await using var command = new Npgsql.NpgsqlCommand("UPDATE album.trimmed_memories SET trimmed_at = now() - interval '31 days' WHERE album_id = @album", connection);
            command.Parameters.AddWithValue("album", album.Value);
            await command.ExecuteNonQueryAsync(Cancellation);
        }

        await PayAsync(Paid(album));

        Assert.Empty((await database.ViewAsync(album)).Memories);
    }

    [Fact]
    public async Task What_its_author_can_no_longer_bring_does_not_come_back()
    {
        var (album, person, _) = await TrimmedAsync();
        await using (var scope = database.CreateScope())
        {
            Assert.True((await scope.ServiceProvider.GetRequiredService<LeaveAlbumHandler>().HandleAsync(new LeaveAlbum(album, person, true), Cancellation)).IsSuccess);
        }

        await PayAsync(Paid(album));

        Assert.Empty((await database.ViewAsync(album)).Memories);
    }
}
