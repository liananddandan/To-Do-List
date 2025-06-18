using Microsoft.EntityFrameworkCore;
using To_Do_List.Configuration.DbContext;

namespace To_Do_List.Configuration;

public class SystemConfigurationProvider : ConfigurationProvider
{
    private Action<DbContextOptionsBuilder> _options { get; set; }

    public SystemConfigurationProvider(Action<DbContextOptionsBuilder> options)
    {
        _options = options;
    }

    public override void Load()
    {
        var builder = new DbContextOptionsBuilder<SystemConfigurationContext>();
        _options(builder);
        using var dbContext = new SystemConfigurationContext(builder.Options);

        if (dbContext == null || dbContext.SystemConfigurations == null)
        {
            throw new Exception("Null Db Context");
        }

        dbContext.Database.EnsureCreated();
        Data = dbContext.SystemConfigurations.ToDictionary(c => c.Key, c => c.Value);
    }
}