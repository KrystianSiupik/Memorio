using Memorio.Modularity;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Memorio.Modules.Succession;

/// <summary>Module Succession. Key question: Who takes over the album or the reading access when the owner is gone?</summary>
public sealed class SuccessionModule : IModule
{
    public string Name => "Succession";

    public void AddServices(IServiceCollection services, IConfiguration configuration)
    {
    }

    public void MapEndpoints(IEndpointRouteBuilder endpoints)
    {
    }
}
