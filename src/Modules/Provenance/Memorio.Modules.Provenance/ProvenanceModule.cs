using Memorio.Modularity;
using Memorio.Modules.Provenance.Application.Commands.RegisterArtifact;
using Memorio.Modules.Provenance.Contracts;
using Memorio.Modules.Provenance.Domain.Artifacts;
using Memorio.Modules.Provenance.Infrastructure;
using Memorio.Modules.Provenance.Infrastructure.Persistence;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Wolverine.EntityFrameworkCore;

namespace Memorio.Modules.Provenance;

/// <summary>Module Provenance. Key question: What was this artifact made from, and may it still exist?</summary>
public sealed class ProvenanceModule : IModule
{
    public string Name => "Provenance";

    public void AddServices(IServiceCollection services, IConfiguration configuration)
    {
        services.AddOptions<ProvenanceOptions>()
            .Bind(configuration.GetSection(ProvenanceOptions.Section))
            .Validate(options => !string.IsNullOrWhiteSpace(options.ConnectionString), "Provenance__ConnectionString is required.")
            .ValidateOnStart();

        // A missing value fails the validation above at startup; the placeholder only lets the host be built.
        var connectionString = configuration.GetSection(ProvenanceOptions.Section).Get<ProvenanceOptions>()?.ConnectionString;
        if (string.IsNullOrWhiteSpace(connectionString))
        {
            connectionString = "Host=unconfigured";
        }

        services.TryAddSingleton(TimeProvider.System);
        services.AddDbContextWithWolverineIntegration<ProvenanceDbContext>(database => database.UseProvenanceDatabase(connectionString));
        services.AddScoped<IArtifacts, Artifacts>();
        services.AddScoped<RegisterArtifactHandler>();
        services.AddScoped<IArtifactRegistry, ArtifactRegistry>();

        services.AddHealthChecks().AddCheck<ProvenanceDatabaseHealthCheck>(ProvenanceDatabaseHealthCheck.Name, tags: [HealthTags.Ready]);
        if (configuration.GetValue("Database:MigrateOnStartup", defaultValue: true))
        {
            services.AddHostedService<ProvenanceDatabaseMigrator>();
        }
    }

    public void MapEndpoints(IEndpointRouteBuilder endpoints)
    {
    }
}
