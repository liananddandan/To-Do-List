using Microsoft.EntityFrameworkCore;
using To_Do_List.Application.Interfaces.Repositories;
using To_Do_List.domain.Entities;
using To_Do_List.Infrastructure.persistent.DbContext;

namespace To_Do_List.Infrastructure.persistent.repositories;

public class TaskRepository(TaskDbContext taskDbContext) : ITaskRepository
{
    public async Task AddTaskAsync(TaskItem task)
    {
        await taskDbContext.Tasks.AddAsync(task);
        await taskDbContext.SaveChangesAsync();
    }

    public async Task<TaskItem?> GetTaskByIdAsync(string taskId, string userId)
    {
        return await taskDbContext.Tasks
            .Include(t => t.Category)
            .FirstOrDefaultAsync(t => t.Id == Convert.ToInt64(taskId) && t.Category.UserId == userId);
    }

    public async Task<IEnumerable<TaskItem>> GetTasksByCategoryIdAsync(string categoryId)
    {
        return await taskDbContext.Tasks
            .Where(t => t.CategoryId == Convert.ToInt64(categoryId))
            .ToListAsync();
    }

    public async Task UpdateTaskAsync(TaskItem task)
    {
        taskDbContext.Tasks.Update(task);
        await taskDbContext.SaveChangesAsync();
    }
}
