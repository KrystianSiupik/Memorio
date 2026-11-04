using Memorio.Modules.Account.Contracts;
using Memorio.Modules.Album.Contracts;
using Memorio.Modules.Notifications.Application.Ports;
using Memorio.Modules.Notifications.Infrastructure.Persistence;
using Memorio.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

[assembly: AssemblyFixture(typeof(Memorio.Modules.Notifications.Tests.NotificationsDatabase))]

namespace Memorio.Modules.Notifications.Tests;

/// <summary>The bootstrapped database with the module's migrations applied by its own role; messages only recorded.</summary>
public sealed class NotificationsDatabase : PostgresDatabase
{
    public const string Role = "memorio_notifications";

    private ServiceProvider _services = null!;

    public FakeNotificationNeighbours Neighbours { get; } = new();

    public AsyncServiceScope CreateScope() => _services.CreateAsyncScope();

    protected override async Task OnBootstrappedAsync()
    {
        var configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["Notifications:ConnectionString"] = ConnectionStringFor(Role),
                ["Database:MigrateOnStartup"] = "false",
            })
            .Build();
        var services = new ServiceCollection();
        services.AddLogging();
        services.AddSingleton<INotificationSender>(Neighbours);
        new NotificationsModule().AddServices(services, configuration);
        services.AddSingleton<IAlbumMessaging>(Neighbours);
        services.AddSingleton<IAccountDirectory>(Neighbours);
        _services = services.BuildServiceProvider();

        await using var scope = CreateScope();
        await scope.ServiceProvider.GetRequiredService<NotificationsDbContext>().Database.MigrateAsync();
    }

    protected override async Task OnDisposingAsync() => await _services.DisposeAsync();
}
