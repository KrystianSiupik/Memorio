using Memorio.Hosting.Messaging;
using Memorio.Modules.Provenance.Infrastructure.Persistence;
using Memorio.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Npgsql;

[assembly: AssemblyFixture(typeof(Memorio.Modules.Provenance.Tests.ProvenanceTestDatabase))]

namespace Memorio.Modules.Provenance.Tests;

/// <summary>The bootstrapped database with the module's migrations applied by its own role, with the outbox as in the API.</summary>
public sealed class ProvenanceTestDatabase : PostgresDatabase
{
    public const string Role = "memorio_provenance";

    private IHost? _host;

    public AsyncServiceScope CreateScope() => _host!.Services.CreateAsyncScope();

    /// <summary>How many events about this id wait in the worker's queue: nothing consumes it here.</summary>
    public async Task<long> QueuedEventsAsync(string eventName, Guid id)
    {
        await using var messaging = await OpenAsAsync(MessagingRole);
        await using var command = new NpgsqlCommand(
            $"""
            SELECT count(*) FROM {MemorioMessaging.QueueSchema}.wolverine_queue_{MemorioMessaging.Queue}
            WHERE message_type LIKE @type AND position(convert_to(@id, 'UTF8') in body) > 0
            """,
            messaging);
        command.Parameters.AddWithValue("type", "%" + eventName + "%");
        command.Parameters.AddWithValue("id", id.ToString());
        return (long)(await command.ExecuteScalarAsync(TestContext.Current.CancellationToken))!;
    }

    protected override async Task OnBootstrappedAsync()
    {
        var builder = Host.CreateApplicationBuilder();
        builder.Configuration["Provenance:ConnectionString"] = ConnectionStringFor(Role);
        builder.Configuration["Messaging:ConnectionString"] = ConnectionStringFor(MessagingRole);
        builder.Configuration["Database:MigrateOnStartup"] = "false";
        builder.Services.AddMemorioMessaging(builder.Configuration, MessagingHost.Api, []);
        new ProvenanceModule().AddServices(builder.Services, builder.Configuration);
        _host = builder.Build();

        await using (var scope = CreateScope())
        {
            await scope.ServiceProvider.GetRequiredService<ProvenanceDbContext>().Database.MigrateAsync();
        }

        await _host.StartAsync();
    }

    protected override async Task OnDisposingAsync()
    {
        if (_host is not null)
        {
            await _host.StopAsync();
            _host.Dispose();
        }
    }
}
