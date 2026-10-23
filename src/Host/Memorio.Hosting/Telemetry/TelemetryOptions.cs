namespace Memorio.Hosting.Telemetry;

/// <summary>Configuration section "Telemetry" (ADR 0014): where both hosts send metrics and logs.</summary>
public sealed class TelemetryOptions
{
    public const string Section = "Telemetry";

    /// <summary>
    /// Base address of the OpenTelemetry Collector's OTLP/HTTP receiver (e.g. http://otel-collector:4318).
    /// Optional: without it nothing is exported (local runs, tests).
    /// </summary>
    public string? OtlpEndpoint { get; set; }

    /// <summary>Optional name of the environment (e.g. "production", "staging"), sent with every signal.</summary>
    public string? Environment { get; set; }
}
