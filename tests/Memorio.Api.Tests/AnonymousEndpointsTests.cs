using System.Net;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace Memorio.Api.Tests;

/// <summary>
/// Fitness function SD10 (ADR 0010): every endpoint requires a signed-in account, except those on this
/// allowlist. Adding an anonymous endpoint means adding it here, in review.
/// </summary>
public sealed class AnonymousEndpointsTests
{
    // "*": any HTTP method (health checks).
    private static readonly string[] Allowlist =
    [
        "GET /account/delete",
        "* /health",
        "GET /openapi/{documentName}.json",
        "* /ready",
        "GET /v1/app/version-policy",
        "POST /v1/auth/external/{provider}",
        "POST /v1/auth/login",
        "POST /v1/auth/refresh",
        "POST /v1/auth/register",
    ];

    [Fact]
    public async Task Only_allowlisted_endpoints_are_anonymous()
    {
        await using var factory = new MemorioApiFactory();
        using var _ = factory.CreateClient();

        var anonymous = factory.Services.GetRequiredService<EndpointDataSource>().Endpoints
            .OfType<RouteEndpoint>()
            .Where(endpoint => endpoint.Metadata.GetMetadata<IAllowAnonymous>() is not null)
            .SelectMany(endpoint => (endpoint.Metadata.GetMetadata<IHttpMethodMetadata>()?.HttpMethods ?? ["*"])
                .Select(method => $"{method} /{endpoint.RoutePattern.RawText?.TrimStart('/')}"))
            .Order(StringComparer.Ordinal);

        Assert.Equal(Allowlist.Order(StringComparer.Ordinal), anonymous);
    }

    [Fact]
    public async Task Everything_else_requires_a_signed_in_account()
    {
        await using var factory = new MemorioApiFactory();
        using var _ = factory.CreateClient();

        var fallback = factory.Services.GetRequiredService<IOptions<AuthorizationOptions>>().Value.FallbackPolicy;

        Assert.NotNull(fallback);
        Assert.Contains(fallback.Requirements, requirement =>
            requirement is Microsoft.AspNetCore.Authorization.Infrastructure.DenyAnonymousAuthorizationRequirement);
    }

    [Fact]
    public async Task A_request_without_a_token_is_unauthorized()
    {
        await using var factory = new MemorioApiFactory();
        using var client = factory.CreateClient();

        using var response = await client.GetAsync(new Uri("/v1/account/me", UriKind.Relative), TestContext.Current.CancellationToken);

        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task The_account_deletion_page_is_public_and_explains_the_steps()
    {
        await using var factory = new MemorioApiFactory();
        using var client = factory.CreateClient();

        using var response = await client.GetAsync(new Uri("/account/delete", UriKind.Relative), TestContext.Current.CancellationToken);
        var page = await response.Content.ReadAsStringAsync(TestContext.Current.CancellationToken);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.Equal("text/html", response.Content.Headers.ContentType?.MediaType);
        Assert.Contains("Usuń konto", page, StringComparison.Ordinal);
        Assert.Contains("30 dni", page, StringComparison.Ordinal);
    }
}
