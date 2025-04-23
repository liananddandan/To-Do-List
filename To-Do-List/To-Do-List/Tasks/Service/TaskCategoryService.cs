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
            category = await categoryRepository.GetCategoryByIdAsync(categoryId, userId);
        }
        
        if (category == null)
        {
            return ApiResponseCode.CategoryIdNotFoundForCurrentUser;
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

    private async Task<Category?> GetOrCreateDefaultCategoryAsync(string userId)
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
            ApiResponseCode code = await categoryRepository.AddCategoryAsync(category);
            if (code != ApiResponseCode.CategoryCreateSuccess)
            {
                return null;
            }
        }

        return category;
    }

    public async Task<IEnumerable<Object>> GetAllCategoryWithTasksAsync(string userId)
    {
        return await categoryRepository.GetAllCategoriesWithTasksAsync(userId);
    }

    public async Task<ApiResponseCode> CreateCategoryAsync(string requestName, 
        string? requestDescription, string userId)
    {
        var category = new Category()
        {
            Name = requestName,
            Description = requestDescription,
            UserId = userId
        };
        return await categoryRepository.AddCategoryAsync(category);
    }

    public async Task<ApiResponseCode> UpdateCategoryAsync(string categoryId, string? requestName, 
        string? requestDescription, string userId)
    {
        var category = await categoryRepository.GetCategoryByIdAsync(categoryId, userId);
        if (category == null)
        {
            return ApiResponseCode.CategoryIdNotFoundForCurrentUser;
        }

        if (!string.IsNullOrEmpty(requestName))
        {
            category.Name = requestName;
        }

        if (!string.IsNullOrEmpty(requestDescription))
        {
            category.Description = requestDescription;
        }

        await categoryRepository.UpdateCategoryAsync(category);
        return ApiResponseCode.CategoryUpdateSuccess;
    }

    public async Task<ApiResponseCode> DeleteCategoryById(string categoryId, string userId)
    {
        var category = await categoryRepository.GetCategoryByIdAsync(categoryId, userId);
        if (category == null)
        {
            return ApiResponseCode.CategoryIdNotFoundForCurrentUser;
        }
        category.IsDeleted = true;
        var defaultCategory = await categoryRepository.GetDefaultCategoryAsync(userId);
        if (defaultCategory == null)
        {
            return ApiResponseCode.CategoryDefaultIsMissing;
        }
        var tasks = await taskRepository.GetTasksByCategoryIdAsync(categoryId);
        foreach (var task in tasks)
        {
            task.Category = defaultCategory;
        }
        await categoryRepository.UpdateCategoryAsync(category);
        return ApiResponseCode.DeleteCategorySuccess;
    }
}