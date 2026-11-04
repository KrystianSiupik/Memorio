using Memorio.Modules.Album.Contracts;
using Memorio.Modules.Notifications.Application.Policies;
using Microsoft.Extensions.DependencyInjection;

namespace Memorio.Modules.Notifications.Tests;

/// <summary>SD6, MEM-E3-POWIADOMIENIA-04 and -09: E59 delivered twice and late on a real database.</summary>
public sealed class ParticipantRemovedHandlerIdempotencyTests(NotificationsDatabase database)
{
    private static CancellationToken Cancellation => TestContext.Current.CancellationToken;

    [Fact]
    public async Task The_person_is_asked_once_and_a_late_event_is_skipped()
    {
        var person = database.Neighbours.NewAccount();
        var album = Guid.CreateVersion7();
        var removed = new ParticipantRemovedEvent(Guid.CreateVersion7(), DateTimeOffset.UtcNow, album, person, 5);

        await DeliverAsync(removed);
        await DeliverAsync(removed);
        await DeliverAsync(new ParticipantRemovedEvent(Guid.CreateVersion7(), DateTimeOffset.UtcNow, album, person, 2));

        Assert.Equal(1, database.Neighbours.SentTo(database.Neighbours.EmailOf(person), "ChooseKeepOrTake"));
    }

    [Fact]
    public async Task A_deleted_account_gets_nothing()
    {
        await DeliverAsync(new ParticipantRemovedEvent(Guid.CreateVersion7(), DateTimeOffset.UtcNow, Guid.CreateVersion7(), Guid.CreateVersion7(), 5));
    }

    private async Task DeliverAsync(ParticipantRemovedEvent removed)
    {
        await using var scope = database.CreateScope();
        await scope.ServiceProvider.GetRequiredService<AskToChooseKeepOrTake>().HandleAsync(removed, Cancellation);
    }
}
