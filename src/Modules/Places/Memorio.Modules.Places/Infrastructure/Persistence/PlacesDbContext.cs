using Memorio.Modules.Places.Domain.Places;
using Microsoft.EntityFrameworkCore;

namespace Memorio.Modules.Places.Infrastructure.Persistence;

/// <summary>
/// The module's tables in its own schema (ADR 0004). places.osm_places belongs to the OpenStreetMap import
/// (D-131, ODbL applies to it alone): the first migration only makes sure it exists, and EF Core does not map it.
/// </summary>
internal sealed class PlacesDbContext(DbContextOptions<PlacesDbContext> options) : DbContext(options)
{
    public const string Schema = "places";

    public DbSet<OwnPlace> OwnPlaces => Set<OwnPlace>();

    protected override void OnModelCreating(ModelBuilder builder)
    {
        builder.HasDefaultSchema(Schema);

        builder.Entity<OwnPlace>(place =>
        {
            place.ToTable("own_places");
            place.HasKey(item => item.Id);
            place.Property(item => item.Id).HasColumnName("id").ValueGeneratedNever();
            place.Property(item => item.OwnerAccountId).HasColumnName("owner_account_id");
            place.Property(item => item.Name).HasColumnName("name").HasMaxLength(OwnPlace.MaximumNameLength);
            place.Property(item => item.Latitude).HasColumnName("latitude");
            place.Property(item => item.Longitude).HasColumnName("longitude");
            place.Property(item => item.CreatedAt).HasColumnName("created_at");
            place.HasIndex(item => item.OwnerAccountId).HasDatabaseName("ix_own_places_owner_account_id");
        });
    }
}
