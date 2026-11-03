using Memorio.SharedKernel.Results;

namespace Memorio.Modules.AiNarration.Domain.Drafts;

internal static class DraftErrors
{
    public static readonly Error ConsentRequired = Error.Validation("AiNarration.ConsentRequired", "The material goes to the AI provider only with the person's consent.");

    public static readonly Error InvalidMaterial = Error.Validation("AiNarration.InvalidMaterial", "A draft needs some material, up to 20 000 characters.");

    /// <summary>E18.</summary>
    public static readonly Error LimitReached = Error.Conflict("AiNarration.LimitReached", "The AI allowance of this month is used up.");

    public static readonly Error IdTaken = Error.Conflict("AiNarration.IdTaken", "This id belongs to another draft.");
}
