using Memorio.Hosting;
using Memorio.Hosting.Configuration;
using Memorio.Hosting.Health;
using Memorio.Hosting.Messaging;
using Memorio.Hosting.Scheduling;
using Memorio.Hosting.Storage;
using Microsoft.Extensions.Options;

// Worker (D-036, ADR 0006, ADR 0012): delivers integration events from the outbox to the modules' handlers
// and runs their scheduled jobs (Quartz.NET, clustered).
// A separate container from the API; it serves only /health and /ready, no /v1 endpoints.
var builder = WebApplication.CreateBuilder(args);

builder.Services.AddMemorioHealthChecks();
builder.Services.AddMemorioMessaging(
    builder.Configuration,
    MessagingHost.Worker,
    ModuleCatalog.All.Select(module => module.GetType().Assembly));
builder.Services.AddMemorioObjectStorage(builder.Configuration);
builder.Services.AddMemorioScheduling(builder.Configuration);

foreach (var module in ModuleCatalog.All)
{
    module.AddServices(builder.Services, builder.Configuration);
}

var app = builder.Build();

app.MapMemorioHealthChecks();

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
