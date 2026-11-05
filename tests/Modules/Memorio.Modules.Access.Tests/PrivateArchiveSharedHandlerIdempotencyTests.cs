using Memorio.Modules.Access.Application.Policies;
using Memorio.Modules.Access.Contracts;
using Memorio.Modules.Account.Contracts;
using Memorio.Modules.Succession.Contracts;
using Microsoft.Extensions.DependencyInjection;

namespace Memorio.Modules.Access.Tests;

/// <summary>SD6 and P31: E126 delivered twice, on a real database. Only who was named, and nobody else.</summary>
public sealed class PrivateArchiveSharedHandlerIdempotencyTests(AccessDatabase database)
{
    private static CancellationToken Cancellation => TestContext.Current.CancellationToken;

    private async Task DeliverAsync(Guid owner)
    {
        await using var scope = database.CreateScope();
        await scope.ServiceProvider.GetRequiredService<GrantLegacyRead>().HandleAsync(new PrivateArchiveSharedEvent(Guid.CreateVersion7(), DateTimeOffset.UtcNow, owner, 2), Cancellation);
    }

    private async Task<IReadOnlyCollection<Guid>> ArchivesOfAsync(Guid person)
    {
        await using var scope = database.CreateScope();
        return await scope.ServiceProvider.GetRequiredService<IAccessReader>().LegacyArchivesAsync(person, Cancellation);
    }

    [Fact]
    public async Task The_contact_reads_and_nobody_else_and_the_address_is_not_stored()
    {
        var (owner, contact, stranger) = (Guid.CreateVersion7(), Guid.CreateVersion7(), Guid.CreateVersion7());
        var email = $"{contact}@example.invalid";
        database.Neighbours.ContactOf(owner, email.ToUpperInvariant());

        await DeliverAsync(owner);
        await DeliverAsync(owner);

        Assert.Equal([owner], await ArchivesOfAsync(contact));
        Assert.Empty(await ArchivesOfAsync(stranger));
        await using var command = database.Access.CreateCommand("SELECT count(*) FROM access.legacy_grants WHERE owner_account_id = $1 AND contact_hash NOT LIKE '%@%'");
        command.Parameters.Add(new() { Value = owner });
        Assert.Equal(1L, await command.ExecuteScalarAsync(Cancellation));
    }

    [Fact]
    public async Task Without_a_contact_nothing_is_granted()
    {
        var owner = Guid.CreateVersion7();

        await DeliverAsync(owner);

        await using var command = database.Access.CreateCommand("SELECT count(*) FROM access.legacy_grants WHERE owner_account_id = $1");
        command.Parameters.Add(new() { Value = owner });
        Assert.Equal(0L, await command.ExecuteScalarAsync(Cancellation));
    }

    [Fact]
    public async Task The_owners_account_gone_takes_the_grant_with_it()
    {
        var (owner, contact) = (Guid.CreateVersion7(), Guid.CreateVersion7());
        database.Neighbours.ContactOf(owner, $"{contact}@example.invalid");
        await DeliverAsync(owner);

        await using var scope = database.CreateScope();
        await scope.ServiceProvider.GetRequiredService<ForgetDeletedAccount>().HandleAsync(new AccountDeleted(Guid.CreateVersion7(), DateTimeOffset.UtcNow, owner, 3), Cancellation);

        Assert.Empty(await ArchivesOfAsync(contact));
    }
}
