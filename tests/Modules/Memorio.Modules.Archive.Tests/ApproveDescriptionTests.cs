using Memorio.Modules.Archive.Application.Commands.ApproveDescription;
using Memorio.Modules.Archive.Domain.Memories;
using Microsoft.Extensions.DependencyInjection;

namespace Memorio.Modules.Archive.Tests;

/// <summary>MEM-E2-ARCHIWUM-03 on a real database.</summary>
public sealed class ApproveDescriptionTests(ArchiveDatabase database)
{
    private static CancellationToken Cancellation => TestContext.Current.CancellationToken;

    [Fact]
    public async Task The_authors_text_is_kept_without_the_marker()
    {
        var account = Guid.CreateVersion7();
        var memory = await database.AddMemoryAsync(account);

        var approved = await ApproveAsync(memory.Id, account, "  W górach.  ");

        Assert.True(approved.IsSuccess);
        var stored = await StoredAsync(memory.Id);
        Assert.Equal("W górach.", stored.Description);
        Assert.Null(stored.DescriptionAiModel);
    }

    [Fact]
    public async Task A_text_from_an_ai_draft_keeps_the_marker_also_after_editing()
    {
        var account = Guid.CreateVersion7();
        var memory = await database.AddMemoryAsync(account);
        var draft = Guid.CreateVersion7();
        database.Neighbours.AddDraft(draft, account);

        await ApproveAsync(memory.Id, account, "Zmieniony szkic.", draft);

        var stored = await StoredAsync(memory.Id);
        Assert.Equal("test-model", stored.DescriptionAiModel);
        Assert.Equal("draft@1", stored.DescriptionAiPromptVersion);
    }

    [Fact]
    public async Task A_draft_of_someone_else_is_refused()
    {
        var account = Guid.CreateVersion7();
        var memory = await database.AddMemoryAsync(account);
        var draft = Guid.CreateVersion7();
        database.Neighbours.AddDraft(draft, Guid.CreateVersion7());

        var refused = await ApproveAsync(memory.Id, account, "Tekst.", draft);

        Assert.Equal(MemoryErrors.DraftNotFound, refused.Error);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("   ")]
    public async Task An_empty_text_is_refused_and_nothing_changes(string? text)
    {
        var account = Guid.CreateVersion7();
        var memory = await database.AddMemoryAsync(account);

        var refused = await ApproveAsync(memory.Id, account, text);

        Assert.Equal(MemoryErrors.InvalidDescription, refused.Error);
        Assert.Null((await StoredAsync(memory.Id)).Description);
    }

    [Fact]
    public async Task Someone_elses_memory_is_refused()
    {
        var memory = await database.AddMemoryAsync(Guid.CreateVersion7());

        var refused = await ApproveAsync(memory.Id, Guid.CreateVersion7(), "Tekst.");

        Assert.Equal(MemoryErrors.NoAccess, refused.Error);
    }

    private async Task<SharedKernel.Results.Result> ApproveAsync(MemoryId memory, Guid account, string? text, Guid? draft = null)
    {
        await using var scope = database.CreateScope();
        return await scope.ServiceProvider.GetRequiredService<ApproveDescriptionHandler>()
            .HandleAsync(new ApproveDescription(memory, account, text, draft), Cancellation);
    }

    private async Task<Memory> StoredAsync(MemoryId id)
    {
        await using var scope = database.CreateScope();
        return (await scope.ServiceProvider.GetRequiredService<IMemories>().FindAsync(id, Cancellation))!;
    }
}
