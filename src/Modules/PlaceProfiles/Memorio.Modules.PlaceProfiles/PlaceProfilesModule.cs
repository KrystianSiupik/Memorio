using Memorio.Modularity;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Memorio.Modules.PlaceProfiles;

/// <summary>Module PlaceProfiles. Key question: Who officially represents this place and what does it publish about it? (after MVP)</summary>
public sealed class PlaceProfilesModule : IModule
{
    public string Name => "PlaceProfiles";

    public void AddServices(IServiceCollection services, IConfiguration configuration)
    {
    }

    public void MapEndpoints(IEndpointRouteBuilder endpoints)
    {
    }
}
