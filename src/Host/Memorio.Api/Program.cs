using Memorio.Api;
using Memorio.Api.AppVersion;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddProblemDetails();
builder.Services.AddOpenApi(ApiDocument.Name, ApiDocument.Configure);
builder.Services.AddAppVersionPolicy(builder.Configuration);

foreach (var module in ModuleCatalog.All)
{
    module.AddServices(builder.Services, builder.Configuration);
}

var app = builder.Build();

app.UseAppVersionPolicy();
app.MapOpenApi();

// Every public endpoint lives under /v1; changes to it must stay backward compatible (ADR 0017).
var v1 = app.MapGroup("/v1");
v1.MapAppVersionEndpoints();

foreach (var module in ModuleCatalog.All)
{
    module.MapEndpoints(v1);
}

await app.RunAsync();

/// <summary>Entry point, public for integration tests (WebApplicationFactory).</summary>
public partial class Program;
