namespace Memorio.Modules.Places.Infrastructure;

/// <summary>Configuration section "Places".</summary>
internal sealed class PlacesOptions
{
    public const string Section = "Places";

    /// <summary>PostgreSQL connection of the role memorio_places (schema places, ADR 0004). Required.</summary>
    public string? ConnectionString { get; set; }
}
