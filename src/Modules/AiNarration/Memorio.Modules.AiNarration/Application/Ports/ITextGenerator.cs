using Memorio.SharedKernel.Results;

namespace Memorio.Modules.AiNarration.Application.Ports;

/// <summary>
/// The AI provider (ADR 0009). Receives only pseudonymized text; implemented in Infrastructure.
/// Failures: "AiNarration.ProviderUnavailable" (no provider configured, outage, timeout) and
/// "AiNarration.InvalidProviderAnswer" (the answer does not match the expected structure).
/// </summary>
internal interface ITextGenerator
{
    Task<Result<ProviderText>> GenerateAsync(TextGenerationRequest request, CancellationToken cancellationToken);
}

/// <param name="Prompt">Versioned instructions (system message).</param>
/// <param name="Material">The user's material, already pseudonymized.</param>
internal sealed record TextGenerationRequest(Prompt Prompt, string Material);

/// <param name="Id">Stable prompt name, e.g. "memory-description-draft".</param>
/// <param name="Version">Increased on every change of <paramref name="Instructions"/>.</param>
internal sealed record Prompt(string Id, int Version, string Instructions)
{
    public string VersionTag => $"{Id}@{Version}";
}

/// <param name="Text">The generated text, still with name tokens.</param>
/// <param name="Model">Model that wrote it, as reported by the provider.</param>
internal sealed record ProviderText(string Text, string Model);

internal static class TextGeneratorErrors
{
    public static readonly Error ProviderUnavailable =
        Error.Failure("AiNarration.ProviderUnavailable", "The AI provider is not available.");

    public static readonly Error InvalidProviderAnswer =
        Error.Failure("AiNarration.InvalidProviderAnswer", "The AI provider answered in an unexpected format.");
}
