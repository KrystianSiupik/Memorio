using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;
using Memorio.Hosting.Messaging;
using Memorio.Modules.Account.Application.Ports;
using Memorio.Modules.Album;
using Memorio.Modules.Archive;
using Memorio.Modules.Limits;
using Memorio.SharedKernel.Scheduling;
using Memorio.SharedKernel.Storage;
using Memorio.Testing;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Hosting;
using Npgsql;

[assembly: AssemblyFixture(typeof(Memorio.Modules.Account.Tests.AccountHost))]

namespace Memorio.Modules.Account.Tests;

/// <summary>
/// The Account module on the bootstrapped database, as the API hosts it: its role, its schema, migrations at
/// start, sign-in required by default; next to it the Archive and Album modules it asks, on their roles.
/// Google sign-in answers through <see cref="FakeGoogle"/>. Needs Docker.
/// </summary>
public sealed class AccountHost : PostgresDatabase
{
    private const string Password = "long-enough-password";

    private WebApplication? _app;
    private IHost? _worker;

    public HttpClient Client { get; private set; } = null!;

    /// <summary>The services of the API side, for running a command the way a request does.</summary>
    public IServiceProvider Api => _app!.Services;

    /// <summary>The events of this module the worker received.</summary>
    public RecordedEvents Recorded { get; } = new();

    /// <summary>The worker's scheduler: tests fire its deadlines themselves, 30 days are long.</summary>
    public FakeDeadlines Deadlines { get; } = new();

    /// <summary>The services of the worker side, for delivering an event or a firing the way it does.</summary>
    public IServiceProvider Worker => _worker!.Services;

    /// <summary>A new account, signed in: its id and access token.</summary>
    public async Task<(Guid AccountId, string AccessToken)> RegisterAsync()
    {
        var body = new { email = $"{Guid.NewGuid():N}@example.com", password = Password, declaresMinimumAge = true };
        using var response = await Client.PostAsJsonAsync(new Uri("/v1/auth/register", UriKind.Relative), body, Cancellation);
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var accessToken = (await response.Content.ReadFromJsonAsync<JsonElement>(Cancellation)).GetProperty("accessToken").GetString()!;

        using var me = await SendAsync(HttpMethod.Get, "/v1/account/me", accessToken);
        Assert.Equal(HttpStatusCode.OK, me.StatusCode);
        return ((await me.Content.ReadFromJsonAsync<JsonElement>(Cancellation)).GetProperty("accountId").GetGuid(), accessToken);
    }

    /// <summary>Opens the window for deleting the account and tells when it ends.</summary>
    public async Task<DateTimeOffset> RequestDeletionAsync(string accessToken)
    {
        using var response = await SendAsync(HttpMethod.Post, "/v1/account/deletion", accessToken, new { consentsToAlbumExpiry = true });
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        return (await response.Content.ReadFromJsonAsync<JsonElement>(Cancellation)).GetProperty("windowEndsAt").GetDateTimeOffset();
    }

    /// <summary>The window of this account ended yesterday, and its deadline never fired.</summary>
    public async Task BackdateDeletionWindowAsync(Guid accountId)
    {
        await using var account = await OpenAsAsync("memorio_account");
        await using var update = new NpgsqlCommand(
            "UPDATE account.account_deletions SET window_ends_at = now() - interval '1 day' WHERE account_id = @account",
            account);
        update.Parameters.AddWithValue("account", accountId);
        Assert.Equal(1, await update.ExecuteNonQueryAsync(Cancellation));
    }

    public async Task<HttpResponseMessage> SendAsync(HttpMethod method, string path, string accessToken, object? body = null)
    {
        using var request = new HttpRequestMessage(method, new Uri(path, UriKind.Relative));
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
        if (body is not null)
        {
            request.Content = JsonContent.Create(body);
        }

        return await Client.SendAsync(request, Cancellation);
    }

    /// <summary>A memory of the account, written the way the Archive module stores it.</summary>
    public async Task AddMemoryAsync(Guid accountId, int photos = 0)
    {
        await using var archive = await OpenAsAsync("memorio_archive");
        var memoryId = Guid.CreateVersion7();
        await using (var memory = new NpgsqlCommand("INSERT INTO archive.memories (id, author_account_id, created_at) VALUES (@id, @author, now())", archive))
        {
            memory.Parameters.AddWithValue("id", memoryId);
            memory.Parameters.AddWithValue("author", accountId);
            await memory.ExecuteNonQueryAsync(Cancellation);
        }

        for (var index = 0; index < photos; index++)
        {
            await using var photo = new NpgsqlCommand("INSERT INTO archive.photos (id, memory_id, storage_key) VALUES (@id, @memory, @key)", archive);
            photo.Parameters.AddWithValue("id", Guid.CreateVersion7());
            photo.Parameters.AddWithValue("memory", memoryId);
            photo.Parameters.AddWithValue("key", $"photos/{Guid.NewGuid():N}/original.jpg");
            await photo.ExecuteNonQueryAsync(Cancellation);
        }
    }

    /// <summary>An album, written the way the Album module stores it.</summary>
    public async Task AddAlbumAsync(Guid ownerAccountId, Guid? successorAccountId = null, params Guid[] participants)
    {
        await using var album = await OpenAsAsync("memorio_album");
        var albumId = Guid.CreateVersion7();
        await using (var insert = new NpgsqlCommand("INSERT INTO album.albums (id, owner_account_id, successor_account_id) VALUES (@id, @owner, @successor)", album))
        {
            insert.Parameters.AddWithValue("id", albumId);
            insert.Parameters.AddWithValue("owner", ownerAccountId);
            insert.Parameters.AddWithValue("successor", (object?)successorAccountId ?? DBNull.Value);
            await insert.ExecuteNonQueryAsync(Cancellation);
        }

        foreach (var participant in participants)
        {
            await using var insert = new NpgsqlCommand("INSERT INTO album.album_participants (album_id, account_id) VALUES (@album, @account)", album);
            insert.Parameters.AddWithValue("album", albumId);
            insert.Parameters.AddWithValue("account", participant);
            await insert.ExecuteNonQueryAsync(Cancellation);
        }
    }

    /// <summary>Polls <paramref name="condition"/> until it holds or a minute (QS7) passes.</summary>
    public static async Task<bool> EventuallyAsync(Func<Task<bool>> condition)
    {
        var deadline = DateTime.UtcNow + TimeSpan.FromMinutes(1);
        while (DateTime.UtcNow < deadline)
        {
            if (await condition())
            {
                return true;
            }

            await Task.Delay(200, Cancellation);
        }

        return await condition();
    }

    protected override async Task OnBootstrappedAsync()
    {
        var builder = WebApplication.CreateBuilder();
        builder.WebHost.UseTestServer();
        Configure(builder.Configuration);
        builder.Services.AddMemorioMessaging(builder.Configuration, MessagingHost.Api, []);
        builder.Services.AddAuthorizationBuilder()
            .SetFallbackPolicy(new AuthorizationPolicyBuilder().RequireAuthenticatedUser().Build());
        builder.Services.AddProblemDetails();

        var module = new AccountModule();
        module.AddServices(builder.Services, builder.Configuration);
        new ArchiveModule().AddServices(builder.Services, builder.Configuration);
        new AlbumModule().AddServices(builder.Services, builder.Configuration);
        new LimitsModule().AddServices(builder.Services, builder.Configuration);
        builder.Services.RemoveAll<IExternalIdentityVerifier>();
        builder.Services.AddSingleton<IExternalIdentityVerifier>(new FakeGoogle());

        _app = builder.Build();
        _app.UseAuthentication();
        _app.UseAuthorization();
        module.MapEndpoints(_app.MapGroup("/v1"));
        await _app.StartAsync();
        Client = _app.GetTestClient();

        // After the API: its start applies the migrations the worker's handlers read.
        var worker = Host.CreateApplicationBuilder();
        Configure(worker.Configuration);
        worker.Configuration["Database:MigrateOnStartup"] = "false";
        worker.Services.AddMemorioMessaging(
            worker.Configuration,
            MessagingHost.Worker,
            [typeof(AccountModule).Assembly, typeof(ArchiveModule).Assembly, typeof(AccountHost).Assembly]);
        worker.Services.AddSingleton(Recorded);
        worker.Services.AddSingleton<IDeadlines>(Deadlines);
        worker.Services.AddSingleton<IObjectStorage>(new UnusedOnThisRouteStorage());
        new AccountModule().AddServices(worker.Services, worker.Configuration);
        new ArchiveModule().AddServices(worker.Services, worker.Configuration);
        new AlbumModule().AddServices(worker.Services, worker.Configuration);
        new LimitsModule().AddServices(worker.Services, worker.Configuration);
        _worker = worker.Build();
        await _worker.StartAsync();
    }

    private void Configure(ConfigurationManager configuration)
    {
        configuration["Messaging:ConnectionString"] = ConnectionStringFor(MessagingRole);
        configuration["Account:ConnectionString"] = ConnectionStringFor("memorio_account");
        configuration["Archive:ConnectionString"] = ConnectionStringFor("memorio_archive");
        configuration["Album:ConnectionString"] = ConnectionStringFor("memorio_album");
        configuration["Limits:ConnectionString"] = ConnectionStringFor("memorio_limits");
        configuration["Access:ConnectionString"] = ConnectionStringFor("memorio_access");
        configuration["Succession:ConnectionString"] = ConnectionStringFor("memorio_succession");
        configuration["Notifications:ConnectionString"] = ConnectionStringFor("memorio_notifications");
    }

    protected override async Task OnDisposingAsync()
    {
        if (_worker is not null)
        {
            await _worker.StopAsync();
            _worker.Dispose();
        }

        if (_app is not null)
        {
            await _app.DisposeAsync();
        }
    }

    private static CancellationToken Cancellation => TestContext.Current.CancellationToken;

    /// <summary>Deleting a file succeeds; nothing else is asked of the storage here.</summary>
    private sealed class UnusedOnThisRouteStorage : IObjectStorage
    {
        public Task DeleteAsync(StorageKey key, CancellationToken cancellationToken) => Task.CompletedTask;

        public Task<IReadOnlyDictionary<StorageKey, SignedUrl>> SignReadsAsync(IReadOnlyCollection<StorageKey> keys, CancellationToken cancellationToken) =>
            throw new NotSupportedException();

        public Task<UploadSlot> CreateUploadAsync(string contentType, CancellationToken cancellationToken) =>
            throw new NotSupportedException();

        public Task<StoredObject?> PromoteUploadAsync(StorageKey upload, StorageKey destination, CancellationToken cancellationToken) =>
            throw new NotSupportedException();
    }

    /// <summary>Accepts tokens "subject;email" (email optional, always verified); anything else is refused.</summary>
    private sealed class FakeGoogle : IExternalIdentityVerifier
    {
        public ExternalProvider Provider => ExternalProvider.Google;

        public bool IsEnabled => true;

        public Task<ExternalIdentity?> VerifyAsync(string idToken, CancellationToken cancellationToken)
        {
            var parts = idToken.Split(';');
            return Task.FromResult(parts[0].StartsWith("google-", StringComparison.Ordinal)
                ? new ExternalIdentity(ExternalProvider.Google, parts[0], parts.ElementAtOrDefault(1), EmailVerified: true)
                : null);
        }
    }
}
