using Memorio.Hosting.Messaging;
using Memorio.Modules.AiNarration.Application.Ports;
using Memorio.Modules.AiNarration.Infrastructure.Persistence;
using Memorio.Modules.Album.Contracts;
using Memorio.Modules.Limits.Contracts;
using Memorio.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Hosting;
using Npgsql;

[assembly: AssemblyFixture(typeof(Memorio.Modules.AiNarration.Tests.AiNarrationDatabase))]

namespace Memorio.Modules.AiNarration.Tests;

/// <summary>
/// The bootstrapped database with the module's migrations applied by its own role (as a host applies them),
/// and the module's services on it, registered by <see cref="AiNarrationModule"/> itself, with the outbox as in the API.
/// </summary>
public sealed class AiNarrationDatabase : PostgresDatabase
{
    public const string Role = "memorio_ai_narration";

    private IHost? _host;

    public AsyncServiceScope CreateScope() => _host!.Services.CreateAsyncScope();

    internal FakeAllowance Allowance { get; } = new();

    internal FakeProviderForDrafts Provider { get; } = new();

    internal FakeAlbums Albums { get; } = new();

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
        builder.Configuration["AiNarration:ConnectionString"] = ConnectionStringFor(Role);
        builder.Configuration["Messaging:ConnectionString"] = ConnectionStringFor(MessagingRole);
        builder.Configuration["Database:MigrateOnStartup"] = "false";
        builder.Services.AddMemorioMessaging(builder.Configuration, MessagingHost.Api, []);
        new AiNarrationModule().AddServices(builder.Services, builder.Configuration);
        builder.Services.AddSingleton<IAiAllowance>(Allowance);
        builder.Services.RemoveAll<ITextGenerator>();
        builder.Services.AddSingleton<ITextGenerator>(Provider);
        builder.Services.AddSingleton<IAlbumMembership>(Albums);
        _host = builder.Build();

        await using (var scope = CreateScope())
        {
            await scope.ServiceProvider.GetRequiredService<AiNarrationDbContext>().Database.MigrateAsync();
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

/// <summary>Who takes part in which album, in memory.</summary>
internal sealed class FakeAlbums : IAlbumMembership
{
    private readonly System.Collections.Concurrent.ConcurrentDictionary<(Guid Account, Guid Album), bool> _members = new();

    public void Join(Guid accountId, Guid albumId) => _members[(accountId, albumId)] = true;

    public void Leave(Guid accountId, Guid albumId) => _members.TryRemove((accountId, albumId), out _);

    public Task<bool> IsMemberAsync(Guid accountId, Guid albumId, CancellationToken cancellationToken) =>
        Task.FromResult(_members.ContainsKey((accountId, albumId)));

    public Task<Guid?> OwnerOfAsync(Guid albumId, CancellationToken cancellationToken) => Task.FromResult<Guid?>(null);

    public Task<IReadOnlyCollection<Guid>> AlbumsOfAsync(Guid accountId, CancellationToken cancellationToken) =>
        Task.FromResult<IReadOnlyCollection<Guid>>([.. _members.Keys.Where(key => key.Account == accountId).Select(key => key.Album)]);
}
