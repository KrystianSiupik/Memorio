using System.Collections.Concurrent;
using Memorio.Modules.Moderation.Contracts;

namespace Memorio.Modules.Account.Tests;

/// <summary>Orders of authorities, as Moderation would answer: an account is held once told so.</summary>
public sealed class FakeLegalHolds : ILegalHolds
{
    private readonly ConcurrentDictionary<Guid, bool> _held = new();

    public void Hold(Guid accountId) => _held[accountId] = true;

    public void Release(Guid accountId) => _held.TryRemove(accountId, out _);

    public Task<bool> IsHeldAsync(Guid accountId, CancellationToken cancellationToken) => Task.FromResult(_held.ContainsKey(accountId));
}
