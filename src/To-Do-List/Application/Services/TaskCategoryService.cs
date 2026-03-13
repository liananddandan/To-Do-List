using To_Do_List.Application.DTOs;
using To_Do_List.Application.Interfaces.Repositories;
using To_Do_List.Application.Services.Interface;
using To_Do_List.domain.Entities;

namespace To_Do_List.Application.Services;

public class TaskCategoryService(ICategoryRepository categoryRepository, ITaskRepository taskRepository) : ITaskCategoryService
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

        try
        {
            await taskRepository.AddTaskAsync(task);
        }
        catch (Exception e)
        {
            return ApiResponseCode.TaskAddExceptionInDB;
        }

        return ApiResponseCode.TaskCreateSuccess;
    }

    public async Task<IEnumerable<CategoryDto>> GetAllCategoryWithTasksAsync(string userId)
    {
        var categories =  await categoryRepository.GetAllCategoriesWithTasksAsync(userId);
        return categories.Select(c => c.ToDto());
    }

    public async Task<ApiResponseCode> UpdateTaskAsync(string taskId, string title, string? description,
        DateTime? dueDate, int priority, string userId, string categoryId, bool isCompleted)
    {
        var task = await taskRepository.GetTaskByIdAsync(taskId, userId);
        if (task == null)
        {
            return ApiResponseCode.TaskIdNotFoundForCurrentUser;
        }

        var category = await categoryRepository.GetCategoryByIdAsync(categoryId, userId);
        if (category == null)
        {
            return ApiResponseCode.CategoryIdNotFoundForCurrentUser;
        }

        task.Title = title;
        task.Description = description;
        task.DueDate = dueDate;
        task.Priority = priority;
        task.IsCompleted = isCompleted;
        task.CategoryId = category.Id;

        try
        {
            await taskRepository.UpdateTaskAsync(task);
        }
        catch (Exception)
        {
            return ApiResponseCode.TaskUpdateFailedInDB;
        }

        return ApiResponseCode.TaskUpdateSuccess;
    }

    public async Task<ApiResponseCode> DeleteTaskById(string taskId, string userId)
    {
        var task = await taskRepository.GetTaskByIdAsync(taskId, userId);
        if (task == null)
        {
            return ApiResponseCode.TaskIdNotFoundForCurrentUser;
        }

        task.IsDeleted = true;

        try
        {
            await taskRepository.UpdateTaskAsync(task);
        }
        catch (Exception)
        {
            return ApiResponseCode.TaskDeleteFailedInDB;
        }

        return ApiResponseCode.TaskDeleteSuccess;
    }

    public async Task<ApiResponseCode> UpdateTaskCompletionAsync(string taskId, bool isCompleted, string userId)
    {
        var task = await taskRepository.GetTaskByIdAsync(taskId, userId);
        if (task == null)
        {
            return ApiResponseCode.TaskIdNotFoundForCurrentUser;
        }

        task.IsCompleted = isCompleted;

        try
        {
            await taskRepository.UpdateTaskAsync(task);
        }
        catch (Exception)
        {
            return ApiResponseCode.TaskUpdateFailedInDB;
        }

        return ApiResponseCode.TaskCompletionUpdateSuccess;
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
        try
        {
            ApiResponseCode code = await categoryRepository.AddCategoryAsync(category);
            return code == ApiResponseCode.CategoryCreateSuccess
                ? ApiResponseCode.DefaultCategoryCreateSuccess
                : ApiResponseCode.DefaultCategoryCreateFailed;
        }
        catch (Exception e)
        {
            return ApiResponseCode.DefaultCategoryCreateFailed;
        }
    }

    public async Task<(ApiResponseCode, CategoryDto?)> CreateCategoryAsync(string name,
        string? description, string userId)
    {
        if (await categoryRepository.IsCategoryNameInUseAsync(userId, name))
        {
            return (ApiResponseCode.CategoryCreateDuplicatedNameFail, null);
        }

        var category = new Category()
        {
            Name = name,
            Description = description,
            UserId = userId
        };
        try
        {
            ApiResponseCode code = await categoryRepository.AddCategoryAsync(category);
            if (code == ApiResponseCode.CategoryCreateSuccess)
            {
                return (code, category.ToDto());
            }
            else
            {
                return (code, null);
            }
        }
        catch (Exception e)
        {
            return (ApiResponseCode.CategoryCreateFailed, null);
        }
    }

    public async Task<ApiResponseCode> UpdateCategoryAsync(string categoryId, string? name,
        string? description, Boolean? isDeleted, string userId)
    {
        var category = await categoryRepository.GetMayDeletedCategoryByIdAsync(categoryId, userId);
        if (category == null)
        {
            return ApiResponseCode.CategoryIdNotFoundForCurrentUser;
        }

        if (!string.IsNullOrEmpty(name))
        {
            if (await categoryRepository.IsCategoryNameInUseAsync(userId, name, category.Id))
            {
                return ApiResponseCode.CategoryCreateDuplicatedNameFail;
            }
            category.Name = name;
        }

        if (!string.IsNullOrEmpty(description))
        {
            category.Description = description;
        }

        if (isDeleted.HasValue)
        {
            category.IsDeleted = isDeleted.Value;
        }

        try
        {
            await categoryRepository.UpdateCategoryAsync(category);
        }
        catch (Exception e)
        {
            return ApiResponseCode.CategoryUpdateFailedInDB;
        }

        return ApiResponseCode.CategoryUpdateSuccess;
    }

    public async Task<ApiResponseCode> DeleteCategoryById(string categoryId, string userId)
    {
        var category = await categoryRepository.GetCategoryByIdAsync(categoryId, userId);
        if (category == null)
        {
            return ApiResponseCode.CategoryIdNotFoundForCurrentUser;
        }

        if (category.IsDefault)
        {
            return ApiResponseCode.CategoryDefaultCannotBeDeleted;
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
            task.CategoryId = defaultCategory.Id;
            task.Category = defaultCategory;
        }

        try
        {
            await categoryRepository.UpdateCategoryAsync(category);
        }
        catch (Exception)
        {
            return ApiResponseCode.CategoryUpdateFailedInDB;
        }

        return ApiResponseCode.DeleteCategorySuccess;
    }

    public async Task<(ApiResponseCode code, IEnumerable<CategoryDto>? categories)>
        GetAllCategoryWithoutTasksAsync(string userId)
    {
        try
        {
            var categories = await categoryRepository.GetAllCategoriesWithoutTasksAsync(userId);
            return (ApiResponseCode.CategoryGetAllWithoutTasksSuccess, categories.Select(c => c.ToDto()));
        }
        catch (Exception e)
        {
            return (ApiResponseCode.CategoryGetAllWithoutTasksFailed, null);
        }
    }
}
