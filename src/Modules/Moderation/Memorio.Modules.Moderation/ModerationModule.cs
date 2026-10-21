using Memorio.Modularity;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Memorio.Modules.Moderation;

/// <summary>Module Moderation. Key question: What do we do with this content after a report or an order, and by when?</summary>
public sealed class ModerationModule : IModule
{
    public string Name => "Moderation";

    public void AddServices(IServiceCollection services, IConfiguration configuration)
    {
    }

    public void MapEndpoints(IEndpointRouteBuilder endpoints)
    {
    }
}
