using Memorio.Modules.Access.Domain.Audiences;
using Memorio.Modules.Account.Contracts;
using Memorio.SharedKernel.Messaging;

namespace Memorio.Modules.Access.Application.Policies;

/// <summary>A deleted account leaves the lists of viewers and blocks; doing it twice changes nothing.</summary>
internal sealed class ForgetDeletedAccount(IAudiences audiences) : IIntegrationEventPolicy<AccountDeleted>
{
    public Task HandleAsync(AccountDeleted integrationEvent, CancellationToken cancellationToken) =>
        audiences.ForgetAsync(integrationEvent.AccountId, cancellationToken);
}
