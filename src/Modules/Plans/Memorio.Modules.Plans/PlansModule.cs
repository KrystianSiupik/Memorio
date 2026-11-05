using Memorio.Modularity;
using Memorio.Modules.Plans.Api;
using Memorio.Modules.Plans.Application.Commands.Cycle;
using Memorio.Modules.Plans.Application.Commands.RecordEntitlement;
using Memorio.Modules.Plans.Application.Entitlements;
using Memorio.Modules.Plans.Application.Policies;
using Memorio.Modules.Plans.Application.Ports;
using Memorio.Modules.Plans.Application.Queries;
using Memorio.Modules.Plans.Contracts;
using Memorio.Modules.Plans.Domain.Cycle;
using Memorio.Modules.Plans.Domain.Entitlements;
using Memorio.Modules.Plans.Infrastructure;
using Memorio.Modules.Plans.Infrastructure.Persistence;
using Memorio.Modules.Plans.Infrastructure.Queries;
using Memorio.Modules.Plans.Infrastructure.Scheduling;
using Memorio.Modules.Plans.Infrastructure.Sources;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Wolverine.EntityFrameworkCore;

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
        services.AddDbContextWithWolverineIntegration<PlansDbContext>(database => database.UsePlansDatabase(connectionString));
        services.AddScoped<IEntitlementLedger, EntitlementLedger>();
        services.AddScoped<RecordEntitlementHandler>();
        services.AddScoped<PlanEntitlements>();
        services.AddScoped<IAlbumSeatLimit, AlbumSeatLimit>();
        services.AddScoped<IPlanCycles, PlanCycles>();
        services.AddScoped<IPlanStateQuery, PlanStateQuery>();
        services.TryAddScoped<ISubscriptionSource, NoSubscriptionSource>();
        services.AddScoped<AdvanceCycleHandler>();
        services.AddScoped<RefreshSubscriptionHandler>();
        services.AddScoped<KeepMaterialHandler>();
        services.AddScoped<ActivateAfterTakeoverHandler>();
        services.AddScoped<MarkAlbumOwnerlessHandler>();
        services.AddScoped<CloseCycleWhenAccountDeleted>();
        services.AddScoped<MarkOwnerlessWhenAlbumLostOwner>();
        services.AddScoped<ActivateWhenAlbumTakenOver>();
        services.AddScoped<RecordWarningDelivered>();
        services.AddScoped<HoldTrimWhenUndelivered>();
        services.AddScheduledJob<PlanCycleJob>(PlanCycleJob.JobName, PlanCycleJob.Cron);

        services.AddHealthChecks().AddCheck<PlansDatabaseHealthCheck>(PlansDatabaseHealthCheck.Name, tags: [HealthTags.Ready]);
        if (configuration.GetValue("Database:MigrateOnStartup", defaultValue: true))
        {
            services.AddHostedService<PlansDatabaseMigrator>();
        }
    }

    public void MapEndpoints(IEndpointRouteBuilder endpoints) => endpoints.MapPlanEndpoints();
}
