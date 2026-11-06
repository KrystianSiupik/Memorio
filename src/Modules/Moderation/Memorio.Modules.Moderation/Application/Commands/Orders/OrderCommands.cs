using Memorio.Modules.Access.Contracts;
using Memorio.Modules.Album.Contracts;
using Memorio.Modules.Archive.Contracts;
using Memorio.Modules.Moderation.Domain.Orders;
using Memorio.Modules.Moderation.Domain.Reports;
using Memorio.SharedKernel.Results;

namespace Memorio.Modules.Moderation.Application.Commands.Orders;

/// <summary>An order as the service entered it (D-312). The id is the service's: entering it again changes nothing.</summary>
internal sealed record ReceiveOrder(OrderId Id, string? Kind, string? Authority, string? Reference, string? ResourceKind, Guid? ResourceId, Guid? AccountId, bool Confidential, string? ReceivedBy);

internal sealed class OrderHandler(IOrders orders, IAlbumContent memories, IAlbumMembership albums, IContentHiding hiding, TimeProvider clock)
{
    public async Task<Result> HandleAsync(ReceiveOrder command, CancellationToken cancellationToken)
    {
        if (await orders.FindAsync(command.Id, cancellationToken) is not null)
        {
            return Result.Success();
        }

        var order = AuthorityOrder.Receive(
            command.Id, command.Kind, command.Authority, command.Reference, command.ResourceKind, command.ResourceId, command.AccountId, command.Confidential, command.ReceivedBy, clock.GetUtcNow());
        if (order.IsFailure)
        {
            return order.Error;
        }

        // Refused only when the same order went in at the same moment: it is in either way.
        orders.Add(order.Value);
        await orders.SaveAsync(cancellationToken);
        return Result.Success();
    }

    /// <summary>C113: through the write port, which confirms (MEM-E8-DOSTEP-02); no confirmation fails the delivery, and the retry asks again.</summary>
    public async Task<Result> HideNowAsync(OrderId id, CancellationToken cancellationToken)
    {
        if (await orders.FindAsync(id, cancellationToken) is not { Kind: OrderKind.RemoveContent } order)
        {
            return OrderErrors.OrderNotFound;
        }

        if (order.HiddenAt is not null)
        {
            return Result.Success();
        }

        var resource = order.ResourceId!.Value;
        var author = order.ResourceKind == ReportTarget.Memory
            ? (await memories.DescribeAsync(resource, cancellationToken))?.AuthorAccountId
            : await albums.OwnerOfAsync(resource, cancellationToken);
        if (!await hiding.HideAsync(new HiddenContent(id.Value, order.ResourceKind!.Value.ToString(), resource, author, order.Sequence), cancellationToken))
        {
            throw new InvalidOperationException("Access did not confirm hiding the content of an order; the retry asks again.");
        }

        order.MarkHidden(author, clock.GetUtcNow());
        return await orders.SaveAsync(cancellationToken) ? Result.Success() : OrderErrors.InvalidOrder;
    }

    /// <summary>C114.</summary>
    public Task<Result> SecureAsync(OrderId id, CancellationToken cancellationToken) =>
        DecideAsync(id, order => order.Secure(clock.GetUtcNow()), cancellationToken);

    /// <summary>C115: the entry in the log goes with the order, in one transaction.</summary>
    public Task<Result> DiscloseAsync(OrderId id, string? what, string? by, CancellationToken cancellationToken) =>
        DecideAsync(
            id,
            order =>
            {
                var entry = order.Disclose(what, by, clock.GetUtcNow());
                if (entry.IsFailure)
                {
                    return entry.Error;
                }

                orders.Log(entry.Value);
                return Result.Success();
            },
            cancellationToken);

    public Task<Result> CloseAsync(OrderId id, CancellationToken cancellationToken) =>
        DecideAsync(id, order => order.Close(clock.GetUtcNow()), cancellationToken);

    private async Task<Result> DecideAsync(OrderId id, Func<AuthorityOrder, Result> decide, CancellationToken cancellationToken)
    {
        for (var attempt = 0; attempt < 3; attempt++)
        {
            if (await orders.FindAsync(id, cancellationToken) is not { } order)
            {
                return OrderErrors.OrderNotFound;
            }

            var decided = decide(order);
            if (decided.IsFailure)
            {
                return decided;
            }

            if (await orders.SaveAsync(cancellationToken))
            {
                return Result.Success();
            }
        }

        return ReportErrors.AlreadyDecided;
    }
}
