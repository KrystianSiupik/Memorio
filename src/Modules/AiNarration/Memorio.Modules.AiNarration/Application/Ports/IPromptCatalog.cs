namespace Memorio.Modules.AiNarration.Application.Ports;

/// <summary>The versioned instructions, kept in Infrastructure.</summary>
internal interface IPromptCatalog
{
    Prompt MemoryDescriptionDraft { get; }

    Prompt PeriodSummary { get; }
}
