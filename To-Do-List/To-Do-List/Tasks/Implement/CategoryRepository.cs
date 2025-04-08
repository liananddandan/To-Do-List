using Microsoft.EntityFrameworkCore;
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

    public async Task<Category?> GetCategoryByIdAsync(string categoryId)
    {
        return await taskDbContext.Categories.FindAsync(categoryId);
    }

    public async Task AddAsync(Category category)
    {
        await taskDbContext.Categories.AddAsync(category);
        await taskDbContext.SaveChangesAsync();
    }
}