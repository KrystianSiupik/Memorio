using Memorio.Modularity;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;

namespace Memorio.Hosting.Health;

/// <summary>
/// RD4: <c>GET /health</c> answers while the process is alive (no dependency is checked, so a database
/// outage does not restart the container); <c>GET /ready</c> runs the checks tagged <see cref="ReadyTag"/>
/// (databases, bucket) and answers 503 while any of them fails.
/// Both are outside /v1 and outside the API contract.
/// </summary>
public static class HealthEndpoints
{
    public const string ReadyTag = HealthTags.Ready;

    public static IServiceCollection AddMemorioHealthChecks(this IServiceCollection services)
    {
        services.AddHealthChecks();
        return services;
    }

    public static IEndpointRouteBuilder MapMemorioHealthChecks(this IEndpointRouteBuilder endpoints)
    {
        // Anonymous on purpose (SD10 allowlist): probes and load balancers carry no token.
        endpoints.MapHealthChecks("/health", new HealthCheckOptions { Predicate = _ => false }).AllowAnonymous();
        endpoints.MapHealthChecks("/ready", new HealthCheckOptions { Predicate = check => check.Tags.Contains(ReadyTag) })
            .AllowAnonymous();
        return endpoints;
    }
}
