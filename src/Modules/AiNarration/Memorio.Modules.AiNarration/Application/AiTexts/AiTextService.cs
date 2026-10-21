using Memorio.Modules.AiNarration.Application.Ports;
using Memorio.Modules.AiNarration.Domain.AiTexts;
using Memorio.Modules.AiNarration.Domain.Pseudonymization;
using Memorio.SharedKernel.Results;

namespace Memorio.Modules.AiNarration.Application.AiTexts;

/// <summary>
/// Writes an AI text from the user's material: hides the names, calls the provider, puts the names
/// back and returns the text with its provenance. Used by the draft (C15) and summary (C82) handlers,
/// which run asynchronously (ADR 0009).
/// </summary>
internal sealed class AiTextService(ITextGenerator generator, TimeProvider clock)
{
    public async Task<Result<AiGeneratedText>> WriteAsync(
        Prompt prompt, string material, IReadOnlyCollection<SensitiveName> names, CancellationToken cancellationToken)
    {
        var pseudonymized = Pseudonymizer.Pseudonymize(material, names);

        var answer = await generator.GenerateAsync(new TextGenerationRequest(prompt, pseudonymized.Text), cancellationToken);
        if (answer.IsFailure)
        {
            return answer.Error;
        }

        var restored = Pseudonymizer.Restore(answer.Value.Text, pseudonymized);
        if (restored.IsFailure)
        {
            return restored.Error;
        }

        return AiGeneratedText.Create(restored.Value, answer.Value.Model, prompt.VersionTag, clock.GetUtcNow());
    }
}
