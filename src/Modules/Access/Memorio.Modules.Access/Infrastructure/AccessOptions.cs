namespace Memorio.Modules.Access.Infrastructure;

/// <summary>Configuration section "Access".</summary>
internal sealed class AccessOptions
{
    public const string Section = "Access";

    /// <summary>PostgreSQL connection of the role memorio_access (schema access, ADR 0004). Required.</summary>
    public string? ConnectionString { get; set; }
}
