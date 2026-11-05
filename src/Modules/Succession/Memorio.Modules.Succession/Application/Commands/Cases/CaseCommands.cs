using System.Net.Mail;
using Memorio.Modules.Account.Contracts;
using Memorio.Modules.Album.Contracts;
using Memorio.Modules.Succession.Domain.Cases;
using Memorio.SharedKernel.Results;
using Memorio.SharedKernel.Storage;

namespace Memorio.Modules.Succession.Application.Commands.Cases;

/// <summary>An address to send a certificate to; it becomes part of a case only through <see cref="ReportDeath"/>.</summary>
internal sealed class StartCertificateUploadHandler(IObjectStorage storage)
{
    private static readonly string[] Accepted = ["image/jpeg", "image/png", "image/webp", "image/heic", "application/pdf"];

    public async Task<Result<UploadSlot>> HandleAsync(string? contentType, CancellationToken cancellationToken) =>
        contentType is not null && Accepted.Contains(contentType, StringComparer.Ordinal)
            ? await storage.CreateUploadAsync(contentType, cancellationToken)
            : CaseErrors.InvalidContentType;
}

/// <summary>C120. Only someone in the album, about its owner. The id comes from the client: the same request again changes nothing.</summary>
internal sealed record ReportDeath(TakeoverCaseId CaseId, Guid AlbumId, Guid AccountId, string? UploadKey);

internal sealed class ReportDeathHandler(ITakeoverCases cases, IAlbumMembership albums, IObjectStorage storage, TimeProvider clock)
{
    public async Task<Result> HandleAsync(ReportDeath command, CancellationToken cancellationToken)
    {
        var owner = await albums.OwnerOfAsync(command.AlbumId, cancellationToken);
        if (owner is null || owner == command.AccountId || !await albums.IsMemberAsync(command.AccountId, command.AlbumId, cancellationToken))
        {
            return CaseErrors.NoAccess;
        }

        if (await cases.FindAsync(command.CaseId, cancellationToken) is { } known)
        {
            return known.ReportedByAccountId == command.AccountId ? Result.Success() : CaseErrors.NoAccess;
        }

        if (await cases.FindOpenAsync(command.AlbumId, cancellationToken) is not null)
        {
            return CaseErrors.AlreadyReported;
        }

        if (!TryUploadKey(command.UploadKey, out var upload))
        {
            return CaseErrors.InvalidUpload;
        }

        var destination = new StorageKey($"certificates/{command.CaseId.Value}/original");
        if (await storage.PromoteUploadAsync(upload, destination, cancellationToken) is null)
        {
            return CaseErrors.UploadMissing;
        }

        cases.Add(TakeoverCase.Report(command.CaseId, command.AlbumId, owner.Value, command.AccountId, destination.Value, clock.GetUtcNow()));
        if (await cases.SaveAsync(cancellationToken))
        {
            return Result.Success();
        }

        // Another report about this album won: its file stands, this one goes.
        await storage.DeleteAsync(destination, cancellationToken);
        return CaseErrors.AlreadyReported;
    }

    private static bool TryUploadKey(string? value, out StorageKey key)
    {
        key = default;
        try
        {
            key = new StorageKey(value!);
            return key.IsUpload;
        }
        catch (ArgumentException)
        {
            return false;
        }
    }
}

/// <summary>C121: the service's decision. The successor is read now and stays in the case (D-020).</summary>
internal sealed record DecideReport(TakeoverCaseId CaseId, bool Confirmed);

internal sealed class DecideReportHandler(ITakeoverCases cases, IAlbumSuccession albums, TimeProvider clock)
{
    public async Task<Result> HandleAsync(DecideReport command, CancellationToken cancellationToken)
    {
        var found = await cases.FindAsync(command.CaseId, cancellationToken);
        if (found is null)
        {
            return CaseErrors.NotFound;
        }

        var now = clock.GetUtcNow();
        var owned = await albums.OwnedByAsync(found.OwnerAccountId, cancellationToken);
        var decided = command.Confirmed
            ? found.Verify(owned.FirstOrDefault(album => album.AlbumId == found.AlbumId)?.SuccessorAccountId, now)
            : found.Reject(now);
        if (decided.IsFailure)
        {
            return decided;
        }

        return await cases.SaveAsync(cancellationToken) ? Result.Success() : CaseErrors.AlreadyDecided;
    }
}

/// <summary>C122 and C123: the answer of the successor.</summary>
internal sealed record AnswerOffer(TakeoverCaseId CaseId, Guid AccountId, bool Accepted);

internal sealed class AnswerOfferHandler(ITakeoverCases cases, TimeProvider clock)
{
    public async Task<Result> HandleAsync(AnswerOffer command, CancellationToken cancellationToken)
    {
        var found = await cases.FindAsync(command.CaseId, cancellationToken);
        if (found is null)
        {
            return CaseErrors.NotSuccessor;
        }

        var now = clock.GetUtcNow();
        var answered = command.Accepted ? found.Accept(command.AccountId, now) : found.Refuse(command.AccountId, now);
        if (answered.IsFailure)
        {
            return answered;
        }

        return await cases.SaveAsync(cancellationToken) ? Result.Success() : CaseErrors.AlreadyDecided;
    }
}

/// <summary>C126 and P38: called with the time the deadline was due.</summary>
internal sealed class ExpireOfferHandler(ITakeoverCases cases)
{
    public async Task HandleAsync(TakeoverCaseId id, DateTimeOffset at, CancellationToken cancellationToken)
    {
        if (await cases.FindAsync(id, cancellationToken) is { } found && found.ExpireIfDue(at).IsSuccess && !await cases.SaveAsync(cancellationToken))
        {
            throw new InvalidOperationException("Another decision about this case got there first; the retry reads it again.");
        }
    }
}

/// <summary>C124.</summary>
internal sealed class NameLegacyContactHandler(ITakeoverCases cases, IAccountDirectory directory)
{
    private const int MaximumLength = 320;

    public async Task<Result> HandleAsync(Guid ownerAccountId, string? email, CancellationToken cancellationToken)
    {
        var normalized = email?.Trim().ToLowerInvariant();
        if (string.IsNullOrEmpty(normalized) || normalized.Length > MaximumLength || !MailAddress.TryCreate(normalized, out var parsed) || parsed.Address != normalized)
        {
            return CaseErrors.InvalidEmail;
        }

        if ((await directory.EmailOfAsync(ownerAccountId, cancellationToken))?.Trim().ToLowerInvariant() == normalized)
        {
            return CaseErrors.CannotNameYourself;
        }

        var contact = await cases.FindContactAsync(ownerAccountId, cancellationToken);
        if (contact is null)
        {
            cases.Add(LegacyContact.Name(ownerAccountId, normalized));
        }
        else
        {
            contact.ChangeTo(normalized);
        }

        return await cases.SaveAsync(cancellationToken) ? Result.Success() : CaseErrors.AlreadyDecided;
    }

    /// <summary>Asking again changes nothing.</summary>
    public async Task<Result> RemoveAsync(Guid ownerAccountId, CancellationToken cancellationToken)
    {
        if (await cases.FindContactAsync(ownerAccountId, cancellationToken) is { } contact)
        {
            cases.Remove(contact);
            return await cases.SaveAsync(cancellationToken) ? Result.Success() : CaseErrors.AlreadyDecided;
        }

        return Result.Success();
    }
}
