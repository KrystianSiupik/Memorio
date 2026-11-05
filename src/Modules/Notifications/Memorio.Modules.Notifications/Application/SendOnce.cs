using Memorio.Modules.Notifications.Application.Ports;
using Memorio.Modules.Notifications.Domain;

namespace Memorio.Modules.Notifications.Application;

/// <summary>
/// The one way a message leaves. The ledger is written first, so a repeated event sends nothing again and a late
/// one, older than what was applied for its subject, is skipped (D-013). A failed send stays unmarked and the
/// retried event finishes it: at most a repeated message, never a lost one.
/// </summary>
internal sealed class SendOnce(INotificationLedger ledger, INotificationSender sender, TimeProvider clock)
{
    public async Task SendAsync(string subject, long sequence, string email, NotificationKind kind, Guid albumId, CancellationToken cancellationToken, string? invitedBy = null)
    {
        var sent = await ledger.FindAsync(subject, cancellationToken);
        if (sent is null)
        {
            sent = SentNotification.Create(subject, sequence);
            ledger.Add(sent);
            await ledger.SaveAsync(cancellationToken);
        }
        else if (sent.Sequence > sequence || (sent.Sequence == sequence && sent.SentAt is not null))
        {
            return;
        }
        else if (sent.Sequence < sequence)
        {
            sent.MoveTo(sequence);
            await ledger.SaveAsync(cancellationToken);
        }

        await sender.SendAsync(email, kind, albumId, invitedBy, cancellationToken);
        sent.MarkSent(clock.GetUtcNow());
        await ledger.SaveAsync(cancellationToken);
    }
}
