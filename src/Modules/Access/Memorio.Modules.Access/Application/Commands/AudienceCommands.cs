using System.Security.Cryptography;
using Memorio.Modules.Access.Contracts;
using Memorio.Modules.Access.Domain.Audiences;
using Memorio.Modules.Album.Contracts;
using Memorio.SharedKernel.Results;

namespace Memorio.Modules.Access.Application.Commands;

/// <summary>A decision made again when a parallel one got there first: the loser reads the new state (D-269).</summary>
internal static class AudienceDecision
{
    private const int Attempts = 3;

    public static async Task<Result> UntilStoredAsync(IAudiences audiences, Func<Task<Result>> decide, CancellationToken cancellationToken)
    {
        for (var attempt = 0; attempt < Attempts; attempt++)
        {
            var decided = await decide();
            if (decided.IsFailure)
            {
                return decided;
            }

            if (await audiences.SaveAsync(cancellationToken))
            {
                return Result.Success();
            }
        }

        return AccessErrors.AlreadyDecided;
    }

    /// <summary>The audience of an album, opened the first time someone needs it.</summary>
    public static async Task<AlbumAudience> OpenAsync(IAudiences audiences, Guid albumId, CancellationToken cancellationToken)
    {
        if (await audiences.FindAsync(albumId, cancellationToken) is { } existing)
        {
            return existing;
        }

        var audience = AlbumAudience.For(albumId);
        audiences.Add(audience);
        return audience;
    }
}

/// <summary>C60.</summary>
internal sealed class ShareByLinkHandler(IAudiences audiences, IAlbumMembership albums, TimeProvider clock)
{
    public async Task<Result<string>> HandleAsync(Guid albumId, Guid accountId, CancellationToken cancellationToken)
    {
        if (await albums.OwnerOfAsync(albumId, cancellationToken) != accountId)
        {
            return AccessErrors.NoAccess;
        }

        var token = "";
        var shared = await AudienceDecision.UntilStoredAsync(
            audiences,
            async () =>
            {
                var audience = await AudienceDecision.OpenAsync(audiences, albumId, cancellationToken);
                token = audience.GenerateLink(Convert.ToHexStringLower(RandomNumberGenerator.GetBytes(32)), clock.GetUtcNow());
                return Result.Success();
            },
            cancellationToken);
        return shared.IsFailure ? shared.Error : token;
    }
}

/// <summary>C62.</summary>
internal sealed class RevokeLinkHandler(IAudiences audiences, IAlbumMembership albums, TimeProvider clock)
{
    public async Task<Result> HandleAsync(Guid albumId, Guid accountId, CancellationToken cancellationToken)
    {
        if (await albums.OwnerOfAsync(albumId, cancellationToken) != accountId)
        {
            return AccessErrors.NoAccess;
        }

        return await AudienceDecision.UntilStoredAsync(
            audiences,
            async () =>
            {
                var audience = await audiences.FindAsync(albumId, cancellationToken);
                return audience is null ? AccessErrors.AlreadyDecided : audience.RevokeLink(clock.GetUtcNow());
            },
            cancellationToken);
    }
}

/// <summary>C61: someone in the album needs no place on the list, but the link and the block still apply.</summary>
internal sealed class EnterByLinkHandler(IAudiences audiences, IAlbumMembership albums, TimeProvider clock)
{
    public async Task<Result<Guid>> HandleAsync(string token, Guid accountId, CancellationToken cancellationToken)
    {
        var albumId = Guid.Empty;
        var entered = await AudienceDecision.UntilStoredAsync(
            audiences,
            async () =>
            {
                var audience = await audiences.FindByTokenAsync(token, cancellationToken);
                if (audience is null)
                {
                    return AccessErrors.LinkRevoked;
                }

                albumId = audience.Id;
                if (audience.IsBlocked(accountId))
                {
                    return AccessErrors.Blocked;
                }

                return await albums.IsMemberAsync(accountId, audience.Id, cancellationToken)
                    ? Result.Success()
                    : audience.Enter(token, accountId, clock.GetUtcNow());
            },
            cancellationToken);
        return entered.IsFailure ? entered.Error : albumId;
    }
}

/// <summary>C63: only someone let in through a link; people in the album leave through Album.</summary>
internal sealed class RevokeAccessHandler(IAudiences audiences, IAlbumMembership albums, TimeProvider clock)
{
    public async Task<Result> HandleAsync(Guid albumId, Guid ownerAccountId, Guid accountId, CancellationToken cancellationToken)
    {
        if (await albums.OwnerOfAsync(albumId, cancellationToken) != ownerAccountId)
        {
            return AccessErrors.NoAccess;
        }

        return await AudienceDecision.UntilStoredAsync(
            audiences,
            async () =>
            {
                var audience = await audiences.FindAsync(albumId, cancellationToken);
                return audience is null ? AccessErrors.AlreadyDecided : audience.RevokeAccess(accountId, clock.GetUtcNow());
            },
            cancellationToken);
    }
}

/// <summary>C64: anyone in the album blocks someone let in through a link; someone in the album only the owner (D-270).</summary>
internal sealed class BlockUserHandler(IAudiences audiences, IAlbumMembership albums, TimeProvider clock)
{
    public async Task<Result> HandleAsync(Guid albumId, Guid byAccountId, Guid accountId, CancellationToken cancellationToken)
    {
        if (!await albums.IsMemberAsync(byAccountId, albumId, cancellationToken))
        {
            return AccessErrors.NoAccess;
        }

        if (byAccountId == accountId)
        {
            return AccessErrors.CannotBlockYourself;
        }

        var owner = await albums.OwnerOfAsync(albumId, cancellationToken);
        if (accountId == owner)
        {
            return AccessErrors.CannotBlockOwner;
        }

        if (byAccountId != owner && await albums.IsMemberAsync(accountId, albumId, cancellationToken))
        {
            return AccessErrors.OnlyOwnerBlocksParticipant;
        }

        return await AudienceDecision.UntilStoredAsync(
            audiences,
            async () =>
            {
                var audience = await AudienceDecision.OpenAsync(audiences, albumId, cancellationToken);
                audience.Block(accountId, clock.GetUtcNow());
                return Result.Success();
            },
            cancellationToken);
    }
}

/// <summary>C65: a rule of another module gives someone the right to read (P31 of E9, D-272).</summary>
internal sealed class ReadGrants(IAudiences audiences, TimeProvider clock) : IReadGrants
{
    public async Task<bool> GrantAsync(Guid albumId, Guid accountId, CancellationToken cancellationToken) =>
        (await AudienceDecision.UntilStoredAsync(
            audiences,
            async () =>
            {
                var audience = await AudienceDecision.OpenAsync(audiences, albumId, cancellationToken);
                return audience.GrantRead(accountId, clock.GetUtcNow());
            },
            cancellationToken)).IsSuccess;
}
