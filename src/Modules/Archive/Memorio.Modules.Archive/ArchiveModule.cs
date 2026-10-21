using Memorio.Modularity;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Memorio.Modules.Archive;

/// <summary>Module Archive. Key question: What did this person record, where and when?</summary>
public sealed class ArchiveModule : IModule
{
    public string Name => "Archive";

    public void AddServices(IServiceCollection services, IConfiguration configuration)
    {
    }

    public void MapEndpoints(IEndpointRouteBuilder endpoints)
    {
    }
}
