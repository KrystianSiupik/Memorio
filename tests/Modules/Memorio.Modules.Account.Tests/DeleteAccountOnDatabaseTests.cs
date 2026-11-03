using System.Net;
using Memorio.Modules.Account.Application.Commands.DeleteAccount;
using Memorio.Modules.Account.Domain.Accounts;
using Memorio.Modules.Account.Domain.Deletions;
using Microsoft.Extensions.DependencyInjection;

namespace Memorio.Modules.Account.Tests;

/// <summary>MEM-E1-KONTO-04 on a real database.</summary>
public sealed class DeleteAccountOnDatabaseTests(AccountHost host)
{
    private static CancellationToken Cancellation => TestContext.Current.CancellationToken;

    [Fact]
    public async Task After_deleting_the_person_cannot_open_the_account_and_the_email_is_free()
    {
        var account = await host.RegisterAsync();
        var windowEnd = await host.RequestDeletionAsync(account.AccessToken);

        var deleted = await DeleteAsync(account.AccountId, windowEnd);

        Assert.True(deleted.IsSuccess);
        using var me = await host.SendAsync(HttpMethod.Get, "/v1/account/me", account.AccessToken);
        Assert.Equal(HttpStatusCode.Unauthorized, me.StatusCode);
    }

    [Fact]
    public async Task Parallel_deletions_of_one_window_have_one_winner()
    {
        var account = await host.RegisterAsync();
        var windowEnd = await host.RequestDeletionAsync(account.AccessToken);

        var results = await Task.WhenAll(Enumerable.Range(0, 5).Select(_ => DeleteAsync(account.AccountId, windowEnd)));

        Assert.Single(results, result => result.IsSuccess);
        Assert.All(results.Where(result => result.IsFailure), result => Assert.Equal(AccountErrors.DeletionAlreadyDecided, result.Error));
    }

    private async Task<SharedKernel.Results.Result> DeleteAsync(Guid accountId, DateTimeOffset at)
    {
        await using var scope = host.Api.CreateAsyncScope();
        return await scope.ServiceProvider.GetRequiredService<DeleteAccountHandler>()
            .HandleAsync(new DeleteAccount(new AccountId(accountId), 1, at), Cancellation);
    }
}
