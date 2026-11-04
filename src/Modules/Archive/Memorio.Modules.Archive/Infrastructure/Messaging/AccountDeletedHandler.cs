using Memorio.Modules.Account.Contracts;
using Memorio.Modules.Archive.Application.Policies;
using Microsoft.Extensions.DependencyInjection;

namespace Memorio.Modules.Archive.Infrastructure.Messaging;

/// <summary>E7 in the worker: only resolves this module's policy and calls it (D-135, D-252).</summary>
public static class AccountDeletedHandler
{
    public static Task Handle(AccountDeleted deleted, IServiceProvider services, CancellationToken cancellationToken) =>
        services.GetRequiredService<DeleteContentWhenAccountDeleted>().HandleAsync(deleted, cancellationToken);
}
