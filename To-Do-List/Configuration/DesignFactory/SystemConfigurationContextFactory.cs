using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using To_Do_List.Configuration.DbContext;

namespace To_Do_List.Configuration.DesignFactory;

public class SystemConfigurationContextFactory : IDesignTimeDbContextFactory<SystemConfigurationContext>
{
    public SystemConfigurationContext CreateDbContext(string[] args)
    {
        var connectionString = Environment.GetEnvironmentVariable("ConnectionStrings__DefaultConnection");

        if (string.IsNullOrWhiteSpace(connectionString))
            throw new InvalidOperationException("Environment variable 'ConnectionStrings__DefaultConnection' is not set.");

        var optionsBuilder = new DbContextOptionsBuilder<SystemConfigurationContext>();
        optionsBuilder.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString));

        return new SystemConfigurationContext(optionsBuilder.Options);
    }
}