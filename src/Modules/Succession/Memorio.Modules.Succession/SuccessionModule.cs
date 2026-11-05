using Memorio.Modularity;
using Memorio.Modules.Succession.Api;
using Memorio.Modules.Succession.Application.Commands.Cases;
using Memorio.Modules.Succession.Application.Commands.NameSuccessor;
using Memorio.Modules.Succession.Application.Policies;
using Memorio.Modules.Succession.Application.Queries;
using Memorio.Modules.Succession.Contracts;
using Memorio.Modules.Succession.Domain.Cases;
using Memorio.Modules.Succession.Infrastructure;
using Memorio.Modules.Succession.Infrastructure.Persistence;
using Memorio.Modules.Succession.Infrastructure.Queries;
using Memorio.Modules.Succession.Infrastructure.Scheduling;
using Memorio.SharedKernel.Scheduling;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Wolverine.EntityFrameworkCore;

namespace Memorio.Modules.Succession;

/// <summary>Module Succession. Key question: Who takes over the album or the reading access when the owner is gone?</summary>
public sealed class SuccessionModule : IModule
{
    public string Name => "Succession";

    public void AddServices(IServiceCollection services, IConfiguration configuration)
    {
        services.AddOptions<SuccessionOptions>()
            .Bind(configuration.GetSection(SuccessionOptions.Section))
            .Validate(options => !string.IsNullOrWhiteSpace(options.ConnectionString), "Succession__ConnectionString is required.")
            .ValidateOnStart();

        // A missing value fails the validation above at startup; the placeholder only lets the host be built.
        var connectionString = configuration.GetSection(SuccessionOptions.Section).Get<SuccessionOptions>()?.ConnectionString;
        if (string.IsNullOrWhiteSpace(connectionString))
        {
            connectionString = "Host=unconfigured";
        }

        services.TryAddSingleton(TimeProvider.System);
        services.AddDbContextWithWolverineIntegration<SuccessionDbContext>(database => database.UseSuccessionDatabase(connectionString));
        services.AddScoped<ITakeoverCases, TakeoverCases>();
        services.AddScoped<TakeoverQueries>();
        services.AddScoped<ITakeoverQueries>(provider => provider.GetRequiredService<TakeoverQueries>());
        services.AddScoped<ITakeoverOffers>(provider => provider.GetRequiredService<TakeoverQueries>());
        services.AddScoped<ILegacyContacts>(provider => provider.GetRequiredService<TakeoverQueries>());
        services.AddScoped<NameSuccessorHandler>();
        services.AddScoped<StartCertificateUploadHandler>();
        services.AddScoped<ReportDeathHandler>();
        services.AddScoped<DecideReportHandler>();
        services.AddScoped<AnswerOfferHandler>();
        services.AddScoped<ExpireOfferHandler>();
        services.AddScoped<NameLegacyContactHandler>();
        services.AddScoped<ActOnConfirmedDeath>();
        services.AddScoped<OfferAlbumsWhenAccountDeleted>();
        services.AddScoped<ApplyCaseToAlbum>();

        // Through a factory: IDeadlines exists only in the worker, and the API host validates its services at startup.
        services.AddScoped(provider => new AnswerDeadline(provider.GetRequiredService<IDeadlines>()));
        services.AddScheduledJob<AnswerDeadlineJob>(AnswerDeadline.JobName);
        services.AddScheduledJob<OperatorDecisionsJob>(OperatorDecisionsJob.JobName, OperatorDecisionsJob.Cron);
        services.AddScheduledJob<SuccessionSweepJob>(SuccessionSweepJob.JobName, SuccessionSweepJob.Cron);

        services.AddHealthChecks().AddCheck<SuccessionDatabaseHealthCheck>(SuccessionDatabaseHealthCheck.Name, tags: [HealthTags.Ready]);
        if (configuration.GetValue("Database:MigrateOnStartup", defaultValue: true))
        {
            services.AddHostedService<SuccessionDatabaseMigrator>();
        }
    }

    public void MapEndpoints(IEndpointRouteBuilder endpoints) => endpoints.MapSuccessionEndpoints();
}
