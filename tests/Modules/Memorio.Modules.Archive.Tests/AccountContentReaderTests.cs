using Memorio.Modules.Archive.Contracts;
using Microsoft.Extensions.DependencyInjection;

namespace Memorio.Modules.Archive.Tests;

/// <summary>MEM-E1-ARCHIWUM-02 on a real database: what goes with an account, and only that account.</summary>
public sealed class AccountContentReaderTests(ArchiveDatabase database)
{
    [Fact]
    public async Task Counts_the_memories_and_photos_of_the_account_and_nobody_else()
    {
        var account = Guid.CreateVersion7();
        await database.AddMemoryAsync(account, photos: 2);
        await database.AddMemoryAsync(account, photos: 1);
        await database.AddMemoryAsync(account);
        await database.AddMemoryAsync(Guid.CreateVersion7(), photos: 5);

        Assert.Equal(new AccountContentSummary(Memories: 3, Photos: 3), await SummarizeAsync(account));
    }

    [Fact]
    public async Task An_account_without_content_has_zeros()
    {
        Assert.Equal(new AccountContentSummary(0, 0), await SummarizeAsync(Guid.CreateVersion7()));
    }

    private async Task<AccountContentSummary> SummarizeAsync(Guid account)
    {
        await using var scope = database.CreateScope();
        return await scope.ServiceProvider.GetRequiredService<IAccountContentReader>()
            .SummarizeAsync(account, TestContext.Current.CancellationToken);
    }
}
