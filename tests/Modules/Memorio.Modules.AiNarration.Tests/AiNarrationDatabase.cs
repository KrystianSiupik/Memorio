using Memorio.Hosting.Messaging;
using Memorio.Modules.Access.Contracts;
using Memorio.Modules.AiNarration.Application.Ports;
using Memorio.Modules.AiNarration.Infrastructure.Persistence;
using Memorio.Modules.Album.Contracts;
using Memorio.Modules.Archive.Contracts;
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
        builder.Services.AddSingleton<IAlbumMemories>(Albums);
        builder.Services.AddSingleton<IReadableContent>(Albums);
        builder.Services.AddSingleton<IMemoryFacts>(Albums);
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

/// <summary>Who takes part in which album, what each album holds, who may read it and the facts of its memories, in memory.</summary>
internal sealed class FakeAlbums : IAlbumMembership, IAlbumMemories, IReadableContent, IMemoryFacts
{
    private readonly System.Collections.Concurrent.ConcurrentDictionary<(Guid Account, Guid Album), bool> _members = new();
    private readonly System.Collections.Concurrent.ConcurrentDictionary<Guid, (Guid Album, MemoryFact Fact)> _memories = new();
    private readonly System.Collections.Concurrent.ConcurrentDictionary<Guid, bool> _hidden = new();

    /// <summary>A memory of the album by this author; its author takes part.</summary>
    public MemoryFact Bring(Guid albumId, Guid authorAccountId, DateOnly date, string place = "Giewont", string? description = "W górach.")
    {
        Join(authorAccountId, albumId);
        var fact = new MemoryFact(Guid.CreateVersion7(), authorAccountId, place, "Day", date, date, null, description);
        _memories[fact.MemoryId] = (albumId, fact);
        return fact;
    }

    public void Hide(Guid memoryId) => _hidden[memoryId] = true;

    public void Delete(Guid memoryId) => _memories.TryRemove(memoryId, out _);

    public Task<IReadOnlyList<AlbumMemoryRef>> MemoriesOfAsync(Guid albumId, CancellationToken cancellationToken) =>
        Task.FromResult<IReadOnlyList<AlbumMemoryRef>>([.. _memories.Values.Where(item => item.Album == albumId).Select(item => new AlbumMemoryRef(item.Fact.MemoryId, item.Fact.AuthorAccountId))]);

    public Task<IReadOnlyCollection<Guid>> ReadableAsync(Guid personId, Guid albumId, IReadOnlyCollection<Guid> memoryIds, CancellationToken cancellationToken) =>
        Task.FromResult<IReadOnlyCollection<Guid>>(_members.ContainsKey((personId, albumId)) ? [.. memoryIds.Where(id => !_hidden.ContainsKey(id))] : []);

    public Task<IReadOnlyList<MemoryFact>> FactsAsync(IReadOnlyCollection<Guid> memoryIds, CancellationToken cancellationToken) =>
        Task.FromResult<IReadOnlyList<MemoryFact>>([.. memoryIds.Where(_memories.ContainsKey).Select(id => _memories[id].Fact).OrderBy(fact => fact.DateFrom)]);

    public void Join(Guid accountId, Guid albumId) => _members[(accountId, albumId)] = true;

    public void Leave(Guid accountId, Guid albumId) => _members.TryRemove((accountId, albumId), out _);

    public Task<bool> IsMemberAsync(Guid accountId, Guid albumId, CancellationToken cancellationToken) =>
        Task.FromResult(_members.ContainsKey((accountId, albumId)));

    public Task<Guid?> OwnerOfAsync(Guid albumId, CancellationToken cancellationToken) => Task.FromResult<Guid?>(null);

    public Task<IReadOnlyCollection<Guid>> AlbumsOfAsync(Guid accountId, CancellationToken cancellationToken) =>
        Task.FromResult<IReadOnlyCollection<Guid>>([.. _members.Keys.Where(key => key.Account == accountId).Select(key => key.Album)]);
}
