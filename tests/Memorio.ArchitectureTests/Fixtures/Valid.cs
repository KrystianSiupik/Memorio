// A correct module layout: every rule passes and every rule has something to check.
using Memorio.ArchitectureTests.Fixtures.Valid.Modules.Alpha.Application;
using Memorio.ArchitectureTests.Fixtures.Valid.Modules.Alpha.Domain;
using Memorio.ArchitectureTests.Fixtures.Valid.Modules.Beta.Contracts;
using Memorio.Modularity;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Memorio.ArchitectureTests.Fixtures.Valid.Modules.Alpha.Domain
{
    internal sealed record AlphaThing(string Name);
}

namespace Memorio.ArchitectureTests.Fixtures.Valid.Modules.Alpha.Application
{
    internal static class CreateAlphaThing
    {
        public static AlphaThing Handle(BetaInfo beta) => new(beta.Name);
    }
}

namespace Memorio.ArchitectureTests.Fixtures.Valid.Modules.Alpha.Infrastructure
{
    internal static class AlphaStore
    {
        public static AlphaThing Save(BetaInfo beta) => CreateAlphaThing.Handle(beta);
    }
}

namespace Memorio.ArchitectureTests.Fixtures.Valid.Modules.Alpha.Api
{
    internal static class AlphaEndpoints
    {
        public static AlphaThing Post(HttpContext context, BetaInfo beta)
        {
            ArgumentNullException.ThrowIfNull(context);
            return CreateAlphaThing.Handle(beta);
        }
    }
}

namespace Memorio.ArchitectureTests.Fixtures.Valid.Modules.Alpha
{
    public sealed class AlphaModule : IModule
    {
        public string Name => "Alpha";

        public void AddServices(IServiceCollection services, IConfiguration configuration)
        {
        }

        public void MapEndpoints(IEndpointRouteBuilder endpoints)
        {
        }
    }
}

namespace Memorio.ArchitectureTests.Fixtures.Valid.Modules.Beta
{
    public sealed class BetaModule : IModule
    {
        public string Name => "Beta";

        public void AddServices(IServiceCollection services, IConfiguration configuration)
        {
        }

        public void MapEndpoints(IEndpointRouteBuilder endpoints)
        {
        }
    }
}

namespace Memorio.ArchitectureTests.Fixtures.Valid.Modules.Beta.Contracts
{
    public sealed record BetaInfo(string Name);
}
