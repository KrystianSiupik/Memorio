using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace Memorio.Modules.Places.Infrastructure.Persistence;

/// <summary>For dotnet ef only (creating migrations); never connects.</summary>
internal sealed class PlacesDbContextDesignFactory : IDesignTimeDbContextFactory<PlacesDbContext>
{
    public PlacesDbContext CreateDbContext(string[] args)
    {
        var options = new DbContextOptionsBuilder<PlacesDbContext>();
        options.UsePlacesDatabase("Host=design-time");
        return new PlacesDbContext(options.Options);
    }
}
