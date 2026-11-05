using Memorio.Modules.Account.Contracts;
using Memorio.Modules.Succession.Application.Policies;
using Memorio.Modules.Succession.Contracts;
using Microsoft.Extensions.DependencyInjection;

namespace Memorio.Modules.Succession.Infrastructure.Messaging;

/// <summary>E121 in the worker: only resolves this module's policy and calls it (D-135, D-252).</summary>
public static class DeathConfirmedHandler
{
    public static Task Handle(DeathConfirmedEvent integrationEvent, IServiceProvider services, CancellationToken cancellationToken) =>
        services.GetRequiredService<ActOnConfirmedDeath>().HandleAsync(integrationEvent, cancellationToken);
}

/// <summary>E7 in the worker.</summary>
public static class AccountDeletedHandler
{
    public static Task Handle(AccountDeleted integrationEvent, IServiceProvider services, CancellationToken cancellationToken) =>
        services.GetRequiredService<OfferAlbumsWhenAccountDeleted>().HandleAsync(integrationEvent, cancellationToken);
}

/// <summary>The offer in the worker.</summary>
public static class TakeoverOfferedHandler
{
    public static Task Handle(TakeoverOfferedEvent integrationEvent, IServiceProvider services, CancellationToken cancellationToken) =>
        services.GetRequiredService<AnswerDeadline>().HandleAsync(integrationEvent, cancellationToken);
}

/// <summary>E123 in the worker.</summary>
public static class AlbumTransferredHandler
{
    public static Task Handle(AlbumTransferredEvent integrationEvent, IServiceProvider services, CancellationToken cancellationToken) =>
        services.GetRequiredService<ApplyCaseToAlbum>().HandleAsync(integrationEvent, cancellationToken);
}

/// <summary>E124 in the worker.</summary>
public static class TakeoverRefusedHandler
{
    public static Task Handle(TakeoverRefusedEvent integrationEvent, IServiceProvider services, CancellationToken cancellationToken) =>
        services.GetRequiredService<ApplyCaseToAlbum>().HandleAsync(integrationEvent, cancellationToken);
}
