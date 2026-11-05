using System.Collections.Concurrent;
using Memorio.Modules.Account.Contracts;
using Memorio.Modules.Album.Contracts;
using Memorio.Modules.Notifications.Application.Ports;
using Memorio.Modules.Succession.Contracts;

namespace Memorio.Modules.Notifications.Tests;

public sealed class FakeNotificationNeighbours : IAlbumMessaging, IAccountDirectory, INotificationSender, ITakeoverOffers, ILegacyContacts
{
    private readonly ConcurrentDictionary<Guid, OpenTakeoverOffer> _offers = new();
    private readonly ConcurrentDictionary<Guid, string> _contacts = new();

    public void OfferOpen(Guid caseId, OpenTakeoverOffer offer) => _offers[caseId] = offer;

    public void OfferAnswered(Guid caseId) => _offers.TryRemove(caseId, out _);

    public void ContactOf(Guid owner, string email) => _contacts[owner] = email;

    public Task<OpenTakeoverOffer?> FindOpenAsync(Guid caseId, CancellationToken cancellationToken) => Task.FromResult(_offers.GetValueOrDefault(caseId));

    public Task<string?> ContactOfAsync(Guid ownerAccountId, CancellationToken cancellationToken) => Task.FromResult(_contacts.GetValueOrDefault(ownerAccountId));

    private readonly ConcurrentDictionary<Guid, IReadOnlyCollection<Guid>> _members = new();
    private readonly ConcurrentDictionary<string, DeliveryOutcome> _refused = new();

    public void Members(Guid albumId, params Guid[] people) => _members[albumId] = people;

    internal void Refuses(string email, DeliveryOutcome outcome = DeliveryOutcome.Refused) => _refused[email] = outcome;

    public void Accepts(string email) => _refused.TryRemove(email, out _);

    public void Gone(Guid accountId) => _emails.TryRemove(accountId, out _);

    public Task<IReadOnlyCollection<Guid>> MembersOfAsync(Guid albumId, CancellationToken cancellationToken) =>
        Task.FromResult(_members.GetValueOrDefault(albumId) ?? []);

    private readonly ConcurrentDictionary<Guid, PendingInvitation> _invitations = new();
    private readonly ConcurrentDictionary<Guid, string> _emails = new();
    private readonly ConcurrentBag<(string Email, string Kind, string? InvitedBy)> _sent = [];
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
        _invitations[id] = new PendingInvitation(Guid.CreateVersion7(), $"{id}@example.invalid", NewAccount());
        return id;
    }

    public string AddressOf(Guid invitationId) => _invitations[invitationId].Email;

    public string InviterOf(Guid invitationId) => _emails[_invitations[invitationId].InvitedByAccountId];

    public string? InvitedByIn(string email, string kind) => _sent.First(item => item.Email == email && item.Kind == kind).InvitedBy;

    public void Answered(Guid invitationId) => _invitations.TryRemove(invitationId, out _);

    public void FailNextSendTo(string email) => _failures[email] = 1;

    public int SentTo(string email, string kind) => _sent.Count(item => item.Email == email && item.Kind == kind);

    public Task<PendingInvitation?> FindPendingInvitationAsync(Guid invitationId, CancellationToken cancellationToken) =>
        Task.FromResult(_invitations.GetValueOrDefault(invitationId));

    public Task<string?> EmailOfAsync(Guid accountId, CancellationToken cancellationToken) => Task.FromResult(_emails.GetValueOrDefault(accountId));

    public Task<IReadOnlyDictionary<Guid, string>> EmailsOfAsync(IReadOnlyCollection<Guid> accountIds, CancellationToken cancellationToken) =>
        Task.FromResult<IReadOnlyDictionary<Guid, string>>(accountIds.Where(_emails.ContainsKey).ToDictionary(id => id, id => _emails[id]));

    Task<DeliveryOutcome> INotificationSender.SendAsync(string email, NotificationKind kind, Guid albumId, string? detail, CancellationToken cancellationToken)
    {
        if (_failures.TryRemove(email, out _))
        {
            throw new InvalidOperationException("The provider does not answer.");
        }

        if (_refused.TryGetValue(email, out var outcome))
        {
            return Task.FromResult(outcome);
        }

        _sent.Add((email, kind.ToString(), detail));
        return Task.FromResult(DeliveryOutcome.Delivered);
    }
}
