using Memorio.Modules.AiNarration.Application.Ports;

namespace Memorio.Modules.AiNarration.Infrastructure.Prompts;

/// <summary>
/// Versioned prompts. Change the instructions only together with a higher version: the version is
/// stored with every AI text (SD9) and tells which instructions produced it.
/// </summary>
internal sealed class PromptCatalog : IPromptCatalog
{
    public Prompt MemoryDescriptionDraft => Prompts.MemoryDescriptionDraft;

    public Prompt PeriodSummary => Prompts.PeriodSummary;
}

/// <summary>The prompts themselves.</summary>
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

    /// <summary>C82: summary of a period built from many memories; every sentence names its facts (S2:A2).</summary>
    public static readonly Prompt PeriodSummary = new(
        "period-summary",
        2,
        """
        You write a short, warm story of a period from the numbered facts below (F1, F2, ...), in chronological order,
        in 3 to 8 sentences. End every sentence with the facts it is based on in square brackets, for example
        "In July you went to the mountains. [F1, F3]". Write no sentence without its facts.
        Tell only what the facts say: never add feelings, intentions, relations between people, events, places or dates.
        Refer to the people only as the facts do ("person A"); never guess who they are.

        """ + Rules);
}
