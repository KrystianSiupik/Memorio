namespace Memorio.Modules.Plans.Infrastructure;

/// <summary>Configuration section "Plans".</summary>
internal sealed class PlansOptions
{
    public const string Section = "Plans";

    /// <summary>PostgreSQL connection of the role memorio_plans (schema plans, ADR 0004). Required.</summary>
    public string? ConnectionString { get; set; }
}
