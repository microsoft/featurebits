using FeatureBits.Data.EF;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

public class DesignTimeDbContextFactory : IDesignTimeDbContextFactory<FeatureBitsEfDbContext>
{
    /// <summary>
    /// This class is used to support generation of EF migrations.
    /// </summary>
    /// <param name="args"></param>
    /// <returns></returns>
    public FeatureBitsEfDbContext CreateDbContext(string[] args)
    {
        var builder = new DbContextOptionsBuilder<FeatureBitsEfDbContext>();
        builder.UseSqlServer("efMigrations");
        return new FeatureBitsEfDbContext(builder.Options);
    }
}