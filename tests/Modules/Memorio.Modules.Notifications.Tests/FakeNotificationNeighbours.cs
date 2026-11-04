using System.Collections.Concurrent;
using Memorio.Modules.Account.Contracts;
using Memorio.Modules.Album.Contracts;
using Memorio.Modules.Notifications.Application.Ports;

namespace Memorio.Modules.Notifications.Tests;

public sealed class FakeNotificationNeighbours : IAlbumMessaging, IAccountDirectory, INotificationSender
{
    private readonly ConcurrentDictionary<Guid, PendingInvitation> _invitations = new();
    private readonly ConcurrentDictionary<Guid, string> _emails = new();
    private readonly ConcurrentBag<(string Email, string Kind)> _sent = [];
    private readonly ConcurrentDictionary<string, int> _failures = new();

    public Guid NewAccount()
    {
        var id = Guid.CreateVersion7();
        _emails[id] = $"{id}@example.invalid";
        return id;
    }

    public string EmailOf(Guid accountId) => _emails[accountId];

    public Guid NewInvitation()
    {
        var id = Guid.CreateVersion7();
        _invitations[id] = new PendingInvitation(Guid.CreateVersion7(), $"{id}@example.invalid");
        return id;
    }

    public string AddressOf(Guid invitationId) => _invitations[invitationId].Email;

    public void Answered(Guid invitationId) => _invitations.TryRemove(invitationId, out _);

    public void FailNextSendTo(string email) => _failures[email] = 1;

    public int SentTo(string email, string kind) => _sent.Count(item => item.Email == email && item.Kind == kind);

    public Task<PendingInvitation?> FindPendingInvitationAsync(Guid invitationId, CancellationToken cancellationToken) =>
        Task.FromResult(_invitations.GetValueOrDefault(invitationId));

    public Task<string?> EmailOfAsync(Guid accountId, CancellationToken cancellationToken) => Task.FromResult(_emails.GetValueOrDefault(accountId));

    public Task<IReadOnlyDictionary<Guid, string>> EmailsOfAsync(IReadOnlyCollection<Guid> accountIds, CancellationToken cancellationToken) =>
        Task.FromResult<IReadOnlyDictionary<Guid, string>>(accountIds.Where(_emails.ContainsKey).ToDictionary(id => id, id => _emails[id]));

    Task INotificationSender.SendAsync(string email, NotificationKind kind, Guid albumId, CancellationToken cancellationToken)
    {
        if (_failures.TryRemove(email, out _))
        {
            throw new InvalidOperationException("The provider does not answer.");
        }

        _sent.Add((email, kind.ToString()));
        return Task.CompletedTask;
    }
}
