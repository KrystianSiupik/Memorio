using System.Collections.Concurrent;
using Memorio.Modules.AiNarration.Application.Commands.RequestDraft;
using Memorio.Modules.AiNarration.Application.Ports;
using Memorio.Modules.AiNarration.Contracts;
using Memorio.Modules.AiNarration.Domain.Drafts;
using Memorio.Modules.Limits.Contracts;
using Memorio.SharedKernel.Results;
using Microsoft.Extensions.DependencyInjection;

namespace Memorio.Modules.AiNarration.Tests;

/// <summary>MEM-E2-NARRACJA-AI-01 on a real database, with the provider and the allowance in memory.</summary>
public sealed class RequestDraftTests(AiNarrationDatabase database)
{
    private static CancellationToken Cancellation => TestContext.Current.CancellationToken;

    [Fact]
    public async Task A_draft_comes_with_what_marks_it_as_ai()
    {
        var account = Guid.CreateVersion7();
        var id = Guid.CreateVersion7();

        var draft = await RequestAsync(id, account);

        Assert.Equal("Wspomnienie.", draft.Value.Text);
        Assert.Equal("test-model", draft.Value.Model);
        Assert.Equal("memory-description-draft@1", draft.Value.PromptVersion);
        await using var scope = database.CreateScope();
        var read = await scope.ServiceProvider.GetRequiredService<IAiDraftReader>().FindAsync(id, account, Cancellation);
        Assert.Equal("test-model", read?.Model);
        Assert.Null(await scope.ServiceProvider.GetRequiredService<IAiDraftReader>().FindAsync(id, Guid.CreateVersion7(), Cancellation));
    }

    [Fact]
    public async Task Names_never_reach_the_provider()
    {
        var id = Guid.CreateVersion7();

        await RequestAsync(id, Guid.CreateVersion7(), material: id + " Anna w Zakopanem", names: [(true, "Anna"), (false, "Zakopanem")]);

        var sent = Assert.Single(database.Provider.Materials, material => material.Contains(id.ToString(), StringComparison.Ordinal));
        Assert.DoesNotContain("Anna", sent, StringComparison.Ordinal);
        Assert.DoesNotContain("Zakopanem", sent, StringComparison.Ordinal);
    }

    [Fact]
    public async Task Without_consent_nothing_goes_anywhere()
    {
        var id = Guid.CreateVersion7();

        var refused = await RequestAsync(id, Guid.CreateVersion7(), material: id.ToString(), consents: false);

        Assert.Equal(DraftErrors.ConsentRequired, refused.Error);
        Assert.DoesNotContain(database.Provider.Materials, material => material.Contains(id.ToString(), StringComparison.Ordinal));
    }

    [Theory]
    [InlineData(null)]
    [InlineData("   ")]
    public async Task Without_material_there_is_no_draft(string? material)
    {
        var refused = await RequestAsync(Guid.CreateVersion7(), Guid.CreateVersion7(), material: material);

        Assert.Equal(DraftErrors.InvalidMaterial, refused.Error);
    }

    [Fact]
    public async Task With_the_allowance_used_up_the_provider_is_not_called_and_nothing_is_stored()
    {
        var account = Guid.CreateVersion7();
        var id = Guid.CreateVersion7();
        database.Allowance.UsedUp[account] = true;

        var refused = await RequestAsync(id, account, material: id.ToString());

        Assert.Equal(DraftErrors.LimitReached, refused.Error);
        Assert.DoesNotContain(database.Provider.Materials, material => material.Contains(id.ToString(), StringComparison.Ordinal));
    }

    [Fact]
    public async Task When_the_provider_is_down_nothing_is_stored_and_the_retry_costs_no_more()
    {
        var account = Guid.CreateVersion7();
        var id = Guid.CreateVersion7();
        database.Provider.DownFor[id.ToString()] = true;

        var refused = await RequestAsync(id, account, material: id.ToString());

        Assert.Equal(TextGeneratorErrors.ProviderUnavailable, refused.Error);
        database.Provider.DownFor.TryRemove(id.ToString(), out _);

        var retried = await RequestAsync(id, account, material: id.ToString());

        Assert.True(retried.IsSuccess);
        Assert.Equal(1, database.Allowance.Consumed[id]);
    }

    [Fact]
    public async Task The_same_id_twice_gets_the_same_draft_and_one_provider_call()
    {
        var account = Guid.CreateVersion7();
        var id = Guid.CreateVersion7();

        await RequestAsync(id, account, material: id.ToString());
        var again = await RequestAsync(id, account, material: id.ToString());

        Assert.True(again.IsSuccess);
        Assert.Single(database.Provider.Materials, material => material.Contains(id.ToString(), StringComparison.Ordinal));
    }

    [Fact]
    public async Task An_id_of_someone_elses_draft_is_refused()
    {
        var id = Guid.CreateVersion7();
        await RequestAsync(id, Guid.CreateVersion7());

        var refused = await RequestAsync(id, Guid.CreateVersion7());

        Assert.Equal(DraftErrors.IdTaken, refused.Error);
    }

    private async Task<Result<MemoryDraft>> RequestAsync(
        Guid id, Guid account, string? material = "Byliśmy w górach.", bool consents = true, (bool IsPerson, string? Name)[]? names = null)
    {
        await using var scope = database.CreateScope();
        return await scope.ServiceProvider.GetRequiredService<RequestDraftHandler>()
            .HandleAsync(new RequestDraft(id, account, material, consents, names ?? []), Cancellation);
    }
}

internal sealed class FakeAllowance : IAiAllowance
{
    public ConcurrentDictionary<Guid, bool> UsedUp { get; } = new();

    public ConcurrentDictionary<Guid, int> Consumed { get; } = new();

    public Task<bool> TryConsumeAsync(Guid accountId, Guid operationKey, CancellationToken cancellationToken)
    {
        if (UsedUp.ContainsKey(accountId))
        {
            return Task.FromResult(false);
        }

        // One key is one use, as in the pool's ledger.
        Consumed[operationKey] = 1;
        return Task.FromResult(true);
    }
}

internal sealed class FakeProviderForDrafts : ITextGenerator
{
    private readonly ConcurrentBag<string> _materials = [];

    public IReadOnlyCollection<string> Materials => _materials;

    public ConcurrentDictionary<string, bool> DownFor { get; } = new();

    /// <summary>What the AI tells of a period: one sentence with its fact, one with none, one with a fact it was not given.</summary>
    public string SummaryAnswer { get; } =
        "W lipcu byliście w [PLACE_1]. [F1] Wszyscy byli bardzo szczęśliwi. Potem przyszło coś jeszcze. [F9]";

    /// <summary>Another answer for the material holding this text, so tests running at once do not change each other's.</summary>
    public ConcurrentDictionary<string, string> SummaryAnswerWhen { get; } = new();

    public Task<Result<ProviderText>> GenerateAsync(TextGenerationRequest request, CancellationToken cancellationToken)
    {
        if (DownFor.Keys.Any(request.Material.Contains))
        {
            return Task.FromResult<Result<ProviderText>>(TextGeneratorErrors.ProviderUnavailable);
        }

        _materials.Add(request.Material);
        return Task.FromResult<Result<ProviderText>>(new ProviderText(
            request.Prompt.Id != "period-summary" ? "Wspomnienie."
                : SummaryAnswerWhen.FirstOrDefault(answer => request.Material.Contains(answer.Key, StringComparison.Ordinal)).Value ?? SummaryAnswer,
            "test-model"));
    }
}
