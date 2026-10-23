using Memorio.Modularity;
using Memorio.Modules.Account.Api;
using Memorio.Modules.Account.Application.Ports;
using Memorio.Modules.Account.Infrastructure;
using Memorio.Modules.Account.Infrastructure.Identity;
using Memorio.Modules.Account.Infrastructure.Metrics;
using Memorio.Modules.Account.Infrastructure.Persistence;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Hosting;

namespace Memorio.Modules.Account;

/// <summary>Module Account. Key question: Is this person an active user, and should their account stay?</summary>
public sealed class AccountModule : IModule
{
    public string Name => "Account";

    public void AddServices(IServiceCollection services, IConfiguration configuration)
    {
        services.AddOptions<AccountOptions>()
            .Bind(configuration.GetSection(AccountOptions.Section))
            .Validate(options => !string.IsNullOrWhiteSpace(options.ConnectionString), "Account__ConnectionString is required.")
            .ValidateOnStart();

        // A missing value fails the validation above at startup; the placeholder only lets the host be built.
        var options = configuration.GetSection(AccountOptions.Section).Get<AccountOptions>() ?? new AccountOptions();
        var connectionString = string.IsNullOrWhiteSpace(options.ConnectionString) ? "Host=unconfigured" : options.ConnectionString;

        services.TryAddSingleton(TimeProvider.System);
        services.AddDbContext<AccountDbContext>(database => database.UseAccountDatabase(connectionString));
        services.AddIdentityCore<AccountUser>(identity =>
            {
                identity.User.RequireUniqueEmail = true;

                // Length over character classes (D-148).
                identity.Password.RequiredLength = 10;
                identity.Password.RequireDigit = false;
                identity.Password.RequireLowercase = false;
                identity.Password.RequireUppercase = false;
                identity.Password.RequireNonAlphanumeric = false;
                identity.Lockout.MaxFailedAccessAttempts = 5;
                identity.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(15);
            })
            .AddEntityFrameworkStores<AccountDbContext>()
            .AddApiEndpoints();
        services.AddAuthentication(IdentityConstants.BearerScheme).AddBearerToken(IdentityConstants.BearerScheme);
        services.AddDataProtection().SetApplicationName("Memorio").PersistKeysToDbContext<AccountDbContext>();

        services.AddSingleton<IExternalIdentityVerifier>(OpenIdIdentityVerifier.ForProvider(ExternalProvider.Google, options.Google.ClientIds));
        services.AddSingleton<IExternalIdentityVerifier>(OpenIdIdentityVerifier.ForProvider(ExternalProvider.Apple, options.Apple.ClientIds));

        services.AddHealthChecks().AddCheck<AccountDatabaseHealthCheck>(AccountDatabaseHealthCheck.Name, tags: [HealthTags.Ready]);
        services.AddHostedService<AccountMetrics>();
        if (configuration.GetValue("Database:MigrateOnStartup", defaultValue: true))
        {
            // Before every other hosted service: Data Protection reads its keys from the account schema
            // as soon as it starts, so the tables must exist by then (hosted services start in order).
            services.Insert(0, ServiceDescriptor.Singleton<IHostedService, AccountDatabaseMigrator>());
        }
    }

    public void MapEndpoints(IEndpointRouteBuilder endpoints) => endpoints.MapAuthEndpoints();

    public void MapPages(IEndpointRouteBuilder root) => root.MapAccountDeletionPage();
}
