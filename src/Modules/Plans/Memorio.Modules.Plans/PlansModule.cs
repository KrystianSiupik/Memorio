using Memorio.Modularity;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Memorio.Modules.Plans;

/// <summary>Module Plans. Key question: What state is the plan of this account or album in now, and what follows from it?</summary>
public sealed class PlansModule : IModule
{
    public string Name => "Plans";

    public void AddServices(IServiceCollection services, IConfiguration configuration)
    {
    }

    public void MapEndpoints(IEndpointRouteBuilder endpoints)
    {
    }
}
