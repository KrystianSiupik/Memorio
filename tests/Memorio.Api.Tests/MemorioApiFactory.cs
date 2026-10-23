using Memorio.Hosting.Messaging;
using Memorio.Hosting.Storage;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Wolverine;

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
        // Not contacted: the factory runs Wolverine without persistence or transports.
        ["Messaging:ConnectionString"] = "Host=localhost;Database=memorio;Username=memorio_messaging",
        // Not contacted either: no API test reads or writes media (Memorio.Storage.Tests covers storage).
        ["Storage:Endpoint"] = "http://localhost:3900",
        ["Storage:Region"] = "garage",
        ["Storage:Bucket"] = "memorio-media",
        ["Storage:AccessKey"] = "test-access-key",
        ["Storage:SecretKey"] = "test-secret-key",
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

        // API tests run without a database or a bucket: Wolverine keeps messages in memory and sends
        // nothing, and neither dependency is part of readiness here (their own test projects cover them).
        builder.ConfigureTestServices(services =>
        {
            services.DisableAllWolverineMessagePersistence();
            services.DisableAllExternalWolverineTransports();
            services.Configure<HealthCheckServiceOptions>(options =>
            {
                foreach (var check in options.Registrations
                    .Where(check => check.Name is MemorioMessaging.HealthCheckName or ObjectStorage.HealthCheckName)
                    .ToList())
                {
                    options.Registrations.Remove(check);
                }
            });
        });
    }
}
