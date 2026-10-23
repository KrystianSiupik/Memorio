using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Memorio.Modules.Account.Infrastructure.Persistence;

/// <summary>
/// Applies the module's migrations when a host starts (stage 1: one instance per host; EF Core locks
/// against concurrent runs). Off with Database__MigrateOnStartup=false, e.g. when deployment applies them.
/// </summary>
internal sealed class AccountDatabaseMigrator(IServiceScopeFactory scopes) : IHostedService
{
    public async Task StartAsync(CancellationToken cancellationToken)
    {
        await using var scope = scopes.CreateAsyncScope();
        await scope.ServiceProvider.GetRequiredService<AccountDbContext>().Database.MigrateAsync(cancellationToken);
    }

    public Task StopAsync(CancellationToken cancellationToken) => Task.CompletedTask;
}
