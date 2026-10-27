using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace Memorio.Modules.Plans.Infrastructure.Persistence;

/// <summary>For dotnet ef only (creating migrations); never connects.</summary>
internal sealed class PlansDbContextDesignFactory : IDesignTimeDbContextFactory<PlansDbContext>
{
    public PlansDbContext CreateDbContext(string[] args)
    {
        var options = new DbContextOptionsBuilder<PlansDbContext>();
        options.UsePlansDatabase("Host=design-time");
        return new PlansDbContext(options.Options);
    }
}
