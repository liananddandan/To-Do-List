using To_Do_List.Tasks.Entities;

namespace To_Do_List.Tasks.Interface;

public interface ICategoryRepository
{
    Task<Category?> GetDefaultCategoryAsync(string userId);
    Task<Category?> GetCategoryByIdAsync(string categoryId, string userId);
    Task AddCategoryAsync(Category category);
    Task<IEnumerable<Object>> GetAllCategoriesWithTasksAsync(string userId);
    Task UpdateCategoryAsync(Category category);
}