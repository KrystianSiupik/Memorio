using Memorio.Modules.Account.Contracts;
using Memorio.Modules.Archive.Application.Commands.DeleteAccountContent;
using Memorio.Modules.Archive.Domain.Memories;
using Memorio.SharedKernel.Messaging;

namespace Memorio.Modules.Archive.Application.Policies;

/// <summary>P35: whenever an account is deleted, delete what the person recorded.</summary>
internal sealed class DeleteContentWhenAccountDeleted(DeleteAccountContentHandler deleteContent, IMemories memories)
    : IIntegrationEventPolicy<AccountDeleted>
{
    public async Task HandleAsync(AccountDeleted integrationEvent, CancellationToken cancellationToken)
    {
        var applied = await memories.FindAppliedAsync(integrationEvent.AccountId, cancellationToken);
        if (applied?.Sequence > integrationEvent.Sequence)
        {
            return;
        }

        await deleteContent.HandleAsync(new DeleteAccountContent(integrationEvent.AccountId), cancellationToken);

        if (applied is null)
        {
            memories.Add(new AppliedAccountEvent(integrationEvent.AccountId, integrationEvent.Sequence));
        }
        else
        {
            applied.MoveTo(integrationEvent.Sequence);
        }

        await memories.SaveAsync(cancellationToken);
    }
}
