using Microsoft.AspNetCore.Diagnostics.HealthChecks;

namespace Memorio.Api.Health;

/// <summary>
/// RD4: <c>GET /health</c> answers while the process is alive (no dependency is checked, so a database
/// outage does not restart the container); <c>GET /ready</c> runs the checks tagged <see cref="ReadyTag"/>
/// (database, storage: added with their tickets) and answers 503 while any of them fails.
/// Both are outside /v1 and outside the API contract.
/// </summary>
internal static class HealthEndpoints
{
    public const string ReadyTag = "ready";

    public static IServiceCollection AddMemorioHealthChecks(this IServiceCollection services)
    {
        services.AddHealthChecks();
        return services;
    }

    public static IEndpointRouteBuilder MapMemorioHealthChecks(this IEndpointRouteBuilder endpoints)
    {
        endpoints.MapHealthChecks("/health", new HealthCheckOptions { Predicate = _ => false });
        endpoints.MapHealthChecks("/ready", new HealthCheckOptions { Predicate = check => check.Tags.Contains(ReadyTag) });
        return endpoints;
    }
}
