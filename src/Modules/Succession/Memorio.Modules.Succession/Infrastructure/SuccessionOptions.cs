namespace Memorio.Modules.Succession.Infrastructure;

/// <summary>Configuration section "Succession".</summary>
internal sealed class SuccessionOptions
{
    public const string Section = "Succession";

    /// <summary>PostgreSQL connection of the role memorio_succession (schema succession, ADR 0004). Required.</summary>
    public string? ConnectionString { get; set; }
}
