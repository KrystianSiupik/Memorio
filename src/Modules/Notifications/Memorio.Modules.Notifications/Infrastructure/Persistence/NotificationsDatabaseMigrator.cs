using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Memorio.Modules.Notifications.Infrastructure.Persistence;

/// <summary>
/// Applies the module's migrations when a host starts (stage 1: one instance per host; EF Core locks
/// against concurrent runs). Off with Database__MigrateOnStartup=false, e.g. when deployment applies them.
/// </summary>
internal sealed class NotificationsDatabaseMigrator(IServiceScopeFactory scopes) : IHostedService
{
    public async Task StartAsync(CancellationToken cancellationToken)
    {
        await using var scope = scopes.CreateAsyncScope();
        await scope.ServiceProvider.GetRequiredService<NotificationsDbContext>().Database.MigrateAsync(cancellationToken);
    }

    public Task StopAsync(CancellationToken cancellationToken) => Task.CompletedTask;
}
