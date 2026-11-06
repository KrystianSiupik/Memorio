using Memorio.Modularity;
using Memorio.Modules.Moderation.Api;
using Memorio.Modules.Moderation.Application.Commands.Reports;
using Memorio.Modules.Moderation.Application.Queries;
using Memorio.Modules.Moderation.Contracts;
using Memorio.Modules.Moderation.Domain.Reports;
using Memorio.Modules.Moderation.Infrastructure;
using Memorio.Modules.Moderation.Infrastructure.Persistence;
using Memorio.Modules.Moderation.Infrastructure.Queries;
using Memorio.Modules.Moderation.Infrastructure.Scheduling;
using Memorio.SharedKernel.Scheduling;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Wolverine.EntityFrameworkCore;

namespace Memorio.Modules.Moderation;

/// <summary>Module Moderation. Key question: What do we do with this content after a report or an order, and by when?</summary>
public sealed class ModerationModule : IModule
{
    public string Name => "Moderation";

    public void AddServices(IServiceCollection services, IConfiguration configuration)
    {
        services.AddOptions<ModerationOptions>()
            .Bind(configuration.GetSection(ModerationOptions.Section))
            .Validate(options => !string.IsNullOrWhiteSpace(options.ConnectionString), "Moderation__ConnectionString is required.")
            .ValidateOnStart();

        // A missing value fails the validation above at startup; the placeholder only lets the host be built.
        var connectionString = configuration.GetSection(ModerationOptions.Section).Get<ModerationOptions>()?.ConnectionString;
        if (string.IsNullOrWhiteSpace(connectionString))
        {
            connectionString = "Host=unconfigured";
        }

        services.TryAddSingleton(TimeProvider.System);
        services.AddDbContextWithWolverineIntegration<ModerationDbContext>(database => database.UseModerationDatabase(connectionString));
        services.AddScoped<IReports, Reports>();
        services.AddScoped<ModerationQueries>();
        services.AddScoped<IModerationQueries>(provider => provider.GetRequiredService<ModerationQueries>());
        services.AddScoped<IReportNotices>(provider => provider.GetRequiredService<ModerationQueries>());
        services.AddScoped<SubmitReportHandler>();
        services.AddScoped<SubmitPublicReportHandler>();
        services.AddScoped<ReportDecisionHandler>();
        services.AddScheduledJob<OperatorActionsJob>(OperatorActionsJob.JobName, OperatorActionsJob.Cron);

        services.AddHealthChecks().AddCheck<ModerationDatabaseHealthCheck>(ModerationDatabaseHealthCheck.Name, tags: [HealthTags.Ready]);
        if (configuration.GetValue("Database:MigrateOnStartup", defaultValue: true))
        {
            services.AddHostedService<ModerationDatabaseMigrator>();
        }
    }

    public void MapEndpoints(IEndpointRouteBuilder endpoints) => endpoints.MapModerationEndpoints();

    public void MapPages(IEndpointRouteBuilder root) => root.MapPublicReportPage();
}
