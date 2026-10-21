using Memorio.Modularity;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Memorio.Modules.Limits;

/// <summary>Module Limits. Key question: Will this operation fit in the pool, and how much is left?</summary>
public sealed class LimitsModule : IModule
{
    public string Name => "Limits";

    public void AddServices(IServiceCollection services, IConfiguration configuration)
    {
    }

    public void MapEndpoints(IEndpointRouteBuilder endpoints)
    {
    }
}
