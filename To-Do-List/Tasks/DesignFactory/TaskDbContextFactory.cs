using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using To_Do_List.Tasks.DbContext;

namespace To_Do_List.Tasks.DesignFactory;

public class TaskDbContextFactory : IDesignTimeDbContextFactory<TaskDbContext>
{
    public TaskDbContext CreateDbContext(string[] args)
    {
        var connectionString = Environment.GetEnvironmentVariable("ConnectionStrings__DefaultConnection");
        if (string.IsNullOrWhiteSpace(connectionString))
        {
            throw new InvalidOperationException("Environment variable 'ConnectionStrings__DefaultConnection' is not set.");
        }

        var optionsBuilder = new DbContextOptionsBuilder<TaskDbContext>();
        optionsBuilder.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString));
        return new TaskDbContext(optionsBuilder.Options);
    }
}