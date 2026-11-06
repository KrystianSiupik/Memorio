using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace Memorio.Modules.Moderation.Infrastructure.Persistence;

/// <summary>For dotnet ef only (creating migrations); never connects.</summary>
internal sealed class ModerationDbContextDesignFactory : IDesignTimeDbContextFactory<ModerationDbContext>
{
    public ModerationDbContext CreateDbContext(string[] args)
    {
        var options = new DbContextOptionsBuilder<ModerationDbContext>();
        options.UseModerationDatabase("Host=design-time");
        return new ModerationDbContext(options.Options);
    }
}
