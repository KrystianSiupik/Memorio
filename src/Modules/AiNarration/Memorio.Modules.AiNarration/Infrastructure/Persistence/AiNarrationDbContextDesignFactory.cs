using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace Memorio.Modules.AiNarration.Infrastructure.Persistence;

/// <summary>For dotnet ef only (creating migrations); never connects.</summary>
internal sealed class AiNarrationDbContextDesignFactory : IDesignTimeDbContextFactory<AiNarrationDbContext>
{
    public AiNarrationDbContext CreateDbContext(string[] args)
    {
        var options = new DbContextOptionsBuilder<AiNarrationDbContext>();
        options.UseAiNarrationDatabase("Host=design-time");
        return new AiNarrationDbContext(options.Options);
    }
}
