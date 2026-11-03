using System.Net;
using System.Net.Http.Json;
using System.Text.Json;

namespace Memorio.Modules.Account.Tests;

/// <summary>MEM-E1-KONTO-03 on a real database, with the outbox and the worker.</summary>
public sealed class AccountDeletionRequestTests(AccountHost host)
{
    private static CancellationToken Cancellation => TestContext.Current.CancellationToken;

    [Fact]
    public async Task A_request_opens_the_window_and_the_event_reaches_the_worker()
    {
        var account = await host.RegisterAsync();

        var windowEnd = await RequestAsync(account.AccessToken);

        Assert.Equal(windowEnd, (await WindowEndAsync(account.AccessToken))!.Value, TimeSpan.FromMilliseconds(1));
        Assert.True(await AccountHost.EventuallyAsync(() => Task.FromResult(host.Recorded.Requests(account.AccountId).Count == 1)));
    }

    [Fact]
    public async Task Asking_twice_in_a_row_or_at_once_gives_one_window_and_one_event()
    {
        var account = await host.RegisterAsync();

        var together = await Task.WhenAll(Enumerable.Range(0, 5).Select(_ => RequestAsync(account.AccessToken)));
        var later = await RequestAsync(account.AccessToken);

        // PostgreSQL keeps microseconds: the winner's own answer can carry more digits than the ones read back.
        Assert.Single(together.Append(later).Select(windowEnd => windowEnd.AddTicks(-(windowEnd.Ticks % 10))).Distinct());
        Assert.True(await AccountHost.EventuallyAsync(() => Task.FromResult(host.Recorded.Requests(account.AccountId).Count == 1)));
        await Task.Delay(TimeSpan.FromSeconds(3), Cancellation);
        Assert.Single(host.Recorded.Requests(account.AccountId));
    }

    [Fact]
    public async Task An_album_without_a_successor_holds_it_back_until_the_person_agrees_to_its_expiry()
    {
        var account = await host.RegisterAsync();
        await host.AddAlbumAsync(account.AccountId);

        using var held = await host.SendAsync(HttpMethod.Post, "/v1/account/deletion", account.AccessToken, new { consentsToAlbumExpiry = false });
        var problem = await held.Content.ReadFromJsonAsync<JsonElement>(Cancellation);

        Assert.Equal(HttpStatusCode.Conflict, held.StatusCode);
        Assert.Equal("Account.AlbumWithoutSuccessor", problem.GetProperty("code").GetString());
        Assert.Null(await WindowEndAsync(account.AccessToken));

        using var agreed = await host.SendAsync(HttpMethod.Post, "/v1/account/deletion", account.AccessToken, new { consentsToAlbumExpiry = true });
        Assert.Equal(HttpStatusCode.OK, agreed.StatusCode);
    }

    private async Task<DateTimeOffset> RequestAsync(string accessToken)
    {
        using var response = await host.SendAsync(HttpMethod.Post, "/v1/account/deletion", accessToken, new { consentsToAlbumExpiry = false });
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        return (await response.Content.ReadFromJsonAsync<JsonElement>(Cancellation)).GetProperty("windowEndsAt").GetDateTimeOffset();
    }

    private async Task<DateTimeOffset?> WindowEndAsync(string accessToken)
    {
        using var response = await host.SendAsync(HttpMethod.Get, "/v1/account/deletion", accessToken);
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var windowEnd = (await response.Content.ReadFromJsonAsync<JsonElement>(Cancellation)).GetProperty("windowEndsAt");
        return windowEnd.ValueKind == JsonValueKind.Null ? null : windowEnd.GetDateTimeOffset();
    }
}
