using Memorio.Modules.Notifications.Contracts;
using Memorio.Modules.Notifications.Domain;
using Memorio.Modules.Notifications.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Memorio.Modules.Notifications.Infrastructure;

internal sealed class InvitationBlocks(NotificationsDbContext database) : IInvitationBlocks
{
    public Task<bool> IsBlockedAsync(string email, CancellationToken cancellationToken)
    {
        var hash = BlockedAddress.HashOf(email);
        return database.BlockedAddresses.AnyAsync(blocked => blocked.AddressHash == hash, cancellationToken);
    }
}
