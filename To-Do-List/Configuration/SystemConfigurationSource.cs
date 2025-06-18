using Microsoft.EntityFrameworkCore;

namespace To_Do_List.Configuration;

public class SystemConfigurationSource : IConfigurationSource
{
    private readonly Action<DbContextOptionsBuilder> _options;

    public SystemConfigurationSource(Action<DbContextOptionsBuilder> options)
    {
        _options = options;
    }

    public IConfigurationProvider Build(IConfigurationBuilder builder)
    {
        return new SystemConfigurationProvider(_options);
    }
}