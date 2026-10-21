using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Memorio.Modularity;

/// <summary>
/// Entry point of a module (bounded context). The host registers every module through this
/// interface only; everything else in a module assembly stays internal.
/// </summary>
public interface IModule
{
    /// <summary>Stable module name, e.g. "Album".</summary>
    string Name { get; }

    void AddServices(IServiceCollection services, IConfiguration configuration);

    void MapEndpoints(IEndpointRouteBuilder endpoints);
}
