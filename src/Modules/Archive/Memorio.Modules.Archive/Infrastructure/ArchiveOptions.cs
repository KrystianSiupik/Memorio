namespace Memorio.Modules.Archive.Infrastructure;

/// <summary>Configuration section "Archive".</summary>
internal sealed class ArchiveOptions
{
    public const string Section = "Archive";

    /// <summary>PostgreSQL connection of the role memorio_archive (schema archive, ADR 0004). Required.</summary>
    public string? ConnectionString { get; set; }
}
