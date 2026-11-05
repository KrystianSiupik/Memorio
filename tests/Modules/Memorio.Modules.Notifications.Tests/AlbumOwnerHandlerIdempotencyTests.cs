using Memorio.Modules.Album.Contracts;
using Memorio.Modules.Notifications.Application.Policies;
using Memorio.Modules.Notifications.Application.Ports;
using Microsoft.Extensions.DependencyInjection;

namespace Memorio.Modules.Notifications.Tests;

/// <summary>SD6, P42 and C166: E167 delivered twice.</summary>
public sealed class AlbumTakenOverHandlerIdempotencyTests(NotificationsDatabase database)
{
    private static CancellationToken Cancellation => TestContext.Current.CancellationToken;

    [Fact]
    public async Task Everyone_but_the_new_owner_hears_once()
    {
        var (album, owner, other) = (Guid.CreateVersion7(), database.Neighbours.NewAccount(), database.Neighbours.NewAccount());
        database.Neighbours.Members(album, owner, other);
        var taken = new AlbumTakenOverEvent(Guid.CreateVersion7(), DateTimeOffset.UtcNow, album, owner, 4);
        await using var scope = database.CreateScope();
        var policy = scope.ServiceProvider.GetRequiredService<NotifyOfNewOwner>();

        await policy.HandleAsync(taken, Cancellation);
        await policy.HandleAsync(taken, Cancellation);

        Assert.Equal(1, database.Neighbours.SentTo(database.Neighbours.EmailOf(other), nameof(NotificationKind.NewOwner)));
        Assert.Equal(0, database.Neighbours.SentTo(database.Neighbours.EmailOf(owner), nameof(NotificationKind.NewOwner)));
    }
}

/// <summary>SD6: the successor took over, delivered twice.</summary>
public sealed class AlbumOwnerChangedHandlerIdempotencyTests(NotificationsDatabase database)
{
    private static CancellationToken Cancellation => TestContext.Current.CancellationToken;

    [Fact]
    public async Task The_others_hear_once()
    {
        var (album, owner, other) = (Guid.CreateVersion7(), database.Neighbours.NewAccount(), database.Neighbours.NewAccount());
        database.Neighbours.Members(album, owner, other);
        var changed = new AlbumOwnerChangedEvent(Guid.CreateVersion7(), DateTimeOffset.UtcNow, album, owner, 4);
        await using var scope = database.CreateScope();
        var policy = scope.ServiceProvider.GetRequiredService<NotifyOfNewOwner>();

        await policy.HandleAsync(changed, Cancellation);
        await policy.HandleAsync(changed, Cancellation);

        Assert.Equal(1, database.Neighbours.SentTo(database.Neighbours.EmailOf(other), nameof(NotificationKind.NewOwner)));
    }
}

/// <summary>SD6: E162 delivered twice.</summary>
public sealed class AlbumWithoutOwnerHandlerIdempotencyTests(NotificationsDatabase database)
{
    private static CancellationToken Cancellation => TestContext.Current.CancellationToken;

    [Fact]
    public async Task Everyone_left_hears_once_that_they_can_take_it_over()
    {
        var (album, person) = (Guid.CreateVersion7(), database.Neighbours.NewAccount());
        database.Neighbours.Members(album, person);
        var lost = new AlbumWithoutOwnerEvent(Guid.CreateVersion7(), DateTimeOffset.UtcNow, album, 4);
        await using var scope = database.CreateScope();
        var policy = scope.ServiceProvider.GetRequiredService<NotifyOfNewOwner>();

        await policy.HandleAsync(lost, Cancellation);
        await policy.HandleAsync(lost, Cancellation);

        Assert.Equal(1, database.Neighbours.SentTo(database.Neighbours.EmailOf(person), nameof(NotificationKind.AlbumWithoutOwner)));
        var (subject, body) = Memorio.Modules.Notifications.Application.NotificationTexts.Of(NotificationKind.AlbumWithoutOwner, "Rodzina", null);
        Assert.NotEmpty(subject);
        Assert.Contains("album „Rodzina”", body);
    }
}
