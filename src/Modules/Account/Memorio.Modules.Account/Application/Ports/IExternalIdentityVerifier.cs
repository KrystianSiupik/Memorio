namespace Memorio.Modules.Account.Application.Ports;

/// <summary>Sign-in providers whose ID tokens the app sends after the native sign-in (D-034).</summary>
internal enum ExternalProvider
{
    Google,
    Apple,
}

/// <summary>Who the provider says signed in: <see cref="Subject"/> is stable per provider and app.</summary>
internal sealed record ExternalIdentity(ExternalProvider Provider, string Subject, string? Email, bool EmailVerified);

/// <summary>Checks an ID token from a sign-in provider: signature, issuer, audience (our app ids), expiry.</summary>
internal interface IExternalIdentityVerifier
{
    ExternalProvider Provider { get; }

    /// <summary>False when no app id is configured for the provider: sign-in with it is off.</summary>
    bool IsEnabled { get; }

    /// <summary>The identity in a valid token, or null when the token is not valid for us.</summary>
    Task<ExternalIdentity?> VerifyAsync(string idToken, CancellationToken cancellationToken);
}
