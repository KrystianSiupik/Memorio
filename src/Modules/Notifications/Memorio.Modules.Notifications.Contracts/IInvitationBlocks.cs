namespace Memorio.Modules.Notifications.Contracts;

/// <summary>Read port for Album (C42): an address that asked for no invitations.</summary>
public interface IInvitationBlocks
{
    Task<bool> IsBlockedAsync(string email, CancellationToken cancellationToken);
}
