using System.Net;
using Memorio.Api.Health;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace Memorio.Api.Tests;

/// <summary>RD4: liveness and readiness.</summary>
public sealed class HealthTests
{
    [Theory]
    [InlineData("/health")]
    [InlineData("/ready")]
    public async Task A_healthy_host_answers_200(string path)
    {
        await using var factory = new MemorioApiFactory();
        using var client = factory.CreateClient();

        using var response = await client.GetAsync(new Uri(path, UriKind.Relative), TestContext.Current.CancellationToken);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Fact]
    public async Task A_failing_dependency_makes_the_host_not_ready_but_still_alive()
    {
        await using var factory = new MemorioApiFactory();
        using var client = factory
            .WithWebHostBuilder(builder => builder.ConfigureTestServices(services =>
                services.AddHealthChecks().AddCheck(
                    "database",
                    () => HealthCheckResult.Unhealthy(),
                    tags: [HealthEndpoints.ReadyTag])))
            .CreateClient();

        using var ready = await client.GetAsync(new Uri("/ready", UriKind.Relative), TestContext.Current.CancellationToken);
        using var health = await client.GetAsync(new Uri("/health", UriKind.Relative), TestContext.Current.CancellationToken);

        Assert.Equal(HttpStatusCode.ServiceUnavailable, ready.StatusCode);
        Assert.Equal(HttpStatusCode.OK, health.StatusCode);
    }

    [Fact]
    public async Task Health_endpoints_ignore_the_app_version_check()
    {
        await using var factory = MemorioApiFactory.With("AppVersion:Minimum", "9.0.0");
        using var client = factory.CreateClient();
        using var request = new HttpRequestMessage(HttpMethod.Get, "/health");
        request.Headers.Add("X-App-Version", "0.0.1");

        using var response = await client.SendAsync(request, TestContext.Current.CancellationToken);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }
}
