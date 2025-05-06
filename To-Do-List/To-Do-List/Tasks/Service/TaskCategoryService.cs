using To_Do_List.Require;
using To_Do_List.Tasks.Entities;
using To_Do_List.Tasks.Interface;

namespace To_Do_List.Tasks.Service;

public class TaskCategoryService(ICategoryRepository categoryRepository, ITaskRepository taskRepository)
{
    public async Task<ApiResponseCode> CreateTaskAsync(string title, string? description, DateTime? dueDate,
        int priority,
        string userId, string categoryId)
    {
        Category? category = await categoryRepository.GetCategoryByIdAsync(categoryId, userId);

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

    public async Task<IEnumerable<Object>> GetAllCategoryWithTasksAsync(string userId)
    {
        return await categoryRepository.GetAllCategoriesWithTasksAsync(userId);
    }

    public async Task<ApiResponseCode> CreateDefaultCategoryAsync(long userId)
    {
        Category category = new Category()
        {
            Name = "Default",
            Description = "Default category.",
            UserId = userId.ToString(),
            IsDefault = true
        };
        ApiResponseCode code = await categoryRepository.AddCategoryAsync(category);
        return code == ApiResponseCode.CategoryCreateSuccess
            ? ApiResponseCode.DefaultCategoryCreateSuccess
            : ApiResponseCode.DefaultCategoryCreateFailed;
    }

    public async Task<(ApiResponseCode, Category)> CreateCategoryAsync(string requestName,
        string? requestDescription, string userId)
    {
        var category = new Category()
        {
            Name = requestName,
            Description = requestDescription,
            UserId = userId
        };
        return (await categoryRepository.AddCategoryAsync(category), category);
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

    public async Task<(ApiResponseCode code, IEnumerable<Category>? categories)>
        GetAllCategoryWithoutTasksAsync(string userId)
    {
        var categories = await categoryRepository.GetAllCategoriesWithoutTasksAsync(userId);
        return (categories == null
                ? ApiResponseCode.CategoryGetAllWithoutTasksFailed
                : ApiResponseCode.CategoryGetAllWithoutTasksSuccess,
            categories);
    }
}