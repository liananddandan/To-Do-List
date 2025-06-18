using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using To_Do_List.Configuration.DbContext;

namespace To_Do_List.Configuration.DesignFactory;

public class SystemConfigurationContextFactory : IDesignTimeDbContextFactory<SystemConfigurationContext>
{
    public SystemConfigurationContext CreateDbContext(string[] args)
    {
        var optionsBuilder = new DbContextOptionsBuilder<SystemConfigurationContext>();
        var connectionString = "Server=localhost;Database=ToDo;User=root;Password=12345678;";
        optionsBuilder.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString));
        return new SystemConfigurationContext(optionsBuilder.Options);
    }
}