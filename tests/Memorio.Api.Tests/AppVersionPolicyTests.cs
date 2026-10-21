using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;

namespace Memorio.Api.Tests;

public sealed class AppVersionPolicyTests : IDisposable
{
    private const string PolicyPath = "/v1/app/version-policy";

    private readonly WebApplicationFactory<Program> _factory = new WebApplicationFactory<Program>()
        .WithWebHostBuilder(builder => builder.UseSetting("AppVersion:Minimum", "1.2.0"));

    public void Dispose() => _factory.Dispose();

    [Fact]
    public async Task The_policy_endpoint_returns_the_minimum_version()
    {
        using var client = _factory.CreateClient();

        var policy = await client.GetFromJsonAsync<JsonElement>(PolicyPath, TestContext.Current.CancellationToken);

        Assert.Equal("1.2.0", policy.GetProperty("minimumVersion").GetString());
    }

    [Theory]
    [InlineData("1.2.0")]
    [InlineData("1.2.1+40")]
    [InlineData("2.0.0")]
    public async Task An_app_at_or_above_the_minimum_is_served(string appVersion)
    {
        using var response = await SendAsync(appVersion);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Fact]
    public async Task A_request_without_the_header_is_served()
    {
        using var response = await SendAsync(appVersion: null);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Fact]
    public async Task An_app_below_the_minimum_must_update()
    {
        using var response = await SendAsync("1.1.9");

        Assert.Equal(HttpStatusCode.UpgradeRequired, response.StatusCode);
        Assert.Equal("application/problem+json", response.Content.Headers.ContentType?.MediaType);
        var problem = await response.Content.ReadFromJsonAsync<JsonElement>(TestContext.Current.CancellationToken);
        Assert.Equal("App.UpdateRequired", problem.GetProperty("code").GetString());
        Assert.Equal("1.2.0", problem.GetProperty("minimumVersion").GetString());
    }

    [Fact]
    public async Task A_malformed_version_header_is_rejected()
    {
        using var response = await SendAsync("latest");

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        var problem = await response.Content.ReadFromJsonAsync<JsonElement>(TestContext.Current.CancellationToken);
        Assert.Equal("App.InvalidVersionHeader", problem.GetProperty("code").GetString());
    }

    [Fact]
    public async Task Paths_outside_v1_are_not_checked()
    {
        using var client = _factory.CreateClient();
        using var request = new HttpRequestMessage(HttpMethod.Get, "/openapi/v1.json");
        request.Headers.Add("X-App-Version", "0.0.1");

        using var response = await client.SendAsync(request, TestContext.Current.CancellationToken);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Fact]
    public void A_malformed_minimum_stops_the_host_at_startup()
    {
        using var factory = new WebApplicationFactory<Program>()
            .WithWebHostBuilder(builder => builder.UseSetting("AppVersion:Minimum", "one"));

        var error = Assert.ThrowsAny<Exception>(() => factory.CreateClient());

        Assert.Contains("AppVersion:Minimum", error.ToString(), StringComparison.Ordinal);
    }

    private async Task<HttpResponseMessage> SendAsync(string? appVersion)
    {
        using var client = _factory.CreateClient();
        using var request = new HttpRequestMessage(HttpMethod.Get, PolicyPath);
        if (appVersion is not null)
        {
            request.Headers.Add("X-App-Version", appVersion);
        }

        return await client.SendAsync(request, TestContext.Current.CancellationToken);
    }
}
