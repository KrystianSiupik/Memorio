using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace Memorio.Modules.Limits.Infrastructure.Persistence;

/// <summary>For dotnet ef only (creating migrations); never connects.</summary>
internal sealed class LimitsDbContextDesignFactory : IDesignTimeDbContextFactory<LimitsDbContext>
{
    public LimitsDbContext CreateDbContext(string[] args)
    {
        var options = new DbContextOptionsBuilder<LimitsDbContext>();
        options.UseLimitsDatabase("Host=design-time");
        return new LimitsDbContext(options.Options);
    }
}
