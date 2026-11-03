using Memorio.Modularity;
using Memorio.Modules.AiNarration.Api;
using Memorio.Modules.AiNarration.Application.AiTexts;
using Memorio.Modules.AiNarration.Application.Commands.RequestDraft;
using Memorio.Modules.AiNarration.Application.Ports;
using Memorio.Modules.AiNarration.Contracts;
using Memorio.Modules.AiNarration.Domain.Drafts;
using Memorio.Modules.AiNarration.Infrastructure;
using Memorio.Modules.AiNarration.Infrastructure.Mistral;
using Memorio.Modules.AiNarration.Infrastructure.Persistence;
using Memorio.Modules.AiNarration.Infrastructure.Prompts;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Hosting;

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

        services.AddOptions<AiNarrationOptions>()
            .Bind(configuration.GetSection(AiNarrationOptions.Section))
            .Validate(options => !string.IsNullOrWhiteSpace(options.ConnectionString), "AiNarration__ConnectionString is required.")
            .ValidateOnStart();

        // A missing value fails the validation above at startup; the placeholder only lets the host be built.
        var connectionString = configuration.GetSection(AiNarrationOptions.Section).Get<AiNarrationOptions>()?.ConnectionString;
        if (string.IsNullOrWhiteSpace(connectionString))
        {
            connectionString = "Host=unconfigured";
        }

        services.AddDbContext<AiNarrationDbContext>(database => database.UseAiNarrationDatabase(connectionString));
        services.AddScoped<IMemoryDrafts, MemoryDrafts>();
        services.AddScoped<IPromptCatalog, PromptCatalog>();
        services.AddScoped<RequestDraftHandler>();
        services.AddScoped<IAiDraftReader, AiDraftReader>();

        services.AddHealthChecks().AddCheck<AiNarrationDatabaseHealthCheck>(AiNarrationDatabaseHealthCheck.Name, tags: [HealthTags.Ready]);
        if (configuration.GetValue("Database:MigrateOnStartup", defaultValue: true))
        {
            services.AddHostedService<AiNarrationDatabaseMigrator>();
        }
    }

    public void MapEndpoints(IEndpointRouteBuilder endpoints) => endpoints.MapDraftsEndpoints();
}
