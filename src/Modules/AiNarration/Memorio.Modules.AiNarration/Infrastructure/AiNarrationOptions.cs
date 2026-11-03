namespace Memorio.Modules.AiNarration.Infrastructure;

/// <summary>Configuration section "AiNarration".</summary>
internal sealed class AiNarrationOptions
{
    public const string Section = "AiNarration";

    /// <summary>PostgreSQL connection of the role memorio_ai_narration (schema ai_narration, ADR 0004). Required.</summary>
    public string? ConnectionString { get; set; }
}
