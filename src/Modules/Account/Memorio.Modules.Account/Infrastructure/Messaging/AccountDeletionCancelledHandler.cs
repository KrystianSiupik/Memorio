using Memorio.Modules.Account.Contracts;
using Memorio.SharedKernel.Messaging;
using Microsoft.Extensions.DependencyInjection;

namespace Memorio.Modules.Account.Infrastructure.Messaging;

/// <summary>E8 in the worker: only resolves the policy and calls it (D-135).</summary>
public static class AccountDeletionCancelledHandler
{
    public static Task Handle(AccountDeletionCancelled cancelled, IServiceProvider services, CancellationToken cancellationToken) =>
        services.GetRequiredService<IIntegrationEventPolicy<AccountDeletionCancelled>>().HandleAsync(cancelled, cancellationToken);
}
