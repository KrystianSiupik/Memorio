using System.Collections.Concurrent;
using Memorio.Modules.Account.Contracts;
using Memorio.SharedKernel.Messaging;
using Microsoft.Extensions.DependencyInjection;

namespace Memorio.Modules.Account.Tests;

/// <summary>What reached the worker, by account: another module would see the same.</summary>
public sealed class RecordedEvents
{
    private readonly ConcurrentBag<IIntegrationEvent> _events = [];

    public void Record(IIntegrationEvent integrationEvent) => _events.Add(integrationEvent);

    public IReadOnlyList<AccountDeletionRequested> Requests(Guid accountId) =>
        [.. _events.OfType<AccountDeletionRequested>().Where(item => item.AccountId == accountId)];

    public IReadOnlyList<AccountDeleted> Deletions(Guid accountId) =>
        [.. _events.OfType<AccountDeleted>().Where(item => item.AccountId == accountId)];
}

/// <summary>Stands for another module listening to E7.</summary>
public static class RecordAccountDeletedHandler
{
    public static void Handle(AccountDeleted deleted, IServiceProvider services) =>
        services.GetRequiredService<RecordedEvents>().Record(deleted);
}

/// <summary>Stands for another module listening to E4.</summary>
public static class RecordAccountDeletionRequestedHandler
{
    public static void Handle(AccountDeletionRequested requested, IServiceProvider services) =>
        services.GetRequiredService<RecordedEvents>().Record(requested);
}
