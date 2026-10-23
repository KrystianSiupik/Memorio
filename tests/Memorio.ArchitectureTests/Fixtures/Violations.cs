// Each root namespace breaks exactly one rule; ModuleRulesTests checks that only that rule fails.
using Memorio.ArchitectureTests.Fixtures.ContractsExposeInside.Modules.Alpha.Domain;
using Microsoft.AspNetCore.Http;

namespace Memorio.ArchitectureTests.Fixtures.CrossModule.Modules.Alpha.Domain
{
    internal static class UsesBetaInside
    {
        public static Beta.Domain.BetaThing Use(Beta.Domain.BetaThing thing) => thing;
    }
}

namespace Memorio.ArchitectureTests.Fixtures.CrossModule.Modules.Beta.Domain
{
    internal sealed record BetaThing(string Name);
}

namespace Memorio.ArchitectureTests.Fixtures.ContractsExposeInside.Modules.Alpha.Domain
{
    internal sealed record AlphaThing(string Name);
}

namespace Memorio.ArchitectureTests.Fixtures.ContractsExposeInside.Modules.Alpha.Contracts
{
    internal static class AlphaContract
    {
        public static AlphaThing Expose(AlphaThing thing) => thing;
    }
}

namespace Memorio.ArchitectureTests.Fixtures.DomainUsesInfrastructure.Modules.Alpha.Domain
{
    internal static class AlphaThing
    {
        public static Infrastructure.AlphaStore Use(Infrastructure.AlphaStore store) => store;
    }
}

namespace Memorio.ArchitectureTests.Fixtures.DomainUsesInfrastructure.Modules.Alpha.Infrastructure
{
    internal sealed record AlphaStore(string Name);
}

namespace Memorio.ArchitectureTests.Fixtures.ApplicationUsesApi.Modules.Alpha.Application
{
    internal static class CreateAlphaThing
    {
        public static Api.AlphaEndpoints Use(Api.AlphaEndpoints endpoints) => endpoints;
    }
}

namespace Memorio.ArchitectureTests.Fixtures.ApplicationUsesApi.Modules.Alpha.Api
{
    internal sealed record AlphaEndpoints(string Route);
}

namespace Memorio.ArchitectureTests.Fixtures.InfrastructureUsesApi.Modules.Alpha.Infrastructure
{
    internal static class AlphaStore
    {
        public static Api.AlphaEndpoints Use(Api.AlphaEndpoints endpoints) => endpoints;
    }
}

namespace Memorio.ArchitectureTests.Fixtures.InfrastructureUsesApi.Modules.Alpha.Api
{
    internal sealed record AlphaEndpoints(string Route);
}

namespace Memorio.ArchitectureTests.Fixtures.DomainUsesFramework.Modules.Alpha.Domain
{
    internal static class AlphaThing
    {
        public static HttpContext Use(HttpContext context) => context;
    }
}

namespace Memorio.ArchitectureTests.Fixtures.ApplicationUsesFramework.Modules.Alpha.Application
{
    internal static class CreateAlphaThing
    {
        public static HttpContext Use(HttpContext context) => context;
    }
}

namespace Memorio.ArchitectureTests.Fixtures.PublicInside.Modules.Alpha.Domain
{
    public sealed record AlphaThing(string Name);
}

namespace Memorio.ArchitectureTests.Fixtures.MessagingNotStaticHandler.Modules.Alpha.Infrastructure.Messaging
{
    public sealed class BetaInfoChangedListener(string prefix)
    {
        public string Handle(string beta) => prefix + beta;
    }
}
