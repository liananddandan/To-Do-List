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
        return await taskDbContext.Categories.FindAsync(Convert.ToInt64(categoryId));
    }

    public async Task AddCategoryAsync(Category category)
    {
        await taskDbContext.Categories.AddAsync(category);
        await taskDbContext.SaveChangesAsync();
    }

    public async Task<IEnumerable<Object>> GetAllCategoriesWithTasksAsync(string userId)
    {
        return await taskDbContext.Categories
            .Where(c => c.UserId == userId)
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
}