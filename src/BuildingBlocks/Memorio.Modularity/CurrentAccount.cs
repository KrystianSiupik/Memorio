using System.Security.Claims;

namespace Memorio.Modularity;

/// <summary>Who is asking. Endpoints require sign-in by default (SD10), so the claim is always there.</summary>
public static class CurrentAccount
{
    public static Guid IdOf(ClaimsPrincipal principal) =>
        Guid.Parse(principal.FindFirstValue(ClaimTypes.NameIdentifier)!);
}
