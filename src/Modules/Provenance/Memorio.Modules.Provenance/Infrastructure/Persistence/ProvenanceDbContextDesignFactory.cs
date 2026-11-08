using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace Memorio.Modules.Provenance.Infrastructure.Persistence;

/// <summary>For dotnet ef only (creating migrations); never connects.</summary>
internal sealed class ProvenanceDbContextDesignFactory : IDesignTimeDbContextFactory<ProvenanceDbContext>
{
    public ProvenanceDbContext CreateDbContext(string[] args)
    {
        var options = new DbContextOptionsBuilder<ProvenanceDbContext>();
        options.UseProvenanceDatabase("Host=design-time");
        return new ProvenanceDbContext(options.Options);
    }
}
