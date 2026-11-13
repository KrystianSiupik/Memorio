using Memorio.Modularity;
using Memorio.Modules.Places.Api;
using Memorio.Modules.Places.Application.Commands.AddOwnPlace;
using Memorio.Modules.Places.Application.Commands.ChoosePlace;
using Memorio.Modules.Places.Application.Queries.PlaceLookup;
using Memorio.Modules.Places.Application.Queries.PlaceSuggestions;
using Memorio.Modules.Places.Contracts;
using Memorio.Modules.Places.Domain.Places;
using Memorio.Modules.Places.Infrastructure;
using Memorio.Modules.Places.Infrastructure.Persistence;
using Memorio.Modules.Places.Infrastructure.Queries;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Memorio.Modules.Places;

/// <summary>Module Places. Key question: Which place in the world is it, and what does the author call it?</summary>
public sealed class PlacesModule : IModule
{
    public string Name => "Places";

    public void AddServices(IServiceCollection services, IConfiguration configuration)
    {
        services.AddOptions<PlacesOptions>()
            .Bind(configuration.GetSection(PlacesOptions.Section))
            .Validate(options => !string.IsNullOrWhiteSpace(options.ConnectionString), "Places__ConnectionString is required.")
            .ValidateOnStart();

        // A missing value fails the validation above at startup; the placeholder only lets the host be built.
        var connectionString = configuration.GetSection(PlacesOptions.Section).Get<PlacesOptions>()?.ConnectionString;
        if (string.IsNullOrWhiteSpace(connectionString))
        {
            connectionString = "Host=unconfigured";
        }

        services.TryAddSingleton(TimeProvider.System);
        services.AddDbContext<PlacesDbContext>(database => database.UsePlacesDatabase(connectionString));
        services.AddScoped<IPlaceSuggestionsQuery, PlaceSuggestionsQuery>();
        services.AddScoped<IPlaceLookup, PlaceLookup>();
        services.AddScoped<IPlaceReader, PlaceReader>();
        services.AddScoped<ChoosePlaceHandler>();
        services.AddScoped<IOwnPlaces, OwnPlaces>();
        services.AddScoped<AddOwnPlaceHandler>();

        services.AddHealthChecks().AddCheck<PlacesDatabaseHealthCheck>(PlacesDatabaseHealthCheck.Name, tags: [HealthTags.Ready]);
        if (configuration.GetValue("Database:MigrateOnStartup", defaultValue: true))
        {
            services.AddHostedService<PlacesDatabaseMigrator>();
        }
    }

    public void MapEndpoints(IEndpointRouteBuilder endpoints)
    {
        endpoints.MapPlacesEndpoints();
    }
}
