namespace Memorio.Modules.Account.Contracts;

/// <summary>Read port: the address of an account, for matching invitations, sending messages and naming people in one album.</summary>
public interface IAccountDirectory
{
    /// <summary>Null when the account no longer exists.</summary>
    Task<string?> EmailOfAsync(Guid accountId, CancellationToken cancellationToken);

    Task<IReadOnlyDictionary<Guid, string>> EmailsOfAsync(IReadOnlyCollection<Guid> accountIds, CancellationToken cancellationToken);
}
