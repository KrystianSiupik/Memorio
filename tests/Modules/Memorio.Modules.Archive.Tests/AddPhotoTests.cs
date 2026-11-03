using Memorio.Modules.Archive.Application.Commands.AddPhoto;
using Memorio.Modules.Archive.Contracts;
using Memorio.Modules.Archive.Domain.Memories;
using Memorio.SharedKernel.Storage;
using Microsoft.Extensions.DependencyInjection;

namespace Memorio.Modules.Archive.Tests;

/// <summary>MEM-E2-ARCHIWUM-02 on a real database.</summary>
public sealed class AddPhotoTests(ArchiveDatabase database)
{
    private static CancellationToken Cancellation => TestContext.Current.CancellationToken;

    [Fact]
    public async Task An_upload_becomes_a_photo_and_takes_its_space()
    {
        var account = Guid.CreateVersion7();
        var memory = await database.AddMemoryAsync(account);
        var photo = Guid.CreateVersion7();

        var added = await AddAsync(memory.Id, account, photo, database.Storage.AddUpload(500).Value);

        Assert.True(added.IsSuccess);
        Assert.Equal(500, database.Neighbours.Reserved[photo]);
        Assert.Equal(1, (await SummarizeAsync(account)).Photos);
    }

    [Fact]
    public async Task A_full_pool_refuses_stores_nothing_and_removes_the_file()
    {
        var account = Guid.CreateVersion7();
        var memory = await database.AddMemoryAsync(account);
        database.Neighbours.WithFullPool(account);
        var photo = Guid.CreateVersion7();

        var refused = await AddAsync(memory.Id, account, photo, database.Storage.AddUpload(500).Value);

        Assert.Equal(MemoryErrors.SpaceLimitExceeded, refused.Error);
        Assert.Equal(0, (await SummarizeAsync(account)).Photos);
        Assert.Contains(new StorageKey($"photos/{photo}/original"), database.Storage.Deleted);
    }

    [Fact]
    public async Task The_same_photo_again_has_one_effect()
    {
        var account = Guid.CreateVersion7();
        var memory = await database.AddMemoryAsync(account);
        var photo = Guid.CreateVersion7();
        var upload = database.Storage.AddUpload(500).Value;

        await AddAsync(memory.Id, account, photo, upload);
        var again = await AddAsync(memory.Id, account, photo, upload);

        Assert.True(again.IsSuccess);
        Assert.Equal(1, (await SummarizeAsync(account)).Photos);
    }

    [Fact]
    public async Task Someone_elses_memory_and_one_that_does_not_exist_look_the_same()
    {
        var memory = await database.AddMemoryAsync(Guid.CreateVersion7());
        var stranger = Guid.CreateVersion7();

        var foreign = await AddAsync(memory.Id, stranger, Guid.CreateVersion7(), database.Storage.AddUpload(1).Value);
        var missing = await AddAsync(new MemoryId(Guid.CreateVersion7()), stranger, Guid.CreateVersion7(), database.Storage.AddUpload(1).Value);

        Assert.Equal(MemoryErrors.NoAccess, foreign.Error);
        Assert.Equal(MemoryErrors.NoAccess, missing.Error);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("photos/someone-elses/original")]
    [InlineData("uploads/../photos/x")]
    public async Task What_is_not_an_upload_is_refused(string? key)
    {
        var account = Guid.CreateVersion7();
        var memory = await database.AddMemoryAsync(account);

        var refused = await AddAsync(memory.Id, account, Guid.CreateVersion7(), key);

        Assert.Equal(MemoryErrors.InvalidUpload, refused.Error);
    }

    [Fact]
    public async Task An_upload_that_expired_is_refused()
    {
        var account = Guid.CreateVersion7();
        var memory = await database.AddMemoryAsync(account);

        var refused = await AddAsync(memory.Id, account, Guid.CreateVersion7(), $"uploads/{Guid.CreateVersion7()}");

        Assert.Equal(MemoryErrors.UploadMissing, refused.Error);
    }

    [Theory]
    [InlineData("application/pdf")]
    [InlineData("image/svg+xml")]
    [InlineData(null)]
    public async Task Only_photos_can_be_uploaded(string? contentType)
    {
        await using var scope = database.CreateScope();
        var refused = await scope.ServiceProvider.GetRequiredService<StartUploadHandler>().HandleAsync(contentType, Cancellation);

        Assert.Equal(MemoryErrors.InvalidContentType, refused.Error);
    }

    private async Task<SharedKernel.Results.Result> AddAsync(MemoryId memory, Guid account, Guid photo, string? upload)
    {
        await using var scope = database.CreateScope();
        return await scope.ServiceProvider.GetRequiredService<AddPhotoHandler>().HandleAsync(new AddPhoto(memory, account, photo, upload), Cancellation);
    }

    private async Task<AccountContentSummary> SummarizeAsync(Guid account)
    {
        await using var scope = database.CreateScope();
        return await scope.ServiceProvider.GetRequiredService<IAccountContentReader>().SummarizeAsync(account, Cancellation);
    }
}
