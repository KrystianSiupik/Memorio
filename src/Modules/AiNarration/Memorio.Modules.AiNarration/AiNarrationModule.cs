using Memorio.Modularity;
using Memorio.Modules.AiNarration.Application.AiTexts;
using Memorio.Modules.AiNarration.Application.Ports;
using Memorio.Modules.AiNarration.Infrastructure.Mistral;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Memorio.Modules.AiNarration;

/// <summary>Module AiNarration. Key question: Which story may be told from these materials without making anything up?</summary>
public sealed class AiNarrationModule : IModule
{
    public string Name => "AiNarration";

    public void AddServices(IServiceCollection services, IConfiguration configuration)
    {
        services.TryAddSingleton(TimeProvider.System);
        services.AddOptions<MistralOptions>()
            .Bind(configuration.GetSection(MistralOptions.Section))
            .Validate(
                options => !options.IsConfigured || options.BaseUrl.Scheme == Uri.UriSchemeHttps,
                "Ai__BaseUrl must be an https URL.")
            .ValidateOnStart();

        var timeout = configuration.GetSection(MistralOptions.Section).Get<MistralOptions>()?.Timeout
            ?? new MistralOptions().Timeout;
        services.AddHttpClient<ITextGenerator, MistralTextGenerator>()
            .AddStandardResilienceHandler(resilience =>
            {
                // Generating text takes far longer than the defaults (10 s per attempt) allow.
                resilience.AttemptTimeout.Timeout = timeout;
                resilience.TotalRequestTimeout.Timeout = timeout * 2.5;
                resilience.CircuitBreaker.SamplingDuration = timeout * 2;
            });
        services.AddTransient<AiTextService>();
    }

    public void MapEndpoints(IEndpointRouteBuilder endpoints)
    {
    }
}
