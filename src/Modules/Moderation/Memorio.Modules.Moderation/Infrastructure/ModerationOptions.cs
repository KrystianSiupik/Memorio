namespace Memorio.Modules.Moderation.Infrastructure;

/// <summary>Configuration section "Moderation".</summary>
internal sealed class ModerationOptions
{
    public const string Section = "Moderation";

    /// <summary>PostgreSQL connection of the role memorio_moderation (schema moderation, ADR 0004). Required.</summary>
    public string? ConnectionString { get; set; }
}
