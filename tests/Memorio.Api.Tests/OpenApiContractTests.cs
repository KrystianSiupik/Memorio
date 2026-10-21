using System.Text.Json;
using System.Text.Json.Nodes;
using Microsoft.AspNetCore.Mvc.Testing;

namespace Memorio.Api.Tests;

/// <summary>
/// The committed contract api/openapi.v1.json must equal what the API serves. The Dart client is
/// generated from that file, and CI checks it for breaking changes (oasdiff).
/// After an intended API change, regenerate it with UPDATE_OPENAPI_SNAPSHOT=1 and review the diff.
/// </summary>
public sealed class OpenApiContractTests
{
    private static readonly JsonSerializerOptions Indented = new() { WriteIndented = true };

    [Fact]
    public async Task The_committed_contract_matches_the_api()
    {
        await using var factory = new WebApplicationFactory<Program>();
        using var client = factory.CreateClient();
        var served = await client.GetStringAsync(new Uri("/openapi/v1.json", UriKind.Relative), TestContext.Current.CancellationToken);
        var actual = JsonNode.Parse(served)!.ToJsonString(Indented).ReplaceLineEndings("\n") + "\n";
        var path = Path.Combine(RepositoryRoot(), "api", "openapi.v1.json");

        if (Environment.GetEnvironmentVariable("UPDATE_OPENAPI_SNAPSHOT") == "1")
        {
            Directory.CreateDirectory(Path.GetDirectoryName(path)!);
            await File.WriteAllTextAsync(path, actual, TestContext.Current.CancellationToken);
        }

        Assert.True(File.Exists(path), $"Missing {path}; generate it with UPDATE_OPENAPI_SNAPSHOT=1.");
        var committed = (await File.ReadAllTextAsync(path, TestContext.Current.CancellationToken)).ReplaceLineEndings("\n");
        Assert.True(
            committed == actual,
            "api/openapi.v1.json differs from the API. If the change is intended, run the tests with " +
            "UPDATE_OPENAPI_SNAPSHOT=1, regenerate the Dart client and review the diff.");
    }

    [Fact]
    public async Task Every_api_path_is_under_v1()
    {
        var contract = JsonNode.Parse(await File.ReadAllTextAsync(
            Path.Combine(RepositoryRoot(), "api", "openapi.v1.json"), TestContext.Current.CancellationToken))!;
        var paths = contract["paths"]!.AsObject().Select(path => path.Key).ToList();

        Assert.NotEmpty(paths);
        Assert.All(paths, path => Assert.StartsWith("/v1/", path, StringComparison.Ordinal));
    }

    private static string RepositoryRoot()
    {
        var directory = new DirectoryInfo(AppContext.BaseDirectory);
        while (directory is not null && !File.Exists(Path.Combine(directory.FullName, "Memorio.slnx")))
        {
            directory = directory.Parent;
        }

        return directory?.FullName ?? throw new InvalidOperationException("Memorio.slnx not found above the test output.");
    }
}
