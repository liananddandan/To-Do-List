using To_Do_List.Require;
using To_Do_List.Tasks.Entities;

namespace To_Do_List.Tasks.Interface;

public interface ICategoryRepository
{
    Task<Category?> GetDefaultCategoryAsync(string userId);
    Task<Category?> GetCategoryByIdAsync(string categoryId, string userId);
    Task<ApiResponseCode> AddCategoryAsync(Category category);
    Task<IEnumerable<Category>> GetAllCategoriesWithTasksAsync(string userId);
    Task<IEnumerable<Category>> GetAllCategoriesWithoutTasksAsync(string userId);
    Task UpdateCategoryAsync(Category category);
}