using Memorio.Modularity;
using Memorio.Modules.Plans.Application.Commands.RecordEntitlement;
using Memorio.Modules.Plans.Application.Entitlements;
using Memorio.Modules.Plans.Domain.Entitlements;
using Memorio.Modules.Plans.Infrastructure;
using Memorio.Modules.Plans.Infrastructure.Persistence;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Memorio.Modules.Plans;

/// <summary>Module Plans. Key question: What state is the plan of this account or album in now, and what follows from it?</summary>
public sealed class PlansModule : IModule
{
    public string Name => "Plans";

    public void AddServices(IServiceCollection services, IConfiguration configuration)
    {
        services.AddOptions<PlansOptions>()
            .Bind(configuration.GetSection(PlansOptions.Section))
            .Validate(options => !string.IsNullOrWhiteSpace(options.ConnectionString), "Plans__ConnectionString is required.")
            .ValidateOnStart();

        // A missing value fails the validation above at startup; the placeholder only lets the host be built.
        var connectionString = configuration.GetSection(PlansOptions.Section).Get<PlansOptions>()?.ConnectionString;
        if (string.IsNullOrWhiteSpace(connectionString))
        {
            connectionString = "Host=unconfigured";
        }

        services.TryAddSingleton(TimeProvider.System);
        services.AddDbContext<PlansDbContext>(database => database.UsePlansDatabase(connectionString));
        services.AddScoped<IEntitlementLedger, EntitlementLedger>();
        services.AddScoped<RecordEntitlementHandler>();
        services.AddScoped<PlanEntitlements>();

        services.AddHealthChecks().AddCheck<PlansDatabaseHealthCheck>(PlansDatabaseHealthCheck.Name, tags: [HealthTags.Ready]);
        if (configuration.GetValue("Database:MigrateOnStartup", defaultValue: true))
        {
            services.AddHostedService<PlansDatabaseMigrator>();
        }
    }

    public void MapEndpoints(IEndpointRouteBuilder endpoints)
    {
    }
}
