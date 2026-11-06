using Memorio.Modules.Access.Application.Commands;
using Memorio.Modules.Access.Application.Policies;
using Memorio.Modules.Access.Contracts;
using Memorio.Modules.Moderation.Contracts;
using Microsoft.Extensions.DependencyInjection;

namespace Memorio.Modules.Access.Tests;

/// <summary>C67 and the write port of Moderation on a real database: what is hidden, only its author sees (E161).</summary>
public sealed class HiddenScopeTests(AccessDatabase database)
{
    private static CancellationToken Cancellation => TestContext.Current.CancellationToken;

    private async Task<bool> HideAsync(Guid hold, Guid resource, Guid? author, int sequence = 1, string kind = ModeratedResources.Album)
    {
        await using var scope = database.CreateScope();
        return await scope.ServiceProvider.GetRequiredService<IContentHiding>().HideAsync(new HiddenContent(hold, kind, resource, author, sequence), Cancellation);
    }

    [Fact]
    public async Task A_hidden_album_is_gone_for_everyone_but_its_owner_and_back_when_the_hold_ends()
    {
        var (album, owner) = database.Neighbours.NewAlbum();
        var person = database.Neighbours.NewParticipant(album);
        var hold = Guid.CreateVersion7();

        Assert.True(await HideAsync(hold, album, owner));

        await using var read = database.CreateScope();
        var reader = read.ServiceProvider.GetRequiredService<IAccessReader>();
        Assert.False(await reader.CanSeeAlbumAsync(person, album, Cancellation));
        Assert.DoesNotContain(album, await reader.VisibleAlbumsAsync(person, Cancellation));
        Assert.DoesNotContain(album, await reader.AlbumsTakingPartInAsync(person, Cancellation));
        Assert.True(await reader.CanSeeAlbumAsync(owner, album, Cancellation));

        await using (var scope = database.CreateScope())
        {
            Assert.True(await scope.ServiceProvider.GetRequiredService<IContentHiding>().ReleaseAsync(hold, album, 2, Cancellation));
        }

        Assert.True(await reader.CanSeeAlbumAsync(person, album, Cancellation));
    }

    [Fact]
    public async Task A_memory_stays_hidden_while_any_hold_stands_and_only_for_others()
    {
        var (memory, author, other) = (Guid.CreateVersion7(), Guid.CreateVersion7(), Guid.CreateVersion7());
        var (first, second) = (Guid.CreateVersion7(), Guid.CreateVersion7());
        await HideAsync(first, memory, author, kind: ModeratedResources.Memory);
        await HideAsync(second, memory, author, kind: ModeratedResources.Memory);
        await using (var scope = database.CreateScope())
        {
            await scope.ServiceProvider.GetRequiredService<IContentHiding>().ReleaseAsync(first, memory, 2, Cancellation);
        }

        await using var read = database.CreateScope();
        var reader = read.ServiceProvider.GetRequiredService<IAccessReader>();
        Assert.Equal([memory], await reader.HiddenFromAsync(other, [memory, Guid.CreateVersion7()], Cancellation));
        Assert.Empty(await reader.HiddenFromAsync(author, [memory], Cancellation));
    }

    [Fact]
    public async Task A_late_or_repeated_hiding_does_not_undo_a_later_release()
    {
        var (memory, other) = (Guid.CreateVersion7(), Guid.CreateVersion7());
        var hold = Guid.CreateVersion7();
        await using (var scope = database.CreateScope())
        {
            await scope.ServiceProvider.GetRequiredService<IContentHiding>().ReleaseAsync(hold, memory, 3, Cancellation);
        }

        await HideAsync(hold, memory, Guid.CreateVersion7(), sequence: 2, kind: ModeratedResources.Memory);

        await using var read = database.CreateScope();
        Assert.Empty(await read.ServiceProvider.GetRequiredService<IAccessReader>().HiddenFromAsync(other, [memory], Cancellation));
    }

    [Theory]
    [InlineData("Photo")]
    [InlineData("")]
    public async Task What_is_not_a_scope_is_refused_without_a_trace(string kind)
    {
        var resource = Guid.CreateVersion7();
        await using var scope = database.CreateScope();
        var handler = scope.ServiceProvider.GetRequiredService<ScopeHandler>();

        Assert.Equal("Access.InvalidScope", (await handler.HideAsync(Guid.CreateVersion7(), kind, resource, null, 1, Cancellation)).Error.Code);
        Assert.Equal("Access.InvalidScope", (await handler.HideAsync(Guid.CreateVersion7(), ModeratedResources.Memory, resource, null, 0, Cancellation)).Error.Code);
        Assert.Equal("Access.InvalidScope", (await handler.HideAsync(Guid.Empty, ModeratedResources.Memory, resource, null, 1, Cancellation)).Error.Code);
        await using var command = database.Access.CreateCommand("SELECT count(*) FROM access.hidden_scopes WHERE resource_id = $1");
        command.Parameters.Add(new() { Value = resource });
        Assert.Equal(0L, await command.ExecuteScalarAsync(Cancellation));
    }
}

/// <summary>SD6 and P32: E113 delivered twice.</summary>
public sealed class ContentHiddenHandlerIdempotencyTests(AccessDatabase database)
{
    [Fact]
    public async Task Hidden_once_whatever_the_deliveries()
    {
        var (album, owner) = database.Neighbours.NewAlbum();
        var person = database.Neighbours.NewParticipant(album);
        var hidden = new ContentHiddenEvent(Guid.CreateVersion7(), DateTimeOffset.UtcNow, Guid.CreateVersion7(), ModeratedResources.Album, album, owner, 1);

        for (var delivery = 0; delivery < 2; delivery++)
        {
            await using var scope = database.CreateScope();
            await scope.ServiceProvider.GetRequiredService<HideWhenContentHidden>().HandleAsync(hidden, TestContext.Current.CancellationToken);
        }

        await using var read = database.CreateScope();
        Assert.False(await read.ServiceProvider.GetRequiredService<IAccessReader>().CanSeeAlbumAsync(person, album, TestContext.Current.CancellationToken));
        await using var command = database.Access.CreateCommand("SELECT count(*) FROM access.hidden_scopes WHERE resource_id = $1");
        command.Parameters.Add(new() { Value = album });
        Assert.Equal(1L, await command.ExecuteScalarAsync(TestContext.Current.CancellationToken));
    }
}

/// <summary>SD6: the end of a hold delivered twice, the second after a newer hiding of the same hold changes nothing.</summary>
public sealed class ContentRestoredHandlerIdempotencyTests(AccessDatabase database)
{
    [Fact]
    public async Task Restored_once_and_an_older_end_loses()
    {
        var (album, owner) = database.Neighbours.NewAlbum();
        var person = database.Neighbours.NewParticipant(album);
        var hold = Guid.CreateVersion7();
        var restored = new ContentRestoredEvent(Guid.CreateVersion7(), DateTimeOffset.UtcNow, hold, album, 2);

        await using (var scope = database.CreateScope())
        {
            var policy = scope.ServiceProvider.GetRequiredService<HideWhenContentHidden>();
            await policy.HandleAsync(new ContentHiddenEvent(Guid.CreateVersion7(), DateTimeOffset.UtcNow, hold, ModeratedResources.Album, album, owner, 1), TestContext.Current.CancellationToken);
            await policy.HandleAsync(restored, TestContext.Current.CancellationToken);
            await policy.HandleAsync(new ContentHiddenEvent(Guid.CreateVersion7(), DateTimeOffset.UtcNow, hold, ModeratedResources.Album, album, owner, 3), TestContext.Current.CancellationToken);
            await policy.HandleAsync(restored, TestContext.Current.CancellationToken);
        }

        await using var read = database.CreateScope();
        Assert.False(await read.ServiceProvider.GetRequiredService<IAccessReader>().CanSeeAlbumAsync(person, album, TestContext.Current.CancellationToken));
    }
}
