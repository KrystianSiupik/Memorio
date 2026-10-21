using Memorio.Modularity;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Memorio.Modules.Publishing;

/// <summary>Module Publishing. Key question: What from this album may be shown publicly, and in what form? (after MVP)</summary>
public sealed class PublishingModule : IModule
{
    public string Name => "Publishing";

    public void AddServices(IServiceCollection services, IConfiguration configuration)
    {
    }

    public void MapEndpoints(IEndpointRouteBuilder endpoints)
    {
    }
}
