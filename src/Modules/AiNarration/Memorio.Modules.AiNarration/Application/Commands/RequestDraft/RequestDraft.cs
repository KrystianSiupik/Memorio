using Memorio.Modules.AiNarration.Application.AiTexts;
using Memorio.Modules.AiNarration.Application.Ports;
using Memorio.Modules.AiNarration.Domain.AiTexts;
using Memorio.Modules.AiNarration.Domain.Drafts;
using Memorio.Modules.AiNarration.Domain.Pseudonymization;
using Memorio.Modules.Limits.Contracts;
using Memorio.SharedKernel.Results;

namespace Memorio.Modules.AiNarration.Application.Commands.RequestDraft;

/// <summary>C15: a person asks for a draft of a description. The id comes from the client: asking again gets the same draft.</summary>
/// <param name="Names">People and places in the material; they never reach the provider (QS8).</param>
internal sealed record RequestDraft(Guid Id, Guid AccountId, string? Material, bool ConsentsToAi, IReadOnlyCollection<(bool IsPerson, string? Name)> Names);

/// <summary>
/// The allowance is used before the provider is called, keyed by the draft, so nobody over it causes a paid
/// call. A provider failure keeps it used; the person retries under the same id and it costs nothing more (D-226).
/// </summary>
internal sealed class RequestDraftHandler(IMemoryDrafts drafts, IAiAllowance allowance, AiTextService writer, IPromptCatalog prompts)
{
    public async Task<Result<MemoryDraft>> HandleAsync(RequestDraft command, CancellationToken cancellationToken)
    {
        if (!command.ConsentsToAi)
        {
            return DraftErrors.ConsentRequired;
        }

        if (string.IsNullOrWhiteSpace(command.Material) || command.Material.Length > AiGeneratedText.MaxLength)
        {
            return DraftErrors.InvalidMaterial;
        }

        var names = new List<SensitiveName>();
        foreach (var (isPerson, name) in command.Names)
        {
            var sensitive = SensitiveName.Create(isPerson ? SensitiveNameKind.Person : SensitiveNameKind.Place, name);
            if (sensitive.IsFailure)
            {
                return sensitive.Error;
            }

            names.Add(sensitive.Value);
        }

        if (await drafts.FindAsync(command.Id, cancellationToken) is { } existing)
        {
            return existing.AccountId == command.AccountId ? existing : DraftErrors.IdTaken;
        }

        if (!await allowance.TryConsumeAsync(command.AccountId, command.Id, cancellationToken))
        {
            return DraftErrors.LimitReached;
        }

        var text = await writer.WriteAsync(prompts.MemoryDescriptionDraft, command.Material, names, cancellationToken);
        if (text.IsFailure)
        {
            return text.Error;
        }

        return await drafts.AddOrFindAsync(MemoryDraft.From(command.Id, command.AccountId, text.Value), cancellationToken);
    }
}
