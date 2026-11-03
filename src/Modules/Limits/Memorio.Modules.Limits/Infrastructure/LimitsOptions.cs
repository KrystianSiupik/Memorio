namespace Memorio.Modules.Limits.Infrastructure;

/// <summary>Configuration section "Limits".</summary>
internal sealed class LimitsOptions
{
    public const string Section = "Limits";

    /// <summary>PostgreSQL connection of the role memorio_limits (schema limits, ADR 0004). Required.</summary>
    public string? ConnectionString { get; set; }
}
