using Memorio.Modularity;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Memorio.Modules.Account;

/// <summary>Module Account. Key question: Is this person an active user, and should their account stay?</summary>
public sealed class AccountModule : IModule
{
    public string Name => "Account";

    public void AddServices(IServiceCollection services, IConfiguration configuration)
    {
    }

    public void MapEndpoints(IEndpointRouteBuilder endpoints)
    {
    }
}
