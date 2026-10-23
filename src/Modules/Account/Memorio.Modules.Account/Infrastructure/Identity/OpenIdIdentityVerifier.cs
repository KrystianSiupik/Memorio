using System.Security.Claims;
using Memorio.Modules.Account.Application.Ports;
using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Protocols;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using Microsoft.IdentityModel.Tokens;

namespace Memorio.Modules.Account.Infrastructure.Identity;

/// <summary>
/// Verifies an OpenID Connect ID token of Google or Apple with the provider's published keys (fetched
/// from its metadata and cached): signature, issuer, audience = one of our app ids, lifetime.
/// </summary>
internal sealed class OpenIdIdentityVerifier(
    ExternalProvider provider,
    IConfigurationManager<OpenIdConnectConfiguration> metadata,
    IReadOnlyCollection<string> issuers,
    IReadOnlyCollection<string> clientIds) : IExternalIdentityVerifier
{
    public const string GoogleMetadata = "https://accounts.google.com/.well-known/openid-configuration";
    public const string AppleMetadata = "https://appleid.apple.com/.well-known/openid-configuration";

    private static readonly string[] GoogleIssuers = ["https://accounts.google.com", "accounts.google.com"];
    private static readonly string[] AppleIssuers = ["https://appleid.apple.com"];

    private readonly JsonWebTokenHandler _tokens = new() { MapInboundClaims = false };

    public ExternalProvider Provider => provider;

    public bool IsEnabled => clientIds.Count > 0;

    public static OpenIdIdentityVerifier ForProvider(ExternalProvider provider, IReadOnlyCollection<string> clientIds)
    {
        var metadata = new ConfigurationManager<OpenIdConnectConfiguration>(
            provider == ExternalProvider.Google ? GoogleMetadata : AppleMetadata,
            new OpenIdConnectConfigurationRetriever(),
            new HttpDocumentRetriever { RequireHttps = true });
        return new OpenIdIdentityVerifier(provider, metadata, IssuersOf(provider), clientIds);
    }

    public static IReadOnlyCollection<string> IssuersOf(ExternalProvider provider) =>
        provider == ExternalProvider.Google ? GoogleIssuers : AppleIssuers;

    public async Task<ExternalIdentity?> VerifyAsync(string idToken, CancellationToken cancellationToken)
    {
        if (!IsEnabled || string.IsNullOrWhiteSpace(idToken))
        {
            return null;
        }

        var configuration = await metadata.GetConfigurationAsync(cancellationToken);
        var result = await _tokens.ValidateTokenAsync(idToken, new TokenValidationParameters
        {
            ValidIssuers = issuers,
            ValidAudiences = clientIds,
            IssuerSigningKeys = configuration.SigningKeys,
            ValidateIssuerSigningKey = true,
            ValidateLifetime = true,
            ClockSkew = TimeSpan.FromMinutes(2),
        });
        if (!result.IsValid || result.ClaimsIdentity.FindFirst("sub")?.Value is not { Length: > 0 } subject)
        {
            return null;
        }

        var email = result.ClaimsIdentity.FindFirst("email")?.Value;
        return new ExternalIdentity(provider, subject, email, IsTrue(result.ClaimsIdentity.FindFirst("email_verified")));
    }

    // Google sends a JSON boolean, Apple sometimes the string "true".
    private static bool IsTrue(Claim? claim) => bool.TryParse(claim?.Value, out var value) && value;
}
