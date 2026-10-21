using System.Net;
using System.Text;
using System.Text.Json;
using Memorio.Modules.AiNarration.Application.AiTexts;
using Memorio.Modules.AiNarration.Domain.Pseudonymization;
using Memorio.Modules.AiNarration.Infrastructure.Mistral;
using Memorio.Modules.AiNarration.Infrastructure.Prompts;
using Microsoft.Extensions.Options;

namespace Memorio.Modules.AiNarration.Tests;

/// <summary>
/// SD9 / QS8 on synthetic data: the payload sent to the provider contains no name from the list, and
/// every AI text carries the AI marker, the model and the prompt version.
/// </summary>
public sealed class AiAdapterTests
{
    private static readonly DateTimeOffset Now = new(2026, 10, 22, 12, 0, 0, TimeSpan.Zero);

    private static readonly string[] PeopleNames = ["Anna Kowalska", "Tomek", "Babcia Halina", "Zofia"];

    private static readonly string[] PlaceNames = ["Zakopane", "Morskie Oko", "Kraków", "Łódź", "Gdańsk"];

    private const string Material =
        "Anna Kowalska i Tomek pojechali z Babcią Haliną do Zakopanego. Następnego dnia weszli nad Morskie Oko, " +
        "a wieczorem Zofia zadzwoniła z Gdańska. W drodze powrotnej zatrzymali się w Krakowie i w ŁODZI.";

    [Fact]
    public async Task The_payload_contains_no_name_from_the_list()
    {
        var provider = new FakeProvider(_ => Answer("Wspomnienie: [PERSON_1] i [PERSON_2] w [PLACE_1]."));

        var result = await Service(provider).WriteAsync(Prompts.PeriodSummary, Material, Names(), TestContext.Current.CancellationToken);

        Assert.True(result.IsSuccess);
        var payload = Assert.Single(provider.Payloads);
        foreach (var name in PeopleNames.Concat(PlaceNames))
        {
            foreach (var part in name.Split(' '))
            {
                Assert.DoesNotContain(part, payload, StringComparison.OrdinalIgnoreCase);
            }
        }
    }

    [Fact]
    public async Task The_ai_text_has_its_names_back_and_its_provenance()
    {
        var provider = new FakeProvider(_ => Answer("[PERSON_1] i [PERSON_2] w [PLACE_1].", model: "mistral-large-2511"));

        var text = (await Service(provider).WriteAsync(
            Prompts.MemoryDescriptionDraft, "Anna i Tomek w Zakopanem.", [Person("Anna"), Person("Tomek"), Place("Zakopane")],
            TestContext.Current.CancellationToken)).Value;

        Assert.Equal("Anna i Tomek w Zakopane.", text.Content);
        Assert.True(text.IsAiGenerated);
        Assert.Equal("mistral-large-2511", text.Model);
        Assert.Equal("memory-description-draft@1", text.PromptVersion);
        Assert.Equal(Now, text.GeneratedAt);
    }

    [Fact]
    public async Task The_request_uses_the_configured_model_json_output_and_the_versioned_prompt()
    {
        var provider = new FakeProvider(_ => Answer("Tekst."));

        await Service(provider).WriteAsync(Prompts.MemoryDescriptionDraft, "Material.", [], TestContext.Current.CancellationToken);

        using var request = JsonDocument.Parse(Assert.Single(provider.Payloads));
        Assert.Equal("test-model", request.RootElement.GetProperty("model").GetString());
        Assert.Equal("json_object", request.RootElement.GetProperty("response_format").GetProperty("type").GetString());
        Assert.Equal(Prompts.MemoryDescriptionDraft.Instructions, request.RootElement.GetProperty("messages")[0].GetProperty("content").GetString());
        Assert.Equal("Bearer test-key", provider.Authorization);
    }

    [Theory]
    [InlineData(HttpStatusCode.InternalServerError, "AiNarration.ProviderUnavailable")]
    [InlineData(HttpStatusCode.TooManyRequests, "AiNarration.ProviderUnavailable")]
    [InlineData(HttpStatusCode.Unauthorized, "AiNarration.ProviderUnavailable")]
    public async Task Provider_errors_mean_unavailable(HttpStatusCode status, string code)
    {
        var provider = new FakeProvider(_ => new HttpResponseMessage(status));

        var result = await Service(provider).WriteAsync(Prompts.MemoryDescriptionDraft, "Material.", [], TestContext.Current.CancellationToken);

        Assert.Equal(code, result.Error.Code);
    }

    [Theory]
    [InlineData("""{"model":"m","choices":[{"message":{"content":"not json"}}]}""")]
    [InlineData("""{"model":"m","choices":[{"message":{"content":"{\"text\":\"\"}"}}]}""")]
    [InlineData("""{"model":"m","choices":[]}""")]
    [InlineData("""{"choices":[{"message":{"content":"{\"text\":\"ok\"}"}}]}""")]
    public async Task An_unexpected_answer_is_rejected(string body)
    {
        var provider = new FakeProvider(_ => Json(body));

        var result = await Service(provider).WriteAsync(Prompts.MemoryDescriptionDraft, "Material.", [], TestContext.Current.CancellationToken);

        Assert.Equal("AiNarration.InvalidProviderAnswer", result.Error.Code);
    }

    [Fact]
    public async Task An_answer_inventing_a_name_token_is_rejected()
    {
        var provider = new FakeProvider(_ => Answer("[PERSON_1] i [PERSON_9]."));

        var result = await Service(provider).WriteAsync(Prompts.MemoryDescriptionDraft, "Anna.", [Person("Anna")], TestContext.Current.CancellationToken);

        Assert.Equal("AiNarration.UnknownToken", result.Error.Code);
    }

    [Fact]
    public async Task Without_an_api_key_nothing_is_sent_and_the_provider_is_unavailable()
    {
        var provider = new FakeProvider(_ => Answer("Tekst."));

        var result = await Service(provider, apiKey: null).WriteAsync(Prompts.MemoryDescriptionDraft, "Material.", [], TestContext.Current.CancellationToken);

        Assert.Equal("AiNarration.ProviderUnavailable", result.Error.Code);
        Assert.Empty(provider.Payloads);
    }

    private static SensitiveName Person(string name) => SensitiveName.Create(SensitiveNameKind.Person, name).Value;

    private static SensitiveName Place(string name) => SensitiveName.Create(SensitiveNameKind.Place, name).Value;

    private static SensitiveName[] Names() => [.. PeopleNames.Select(Person), .. PlaceNames.Select(Place)];

    private static AiTextService Service(FakeProvider provider, string? apiKey = "test-key")
    {
        var options = Options.Create(new MistralOptions { ApiKey = apiKey, Model = "test-model", BaseUrl = new Uri("https://ai.example.invalid/") });
        var generator = new MistralTextGenerator(new HttpClient(provider), options);
        return new AiTextService(generator, new FixedClock(Now));
    }

    private static HttpResponseMessage Answer(string text, string model = "test-model") =>
        Json(JsonSerializer.Serialize(new
        {
            model,
            choices = new[] { new { message = new { content = JsonSerializer.Serialize(new { text }) } } },
        }));

    private static HttpResponseMessage Json(string body) =>
        new(HttpStatusCode.OK) { Content = new StringContent(body, Encoding.UTF8, "application/json") };

    private sealed class FakeProvider(Func<HttpRequestMessage, HttpResponseMessage> answer) : HttpMessageHandler
    {
        public List<string> Payloads { get; } = [];

        public string? Authorization { get; private set; }

        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            Payloads.Add(await request.Content!.ReadAsStringAsync(cancellationToken));
            Authorization = request.Headers.Authorization?.ToString();
            return answer(request);
        }
    }

    private sealed class FixedClock(DateTimeOffset now) : TimeProvider
    {
        public override DateTimeOffset GetUtcNow() => now;
    }
}
