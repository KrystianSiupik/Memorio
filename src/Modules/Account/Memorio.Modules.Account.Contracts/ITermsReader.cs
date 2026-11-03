namespace Memorio.Modules.Account.Contracts;

/// <summary>Read port for Archive: terms come before the first content (S2:C11).</summary>
public interface ITermsReader
{
    Task<bool> HasAcceptedCurrentTermsAsync(Guid accountId, CancellationToken cancellationToken);
}
