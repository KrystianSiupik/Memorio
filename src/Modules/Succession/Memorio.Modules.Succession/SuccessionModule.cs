using Memorio.Modularity;
using Memorio.Modules.Succession.Api;
using Memorio.Modules.Succession.Application.Commands.NameSuccessor;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Memorio.Modules.Succession;

/// <summary>Module Succession. Key question: Who takes over the album or the reading access when the owner is gone?</summary>
public sealed class SuccessionModule : IModule
{
    public string Name => "Succession";

    public void AddServices(IServiceCollection services, IConfiguration configuration) =>
        services.AddScoped<NameSuccessorHandler>();

    public void MapEndpoints(IEndpointRouteBuilder endpoints) => endpoints.MapSuccessionEndpoints();
}
