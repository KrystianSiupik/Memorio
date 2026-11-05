using Memorio.Modules.Notifications.Application.Ports;
using Memorio.SharedKernel.Messaging;
using Wolverine.EntityFrameworkCore;
using Wolverine.Runtime;

namespace Memorio.Modules.Notifications.Infrastructure.Persistence;

internal sealed class TrimWarningReports(IDbContextOutbox<NotificationsDbContext> outbox) : ITrimWarningReports
{
    public async Task PublishAsync<TEvent>(TEvent report, CancellationToken cancellationToken)
        where TEvent : IIntegrationEvent
    {
        await outbox.PublishAsync(report);

        // Every save flushes (D-278): by default what a second save in a scope publishes is dropped.
        await outbox.SaveChangesAndFlushMessagesAsync(MultiFlushMode.AllowMultiples, cancellationToken);
    }
}
