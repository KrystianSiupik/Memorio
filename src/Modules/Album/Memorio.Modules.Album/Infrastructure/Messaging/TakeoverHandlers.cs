using Memorio.Modules.Album.Application.Policies;
using Memorio.Modules.Album.Contracts;
using Memorio.Modules.Plans.Contracts;
using Microsoft.Extensions.DependencyInjection;

namespace Memorio.Modules.Album.Infrastructure.Messaging;

/// <summary>E165 in the worker: only resolves this module's policy and calls it (D-135, D-252).</summary>
public static class TakeoverReservedHandler
{
    public static Task Handle(TakeoverReservedEvent integrationEvent, IServiceProvider services, CancellationToken cancellationToken) =>
        services.GetRequiredService<TakeoverReservationDeadline>().HandleAsync(integrationEvent, cancellationToken);
}

/// <summary>E171 in the worker.</summary>
public static class PaidWhileOwnerlessHandler
{
    public static Task Handle(PaidWhileOwnerlessEvent integrationEvent, IServiceProvider services, CancellationToken cancellationToken) =>
        services.GetRequiredService<TakeOverWhenPaid>().HandleAsync(integrationEvent, cancellationToken);
}
