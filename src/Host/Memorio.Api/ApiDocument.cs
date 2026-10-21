using Microsoft.AspNetCore.OpenApi;

namespace Memorio.Api;

/// <summary>
/// The OpenAPI document of API v1, served at /openapi/v1.json and committed as api/openapi.v1.json,
/// from which the Dart client is generated.
/// </summary>
internal static class ApiDocument
{
    public const string Name = "v1";

    public static void Configure(OpenApiOptions options) =>
        options.AddDocumentTransformer((document, _, _) =>
        {
            document.Info.Title = "Memorio API";
            document.Info.Version = "v1";
            // Servers depend on the host that serves the document; the committed contract must not.
            document.Servers?.Clear();
            return Task.CompletedTask;
        });
}
