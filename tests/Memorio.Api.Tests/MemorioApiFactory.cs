using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;

namespace Memorio.Api.Tests;

/// <summary>
/// The API host with the required configuration a test environment provides (as environment
/// variables would in a container). Tests change single values with <see cref="With"/>.
/// </summary>
internal sealed class MemorioApiFactory : WebApplicationFactory<Program>
{
    public static readonly IReadOnlyDictionary<string, string?> RequiredSettings = new Dictionary<string, string?>
    {
        ["AppVersion:Minimum"] = "0.1.0",
    };

    private readonly IReadOnlyDictionary<string, string?> _settings;

    public MemorioApiFactory()
        : this(RequiredSettings)
    {
    }

    private MemorioApiFactory(IReadOnlyDictionary<string, string?> settings)
    {
        _settings = settings;
    }

    /// <summary>A host with <paramref name="key"/> set to <paramref name="value"/>; null removes the setting.</summary>
    public static MemorioApiFactory With(string key, string? value)
    {
        var settings = new Dictionary<string, string?>(RequiredSettings) { [key] = value };
        if (value is null)
        {
            settings.Remove(key);
        }

        return new MemorioApiFactory(settings);
    }

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        foreach (var (key, value) in _settings)
        {
            builder.UseSetting(key, value);
        }
    }
}
