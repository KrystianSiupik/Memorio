using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;
using System.Text.Json.Serialization;
using Memorio.Modules.AiNarration.Application.Ports;
using Memorio.SharedKernel.Results;
using Microsoft.Extensions.Options;

namespace Memorio.Modules.AiNarration.Infrastructure.Mistral;

/// <summary>
/// Calls the Mistral chat completions API (EU provider, D-080) with JSON output. Never logs the
/// material or the answer: they are user content.
/// </summary>
internal sealed class MistralTextGenerator(HttpClient http, IOptions<MistralOptions> options) : ITextGenerator
{
    public async Task<Result<ProviderText>> GenerateAsync(TextGenerationRequest request, CancellationToken cancellationToken)
    {
        var settings = options.Value;
        if (!settings.IsConfigured)
        {
            return TextGeneratorErrors.ProviderUnavailable;
        }

        using var message = new HttpRequestMessage(HttpMethod.Post, new Uri(settings.BaseUrl, "v1/chat/completions"))
        {
            Content = JsonContent.Create(new ChatRequest(
                settings.Model,
                [new ChatMessage("system", request.Prompt.Instructions), new ChatMessage("user", request.Material)],
                new ResponseFormat("json_object"),
                Temperature: 0.3)),
        };
        message.Headers.Authorization = new AuthenticationHeaderValue("Bearer", settings.ApiKey);

        HttpResponseMessage response;
        try
        {
            response = await http.SendAsync(message, cancellationToken);
        }
        catch (HttpRequestException)
        {
            return TextGeneratorErrors.ProviderUnavailable;
        }
        catch (TaskCanceledException) when (!cancellationToken.IsCancellationRequested)
        {
            return TextGeneratorErrors.ProviderUnavailable;
        }

        using (response)
        {
            if (!response.IsSuccessStatusCode)
            {
                return TextGeneratorErrors.ProviderUnavailable;
            }

            return await ParseAsync(response, cancellationToken);
        }
    }

    private static async Task<Result<ProviderText>> ParseAsync(HttpResponseMessage response, CancellationToken cancellationToken)
    {
        try
        {
            var completion = await response.Content.ReadFromJsonAsync<ChatResponse>(cancellationToken);
            var content = completion?.Choices is [{ Message.Content: { } json }, ..] ? json : null;
            if (content is null || completion?.Model is not { Length: > 0 } model)
            {
                return TextGeneratorErrors.InvalidProviderAnswer;
            }

            var answer = JsonSerializer.Deserialize<Answer>(content);
            return string.IsNullOrWhiteSpace(answer?.Text)
                ? TextGeneratorErrors.InvalidProviderAnswer
                : new ProviderText(answer.Text, model);
        }
        catch (JsonException)
        {
            return TextGeneratorErrors.InvalidProviderAnswer;
        }
    }

    private sealed record ChatRequest(
        [property: JsonPropertyName("model")] string Model,
        [property: JsonPropertyName("messages")] IReadOnlyList<ChatMessage> Messages,
        [property: JsonPropertyName("response_format")] ResponseFormat ResponseFormat,
        [property: JsonPropertyName("temperature")] double Temperature);

    private sealed record ChatMessage(
        [property: JsonPropertyName("role")] string Role,
        [property: JsonPropertyName("content")] string Content);

    private sealed record ResponseFormat([property: JsonPropertyName("type")] string Type);

    private sealed record ChatResponse(
        [property: JsonPropertyName("model")] string? Model,
        [property: JsonPropertyName("choices")] IReadOnlyList<Choice>? Choices);

    private sealed record Choice([property: JsonPropertyName("message")] ChoiceMessage? Message);

    private sealed record ChoiceMessage([property: JsonPropertyName("content")] string? Content);

    private sealed record Answer([property: JsonPropertyName("text")] string? Text);
}
