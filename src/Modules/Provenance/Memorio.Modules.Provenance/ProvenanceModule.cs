using Memorio.Modularity;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Memorio.Modules.Provenance;

/// <summary>Module Provenance. Key question: What was this artifact made from, and may it still exist?</summary>
public sealed class ProvenanceModule : IModule
{
    public string Name => "Provenance";

    public void AddServices(IServiceCollection services, IConfiguration configuration)
    {
    }

    public void MapEndpoints(IEndpointRouteBuilder endpoints)
    {
    }
}
