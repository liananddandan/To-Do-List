using Microsoft.EntityFrameworkCore;
using To_Do_List.Tasks.DbContext;
using To_Do_List.Tasks.Entities;
using To_Do_List.Tasks.Interface;

namespace To_Do_List.Tasks.Implement;

public class TaskRepository(TaskDbContext taskDbContext) : ITaskRepository
{
    public async Task AddTaskAsync(TaskItem task)
    {
        await taskDbContext.Tasks.AddAsync(task);
        await taskDbContext.SaveChangesAsync();
    }

    public async Task<IEnumerable<TaskItem>> GetTasksByCategoryIdAsync(string categoryId)
    {
        return await taskDbContext.Tasks
            .Where(t => t.Category.Id == Convert.ToInt64(categoryId))
            .ToListAsync();
    }
}