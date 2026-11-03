using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace Memorio.Modules.Archive.Infrastructure.Persistence;

/// <summary>For dotnet ef only (creating migrations); never connects.</summary>
internal sealed class ArchiveDbContextDesignFactory : IDesignTimeDbContextFactory<ArchiveDbContext>
{
    public ArchiveDbContext CreateDbContext(string[] args)
    {
        var options = new DbContextOptionsBuilder<ArchiveDbContext>();
        options.UseArchiveDatabase("Host=design-time");
        return new ArchiveDbContext(options.Options);
    }
}
