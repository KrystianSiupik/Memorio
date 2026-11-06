using Memorio.Modules.Moderation.Domain.Orders;
using Microsoft.EntityFrameworkCore;
using Wolverine.EntityFrameworkCore;

namespace Memorio.Modules.Moderation.Infrastructure.Persistence;

internal sealed class Orders(IDbContextOutbox<ModerationDbContext> outbox) : IOrders
{
    private ModerationDbContext Database => outbox.DbContext;

    public Task<AuthorityOrder?> FindAsync(OrderId id, CancellationToken cancellationToken) =>
        Database.Orders.FirstOrDefaultAsync(found => found.Id == id, cancellationToken);

    public void Add(AuthorityOrder order) => Database.Orders.Add(order);

    public void Log(AccessLogEntry entry) => Database.AccessLog.Add(entry);

    public async Task<IReadOnlyList<Guid>> FindIncomingAsync(CancellationToken cancellationToken) =>
        await Database.IncomingOrders.AsNoTracking().Where(order => order.AppliedAt == null).OrderBy(order => order.ReceivedAt).Select(order => order.Id).ToListAsync(cancellationToken);

    public Task<IncomingOrder?> FindIncomingAsync(Guid id, CancellationToken cancellationToken) =>
        Database.IncomingOrders.FirstOrDefaultAsync(order => order.Id == id, cancellationToken);

    public async Task<IReadOnlyList<Guid>> FindPendingActionsAsync(CancellationToken cancellationToken) =>
        await Database.OrderActions.AsNoTracking().Where(action => action.AppliedAt == null).OrderBy(action => action.DecidedAt).Select(action => action.Id).ToListAsync(cancellationToken);

    public Task<OrderAction?> FindActionAsync(Guid id, CancellationToken cancellationToken) =>
        Database.OrderActions.FirstOrDefaultAsync(action => action.Id == id, cancellationToken);

    public Task<bool> SaveAsync(CancellationToken cancellationToken) => ModerationSaving.SaveAsync(outbox, cancellationToken);
}
