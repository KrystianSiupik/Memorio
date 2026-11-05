using System.Security.Cryptography;
using System.Text;

namespace Memorio.Modules.Access.Domain.Audiences;

/// <summary>E126 applied here. Only a hash of the address: this list must not become a list of addresses.</summary>
internal sealed class LegacyGrant
{
    private LegacyGrant()
    {
    }

    public Guid OwnerAccountId { get; private set; }

    public string ContactHash { get; private set; } = "";

    public DateTimeOffset GrantedAt { get; private set; }

    public static LegacyGrant Create(Guid ownerAccountId, string contactEmail, DateTimeOffset now) =>
        new() { OwnerAccountId = ownerAccountId, ContactHash = HashOf(contactEmail), GrantedAt = now };

    public static string HashOf(string email) =>
        Convert.ToHexStringLower(SHA256.HashData(Encoding.UTF8.GetBytes(email.Trim().ToLowerInvariant())));
}
