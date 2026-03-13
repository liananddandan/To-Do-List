using Microsoft.EntityFrameworkCore;
using To_Do_List.domain.Entities;

namespace To_Do_List.Infrastructure.persistent.DbContext;

public class TaskDbContext : Microsoft.EntityFrameworkCore.DbContext
{
    public DbSet<TaskItem> Tasks { get; set; }
    public DbSet<Category> Categories { get; set; }

    public TaskDbContext(DbContextOptions<TaskDbContext> options) : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(TaskDbContext).Assembly);
    }
}
