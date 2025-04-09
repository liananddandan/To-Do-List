using To_Do_List.Tasks.Entities;

namespace To_Do_List.Tasks.Interface;

public interface ITaskRepository
{
    Task AddTaskAsync(TaskItem task);
    Task<IEnumerable<TaskItem>> GetTasksByCategoryIdAsync(string categoryId);
}