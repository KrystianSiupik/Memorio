using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;
using Memorio.Modules.Account.Application.Ports;
using Memorio.Modules.Album;
using Memorio.Modules.Archive;
using Memorio.Testing;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
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

    public HttpClient Client { get; private set; } = null!;

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

    protected override async Task OnBootstrappedAsync()
    {
        var builder = WebApplication.CreateBuilder();
        builder.WebHost.UseTestServer();
        builder.Configuration["Account:ConnectionString"] = ConnectionStringFor("memorio_account");
        builder.Configuration["Archive:ConnectionString"] = ConnectionStringFor("memorio_archive");
        builder.Configuration["Album:ConnectionString"] = ConnectionStringFor("memorio_album");
        builder.Services.AddAuthorizationBuilder()
            .SetFallbackPolicy(new AuthorizationPolicyBuilder().RequireAuthenticatedUser().Build());
        builder.Services.AddProblemDetails();

        var module = new AccountModule();
        module.AddServices(builder.Services, builder.Configuration);
        new ArchiveModule().AddServices(builder.Services, builder.Configuration);
        new AlbumModule().AddServices(builder.Services, builder.Configuration);
        builder.Services.RemoveAll<IExternalIdentityVerifier>();
        builder.Services.AddSingleton<IExternalIdentityVerifier>(new FakeGoogle());

        _app = builder.Build();
        _app.UseAuthentication();
        _app.UseAuthorization();
        module.MapEndpoints(_app.MapGroup("/v1"));
        await _app.StartAsync();
        Client = _app.GetTestClient();
    }

    protected override async Task OnDisposingAsync()
    {
        if (_app is not null)
        {
            await _app.DisposeAsync();
        }
    }

    private static CancellationToken Cancellation => TestContext.Current.CancellationToken;

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
