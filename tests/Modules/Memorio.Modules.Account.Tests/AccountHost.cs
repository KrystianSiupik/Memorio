using Memorio.Modules.Account.Application.Ports;
using Memorio.Testing;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Memorio.Modules.Account.Tests;

/// <summary>
/// The Account module alone on the bootstrapped database, as the API hosts it: its role, its schema,
/// migrations at start, sign-in required by default. Google sign-in answers through <see cref="FakeGoogle"/>.
/// Needs Docker.
/// </summary>
public sealed class AccountHost : PostgresDatabase
{
    private WebApplication? _app;

    public HttpClient Client { get; private set; } = null!;

    protected override async Task OnBootstrappedAsync()
    {
        var builder = WebApplication.CreateBuilder();
        builder.WebHost.UseTestServer();
        builder.Configuration["Account:ConnectionString"] = ConnectionStringFor("memorio_account");
        builder.Services.AddAuthorizationBuilder()
            .SetFallbackPolicy(new AuthorizationPolicyBuilder().RequireAuthenticatedUser().Build());
        builder.Services.AddProblemDetails();

        var module = new AccountModule();
        module.AddServices(builder.Services, builder.Configuration);
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
