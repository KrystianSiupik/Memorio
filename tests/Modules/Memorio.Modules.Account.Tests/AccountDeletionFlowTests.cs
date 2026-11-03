using System.Net;
using Memorio.Modules.Account.Application.Policies;
using Memorio.Modules.Archive.Contracts;
using Microsoft.Extensions.DependencyInjection;

namespace Memorio.Modules.Account.Tests;

/// <summary>
/// E1 across modules on a real database, with the outbox and the worker's queues: from asking for the
/// deletion, through the deadline, to the account and everything the person recorded being gone.
/// </summary>
public sealed class AccountDeletionFlowTests(AccountHost host)
{
    private static CancellationToken Cancellation => TestContext.Current.CancellationToken;

    [Fact]
    public async Task After_the_window_the_account_and_its_content_are_gone_and_others_stay()
    {
        var person = await host.RegisterAsync();
        var other = await host.RegisterAsync();
        await host.AddMemoryAsync(person.AccountId, photos: 2);
        await host.AddMemoryAsync(other.AccountId, photos: 1);

        await host.RequestDeletionAsync(person.AccessToken);
        Assert.True(await AccountHost.EventuallyAsync(
            () => Task.FromResult(host.Deadlines.Find(AccountDeletionWindowDeadline.JobName, person.AccountId.ToString()) is not null)));
        Assert.Equal(new AccountContentSummary(1, 2), await ContentAsync(person.AccountId));

        await FakeDeadlines.FireAsync(
            host.Worker,
            host.Deadlines.Find(AccountDeletionWindowDeadline.JobName, person.AccountId.ToString())!,
            Cancellation);

        Assert.True(await AccountHost.EventuallyAsync(async () => await ContentAsync(person.AccountId) == new AccountContentSummary(0, 0)));
        Assert.Equal(new AccountContentSummary(1, 1), await ContentAsync(other.AccountId));
        using var gone = await host.SendAsync(HttpMethod.Get, "/v1/account/me", person.AccessToken);
        using var stays = await host.SendAsync(HttpMethod.Get, "/v1/account/me", other.AccessToken);
        Assert.Equal(HttpStatusCode.Unauthorized, gone.StatusCode);
        Assert.Equal(HttpStatusCode.OK, stays.StatusCode);
    }

    private async Task<AccountContentSummary> ContentAsync(Guid accountId)
    {
        await using var scope = host.Api.CreateAsyncScope();
        return await scope.ServiceProvider.GetRequiredService<IAccountContentReader>().SummarizeAsync(accountId, Cancellation);
    }
}
