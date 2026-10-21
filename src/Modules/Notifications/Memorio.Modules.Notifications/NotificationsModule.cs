using Memorio.Modularity;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Memorio.Modules.Notifications;

/// <summary>Module Notifications. Key question: Did the message arrive, and whom must we not write to?</summary>
public sealed class NotificationsModule : IModule
{
    public string Name => "Notifications";

    public void AddServices(IServiceCollection services, IConfiguration configuration)
    {
    }

    public void MapEndpoints(IEndpointRouteBuilder endpoints)
    {
    }
}
