namespace Memorio.Modules.AiNarration.Infrastructure.Mistral;

/// <summary>
/// Configuration section "Ai" (environment variables Ai__ApiKey, Ai__Model, Ai__BaseUrl).
/// Optional: without an API key the AI features answer "provider unavailable" (E18) and the rest
/// of the application works. The provider must process data in the EU (K1); the final choice
/// follows the DPIA and the data processing agreement (ADR 0009).
/// </summary>
internal sealed class MistralOptions
{
    public const string Section = "Ai";

    public string? ApiKey { get; set; }

    public string Model { get; set; } = "mistral-large-latest";

    public Uri BaseUrl { get; set; } = new("https://api.mistral.ai/");

    public TimeSpan Timeout { get; set; } = TimeSpan.FromSeconds(60);

    public bool IsConfigured => !string.IsNullOrWhiteSpace(ApiKey);
}
