namespace Memorio.Modules.Notifications.Domain;

internal interface INotificationLedger
{
    Task<SentNotification?> FindAsync(string subject, CancellationToken cancellationToken);

    void Add(SentNotification sent);

    /// <summary>Asking again changes nothing.</summary>
    Task BlockAsync(string email, DateTimeOffset now, CancellationToken cancellationToken);

    Task SaveAsync(CancellationToken cancellationToken);
}
