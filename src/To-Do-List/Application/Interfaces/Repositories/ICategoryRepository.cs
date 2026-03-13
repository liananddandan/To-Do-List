using To_Do_List.Application.DTOs;
using To_Do_List.domain.Entities;

namespace To_Do_List.Application.Interfaces.Repositories;

public interface ICategoryRepository
{
    Task<Category?> GetDefaultCategoryAsync(string userId);
    Task<Category?> GetCategoryByIdAsync(string categoryId, string userId);
    Task<Category?> GetMayDeletedCategoryByIdAsync(string categoryId, string userId);
    Task<ApiResponseCode> AddCategoryAsync(Category category);
    Task<IEnumerable<Category>> GetAllCategoriesWithTasksAsync(string userId);
    Task<IEnumerable<Category>> GetAllCategoriesWithoutTasksAsync(string userId);
    Task UpdateCategoryAsync(Category category);
    Task<bool> IsCategoryNameInUseAsync(string userId, string name, long? excludedCategoryId = null);
}
