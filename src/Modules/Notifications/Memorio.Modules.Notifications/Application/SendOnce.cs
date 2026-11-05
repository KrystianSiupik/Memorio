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
    /// <summary>For messages nothing depends on: an address refused for good ends here.</summary>
    public async Task SendAsync(string subject, long sequence, string email, NotificationKind kind, Guid albumId, CancellationToken cancellationToken, string? detail = null)
    {
        if (await PrepareAsync(subject, sequence, cancellationToken) is not { } sent)
        {
            return;
        }

        await sender.SendAsync(email, kind, albumId, detail, cancellationToken);
        sent.MarkSent(clock.GetUtcNow());
        await ledger.SaveAsync(cancellationToken);
    }

    /// <summary>For messages something waits for: marked only when delivered, so a later try goes to whoever it failed for.</summary>
    public async Task<DeliveryOutcome> TrySendAsync(string subject, long sequence, string email, NotificationKind kind, Guid albumId, string? detail, CancellationToken cancellationToken)
    {
        if (await PrepareAsync(subject, sequence, cancellationToken) is not { } sent)
        {
            return DeliveryOutcome.Delivered;
        }

        var outcome = await sender.SendAsync(email, kind, albumId, detail, cancellationToken);
        if (outcome == DeliveryOutcome.Delivered)
        {
            sent.MarkSent(clock.GetUtcNow());
            await ledger.SaveAsync(cancellationToken);
        }

        return outcome;
    }

    /// <summary>Null when this message went out already or is late.</summary>
    private async Task<SentNotification?> PrepareAsync(string subject, long sequence, CancellationToken cancellationToken)
    {
        var sent = await ledger.FindAsync(subject, cancellationToken);
        if (sent is null)
        {
            sent = SentNotification.Create(subject, sequence);
            ledger.Add(sent);
            await ledger.SaveAsync(cancellationToken);
            return sent;
        }

        if (sent.Sequence > sequence || (sent.Sequence == sequence && sent.SentAt is not null))
        {
            return null;
        }

        if (sent.Sequence < sequence)
        {
            sent.MoveTo(sequence);
            await ledger.SaveAsync(cancellationToken);
        }

        return sent;
    }
}
