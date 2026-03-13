using To_Do_List.Application.DTOs;
using To_Do_List.domain.Entities;

namespace To_Do_List.Application.Services.Interface;

public interface ITaskCategoryService
{
    Task<ApiResponseCode> CreateTaskAsync(
        string title,
        string? description,
        DateTime? dueDate,
        int priority,
        string userId,
        string categoryId);

    Task<IEnumerable<CategoryDto>> GetAllCategoryWithTasksAsync(string userId);

    Task<ApiResponseCode> UpdateTaskAsync(
        string taskId,
        string title,
        string? description,
        DateTime? dueDate,
        int priority,
        string userId,
        string categoryId,
        bool isCompleted);

    Task<ApiResponseCode> DeleteTaskById(string taskId, string userId);

    Task<ApiResponseCode> UpdateTaskCompletionAsync(string taskId, bool isCompleted, string userId);

    Task<ApiResponseCode> CreateDefaultCategoryAsync(long userId);

    Task<(ApiResponseCode code, CategoryDto? category)> CreateCategoryAsync(
        string name,
        string? description,
        string userId);

    Task<ApiResponseCode> UpdateCategoryAsync(
        string categoryId,
        string? name,
        string? description,
        bool? isDeleted,
        string userId);

    Task<ApiResponseCode> DeleteCategoryById(
        string categoryId,
        string userId);

    Task<(ApiResponseCode code, IEnumerable<CategoryDto>? categories)>
        GetAllCategoryWithoutTasksAsync(string userId);
}
