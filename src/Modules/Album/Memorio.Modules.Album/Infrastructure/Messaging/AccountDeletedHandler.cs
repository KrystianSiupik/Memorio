using Memorio.Modules.Account.Contracts;
using Memorio.Modules.Album.Application.Policies;
using Microsoft.Extensions.DependencyInjection;

namespace Memorio.Modules.Album.Infrastructure.Messaging;

/// <summary>E7 in the worker: only resolves this module's policy and calls it (D-135, D-252).</summary>
public static class AccountDeletedHandler
{
    public static Task Handle(AccountDeleted integrationEvent, IServiceProvider services, CancellationToken cancellationToken) =>
        services.GetRequiredService<WithdrawContributionsWhenAccountDeleted>().HandleAsync(integrationEvent, cancellationToken);
}
