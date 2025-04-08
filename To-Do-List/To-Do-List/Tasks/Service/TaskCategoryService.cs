using To_Do_List.Require;
using To_Do_List.Tasks.Entities;
using To_Do_List.Tasks.Interface;

namespace To_Do_List.Tasks.Service;

public class TaskCategoryService(ICategoryRepository categoryRepository, ITaskRepository taskRepository)
{
    public async Task<ApiResponseCode> CreateTaskAsync(string title, string? description, DateTime? dueDate, int priority,
        string userId, string? categoryId)
    {
        Category? category;
        if (categoryId == null)
        {
            category = await GetOrCreateDefaultCategoryAsync(userId);
        }
        else
        {
            category = await categoryRepository.GetCategoryByIdAsync(categoryId);
            if (category == null)
            {
                return ApiResponseCode.CategoryIdNotFound;
            }
        }

        var task = new TaskItem()
        {
            Title = title,
            Description = description,
            Category = category,
            DueDate = dueDate,
            Priority = priority,
            IsCompleted = false
        };

        await taskRepository.AddTaskAsync(task);
        return ApiResponseCode.TaskCreateSuccess;
    }

    private async Task<Category> GetOrCreateDefaultCategoryAsync(string userId)
    {
        var category = await categoryRepository.GetDefaultCategoryAsync(userId);
        if (category == null)
        {
            category = new Category()
            {
                Name = "Default",
                Description = "Default category",
                UserId = userId,
                IsDefault = true
            };
            await categoryRepository.AddAsync(category);
        }

        return category;
    }
}