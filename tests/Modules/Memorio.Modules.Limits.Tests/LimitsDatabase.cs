using Memorio.Modules.Limits.Infrastructure.Persistence;
using Memorio.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

[assembly: AssemblyFixture(typeof(Memorio.Modules.Limits.Tests.LimitsDatabase))]

namespace Memorio.Modules.Limits.Tests;

/// <summary>
/// The bootstrapped database with the module's migrations applied by its own role (as a host applies them),
/// and the module's services on it, registered by <see cref="LimitsModule"/> itself.
/// </summary>
public sealed class LimitsDatabase : PostgresDatabase
{
    public const string Role = "memorio_limits";

    private ServiceProvider _services = null!;

    public AsyncServiceScope CreateScope() => _services.CreateAsyncScope();

    protected override async Task OnBootstrappedAsync()
    {
        var configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["Limits:ConnectionString"] = ConnectionStringFor(Role),
                ["Database:MigrateOnStartup"] = "false",
            })
            .Build();
        var services = new ServiceCollection();
        new LimitsModule().AddServices(services, configuration);
        _services = services.BuildServiceProvider();

        await using var scope = CreateScope();
        await scope.ServiceProvider.GetRequiredService<LimitsDbContext>().Database.MigrateAsync();
    }

    protected override async Task OnDisposingAsync() => await _services.DisposeAsync();
}
