using Microsoft.EntityFrameworkCore;
using To_Do_List.Tasks.Entities;

namespace To_Do_List.Tasks.DbContext;

public class TaskDbContext : Microsoft.EntityFrameworkCore.DbContext
{
    public DbSet<TaskItem> Tasks { get; set; }
    public DbSet<Category> Categories { get; set; }

    public TaskDbContext(DbContextOptions<TaskDbContext> options) : base(options)
    {
    }

}