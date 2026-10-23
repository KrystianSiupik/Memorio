using Memorio.SharedKernel.Results;

namespace Memorio.Modules.Account.Domain.Accounts;

/// <summary>
/// The person's own statement that they are at least <see cref="MinimumAge"/> (S3:L05). Nobody under
/// it may have an account, so an account is created only with this statement, and we keep when it was made.
/// </summary>
internal sealed record AgeDeclaration
{
    public const int MinimumAge = 16;

    private AgeDeclaration(DateTimeOffset declaredAt)
    {
        DeclaredAt = declaredAt;
    }

    public DateTimeOffset DeclaredAt { get; }

    public static Result<AgeDeclaration> Declare(bool isAtLeastMinimumAge, DateTimeOffset now) =>
        isAtLeastMinimumAge ? new AgeDeclaration(now) : AccountErrors.AgeDeclarationRequired;
}
