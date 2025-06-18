using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using To_Do_List.Identity.DbContext;

namespace To_Do_List.Identity.DesignFactory;

public class MyIdentityDbContextFactory : IDesignTimeDbContextFactory<MyIdentityDbContext>
{
    public MyIdentityDbContext CreateDbContext(string[] args)
    {
        var connectionString = Environment.GetEnvironmentVariable("ConnectionStrings__DefaultConnection");

        if (string.IsNullOrWhiteSpace(connectionString))
            throw new InvalidOperationException("Environment variable 'ConnectionStrings__DefaultConnection' is not set.");

        var optionsBuilder = new DbContextOptionsBuilder<MyIdentityDbContext>();
        optionsBuilder.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString));

        return new MyIdentityDbContext(optionsBuilder.Options);
    }
}