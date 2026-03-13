
using To_Do_List.domain.Entities;

namespace To_Do_List.Application.Interfaces.Repositories;

public interface ITaskRepository
{
    Task AddTaskAsync(TaskItem task);
    Task<TaskItem?> GetTaskByIdAsync(string taskId, string userId);
    Task<IEnumerable<TaskItem>> GetTasksByCategoryIdAsync(string categoryId);
    Task UpdateTaskAsync(TaskItem task);
}
