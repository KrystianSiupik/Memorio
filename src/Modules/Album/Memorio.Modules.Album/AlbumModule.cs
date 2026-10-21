using Memorio.Modularity;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Memorio.Modules.Album;

/// <summary>Module Album. Key question: Who builds this shared collection together and what did each of them bring?</summary>
public sealed class AlbumModule : IModule
{
    public string Name => "Album";

    public void AddServices(IServiceCollection services, IConfiguration configuration)
    {
    }

    public void MapEndpoints(IEndpointRouteBuilder endpoints)
    {
    }
}
