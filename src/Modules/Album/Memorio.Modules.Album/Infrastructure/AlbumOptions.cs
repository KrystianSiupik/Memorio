namespace Memorio.Modules.Album.Infrastructure;

/// <summary>Configuration section "Album".</summary>
internal sealed class AlbumOptions
{
    public const string Section = "Album";

    /// <summary>PostgreSQL connection of the role memorio_album (schema album, ADR 0004). Required.</summary>
    public string? ConnectionString { get; set; }
}
