namespace Memorio.Modules.Album.Domain.Albums;

internal enum InvitationState
{
    Pending,
    Accepted,
    Declined,
    Withdrawn,
}

/// <summary>An address asked to join. It holds a seat while unanswered (S2:B1); answered ones stay, so a late answer can be told apart.</summary>
internal sealed class AlbumInvitation
{
    private AlbumInvitation()
    {
    }

    public Guid Id { get; private set; }

    /// <summary>Trimmed and in lower case, as <see cref="InvitationEmail"/> makes it.</summary>
    public string Email { get; private set; } = "";

    public Guid InvitedByAccountId { get; private set; }

    public DateTimeOffset InvitedAt { get; private set; }

    public InvitationState State { get; private set; }

    /// <summary>When 7 days passed without an answer; there is one reminder only.</summary>
    public DateTimeOffset? ReminderDueAt { get; private set; }

    public static AlbumInvitation Create(Guid id, string email, Guid byAccountId, DateTimeOffset now) =>
        new() { Id = id, Email = email, InvitedByAccountId = byAccountId, InvitedAt = now, State = InvitationState.Pending };

    public void Refresh(DateTimeOffset now)
    {
        InvitedAt = now;
        ReminderDueAt = null;
    }

    public void Answer(InvitationState state) => State = state;

    public void MarkReminderDue(DateTimeOffset now) => ReminderDueAt = now;
}

/// <summary>One way to write an address, so the same person is never invited twice under different spellings.</summary>
internal static class InvitationEmail
{
    public const int MaximumLength = 256;

    public static string? Normalize(string? email)
    {
        var trimmed = email?.Trim().ToLowerInvariant();
        if (string.IsNullOrEmpty(trimmed) || trimmed.Length > MaximumLength || trimmed.Any(char.IsWhiteSpace))
        {
            return null;
        }

        var at = trimmed.IndexOf('@');
        return at > 0 && at == trimmed.LastIndexOf('@') && at < trimmed.Length - 1 ? trimmed : null;
    }
}
