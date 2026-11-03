using System.Net;
using System.Net.Http.Json;
using System.Text.Json;

namespace Memorio.Modules.Account.Tests;

/// <summary>
/// MEM-E1-KONTO-06 on a real database: person x resource. Everyone sees the numbers of their own account
/// and no way exists to ask about another one: the query takes no account id, only the session.
/// </summary>
public sealed class AccountDeletionImpactTests(AccountHost host)
{
    [Fact]
    public async Task Each_person_gets_their_own_numbers_and_nobody_elses()
    {
        var ola = await host.RegisterAsync();
        var jan = await host.RegisterAsync();
        await host.AddMemoryAsync(ola.AccountId, photos: 2);
        await host.AddMemoryAsync(ola.AccountId);
        await host.AddMemoryAsync(jan.AccountId, photos: 7);
        await host.AddAlbumAsync(ola.AccountId);
        await host.AddAlbumAsync(ola.AccountId, successorAccountId: jan.AccountId);
        await host.AddAlbumAsync(jan.AccountId, null, ola.AccountId);

        var seenByOla = await ImpactAsync(ola.AccessToken);
        var seenByJan = await ImpactAsync(jan.AccessToken);

        Assert.Equal((2, 2, 2, 1, 1), seenByOla);
        Assert.Equal((1, 7, 1, 1, 0), seenByJan);
    }

    [Fact]
    public async Task A_new_account_has_nothing_to_lose()
    {
        var account = await host.RegisterAsync();

        Assert.Equal((0, 0, 0, 0, 0), await ImpactAsync(account.AccessToken));
    }

    [Fact]
    public async Task Without_signing_in_there_is_no_answer()
    {
        using var response = await host.Client.GetAsync(new Uri("/v1/account/deletion-impact", UriKind.Relative), TestContext.Current.CancellationToken);

        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    private async Task<(int, int, int, int, int)> ImpactAsync(string accessToken)
    {
        using var response = await host.SendAsync(HttpMethod.Get, "/v1/account/deletion-impact", accessToken);
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var impact = await response.Content.ReadFromJsonAsync<JsonElement>(TestContext.Current.CancellationToken);
        return (
            impact.GetProperty("memories").GetInt32(),
            impact.GetProperty("photos").GetInt32(),
            impact.GetProperty("ownAlbums").GetInt32(),
            impact.GetProperty("ownAlbumsWithoutSuccessor").GetInt32(),
            impact.GetProperty("otherPeoplesAlbums").GetInt32());
    }
}
