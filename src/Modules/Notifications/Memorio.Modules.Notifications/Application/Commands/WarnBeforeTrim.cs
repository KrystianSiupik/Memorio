using Memorio.Modules.Account.Contracts;
using Memorio.Modules.Album.Contracts;
using Memorio.Modules.Notifications.Application.Ports;
using Memorio.Modules.Notifications.Contracts;

namespace Memorio.Modules.Notifications.Application.Commands;

/// <summary>C92. <paramref name="WhenText"/> says when in the message; the subject of what is sent stays <paramref name="DaysBefore"/>.</summary>
internal sealed record WarnBeforeTrim(Guid ResourceId, bool IsAlbum, int DaysBefore, int Round, string WhenText);

/// <summary>
/// Everyone it concerns gets their own message: the owner and each participant, or the person (S2:B1). Whoever got it
/// is not written to again, so a retry reaches only those it failed for. Someone whose account is gone needs no warning.
/// </summary>
internal sealed class WarnBeforeTrimHandler(IAlbumMessaging albums, IAccountDirectory directory, SendOnce send, ITrimWarningReports reports, TimeProvider clock)
{
    public async Task<bool> HandleAsync(WarnBeforeTrim command, CancellationToken cancellationToken)
    {
        IReadOnlyCollection<Guid> people = command.IsAlbum
            ? await albums.MembersOfAsync(command.ResourceId, cancellationToken)
            : [command.ResourceId];
        var kind = command.IsAlbum ? NotificationKind.TrimWarningAlbum : NotificationKind.TrimWarningAccount;
        var albumId = command.IsAlbum ? command.ResourceId : Guid.Empty;

        var delivered = true;
        foreach (var person in people)
        {
            if (await directory.EmailOfAsync(person, cancellationToken) is not { } email)
            {
                continue;
            }

            var outcome = await send.TrySendAsync(
                $"trim:{command.ResourceId}:{command.Round}:{command.DaysBefore}:{person}",
                command.Round,
                email,
                kind,
                albumId,
                command.WhenText,
                cancellationToken);
            delivered &= outcome == DeliveryOutcome.Delivered;
        }

        var now = clock.GetUtcNow();
        if (delivered)
        {
            await reports.PublishAsync(new TrimWarningDeliveredEvent(Guid.CreateVersion7(), now, command.ResourceId, command.IsAlbum, command.DaysBefore, command.Round), cancellationToken);
        }
        else
        {
            await reports.PublishAsync(new TrimWarningUndeliveredEvent(Guid.CreateVersion7(), now, command.ResourceId, command.IsAlbum, command.DaysBefore, command.Round), cancellationToken);
        }

        return delivered;
    }
}
