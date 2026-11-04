using System.Security.Cryptography;
using System.Text;

namespace Memorio.Modules.Notifications.Domain;

/// <summary>One message about one subject: <see cref="Sequence"/> of the event it came from orders late ones (D-013).</summary>
internal sealed class SentNotification
{
    private SentNotification()
    {
    }

    /// <summary>What the message is about: the kind, the album and the person or invitation.</summary>
    public string Subject { get; private set; } = "";

    public long Sequence { get; private set; }

    public DateTimeOffset? SentAt { get; private set; }

    public static SentNotification Create(string subject, long sequence) => new() { Subject = subject, Sequence = sequence };

    public void MoveTo(long sequence)
    {
        Sequence = sequence;
        SentAt = null;
    }

    public void MarkSent(DateTimeOffset now) => SentAt = now;
}

/// <summary>E49. Only a hash: this list must not become a list of addresses.</summary>
internal sealed class BlockedAddress
{
    private BlockedAddress()
    {
    }

    public string AddressHash { get; private set; } = "";

    public DateTimeOffset BlockedAt { get; private set; }

    public static BlockedAddress Create(string email, DateTimeOffset now) => new() { AddressHash = HashOf(email), BlockedAt = now };

    public static string HashOf(string email) =>
        Convert.ToHexStringLower(SHA256.HashData(Encoding.UTF8.GetBytes(email.Trim().ToLowerInvariant())));
}
