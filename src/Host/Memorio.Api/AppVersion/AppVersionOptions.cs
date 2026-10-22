namespace Memorio.Api.AppVersion;

/// <summary>Configuration section "AppVersion" (ADR 0017: force an update below the minimum).</summary>
internal sealed class AppVersionOptions
{
    public const string Section = "AppVersion";

    /// <summary>Oldest app version the API still serves, "major.minor.patch". Required.</summary>
    public string? Minimum { get; set; }
}
