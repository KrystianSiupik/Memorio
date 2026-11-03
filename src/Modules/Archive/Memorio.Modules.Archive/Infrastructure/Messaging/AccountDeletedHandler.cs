using Memorio.Modules.Account.Contracts;
using Memorio.SharedKernel.Messaging;
using Microsoft.Extensions.DependencyInjection;

namespace Memorio.Modules.Archive.Infrastructure.Messaging;

/// <summary>E7 in the worker: only resolves the policy and calls it (D-135).</summary>
public static class AccountDeletedHandler
{
    public static Task Handle(AccountDeleted deleted, IServiceProvider services, CancellationToken cancellationToken) =>
        services.GetRequiredService<IIntegrationEventPolicy<AccountDeleted>>().HandleAsync(deleted, cancellationToken);
}
