using Memorio.Modularity;
using Memorio.Modules.Account.Infrastructure.Persistence;
using Memorio.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Memorio.Modules.Account.Tests;

/// <summary>
/// D-373: the API and the worker start together and both migrate. On a fresh database the second one used to
/// fail with "column already exists" and stop. Its own container, because the fixture's database is migrated.
/// </summary>
public sealed class MigrationsAtOnceTests
{
    private static CancellationToken Cancellation => TestContext.Current.CancellationToken;

    [Fact]
    public async Task Two_hosts_migrating_a_fresh_database_at_once_both_start()
    {
        await using var database = new PostgresDatabase();
        await database.InitializeAsync();
        await using var api = Host(database);
        await using var worker = Host(database);

        await Task.WhenAll(MigrateAsync(api), MigrateAsync(worker));

        await using var scope = api.CreateAsyncScope();
        var context = scope.ServiceProvider.GetRequiredService<AccountDbContext>();
        Assert.Empty(await context.Database.GetPendingMigrationsAsync(Cancellation));
    }

    private static ServiceProvider Host(PostgresDatabase database)
    {
        var configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["Account:ConnectionString"] = database.ConnectionStringFor("memorio_account"),
                ["Database:MigrateOnStartup"] = "false",
            })
            .Build();
        var services = new ServiceCollection();
        services.AddLogging();
        new AccountModule().AddServices(services, configuration);
        return services.BuildServiceProvider();
    }

    private static async Task MigrateAsync(ServiceProvider host)
    {
        await using var scope = host.CreateAsyncScope();
        await scope.ServiceProvider.GetRequiredService<AccountDbContext>().Database.MigrateOneAtATimeAsync(Cancellation);
    }
}
