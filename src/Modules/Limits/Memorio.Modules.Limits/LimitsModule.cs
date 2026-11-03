using Memorio.Modularity;
using Memorio.Modules.Limits.Application.Commands.ReleaseUnits;
using Memorio.Modules.Limits.Application.Commands.ReserveUnits;
using Memorio.Modules.Limits.Contracts;
using Memorio.Modules.Limits.Domain.Pools;
using Memorio.Modules.Limits.Infrastructure;
using Memorio.Modules.Limits.Infrastructure.Persistence;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Memorio.Modules.Limits;

/// <summary>Module Limits. Key question: Will this operation fit in the pool, and how much is left?</summary>
public sealed class LimitsModule : IModule
{
    public string Name => "Limits";

    public void AddServices(IServiceCollection services, IConfiguration configuration)
    {
        services.AddOptions<LimitsOptions>()
            .Bind(configuration.GetSection(LimitsOptions.Section))
            .Validate(options => !string.IsNullOrWhiteSpace(options.ConnectionString), "Limits__ConnectionString is required.")
            .ValidateOnStart();

        // A missing value fails the validation above at startup; the placeholder only lets the host be built.
        var connectionString = configuration.GetSection(LimitsOptions.Section).Get<LimitsOptions>()?.ConnectionString;
        if (string.IsNullOrWhiteSpace(connectionString))
        {
            connectionString = "Host=unconfigured";
        }

        services.TryAddSingleton(TimeProvider.System);
        services.AddDbContext<LimitsDbContext>(database => database.UseLimitsDatabase(connectionString));
        services.AddScoped<IPools, Pools>();
        services.AddScoped<ReserveUnitsHandler>();
        services.AddScoped<ReleaseUnitsHandler>();
        services.AddScoped<ISpacePool, SpacePool>();
        services.AddScoped<IAiAllowance, AiAllowance>();

        services.AddHealthChecks().AddCheck<LimitsDatabaseHealthCheck>(LimitsDatabaseHealthCheck.Name, tags: [HealthTags.Ready]);
        if (configuration.GetValue("Database:MigrateOnStartup", defaultValue: true))
        {
            services.AddHostedService<LimitsDatabaseMigrator>();
        }
    }

    public void MapEndpoints(IEndpointRouteBuilder endpoints)
    {
    }
}
