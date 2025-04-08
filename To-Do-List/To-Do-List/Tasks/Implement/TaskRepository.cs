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
}