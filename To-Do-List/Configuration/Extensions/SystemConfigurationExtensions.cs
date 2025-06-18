using Microsoft.EntityFrameworkCore;

namespace To_Do_List.Configuration.Extensions;

public static class SystemConfigurationExtensions
{
    public static IConfigurationBuilder AddEFConfiguration(this IConfigurationBuilder builder,
        Action<DbContextOptionsBuilder> options)
    {
        return builder.Add(new SystemConfigurationSource(options));
    }
}