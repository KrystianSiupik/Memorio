namespace Memorio.Modules.Succession.Contracts;

/// <summary>Read port for Notifications: whether an offer still waits for its answer.</summary>
public interface ITakeoverOffers
{
    /// <summary>Null when it was answered, ended or never existed: nothing to remind about.</summary>
    Task<OpenTakeoverOffer?> FindOpenAsync(Guid caseId, CancellationToken cancellationToken);
}

public sealed record OpenTakeoverOffer(Guid AlbumId, Guid SuccessorAccountId, DateTimeOffset AnswerBy);

/// <summary>Read port for Access and Notifications: an address never travels in an event.</summary>
public interface ILegacyContacts
{
    /// <summary>Null when the owner named nobody.</summary>
    Task<string?> ContactOfAsync(Guid ownerAccountId, CancellationToken cancellationToken);
}
