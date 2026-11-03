using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using Memorio.Modules.Account.Application.Commands.CancelAccountDeletion;
using Memorio.Modules.Account.Application.Commands.DeleteAccount;
using Memorio.Modules.Account.Domain.Accounts;
using Microsoft.Extensions.DependencyInjection;

namespace Memorio.Modules.Account.Tests;

/// <summary>MEM-E1-KONTO-05 on a real database.</summary>
public sealed class CancelAccountDeletionOnDatabaseTests(AccountHost host)
{
    private static CancellationToken Cancellation => TestContext.Current.CancellationToken;

    [Fact]
    public async Task Cancelling_closes_the_window_and_the_account_keeps_working()
    {
        var account = await host.RegisterAsync();
        await host.RequestDeletionAsync(account.AccessToken);

        using var cancelled = await host.SendAsync(HttpMethod.Delete, "/v1/account/deletion", account.AccessToken);
        using var status = await host.SendAsync(HttpMethod.Get, "/v1/account/deletion", account.AccessToken);
        using var me = await host.SendAsync(HttpMethod.Get, "/v1/account/me", account.AccessToken);

        Assert.Equal(HttpStatusCode.NoContent, cancelled.StatusCode);
        Assert.Equal(JsonValueKind.Null, (await status.Content.ReadFromJsonAsync<JsonElement>(Cancellation)).GetProperty("windowEndsAt").ValueKind);
        Assert.Equal(HttpStatusCode.OK, me.StatusCode);
    }

    [Fact]
    public async Task Without_a_request_cancelling_answers_not_found()
    {
        var account = await host.RegisterAsync();

        using var response = await host.SendAsync(HttpMethod.Delete, "/v1/account/deletion", account.AccessToken);

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        Assert.Equal("Account.NoDeletionRequested", (await response.Content.ReadFromJsonAsync<JsonElement>(Cancellation)).GetProperty("code").GetString());
    }

    [Fact]
    public async Task Cancelling_and_deleting_in_parallel_have_one_winner()
    {
        for (var round = 0; round < 10; round++)
        {
            var account = await host.RegisterAsync();
            var windowEnd = await host.RequestDeletionAsync(account.AccessToken);
            var id = new AccountId(account.AccountId);

            var deleting = RunAsync(services => services.GetRequiredService<DeleteAccountHandler>()
                .HandleAsync(new DeleteAccount(id, 1, windowEnd), Cancellation));
            var cancelling = RunAsync(services => services.GetRequiredService<CancelAccountDeletionHandler>()
                .HandleAsync(new CancelAccountDeletion(id), Cancellation));
            var results = await Task.WhenAll(deleting, cancelling);

            Assert.Single(results, result => result.IsSuccess);
            Assert.Equal(AccountErrors.DeletionAlreadyDecided, Assert.Single(results, result => result.IsFailure).Error);
            using var me = await host.SendAsync(HttpMethod.Get, "/v1/account/me", account.AccessToken);
            Assert.Equal(results[0].IsSuccess ? HttpStatusCode.Unauthorized : HttpStatusCode.OK, me.StatusCode);
        }
    }

    private async Task<SharedKernel.Results.Result> RunAsync(Func<IServiceProvider, Task<SharedKernel.Results.Result>> command)
    {
        await using var scope = host.Api.CreateAsyncScope();
        return await command(scope.ServiceProvider);
    }
}
