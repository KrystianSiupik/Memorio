using Memorio.Modules.Album.Domain.Albums;
using Memorio.Modules.Archive.Contracts;
using Memorio.Modules.Limits.Contracts;
using Memorio.Modules.Plans.Contracts;
using Memorio.SharedKernel.Results;

namespace Memorio.Modules.Album.Application.Commands.Contributions;

/// <summary>C47: an author brings their own memory; the album's pool pays for it from now on.</summary>
internal sealed record AddMemoryToAlbum(AlbumId AlbumId, Guid AccountId, Guid MemoryId);

internal sealed class AddMemoryToAlbumHandler(IAlbums albums, IAlbumContent content, IAlbumPool pool, IAlbumSeatLimit plan, TimeProvider clock)
{
    public async Task<Result> HandleAsync(AddMemoryToAlbum command, CancellationToken cancellationToken)
    {
        var album = await albums.FindAsync(command.AlbumId, cancellationToken);
        if (album is null || !album.HasMember(command.AccountId))
        {
            return AlbumErrors.NoAccess;
        }

        var facts = await content.DescribeAsync(command.MemoryId, cancellationToken);
        if (facts is null || facts.AuthorAccountId != command.AccountId)
        {
            return AlbumErrors.NoAccess;
        }

        if (await albums.FindMemoryAsync(command.AlbumId, command.MemoryId, cancellationToken) is not null)
        {
            return Result.Success();
        }

        if (await plan.IsReadOnlyAsync(command.AlbumId.Value, cancellationToken))
        {
            return AlbumErrors.ReadOnly;
        }

        var now = clock.GetUtcNow();
        var key = PoolKeys.For(command.AlbumId.Value, command.MemoryId);
        if (facts.SizeBytes > 0 && !await pool.TryReserveAsync(command.AlbumId.Value, key, facts.SizeBytes, cancellationToken))
        {
            album.RecordPoolExhausted(now);
            await albums.SaveAsync(cancellationToken);
            return AlbumErrors.PoolFull;
        }

        await content.LinkAsync(command.MemoryId, command.AlbumId.Value, cancellationToken);
        albums.Add(AlbumMemory.Create(command.AlbumId, command.MemoryId, command.AccountId, now));
        album.RecordMemoryAdded(command.MemoryId, command.AccountId, now);
        if (await albums.SaveAsync(cancellationToken))
        {
            return Result.Success();
        }

        // The same request twice at once: the winner keeps the link and the reservation under the same key.
        return await albums.FindMemoryAsync(command.AlbumId, command.MemoryId, cancellationToken) is not null
            ? Result.Success()
            : AlbumErrors.AlreadyDecided;
    }
}

/// <summary>C56: the author adds a photo to their memory in the album; the album's pool pays (D-248).</summary>
internal sealed record AddPhotoInAlbum(AlbumId AlbumId, Guid AccountId, Guid MemoryId, Guid PhotoId, string? UploadKey);

internal sealed class AddPhotoInAlbumHandler(IAlbums albums, IAlbumContent content, IAlbumSeatLimit plan, TimeProvider clock)
{
    public async Task<Result> HandleAsync(AddPhotoInAlbum command, CancellationToken cancellationToken)
    {
        var album = await albums.FindAsync(command.AlbumId, cancellationToken);
        var memory = await albums.FindMemoryAsync(command.AlbumId, command.MemoryId, cancellationToken);
        if (album is null || !album.HasMember(command.AccountId) || memory?.AuthorAccountId != command.AccountId)
        {
            return AlbumErrors.NoAccess;
        }

        if (await plan.IsReadOnlyAsync(command.AlbumId.Value, cancellationToken))
        {
            return AlbumErrors.ReadOnly;
        }

        var outcome = await content.AddPhotoAsync(
            new AlbumPhoto(command.AlbumId.Value, command.MemoryId, command.AccountId, command.PhotoId, command.UploadKey),
            cancellationToken);
        var now = clock.GetUtcNow();
        switch (outcome)
        {
            case AlbumPhotoOutcome.Added:
                album.RecordPhotoAdded(command.MemoryId, command.PhotoId, now);
                await albums.SaveAsync(cancellationToken);
                return Result.Success();
            case AlbumPhotoOutcome.AlreadyThere:
                return Result.Success();
            case AlbumPhotoOutcome.PoolFull:
                album.RecordPoolExhausted(now);
                await albums.SaveAsync(cancellationToken);
                return AlbumErrors.PoolFull;
            case AlbumPhotoOutcome.UploadMissing:
                return AlbumErrors.UploadMissing;
            default:
                return AlbumErrors.NoAccess;
        }
    }
}

/// <summary>C52: the person chooses what happens to what they brought.</summary>
internal sealed record LeaveAlbum(AlbumId AlbumId, Guid AccountId, bool KeepContributions);

internal sealed class LeaveAlbumHandler(IAlbums albums, ContributionWithdrawal withdrawal, TimeProvider clock)
{
    public Task<Result> HandleAsync(LeaveAlbum command, CancellationToken cancellationToken) =>
        AlbumDecision.UntilStoredAsync(
            albums,
            async () =>
            {
                var album = await albums.FindAsync(command.AlbumId, cancellationToken);
                if (album is null)
                {
                    return AlbumErrors.NoAccess;
                }

                var now = clock.GetUtcNow();
                var left = album.Leave(command.AccountId, now);
                if (left.IsFailure)
                {
                    return left;
                }

                if (!command.KeepContributions)
                {
                    await withdrawal.WithdrawAsync(album, command.AccountId, now, cancellationToken);
                }
                else if ((await albums.FindMemoriesOfAuthorAsync(command.AlbumId, command.AccountId, cancellationToken)).Count > 0)
                {
                    albums.Add(KeptContribution.Create(command.AlbumId, command.AccountId, chosenByAuthor: true, now));
                    album.RecordContributionsKept(command.AccountId, now);
                }

                return Result.Success();
            },
            cancellationToken);
}

/// <summary>C53: what the person brought stays until they choose (P8, D-249).</summary>
internal sealed record RemoveParticipant(AlbumId AlbumId, Guid OwnerAccountId, Guid AccountId);

internal sealed class RemoveParticipantHandler(IAlbums albums, TimeProvider clock)
{
    public Task<Result> HandleAsync(RemoveParticipant command, CancellationToken cancellationToken) =>
        AlbumDecision.UntilStoredAsync(
            albums,
            async () =>
            {
                var album = await albums.FindAsync(command.AlbumId, cancellationToken);
                if (album is null)
                {
                    return AlbumErrors.NoAccess;
                }

                var now = clock.GetUtcNow();
                var removed = album.RemoveParticipant(command.OwnerAccountId, command.AccountId, now);
                if (removed.IsFailure)
                {
                    return removed;
                }

                if ((await albums.FindMemoriesOfAuthorAsync(command.AlbumId, command.AccountId, cancellationToken)).Count > 0)
                {
                    albums.Add(KeptContribution.Create(command.AlbumId, command.AccountId, chosenByAuthor: false, now));
                }

                return Result.Success();
            },
            cancellationToken);
}

/// <summary>C55: the author takes back what stayed in an album they are no longer in.</summary>
internal sealed record RevokeKeptContributions(AlbumId AlbumId, Guid AccountId);

internal sealed class RevokeKeptContributionsHandler(IAlbums albums, ContributionWithdrawal withdrawal, TimeProvider clock)
{
    public Task<Result> HandleAsync(RevokeKeptContributions command, CancellationToken cancellationToken) =>
        AlbumDecision.UntilStoredAsync(
            albums,
            async () =>
            {
                var album = await albums.FindAsync(command.AlbumId, cancellationToken);
                if (album is null || await albums.FindKeptAsync(command.AlbumId, command.AccountId, cancellationToken) is null)
                {
                    return AlbumErrors.AlreadyDecided;
                }

                await withdrawal.WithdrawAsync(album, command.AccountId, clock.GetUtcNow(), cancellationToken);
                return Result.Success();
            },
            cancellationToken);
}

/// <summary>C49: replacing the curator is the same command.</summary>
internal sealed record EntrustToCurator(AlbumId AlbumId, Guid AccountId, Guid CuratorAccountId);

internal sealed class EntrustToCuratorHandler(IAlbums albums, TimeProvider clock)
{
    public async Task<Result> HandleAsync(EntrustToCurator command, CancellationToken cancellationToken)
    {
        if (command.AccountId == command.CuratorAccountId)
        {
            return AlbumErrors.CannotEntrustToYourself;
        }

        var album = await albums.FindAsync(command.AlbumId, cancellationToken);
        if (album is null || !album.HasMember(command.AccountId))
        {
            return AlbumErrors.NoAccess;
        }

        if (!album.HasMember(command.CuratorAccountId))
        {
            return AlbumErrors.NotParticipant;
        }

        var now = clock.GetUtcNow();
        var existing = await albums.FindEntrustmentAsync(command.AlbumId, command.AccountId, cancellationToken);
        if (existing is null)
        {
            albums.Add(Entrustment.Create(command.AlbumId, command.AccountId, command.CuratorAccountId, now));
        }
        else
        {
            existing.HandOver(command.CuratorAccountId, now);
        }

        album.RecordEntrusted(command.AccountId, command.CuratorAccountId, now);
        return await albums.SaveAsync(cancellationToken) ? Result.Success() : AlbumErrors.AlreadyDecided;
    }
}

/// <summary>C50: asking again changes nothing.</summary>
internal sealed record RevokeEntrustment(AlbumId AlbumId, Guid AccountId);

internal sealed class RevokeEntrustmentHandler(IAlbums albums, TimeProvider clock)
{
    public async Task<Result> HandleAsync(RevokeEntrustment command, CancellationToken cancellationToken)
    {
        var album = await albums.FindAsync(command.AlbumId, cancellationToken);
        if (album is null)
        {
            return AlbumErrors.NoAccess;
        }

        if (await albums.FindEntrustmentAsync(command.AlbumId, command.AccountId, cancellationToken) is not { } entrustment)
        {
            return Result.Success();
        }

        albums.Remove(entrustment);
        album.RecordEntrustmentRevoked(command.AccountId, clock.GetUtcNow());
        return await albums.SaveAsync(cancellationToken) ? Result.Success() : AlbumErrors.AlreadyDecided;
    }
}

/// <summary>C51: the curator orders only what was entrusted to them, and only while it is.</summary>
internal sealed record ArrangeEntrusted(AlbumId AlbumId, Guid AccountId, IReadOnlyList<Guid> MemoryIds);

internal sealed class ArrangeEntrustedHandler(IAlbums albums, TimeProvider clock)
{
    public async Task<Result> HandleAsync(ArrangeEntrusted command, CancellationToken cancellationToken)
    {
        if (command.MemoryIds.Count == 0 || command.MemoryIds.Distinct().Count() != command.MemoryIds.Count)
        {
            return AlbumErrors.InvalidArrangement;
        }

        var album = await albums.FindAsync(command.AlbumId, cancellationToken);
        if (album is null || !album.HasMember(command.AccountId))
        {
            return AlbumErrors.NoAccess;
        }

        for (var position = 0; position < command.MemoryIds.Count; position++)
        {
            var memory = await albums.FindMemoryAsync(command.AlbumId, command.MemoryIds[position], cancellationToken);
            var entrustment = memory is null ? null : await albums.FindEntrustmentAsync(command.AlbumId, memory.AuthorAccountId, cancellationToken);
            if (entrustment?.CuratorAccountId != command.AccountId)
            {
                return AlbumErrors.NoAccess;
            }

            memory!.MoveTo(position);
        }

        album.RecordArranged(command.AccountId, clock.GetUtcNow());
        return await albums.SaveAsync(cancellationToken) ? Result.Success() : AlbumErrors.AlreadyDecided;
    }
}

/// <summary>C169: D-079, only someone in this album.</summary>
internal sealed record TagParticipant(AlbumId AlbumId, Guid AccountId, Guid MemoryId, Guid TaggedAccountId);

internal sealed class TagParticipantHandler(IAlbums albums, TimeProvider clock)
{
    public async Task<Result> HandleAsync(TagParticipant command, CancellationToken cancellationToken)
    {
        var album = await albums.FindAsync(command.AlbumId, cancellationToken);
        var memory = await albums.FindMemoryAsync(command.AlbumId, command.MemoryId, cancellationToken);
        if (album is null || memory is null || !album.HasMember(command.AccountId))
        {
            return AlbumErrors.NoAccess;
        }

        if (!album.HasMember(command.TaggedAccountId))
        {
            return AlbumErrors.NotParticipant;
        }

        var now = clock.GetUtcNow();
        if (!memory.Tag(command.TaggedAccountId, command.AccountId, now))
        {
            return Result.Success();
        }

        album.RecordTagged(command.MemoryId, command.TaggedAccountId, command.AccountId, now);
        return await albums.SaveAsync(cancellationToken) ? Result.Success() : AlbumErrors.AlreadyDecided;
    }
}

/// <summary>C170: one's own tag only; asking again changes nothing.</summary>
internal sealed record RemoveOwnTag(AlbumId AlbumId, Guid AccountId, Guid MemoryId);

internal sealed class RemoveOwnTagHandler(IAlbums albums, TimeProvider clock)
{
    public async Task<Result> HandleAsync(RemoveOwnTag command, CancellationToken cancellationToken)
    {
        var album = await albums.FindAsync(command.AlbumId, cancellationToken);
        var memory = await albums.FindMemoryAsync(command.AlbumId, command.MemoryId, cancellationToken);
        if (album is null || memory is null)
        {
            return AlbumErrors.NoAccess;
        }

        if (!memory.Untag(command.AccountId))
        {
            return Result.Success();
        }

        album.RecordTagRemoved(command.MemoryId, command.AccountId, clock.GetUtcNow());
        return await albums.SaveAsync(cancellationToken) ? Result.Success() : AlbumErrors.AlreadyDecided;
    }
}
