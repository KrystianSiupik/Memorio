using System.Reflection;
using Memorio.Hosting.Configuration;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using OpenTelemetry;
using OpenTelemetry.Exporter;
using OpenTelemetry.Logs;
using OpenTelemetry.Metrics;
using OpenTelemetry.Resources;

namespace Memorio.Hosting.Telemetry;

/// <summary>
/// Metrics and logs of both hosts with OpenTelemetry (ADR 0014, D-038), sent over OTLP to the
/// OpenTelemetry Collector, which hands metrics to Prometheus and logs to Loki. Metrics: HTTP server and
/// client, .NET runtime and every meter named <see cref="MeterPrefix"/>*. Logs carry the message and its
/// parameters only, never request bodies or scopes: no content of users reaches monitoring (D-038).
/// No traces in stage 1 (D-161).
/// </summary>
public static class MemorioTelemetry
{
    /// <summary>Every Memorio meter is named "Memorio.&lt;Area&gt;", so it is exported without being listed here.</summary>
    public const string MeterPrefix = "Memorio.";

    public static IServiceCollection AddMemorioTelemetry(this IServiceCollection services, IConfiguration configuration, string serviceName)
    {
        services.AddOptions<TelemetryOptions>()
            .Bind(configuration.GetSection(TelemetryOptions.Section))
            .Validate(
                options => string.IsNullOrEmpty(options.OtlpEndpoint) || IsAddress(options.OtlpEndpoint),
                $"{ConfigurationErrors.Variable($"{TelemetryOptions.Section}:{nameof(TelemetryOptions.OtlpEndpoint)}")} must be an http(s) address when set.")
            .ValidateOnStart();

        var options = configuration.GetSection(TelemetryOptions.Section).Get<TelemetryOptions>() ?? new TelemetryOptions();
        if (!IsAddress(options.OtlpEndpoint))
        {
            return services;
        }

        var version = typeof(MemorioTelemetry).Assembly.GetCustomAttribute<AssemblyInformationalVersionAttribute>()?.InformationalVersion;
        services.AddOpenTelemetry()
            .ConfigureResource(resource =>
            {
                resource.AddService(serviceName, serviceVersion: version);
                if (!string.IsNullOrWhiteSpace(options.Environment))
                {
                    resource.AddAttributes([new("deployment.environment.name", options.Environment)]);
                }
            })
            .WithMetrics(metrics => metrics
                .AddAspNetCoreInstrumentation()
                .AddHttpClientInstrumentation()
                .AddRuntimeInstrumentation()
                .AddMeter(MeterPrefix + "*"))
            .WithLogging(configureBuilder: null, configureOptions: logging =>
            {
                logging.IncludeFormattedMessage = true;
                logging.IncludeScopes = false;
            })
            .UseOtlpExporter(OtlpExportProtocol.HttpProtobuf, new Uri(options.OtlpEndpoint!));

        return services;
    }

    private static bool IsAddress(string? value) =>
        Uri.TryCreate(value, UriKind.Absolute, out var uri) && (uri.Scheme == Uri.UriSchemeHttp || uri.Scheme == Uri.UriSchemeHttps);
}
