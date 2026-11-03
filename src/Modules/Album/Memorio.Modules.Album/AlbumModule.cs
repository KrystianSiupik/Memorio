using Memorio.Modularity;
using Memorio.Modules.Album.Contracts;
using Memorio.Modules.Album.Infrastructure;
using Memorio.Modules.Album.Infrastructure.Persistence;
using Memorio.Modules.Album.Infrastructure.Queries;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Memorio.Modules.Album;

/// <summary>Module Album. Key question: Who builds this shared collection together and what did each of them bring?</summary>
public sealed class AlbumModule : IModule
{
    public string Name => "Album";

    public void AddServices(IServiceCollection services, IConfiguration configuration)
    {
        services.AddOptions<AlbumOptions>()
            .Bind(configuration.GetSection(AlbumOptions.Section))
            .Validate(options => !string.IsNullOrWhiteSpace(options.ConnectionString), "Album__ConnectionString is required.")
            .ValidateOnStart();

        // A missing value fails the validation above at startup; the placeholder only lets the host be built.
        var connectionString = configuration.GetSection(AlbumOptions.Section).Get<AlbumOptions>()?.ConnectionString;
        if (string.IsNullOrWhiteSpace(connectionString))
        {
            connectionString = "Host=unconfigured";
        }

        services.TryAddSingleton(TimeProvider.System);
        services.AddDbContext<AlbumDbContext>(database => database.UseAlbumDatabase(connectionString));
        services.AddScoped<IAccountAlbumsReader, AccountAlbumsReader>();

        services.AddHealthChecks().AddCheck<AlbumDatabaseHealthCheck>(AlbumDatabaseHealthCheck.Name, tags: [HealthTags.Ready]);
        if (configuration.GetValue("Database:MigrateOnStartup", defaultValue: true))
        {
            services.AddHostedService<AlbumDatabaseMigrator>();
        }
    }

    public void MapEndpoints(IEndpointRouteBuilder endpoints)
    {
    }
}
