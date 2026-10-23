using System.Security.Cryptography;
using Memorio.Modules.Account.Application.Ports;
using Memorio.Modules.Account.Infrastructure.Identity;
using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Protocols;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using Microsoft.IdentityModel.Tokens;

namespace Memorio.Modules.Account.Tests;

/// <summary>ID tokens of a provider, signed with a test key standing in for the provider's published keys.</summary>
public sealed class OpenIdIdentityVerifierTests : IDisposable
{
    private const string AppId = "app.memorio.ios";

    private readonly RSA _providerKey = RSA.Create(2048);
    private readonly RSA _otherKey = RSA.Create(2048);

    [Fact]
    public async Task A_valid_token_gives_the_stable_subject_and_the_verified_email()
    {
        var identity = await Verifier(ExternalProvider.Google).VerifyAsync(
            Token(issuer: "https://accounts.google.com", claims: new() { ["email"] = "ola@example.com", ["email_verified"] = true }),
            TestContext.Current.CancellationToken);

        Assert.Equal(new ExternalIdentity(ExternalProvider.Google, "subject-1", "ola@example.com", EmailVerified: true), identity);
    }

    [Fact]
    public async Task Apple_sends_email_verified_as_text()
    {
        var identity = await Verifier(ExternalProvider.Apple).VerifyAsync(
            Token(issuer: "https://appleid.apple.com", claims: new() { ["email"] = "x@privaterelay.appleid.com", ["email_verified"] = "true" }),
            TestContext.Current.CancellationToken);

        Assert.True(identity?.EmailVerified);
    }

    [Theory]
    [InlineData("audience")]
    [InlineData("issuer")]
    [InlineData("expired")]
    [InlineData("signature")]
    [InlineData("subject")]
    public async Task A_token_not_meant_for_us_is_refused(string broken)
    {
        var token = broken switch
        {
            "audience" => Token(audience: "someone.elses.app"),
            "issuer" => Token(issuer: "https://appleid.apple.com"),
            "expired" => Token(expires: DateTime.UtcNow.AddMinutes(-10)),
            "signature" => Token(key: _otherKey),
            _ => Token(subject: null),
        };

        Assert.Null(await Verifier(ExternalProvider.Google).VerifyAsync(token, TestContext.Current.CancellationToken));
    }

    [Fact]
    public async Task Without_our_app_ids_the_provider_is_off()
    {
        var verifier = Verifier(ExternalProvider.Google, clientIds: []);

        Assert.False(verifier.IsEnabled);
        Assert.Null(await verifier.VerifyAsync(Token(), TestContext.Current.CancellationToken));
    }

    public void Dispose()
    {
        _providerKey.Dispose();
        _otherKey.Dispose();
    }

    private OpenIdIdentityVerifier Verifier(ExternalProvider provider, string[]? clientIds = null)
    {
        var configuration = new OpenIdConnectConfiguration();
        configuration.SigningKeys.Add(new RsaSecurityKey(_providerKey) { KeyId = "provider-key" });
        return new OpenIdIdentityVerifier(
            provider,
            new StaticConfigurationManager<OpenIdConnectConfiguration>(configuration),
            OpenIdIdentityVerifier.IssuersOf(provider),
            clientIds ?? [AppId]);
    }

    private string Token(
        string issuer = "https://accounts.google.com",
        string audience = AppId,
        string? subject = "subject-1",
        DateTime? expires = null,
        RSA? key = null,
        Dictionary<string, object>? claims = null)
    {
        var all = new Dictionary<string, object>(claims ?? []);
        if (subject is not null)
        {
            all["sub"] = subject;
        }

        var expiresAt = expires ?? DateTime.UtcNow.AddMinutes(10);
        return new JsonWebTokenHandler().CreateToken(new SecurityTokenDescriptor
        {
            Issuer = issuer,
            Audience = audience,
            Claims = all,
            IssuedAt = expiresAt.AddMinutes(-20),
            NotBefore = expiresAt.AddMinutes(-20),
            Expires = expiresAt,
            SigningCredentials = new SigningCredentials(
                new RsaSecurityKey(key ?? _providerKey) { KeyId = "provider-key" },
                SecurityAlgorithms.RsaSha256),
        });
    }
}
