using Memorio.Api;

var builder = WebApplication.CreateBuilder(args);

foreach (var module in ModuleCatalog.All)
{
    module.AddServices(builder.Services, builder.Configuration);
}

var app = builder.Build();

foreach (var module in ModuleCatalog.All)
{
    module.MapEndpoints(app);
}

await app.RunAsync();

/// <summary>Entry point, public for integration tests (WebApplicationFactory).</summary>
public partial class Program;
