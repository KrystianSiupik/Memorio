using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace Memorio.Modules.Access.Infrastructure.Persistence;

/// <summary>For dotnet ef only (creating migrations); never connects.</summary>
internal sealed class AccessDbContextDesignFactory : IDesignTimeDbContextFactory<AccessDbContext>
{
    public AccessDbContext CreateDbContext(string[] args)
    {
        var options = new DbContextOptionsBuilder<AccessDbContext>();
        options.UseAccessDatabase("Host=design-time");
        return new AccessDbContext(options.Options);
    }
}
