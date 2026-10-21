using Memorio.Modules.AiNarration.Application.Ports;

namespace Memorio.Modules.AiNarration.Infrastructure.Prompts;

/// <summary>
/// Versioned prompts. Change the instructions only together with a higher version: the version is
/// stored with every AI text (SD9) and tells which instructions produced it.
/// </summary>
internal static class Prompts
{
    private const string Rules =
        """
        Names of people and places are replaced with tokens such as [PERSON_1] or [PLACE_1].
        Keep every token exactly as written and never invent new ones.
        Use only facts present in the material; never add events, people, places or dates.
        Answer in the language of the material, as JSON: {"text": "..."}.
        """;

    /// <summary>C15: draft of a memory description.</summary>
    public static readonly Prompt MemoryDescriptionDraft = new(
        "memory-description-draft",
        1,
        "You help a person describe one memory in a few warm, factual sentences.\n" + Rules);

    /// <summary>C82: summary of a period built from many memories.</summary>
    public static readonly Prompt PeriodSummary = new(
        "period-summary",
        1,
        "You write a short story of a period from the memories below, in chronological order.\n" + Rules);
}
