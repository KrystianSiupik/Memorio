using Memorio.Modularity;
using Memorio.Modules.Access.Api;
using Memorio.Modules.Access.Application.Commands;
using Memorio.Modules.Access.Application.Policies;
using Memorio.Modules.Access.Application.Queries;
using Memorio.Modules.Access.Contracts;
using Memorio.Modules.Access.Domain.Audiences;
using Memorio.Modules.Access.Infrastructure;
using Memorio.Modules.Access.Infrastructure.Persistence;
using Memorio.Modules.Access.Infrastructure.Queries;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Wolverine.EntityFrameworkCore;

namespace Memorio.Modules.Access;

/// <summary>Module Access. Key question: May this person see this content now?</summary>
public sealed class AccessModule : IModule
{
    public string Name => "Access";

    public void AddServices(IServiceCollection services, IConfiguration configuration)
    {
        services.AddOptions<AccessOptions>()
            .Bind(configuration.GetSection(AccessOptions.Section))
            .Validate(options => !string.IsNullOrWhiteSpace(options.ConnectionString), "Access__ConnectionString is required.")
            .ValidateOnStart();

        // A missing value fails the validation above at startup; the placeholder only lets the host be built.
        var connectionString = configuration.GetSection(AccessOptions.Section).Get<AccessOptions>()?.ConnectionString;
        if (string.IsNullOrWhiteSpace(connectionString))
        {
            connectionString = "Host=unconfigured";
        }

        services.TryAddSingleton(TimeProvider.System);
        services.AddDbContextWithWolverineIntegration<AccessDbContext>(database => database.UseAccessDatabase(connectionString));
        services.AddScoped<AccessReader>();
        services.AddScoped<IAccessReader>(provider => provider.GetRequiredService<AccessReader>());
        services.AddScoped<IBlockList>(provider => provider.GetRequiredService<AccessReader>());
        services.AddScoped<IAudiences, Audiences>();
        services.AddScoped<IAudienceQueries, AudienceQueries>();
        services.AddScoped<IReadGrants, ReadGrants>();
        services.AddScoped<ShareByLinkHandler>();
        services.AddScoped<RevokeLinkHandler>();
        services.AddScoped<EnterByLinkHandler>();
        services.AddScoped<RevokeAccessHandler>();
        services.AddScoped<BlockUserHandler>();
        services.AddScoped<UnblockUserHandler>();
        services.AddScoped<ForgetDeletedAccount>();

        services.AddHealthChecks().AddCheck<AccessDatabaseHealthCheck>(AccessDatabaseHealthCheck.Name, tags: [HealthTags.Ready]);
        if (configuration.GetValue("Database:MigrateOnStartup", defaultValue: true))
        {
            services.AddHostedService<AccessDatabaseMigrator>();
        }
    }

    public void MapEndpoints(IEndpointRouteBuilder endpoints)
    {
        endpoints.MapSharedWithMeEndpoints();
        endpoints.MapAudienceEndpoints();
    }
}
