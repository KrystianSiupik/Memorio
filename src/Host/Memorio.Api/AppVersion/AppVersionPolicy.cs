using Memorio.Hosting.Configuration;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace Memorio.Api.AppVersion;

/// <summary>
/// Minimum app version (ADR 0017). Every /v1 request carrying <see cref="Header"/> below the
/// minimum gets 426 with code "App.UpdateRequired"; the app then shows the update screen.
/// Requests without the header (web pages, tools) are served.
/// </summary>
internal static class AppVersionPolicy
{
    public const string Header = "X-App-Version";
    public const string UpdateRequiredCode = "App.UpdateRequired";
    public const string InvalidVersionCode = "App.InvalidVersionHeader";

    public static IServiceCollection AddAppVersionPolicy(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddOptions<AppVersionOptions>()
            .Bind(configuration.GetSection(AppVersionOptions.Section))
            .Validate(
                options => AppVersionNumber.TryParse(options.Minimum, out _),
                $"{ConfigurationErrors.Variable($"{AppVersionOptions.Section}:{nameof(AppVersionOptions.Minimum)}")} " +
                "is required and must be a version major.minor.patch.")
            .ValidateOnStart();
        return services;
    }

    public static IApplicationBuilder UseAppVersionPolicy(this IApplicationBuilder app) =>
        app.UseWhen(
            context => context.Request.Path.StartsWithSegments("/v1", StringComparison.OrdinalIgnoreCase),
            branch => branch.Use(CheckAsync));

    public static RouteGroupBuilder MapAppVersionEndpoints(this RouteGroupBuilder v1)
    {
        v1.MapGet("/app/version-policy", GetPolicy)
            .WithName("GetAppVersionPolicy")
            .WithTags("App")
            .WithSummary("Minimum app version the API serves.")
            .AllowAnonymous();
        return v1;
    }

    private static Ok<AppVersionPolicyResponse> GetPolicy(IOptions<AppVersionOptions> options) =>
        TypedResults.Ok(new AppVersionPolicyResponse(Minimum(options.Value).ToString()));

    private static async Task CheckAsync(HttpContext context, RequestDelegate next)
    {
        if (!context.Request.Headers.TryGetValue(Header, out var values))
        {
            await next(context);
            return;
        }

        if (!AppVersionNumber.TryParse(values.ToString(), out var appVersion))
        {
            await WriteProblemAsync(
                context,
                StatusCodes.Status400BadRequest,
                InvalidVersionCode,
                $"Header {Header} must be a version major.minor.patch.",
                minimumVersion: null);
            return;
        }

        var minimum = Minimum(context.RequestServices.GetRequiredService<IOptions<AppVersionOptions>>().Value);
        if (appVersion < minimum)
        {
            await WriteProblemAsync(
                context,
                StatusCodes.Status426UpgradeRequired,
                UpdateRequiredCode,
                "This app version is no longer supported.",
                minimum.ToString());
            return;
        }

        await next(context);
    }

    private static AppVersionNumber Minimum(AppVersionOptions options) =>
        AppVersionNumber.TryParse(options.Minimum, out var minimum)
            ? minimum
            : throw new InvalidOperationException("AppVersion:Minimum is validated at startup.");

    private static ValueTask WriteProblemAsync(
        HttpContext context, int status, string code, string title, string? minimumVersion)
    {
        var problem = new ProblemDetails { Status = status, Title = title };
        problem.Extensions["code"] = code;
        if (minimumVersion is not null)
        {
            problem.Extensions["minimumVersion"] = minimumVersion;
        }

        context.Response.StatusCode = status;
        return context.RequestServices.GetRequiredService<IProblemDetailsService>()
            .WriteAsync(new ProblemDetailsContext { HttpContext = context, ProblemDetails = problem });
    }
}

/// <summary>Response of GET /v1/app/version-policy.</summary>
/// <param name="MinimumVersion">Oldest app version the API serves, "major.minor.patch".</param>
internal sealed record AppVersionPolicyResponse(string MinimumVersion);
