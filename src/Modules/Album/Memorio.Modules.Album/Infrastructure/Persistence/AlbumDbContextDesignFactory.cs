using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace Memorio.Modules.Album.Infrastructure.Persistence;

/// <summary>For dotnet ef only (creating migrations); never connects.</summary>
internal sealed class AlbumDbContextDesignFactory : IDesignTimeDbContextFactory<AlbumDbContext>
{
    public AlbumDbContext CreateDbContext(string[] args)
    {
        var options = new DbContextOptionsBuilder<AlbumDbContext>();
        options.UseAlbumDatabase("Host=design-time");
        return new AlbumDbContext(options.Options);
    }
}
