using Memorio.Modules.Album.Contracts;
using Memorio.Modules.Notifications.Application.Policies;
using Microsoft.Extensions.DependencyInjection;

namespace Memorio.Modules.Notifications.Tests;

/// <summary>SD6, MEM-E3-POWIADOMIENIA-05 and -10: E175 delivered twice on a real database.</summary>
public sealed class ParticipantTaggedHandlerIdempotencyTests(NotificationsDatabase database)
{
    private static CancellationToken Cancellation => TestContext.Current.CancellationToken;

    [Fact]
    public async Task The_tagged_person_hears_once()
    {
        var person = database.Neighbours.NewAccount();
        var tagged = new ParticipantTaggedEvent(Guid.CreateVersion7(), DateTimeOffset.UtcNow, Guid.CreateVersion7(), Guid.CreateVersion7(), person, 3);

        await DeliverAsync(tagged);
        await DeliverAsync(tagged);

        Assert.Equal(1, database.Neighbours.SentTo(database.Neighbours.EmailOf(person), "Tagged"));
    }

    private async Task DeliverAsync(ParticipantTaggedEvent tagged)
    {
        await using var scope = database.CreateScope();
        await scope.ServiceProvider.GetRequiredService<NotifyTagged>().HandleAsync(tagged, Cancellation);
    }
}
