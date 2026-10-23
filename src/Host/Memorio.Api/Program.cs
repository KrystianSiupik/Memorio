using Memorio.Api;
using Memorio.Api.AppVersion;
using Memorio.Hosting;
using Memorio.Hosting.Configuration;
using Memorio.Hosting.Health;
using Memorio.Hosting.Messaging;
using Microsoft.Extensions.Options;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddProblemDetails();
builder.Services.AddOpenApi(ApiDocument.Name, ApiDocument.Configure);
builder.Services.AddMemorioHealthChecks();
builder.Services.AddAppVersionPolicy(builder.Configuration);
builder.Services.AddMemorioMessaging(builder.Configuration, MessagingHost.Api, handlerAssemblies: []);

foreach (var module in ModuleCatalog.All)
{
    module.AddServices(builder.Services, builder.Configuration);
}

var app = builder.Build();

app.UseAppVersionPolicy();
app.MapOpenApi();
app.MapMemorioHealthChecks();

// Every public endpoint lives under /v1; changes to it must stay backward compatible (ADR 0017).
var v1 = app.MapGroup("/v1");
v1.MapAppVersionEndpoints();

foreach (var module in ModuleCatalog.All)
{
    module.MapEndpoints(v1);
}

try
{
    await app.StartAsync();
}
catch (OptionsValidationException exception)
{
    // RD1: refuse to start with invalid configuration; the log names the variables, never their values.
    ConfigurationErrors.Log(app.Logger, exception);
    throw;
}

await app.WaitForShutdownAsync();

/// <summary>Entry point, public for integration tests (WebApplicationFactory).</summary>
public partial class Program;
