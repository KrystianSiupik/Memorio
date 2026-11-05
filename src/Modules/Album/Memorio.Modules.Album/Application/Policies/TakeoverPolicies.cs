using Memorio.Modules.Album.Contracts;
using Memorio.Modules.Album.Domain.Albums;
using Memorio.Modules.Plans.Contracts;
using Memorio.SharedKernel.Messaging;
using Memorio.SharedKernel.Results;
using Memorio.SharedKernel.Scheduling;
using Microsoft.Extensions.Logging;

namespace Memorio.Modules.Album.Application.Policies;

/// <summary>E165 starts the 30 minutes; a later reservation replaces the deadline under the same key.</summary>
internal sealed class TakeoverReservationDeadline(IDeadlines deadlines) : IIntegrationEventPolicy<TakeoverReservedEvent>
{
    public const string JobName = "Album.TakeoverReservation";

    public Task HandleAsync(TakeoverReservedEvent integrationEvent, CancellationToken cancellationToken) =>
        deadlines.ScheduleAsync(JobName, integrationEvent.AlbumId.ToString(), integrationEvent.ReservedUntil, null, cancellationToken);
}

/// <summary>P39 and C157: the time ended without a payment.</summary>
internal sealed class ExpireTakeoverReservation(IAlbums albums)
{
    public async Task HandleAsync(Guid albumId, DateTimeOffset at, CancellationToken cancellationToken)
    {
        (await albums.FindAsync(new AlbumId(albumId), cancellationToken))?.ExpireReservation(at);
        if (!await albums.SaveAsync(cancellationToken))
        {
            throw new InvalidOperationException("Another decision about this album got there first; the retry reads it again.");
        }
    }
}

/// <summary>
/// P40 and C156: whoever holds the reservation is the owner now. A payment without one is E168: nothing changes, and it
/// is logged, because someone paid for a plan they did not get (D-294).
/// </summary>
internal sealed partial class TakeOverWhenPaid(IAlbums albums, TimeProvider clock, ILogger<TakeOverWhenPaid> logger) : IIntegrationEventPolicy<PaidWhileOwnerlessEvent>
{
    public async Task HandleAsync(PaidWhileOwnerlessEvent integrationEvent, CancellationToken cancellationToken)
    {
        var album = await albums.FindAsync(new AlbumId(integrationEvent.AlbumId), cancellationToken);
        if (album?.IsOwnerless != true)
        {
            return;
        }

        if (album.TakeOver(clock.GetUtcNow()).IsFailure)
        {
            LogPaidWithoutReservation(logger, integrationEvent.AlbumId);
            return;
        }

        if (!await albums.SaveAsync(cancellationToken))
        {
            throw new InvalidOperationException("Another decision about this album got there first; the retry reads it again.");
        }
    }

    [LoggerMessage(Level = LogLevel.Warning, Message = "A plan of album {AlbumId} was paid without a running reservation: nobody took it over.")]
    private static partial void LogPaidWithoutReservation(ILogger logger, Guid albumId);
}
