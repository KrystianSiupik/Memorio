using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;

namespace Memorio.Modules.Account.Tests;

/// <summary>MEM-E0-PLAT-07 on a real database: accounts, 16+ declaration, tokens, provider sign-in.</summary>
public sealed class SignInTests(AccountHost host) : IClassFixture<AccountHost>
{
    private const string Password = "long-enough-password";

    private HttpClient Client => host.Client;

    [Fact]
    public async Task Registering_signs_in_and_the_token_opens_the_account()
    {
        var email = NewEmail();

        var tokens = await PostAsync("/v1/auth/register", new { email, password = Password, declaresMinimumAge = true });

        Assert.Equal(HttpStatusCode.OK, tokens.StatusCode);
        var me = await GetMeAsync(await AccessTokenAsync(tokens));
        Assert.Equal(email, me.GetProperty("email").GetString());
    }

    [Fact]
    public async Task Without_the_16_plus_declaration_no_account_is_created()
    {
        var email = NewEmail();

        var refused = await PostAsync("/v1/auth/register", new { email, password = Password, declaresMinimumAge = false });
        var login = await PostAsync("/v1/auth/login", new { email, password = Password });

        await AssertProblemAsync(refused, HttpStatusCode.BadRequest, "Account.AgeDeclarationRequired");
        await AssertProblemAsync(login, HttpStatusCode.Unauthorized, "Account.InvalidCredentials");
    }

    [Fact]
    public async Task An_email_has_one_account_and_short_passwords_are_refused()
    {
        var email = NewEmail();
        await PostAsync("/v1/auth/register", new { email, password = Password, declaresMinimumAge = true });

        var again = await PostAsync("/v1/auth/register", new { email, password = Password, declaresMinimumAge = true });
        var shortPassword = await PostAsync("/v1/auth/register", new { email = NewEmail(), password = "short", declaresMinimumAge = true });

        await AssertProblemAsync(again, HttpStatusCode.Conflict, "Account.EmailTaken");
        await AssertProblemAsync(shortPassword, HttpStatusCode.BadRequest, "Account.InvalidPassword");
    }

    [Fact]
    public async Task Signing_in_with_the_password_works_and_a_wrong_one_does_not()
    {
        var email = NewEmail();
        await PostAsync("/v1/auth/register", new { email, password = Password, declaresMinimumAge = true });

        var wrong = await PostAsync("/v1/auth/login", new { email, password = "not-the-password" });
        var right = await PostAsync("/v1/auth/login", new { email, password = Password });

        await AssertProblemAsync(wrong, HttpStatusCode.Unauthorized, "Account.InvalidCredentials");
        Assert.Equal(HttpStatusCode.OK, right.StatusCode);
    }

    [Fact]
    public async Task A_refresh_token_gives_new_working_tokens_and_a_forged_one_nothing()
    {
        var registered = await PostAsync("/v1/auth/register", new { email = NewEmail(), password = Password, declaresMinimumAge = true });
        var refreshToken = (await registered.Content.ReadFromJsonAsync<JsonElement>(Cancellation)).GetProperty("refreshToken").GetString();

        var renewed = await PostAsync("/v1/auth/refresh", new { refreshToken });
        var forged = await PostAsync("/v1/auth/refresh", new { refreshToken = "forged" });

        Assert.Equal(HttpStatusCode.OK, renewed.StatusCode);
        await GetMeAsync(await AccessTokenAsync(renewed));
        await AssertProblemAsync(forged, HttpStatusCode.Unauthorized, "Account.InvalidCredentials");
    }

    [Fact]
    public async Task A_provider_sign_in_creates_the_account_once_with_the_declaration()
    {
        var idToken = $"google-{Guid.NewGuid():N};{NewEmail()}";

        var undeclared = await PostAsync("/v1/auth/external/google", new { idToken, declaresMinimumAge = false });
        var created = await PostAsync("/v1/auth/external/google", new { idToken, declaresMinimumAge = true });
        var later = await PostAsync("/v1/auth/external/google", new { idToken, declaresMinimumAge = false });

        await AssertProblemAsync(undeclared, HttpStatusCode.BadRequest, "Account.AgeDeclarationRequired");
        Assert.Equal(HttpStatusCode.OK, created.StatusCode);
        Assert.Equal(HttpStatusCode.OK, later.StatusCode);
        Assert.Equal(
            (await GetMeAsync(await AccessTokenAsync(created))).GetProperty("accountId").GetString(),
            (await GetMeAsync(await AccessTokenAsync(later))).GetProperty("accountId").GetString());
    }

    [Fact]
    public async Task A_provider_sign_in_never_takes_over_an_account_with_the_same_email()
    {
        var email = NewEmail();
        await PostAsync("/v1/auth/register", new { email, password = Password, declaresMinimumAge = true });

        var external = await PostAsync("/v1/auth/external/google", new { idToken = $"google-{Guid.NewGuid():N};{email}", declaresMinimumAge = true });

        await AssertProblemAsync(external, HttpStatusCode.Conflict, "Account.EmailTaken");
    }

    [Theory]
    [InlineData("google", "not-a-google-token", HttpStatusCode.Unauthorized, "Account.InvalidExternalToken")]
    [InlineData("apple", "google-x", HttpStatusCode.NotFound, "Account.ProviderUnavailable")]
    public async Task Invalid_tokens_and_unknown_providers_are_refused(string provider, string idToken, HttpStatusCode status, string code)
    {
        var response = await PostAsync($"/v1/auth/external/{provider}", new { idToken, declaresMinimumAge = true });

        await AssertProblemAsync(response, status, code);
    }

    private static CancellationToken Cancellation => TestContext.Current.CancellationToken;

    private static string NewEmail() => $"{Guid.NewGuid():N}@example.com";

    private Task<HttpResponseMessage> PostAsync(string path, object body) =>
        Client.PostAsJsonAsync(new Uri(path, UriKind.Relative), body, Cancellation);

    private static async Task<string> AccessTokenAsync(HttpResponseMessage response) =>
        (await response.Content.ReadFromJsonAsync<JsonElement>(Cancellation)).GetProperty("accessToken").GetString()!;

    private async Task<JsonElement> GetMeAsync(string accessToken)
    {
        using var request = new HttpRequestMessage(HttpMethod.Get, "/v1/account/me");
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
        using var response = await Client.SendAsync(request, Cancellation);
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        return await response.Content.ReadFromJsonAsync<JsonElement>(Cancellation);
    }

    private static async Task AssertProblemAsync(HttpResponseMessage response, HttpStatusCode status, string code)
    {
        Assert.Equal(status, response.StatusCode);
        var problem = await response.Content.ReadFromJsonAsync<JsonElement>(Cancellation);
        Assert.Equal(code, problem.GetProperty("code").GetString());
    }
}
