namespace Memorio.Modules.Provenance.Infrastructure;

/// <summary>Configuration section "Provenance".</summary>
internal sealed class ProvenanceOptions
{
    public const string Section = "Provenance";

    /// <summary>PostgreSQL connection of the role memorio_provenance (schema provenance, ADR 0004). Required.</summary>
    public string? ConnectionString { get; set; }
}
