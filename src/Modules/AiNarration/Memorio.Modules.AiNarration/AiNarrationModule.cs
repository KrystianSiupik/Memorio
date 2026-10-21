using Memorio.Modularity;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Memorio.Modules.AiNarration;

/// <summary>Module AiNarration. Key question: Which story may be told from these materials without making anything up?</summary>
public sealed class AiNarrationModule : IModule
{
    public string Name => "AiNarration";

    public void AddServices(IServiceCollection services, IConfiguration configuration)
    {
    }

    public void MapEndpoints(IEndpointRouteBuilder endpoints)
    {
    }
}
