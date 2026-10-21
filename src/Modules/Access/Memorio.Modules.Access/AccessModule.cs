using Memorio.Modularity;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Memorio.Modules.Access;

/// <summary>Module Access. Key question: May this person see this content now?</summary>
public sealed class AccessModule : IModule
{
    public string Name => "Access";

    public void AddServices(IServiceCollection services, IConfiguration configuration)
    {
    }

    public void MapEndpoints(IEndpointRouteBuilder endpoints)
    {
    }
}
