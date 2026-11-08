using Memorio.Modules.AiNarration.Domain.Consents;
using Memorio.Modules.Album.Contracts;
using Memorio.SharedKernel.Results;

namespace Memorio.Modules.AiNarration.Application.Commands.Consents;

/// <summary>C80: a participant lets the AI use their content in this album. Given again, nothing changes.</summary>
internal sealed record GiveAiConsent(Guid AccountId, Guid AlbumId);

internal sealed class GiveAiConsentHandler(IAiConsents consents, IAlbumMembership membership, TimeProvider clock)
{
    public async Task<Result> HandleAsync(GiveAiConsent command, CancellationToken cancellationToken)
    {
        if (command.AlbumId == Guid.Empty)
        {
            return ConsentErrors.InvalidAlbum;
        }

        if (!await membership.IsMemberAsync(command.AccountId, command.AlbumId, cancellationToken))
        {
            return ConsentErrors.NotMember;
        }

        for (var attempt = 0; attempt < 2; attempt++)
        {
            var consent = await consents.FindAsync(command.AccountId, command.AlbumId, cancellationToken);
            if (consent is null)
            {
                consents.Add(AiConsent.GiveFirst(command.AccountId, command.AlbumId, clock.GetUtcNow()));
            }
            else if (!consent.Give(clock.GetUtcNow()))
            {
                return Result.Success();
            }

            if (await consents.SaveAsync(cancellationToken))
            {
                return Result.Success();
            }
        }

        // Given at the same moment by another call: it is given.
        return Result.Success();
    }
}

/// <summary>Whether the person's consent for this album is given now.</summary>
internal sealed class AiConsentQuery(IAiConsents consents)
{
    public async Task<bool> IsGivenAsync(Guid accountId, Guid albumId, CancellationToken cancellationToken) =>
        await consents.FindAsync(accountId, albumId, cancellationToken) is { IsGiven: true };
}
