using System.Security.Claims;
using Memorio.Modularity;
using Memorio.Modules.Account.Application.Ports;
using Memorio.Modules.Account.Domain.Accounts;
using Memorio.Modules.Account.Infrastructure.Identity;
using Memorio.SharedKernel.Results;
using Microsoft.AspNetCore.Authentication.BearerToken;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.Data;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Options;

namespace Memorio.Modules.Account.Api;

/// <summary>
/// Sign-in (ADR 0010, D-034): ASP.NET Core Identity issues bearer access tokens (1 h) and refresh tokens
/// (14 days), protected by keys kept in the account schema. A new account always needs the 16+
/// declaration. Anonymous endpoints here are on the SD10 allowlist.
/// </summary>
internal static class AuthEndpoints
{
    public const string InvalidCredentialsCode = "Account.InvalidCredentials";
    public const string InvalidExternalTokenCode = "Account.InvalidExternalToken";

    public static void MapAuthEndpoints(this IEndpointRouteBuilder v1)
    {
        var auth = v1.MapGroup("/auth").WithTags("Auth").AllowAnonymous();

        auth.MapPost("/register", RegisterAsync)
            .WithName("Register")
            .WithSummary("Creates an account with email and password (16+ declaration required) and signs in.");
        auth.MapPost("/login", LoginAsync)
            .WithName("Login")
            .WithSummary("Signs in with email and password.");
        auth.MapPost("/refresh", RefreshAsync)
            .WithName("RefreshToken")
            .WithSummary("Exchanges a refresh token for new tokens.");
        auth.MapPost("/external/{provider}", SignInWithProviderAsync)
            .WithName("SignInWithProvider")
            .WithSummary("Signs in with a Google or Apple ID token; a new account needs the 16+ declaration.");

        v1.MapGet("/account/me", GetMeAsync)
            .WithTags("Account")
            .WithName("GetMe")
            .WithSummary("The signed-in account.");
    }

    private static async Task<Results<Ok<AccessTokenResponse>, EmptyHttpResult, ProblemHttpResult>> RegisterAsync(
        RegisterRequest request,
        UserManager<AccountUser> users,
        SignInManager<AccountUser> signIn,
        TimeProvider time)
    {
        var declaration = AgeDeclaration.Declare(request.DeclaresMinimumAge, time.GetUtcNow());
        if (declaration.IsFailure)
        {
            return Problems.From(declaration.Error);
        }

        var user = new AccountUser
        {
            Id = Guid.CreateVersion7(),
            UserName = request.Email,
            Email = request.Email,
            AgeDeclaredAt = declaration.Value.DeclaredAt,
        };
        var created = await users.CreateAsync(user, request.Password ?? "");
        if (!created.Succeeded)
        {
            return Problems.From(ErrorOf(created));
        }

        await SignInAsync(signIn, user);
        return TypedResults.Empty;
    }

    private static async Task<Results<Ok<AccessTokenResponse>, EmptyHttpResult, ProblemHttpResult>> LoginAsync(
        LoginRequest request,
        SignInManager<AccountUser> signIn)
    {
        signIn.AuthenticationScheme = IdentityConstants.BearerScheme;
        var result = await signIn.PasswordSignInAsync(request.Email ?? "", request.Password ?? "", isPersistent: false, lockoutOnFailure: true);

        // One answer for a wrong email, a wrong password and a locked account: no account enumeration.
        return result.Succeeded
            ? TypedResults.Empty
            : Problems.WithCode(StatusCodes.Status401Unauthorized, InvalidCredentialsCode);
    }

    private static async Task<Results<Ok<AccessTokenResponse>, SignInHttpResult, ProblemHttpResult>> RefreshAsync(
        RefreshRequest request,
        SignInManager<AccountUser> signIn,
        IOptionsMonitor<BearerTokenOptions> bearerOptions,
        TimeProvider time)
    {
        var protector = bearerOptions.Get(IdentityConstants.BearerScheme).RefreshTokenProtector;
        var ticket = protector.Unprotect(request.RefreshToken);

        // Expired, tampered with, or issued before the account's security stamp changed (e.g. password change).
        if (ticket?.Properties.ExpiresUtc is not { } expiresAt
            || time.GetUtcNow() >= expiresAt
            || await signIn.ValidateSecurityStampAsync(ticket.Principal) is not { } user)
        {
            return Problems.WithCode(StatusCodes.Status401Unauthorized, InvalidCredentialsCode);
        }

        return TypedResults.SignIn(await signIn.CreateUserPrincipalAsync(user), authenticationScheme: IdentityConstants.BearerScheme);
    }

    private static async Task<Results<Ok<AccessTokenResponse>, EmptyHttpResult, ProblemHttpResult>> SignInWithProviderAsync(
        string provider,
        ExternalSignInRequest request,
        IEnumerable<IExternalIdentityVerifier> verifiers,
        UserManager<AccountUser> users,
        SignInManager<AccountUser> signIn,
        TimeProvider time,
        CancellationToken cancellationToken)
    {
        var verifier = verifiers.FirstOrDefault(candidate =>
            candidate.IsEnabled && string.Equals(candidate.Provider.ToString(), provider, StringComparison.OrdinalIgnoreCase));
        if (verifier is null)
        {
            return Problems.From(AccountErrors.ProviderUnavailable);
        }

        var identity = await verifier.VerifyAsync(request.IdToken ?? "", cancellationToken);
        if (identity is null)
        {
            return Problems.WithCode(StatusCodes.Status401Unauthorized, InvalidExternalTokenCode);
        }

        var loginProvider = identity.Provider.ToString();
        var user = await users.FindByLoginAsync(loginProvider, identity.Subject);
        if (user is null)
        {
            var created = await CreateExternalAccountAsync(identity, request.DeclaresMinimumAge, users, time);
            if (created.IsFailure)
            {
                return Problems.From(created.Error);
            }

            user = created.Value;
        }

        await SignInAsync(signIn, user);
        return TypedResults.Empty;
    }

    private static async Task<Result<AccountUser>> CreateExternalAccountAsync(
        ExternalIdentity identity,
        bool declaresMinimumAge,
        UserManager<AccountUser> users,
        TimeProvider time)
    {
        var declaration = AgeDeclaration.Declare(declaresMinimumAge, time.GetUtcNow());
        if (declaration.IsFailure)
        {
            return declaration.Error;
        }

        // Never attach a provider sign-in to an existing account by email (D-149): whoever registered
        // the email first, possibly not its owner, would get the other person's sign-in.
        var email = identity.EmailVerified ? identity.Email : null;
        if (email is not null && await users.FindByEmailAsync(email) is not null)
        {
            return AccountErrors.EmailTaken;
        }

        var user = new AccountUser
        {
            Id = Guid.CreateVersion7(),
            UserName = $"{identity.Provider}-{identity.Subject}".ToLowerInvariant(),
            Email = email,
            EmailConfirmed = email is not null,
            AgeDeclaredAt = declaration.Value.DeclaredAt,
        };
        var created = await users.CreateAsync(user);
        if (created.Succeeded)
        {
            created = await users.AddLoginAsync(user, new UserLoginInfo(identity.Provider.ToString(), identity.Subject, identity.Provider.ToString()));
        }

        return created.Succeeded ? user : ErrorOf(created);
    }

    private static async Task<Results<Ok<AccountResponse>, UnauthorizedHttpResult>> GetMeAsync(
        ClaimsPrincipal principal,
        UserManager<AccountUser> users)
    {
        var user = await users.GetUserAsync(principal);
        return user is null
            ? TypedResults.Unauthorized()
            : TypedResults.Ok(new AccountResponse(user.Id, user.Email));
    }

    /// <summary>Signs in on the bearer scheme: the handler writes the token response itself.</summary>
    private static Task SignInAsync(SignInManager<AccountUser> signIn, AccountUser user)
    {
        signIn.AuthenticationScheme = IdentityConstants.BearerScheme;
        return signIn.SignInAsync(user, isPersistent: false);
    }

    private static Error ErrorOf(IdentityResult result) =>
        result.Errors.Select(error => error.Code).ToList() switch
        {
            var codes when codes.Exists(code => code is "DuplicateEmail" or "DuplicateUserName") => AccountErrors.EmailTaken,
            var codes when codes.Exists(code => code.StartsWith("Password", StringComparison.Ordinal)) => AccountErrors.InvalidPassword,
            var codes when codes.Exists(code => code is "InvalidEmail" or "InvalidUserName") => AccountErrors.InvalidEmail,
            _ => Error.Failure("Account.CreateFailed", string.Join("; ", result.Errors.Select(error => error.Code))),
        };
}

/// <param name="DeclaresMinimumAge">The person declares being at least 16; required for a new account.</param>
internal sealed record RegisterRequest(string? Email, string? Password, bool DeclaresMinimumAge);

internal sealed record LoginRequest(string? Email, string? Password);

/// <param name="IdToken">The ID token the native Google or Apple sign-in returned to the app.</param>
/// <param name="DeclaresMinimumAge">Required when this sign-in creates the account.</param>
internal sealed record ExternalSignInRequest(string? IdToken, bool DeclaresMinimumAge);

/// <summary>Response of GET /v1/account/me.</summary>
internal sealed record AccountResponse(Guid AccountId, string? Email);
