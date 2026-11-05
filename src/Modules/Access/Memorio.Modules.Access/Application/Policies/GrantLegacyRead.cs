using Memorio.Modules.Access.Domain.Audiences;
using Memorio.Modules.Succession.Contracts;
using Memorio.SharedKernel.Messaging;

namespace Memorio.Modules.Access.Application.Policies;

/// <summary>P31: whenever a private archive is shared with the contact named in advance, they may read it. Twice changes nothing.</summary>
internal sealed class GrantLegacyRead(IAudiences audiences, ILegacyContacts contacts, TimeProvider clock) : IIntegrationEventPolicy<PrivateArchiveSharedEvent>
{
    public async Task HandleAsync(PrivateArchiveSharedEvent integrationEvent, CancellationToken cancellationToken)
    {
        if (await contacts.ContactOfAsync(integrationEvent.OwnerAccountId, cancellationToken) is { } email)
        {
            await audiences.GrantLegacyAsync(LegacyGrant.Create(integrationEvent.OwnerAccountId, email, clock.GetUtcNow()), cancellationToken);
        }
    }
}
