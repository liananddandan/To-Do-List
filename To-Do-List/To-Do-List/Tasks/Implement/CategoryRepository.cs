using Microsoft.EntityFrameworkCore;
using To_Do_List.Require;
using To_Do_List.Tasks.DbContext;
using To_Do_List.Tasks.Entities;
using To_Do_List.Tasks.Interface;

namespace To_Do_List.Tasks.Implement;

public class CategoryRepository(TaskDbContext taskDbContext) : ICategoryRepository
{
    public async Task<Category?> GetDefaultCategoryAsync(string userId)
    {
        return await taskDbContext.Categories
            .Where(c => c.UserId == userId && c.IsDefault)
            .FirstOrDefaultAsync();
    }

    public async Task<Category?> GetCategoryByIdAsync(string categoryId, string userId)
    {
        return await taskDbContext.Categories
            .Where(c => c.UserId == userId 
                        && c.Id == Convert.ToInt64(categoryId)
                        && !c.IsDeleted)
            .FirstOrDefaultAsync();
    }

    public async Task<ApiResponseCode> AddCategoryAsync(Category category)
    {
        try
        {
            if (category.UserId == null)
            {
                return ApiResponseCode.CategoryCreateWithoutUserIdFail;
            }

            if (taskDbContext.Categories
                .Any(c => c.UserId == category.UserId
                                                    && c.Name == category.Name))
            {
                return ApiResponseCode.CategoryCreateDuplicatedNameFail;
            }

            await taskDbContext.Categories.AddAsync(category);
            await taskDbContext.SaveChangesAsync();
            return ApiResponseCode.CategoryCreateSuccess;
        }
        catch
        {
            return ApiResponseCode.CategoryCreateFailed;
        }
    }

    public async Task<IEnumerable<Object>> GetAllCategoriesWithTasksAsync(string userId)
    {
        return await taskDbContext.Categories
            .Where(c => c.UserId == userId && !c.IsDeleted)
            .Select(c => new
            {
                c.Id,
                c.Name,
                c.Description,
                c.CreatedAt,
                c.IsDefault,
                Tasks = c.Tasks.Where(t => !t.IsDeleted)
                    .OrderBy(t => t.Priority)
                    .Select(t => new
                {
                    t.Id,
                    t.Description,
                    t.Title,
                    t.IsCompleted,
                    t.DueDate,
                    t.CreatedDate,
                    t.Priority
                }).ToList()
            })
            .ToListAsync();
    }

    public async Task<IEnumerable<Category>?> GetAllCategoriesWithoutTasksAsync(string userId)
    {
        try
        {
            return await taskDbContext.Categories.Where(c => c.UserId == userId)
                .ToListAsync();
        }
        catch (Exception ex)
        {
            return null;
        }
    }

    public async Task UpdateCategoryAsync(Category category)
    {
        taskDbContext.Categories.Update(category);
        await taskDbContext.SaveChangesAsync();
    }
    
}