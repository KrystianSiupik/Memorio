using Memorio.Modules.Limits.Domain.Pools;
using Microsoft.EntityFrameworkCore;

namespace Memorio.Modules.Limits.Infrastructure.Persistence;

/// <summary>The module's tables in its own schema (ADR 0004). Column names in snake_case: the conditional updates are SQL (D-016).</summary>
internal sealed class LimitsDbContext(DbContextOptions<LimitsDbContext> options) : DbContext(options)
{
    public const string Schema = "limits";

    public DbSet<AccountPool> AccountPools => Set<AccountPool>();

    public DbSet<PoolOperation> PoolOperations => Set<PoolOperation>();

    protected override void OnModelCreating(ModelBuilder builder)
    {
        builder.HasDefaultSchema(Schema);

        builder.Entity<AccountPool>(pool =>
        {
            pool.ToTable("account_pools");
            pool.HasKey(item => item.AccountId);
            pool.Property(item => item.AccountId).HasColumnName("account_id").ValueGeneratedNever();
            pool.Property(item => item.UsedBytes).HasColumnName("used_bytes");
            pool.Property(item => item.LimitBytes).HasColumnName("limit_bytes");
            pool.Property(item => item.LimitSequence).HasColumnName("limit_sequence");
        });
        builder.Entity<PoolOperation>(operation =>
        {
            operation.ToTable("pool_operations");
            operation.HasKey(item => item.OperationKey);
            operation.Property(item => item.OperationKey).HasColumnName("operation_key").ValueGeneratedNever();
            operation.Property(item => item.AccountId).HasColumnName("account_id");
            operation.Property(item => item.Bytes).HasColumnName("bytes");
            operation.Property(item => item.ReservedAt).HasColumnName("reserved_at");
        });
    }
}
