using Memorio.Modules.Notifications.Domain;
using Microsoft.EntityFrameworkCore;
using Npgsql;

namespace Memorio.Modules.Notifications.Infrastructure.Persistence;

internal sealed class NotificationLedger(NotificationsDbContext database) : INotificationLedger
{
    public Task<SentNotification?> FindAsync(string subject, CancellationToken cancellationToken) =>
        database.SentNotifications.FirstOrDefaultAsync(item => item.Subject == subject, cancellationToken);

    public void Add(SentNotification sent) => database.SentNotifications.Add(sent);

    public async Task BlockAsync(string email, DateTimeOffset now, CancellationToken cancellationToken)
    {
        var blocked = BlockedAddress.Create(email, now);
        if (await database.BlockedAddresses.AnyAsync(item => item.AddressHash == blocked.AddressHash, cancellationToken))
        {
            return;
        }

        database.BlockedAddresses.Add(blocked);
        try
        {
            await database.SaveChangesAsync(cancellationToken);
        }
        catch (DbUpdateException exception) when (exception.InnerException is PostgresException { SqlState: PostgresErrorCodes.UniqueViolation })
        {
            // The same request twice at once.
            database.ChangeTracker.Clear();
        }
    }

    public Task SaveAsync(CancellationToken cancellationToken) => database.SaveChangesAsync(cancellationToken);
}
