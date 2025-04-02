using Microsoft.EntityFrameworkCore;
using To_Do_List.Configuration.Entities;

namespace To_Do_List.Configuration.DbContext;

public class SystemConfigurationContext : Microsoft.EntityFrameworkCore.DbContext
{
    public DbSet<SystemConfiguration> SystemConfigurations => Set<SystemConfiguration>();
    public SystemConfigurationContext(DbContextOptions<SystemConfigurationContext> options) : base(options)
    {
        
    }
}