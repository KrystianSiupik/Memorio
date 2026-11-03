using System.Collections.Concurrent;
using Memorio.SharedKernel.Storage;

namespace Memorio.Modules.Archive.Tests;

/// <summary>Remembers what was deleted; can stop answering, for the retry after a failure.</summary>
public sealed class FakeObjectStorage : IObjectStorage
{
    private readonly ConcurrentBag<StorageKey> _deleted = [];
    private readonly ConcurrentDictionary<StorageKey, bool> _refused = new();

    public IReadOnlyCollection<StorageKey> Deleted => _deleted;

    /// <summary>The storage fails on this file; per file, because tests of other classes run at the same time.</summary>
    public void Refuse(StorageKey key) => _refused[key] = true;

    public void Accept(StorageKey key) => _refused.TryRemove(key, out _);

    public Task DeleteAsync(StorageKey key, CancellationToken cancellationToken)
    {
        if (_refused.ContainsKey(key))
        {
            throw new InvalidOperationException("The storage does not answer.");
        }

        _deleted.Add(key);
        return Task.CompletedTask;
    }

    public Task<IReadOnlyDictionary<StorageKey, SignedUrl>> SignReadsAsync(IReadOnlyCollection<StorageKey> keys, CancellationToken cancellationToken) =>
        throw new NotSupportedException();

    public Task<UploadSlot> CreateUploadAsync(string contentType, CancellationToken cancellationToken) =>
        throw new NotSupportedException();

    public Task<StoredObject?> PromoteUploadAsync(StorageKey upload, StorageKey destination, CancellationToken cancellationToken) =>
        throw new NotSupportedException();
}
