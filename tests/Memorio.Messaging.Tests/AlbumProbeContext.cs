using Microsoft.EntityFrameworkCore;

namespace Memorio.Messaging.Tests;

/// <summary>A module's DbContext stand-in: the album role writing its own schema, with the outbox.</summary>
internal sealed class AlbumProbeContext(DbContextOptions<AlbumProbeContext> options) : DbContext(options)
{
    public const string CreateTable = "CREATE TABLE album.probe_items (id uuid PRIMARY KEY)";

    public DbSet<ProbeItem> Items => Set<ProbeItem>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<ProbeItem>(item =>
        {
            item.ToTable("probe_items", "album");
            item.Property(probe => probe.Id).HasColumnName("id");
        });
    }
}

internal sealed class ProbeItem
{
    public Guid Id { get; set; }
}
