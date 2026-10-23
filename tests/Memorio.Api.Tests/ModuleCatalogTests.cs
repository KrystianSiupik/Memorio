using Memorio.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;

namespace Memorio.Api.Tests;

public sealed class ModuleCatalogTests
{
    [Fact]
    public void The_catalog_registers_the_14_modules_of_the_map_once_each()
    {
        var names = ModuleCatalog.All.Select(module => module.Name).Order(StringComparer.Ordinal);

        Assert.Equal(
            [
                "Access", "Account", "AiNarration", "Album", "Archive", "Limits", "Moderation",
                "Notifications", "PlaceProfiles", "Places", "Plans", "Provenance", "Publishing", "Succession",
            ],
            names);
    }

    [Fact]
    public async Task The_host_starts_with_every_module_registered()
    {
        await using var factory = new MemorioApiFactory();
        using var client = factory.CreateClient();

        // Starting and answering proves the modules registered their services and endpoints without
        // errors. An unknown address needs sign-in like any other (SD10), so it does not reveal what exists.
        using var response = await client.GetAsync(new Uri("/does-not-exist", UriKind.Relative), TestContext.Current.CancellationToken);

        Assert.Equal(System.Net.HttpStatusCode.Unauthorized, response.StatusCode);
    }
}
