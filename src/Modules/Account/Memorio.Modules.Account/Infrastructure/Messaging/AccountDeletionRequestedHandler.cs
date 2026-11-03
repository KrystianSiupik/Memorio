using Memorio.Modules.Account.Contracts;
using Memorio.SharedKernel.Messaging;
using Microsoft.Extensions.DependencyInjection;

namespace Memorio.Modules.Account.Infrastructure.Messaging;

/// <summary>E4 in the worker: only resolves the policy and calls it (D-135).</summary>
public static class AccountDeletionRequestedHandler
{
    public static Task Handle(AccountDeletionRequested requested, IServiceProvider services, CancellationToken cancellationToken) =>
        services.GetRequiredService<IIntegrationEventPolicy<AccountDeletionRequested>>().HandleAsync(requested, cancellationToken);
}
