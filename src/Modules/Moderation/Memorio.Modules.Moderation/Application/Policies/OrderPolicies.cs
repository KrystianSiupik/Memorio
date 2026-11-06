using Memorio.Modules.Moderation.Application.Commands.Orders;
using Memorio.Modules.Moderation.Contracts;
using Memorio.Modules.Moderation.Domain.Orders;
using Memorio.SharedKernel.Messaging;

namespace Memorio.Modules.Moderation.Application.Policies;

/// <summary>P19: whenever an order to remove content comes, hide it at once. Twice changes nothing.</summary>
internal sealed class HideWhenRemovalOrdered(OrderHandler orders) : IIntegrationEventPolicy<RemovalOrderReceivedEvent>
{
    public async Task HandleAsync(RemovalOrderReceivedEvent integrationEvent, CancellationToken cancellationToken)
    {
        var hidden = await orders.HideNowAsync(new OrderId(integrationEvent.OrderId), cancellationToken);
        if (hidden.IsFailure)
        {
            throw new InvalidOperationException(hidden.Error.Code);
        }
    }
}

/// <summary>P20: whenever an order to preserve or produce data comes, secure the account's data first (L18). Twice changes nothing.</summary>
internal sealed class SecureWhenDataOrdered(OrderHandler orders) : IIntegrationEventPolicy<DataOrderReceivedEvent>
{
    public async Task HandleAsync(DataOrderReceivedEvent integrationEvent, CancellationToken cancellationToken)
    {
        var secured = await orders.SecureAsync(new OrderId(integrationEvent.OrderId), cancellationToken);
        if (secured.IsFailure)
        {
            throw new InvalidOperationException(secured.Error.Code);
        }
    }
}
