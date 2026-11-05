using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace Memorio.Modules.Succession.Infrastructure.Persistence;

/// <summary>For dotnet ef only (creating migrations); never connects.</summary>
internal sealed class SuccessionDbContextDesignFactory : IDesignTimeDbContextFactory<SuccessionDbContext>
{
    public SuccessionDbContext CreateDbContext(string[] args)
    {
        var options = new DbContextOptionsBuilder<SuccessionDbContext>();
        options.UseSuccessionDatabase("Host=design-time");
        return new SuccessionDbContext(options.Options);
    }
}
