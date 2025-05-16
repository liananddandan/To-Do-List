using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using To_Do_List.Require;
using To_Do_List.Tasks.DbContext;
using To_Do_List.Tasks.Entities;
using To_Do_List.Tasks.Implement;

namespace To_Do_List.Tests.Tasks.Implement;

public class CategoryRepositoryTests
{
    [Fact]
    public async Task GetDefaultCategoryAsync_ReturnsDefaultCategory_ForGivenUser()
    {
        // Arrange
        var options = new DbContextOptionsBuilder<TaskDbContext>()
            .UseInMemoryDatabase(databaseName: "Test_GetDefaultCategory")
            .Options;

        var userId = "1";

        // 使用上下文添加测试数据
        using (var context = new TaskDbContext(options))
        {
            context.Categories.Add(new Category { Id = 1, Name = "Work", UserId = userId, IsDefault = false });
            context.Categories.Add(new Category { Id = 2, Name = "Personal", UserId = userId, IsDefault = true });
            context.Categories.Add(new Category { Id = 3, Name = "Hobby", UserId = "2", IsDefault = true });
            await context.SaveChangesAsync();
        }

        // Act
        using (var context = new TaskDbContext(options))
        {
            var service = new CategoryRepository(context);
            var result = await service.GetDefaultCategoryAsync(userId);

            // Assert
            Assert.NotNull(result);
            Assert.Equal("Personal", result.Name);
            Assert.True(result.IsDefault);
            Assert.Equal(userId, result.UserId);
        }
    }
    
    [Fact]
    public async Task GetCategoryByIdAsync_ReturnsSpecificCategory_ForGivenUser()
    {
        // Arrange
        var options = new DbContextOptionsBuilder<TaskDbContext>()
            .UseInMemoryDatabase(databaseName: "Test_GetCategoryById")
            .Options;

        var userId = "1";

        // 使用上下文添加测试数据
        using (var context = new TaskDbContext(options))
        {
            context.Categories.Add(new Category { Id = 1, Name = "Work", UserId = userId, IsDefault = false });
            context.Categories.Add(new Category { Id = 2, Name = "Personal", UserId = userId, IsDefault = true });
            context.Categories.Add(new Category { Id = 3, Name = "Hobby", UserId = "2", IsDefault = true });
            await context.SaveChangesAsync();
        }

        // Act
        using (var context = new TaskDbContext(options))
        {
            var service = new CategoryRepository(context);
            var result = await service.GetCategoryByIdAsync("1", userId);

            // Assert
            Assert.NotNull(result);
            Assert.Equal("Work", result.Name);
            Assert.False(result.IsDefault);
            Assert.Equal(userId, result.UserId);
        }
    }

    [Fact]
    public async Task AddCategoryAsync_AddCategoryFailed_ForUserIdIsNull()
    {
        // Arrange
        var options = new DbContextOptionsBuilder<TaskDbContext>()
            .UseInMemoryDatabase(databaseName: "Test_AddCategoryFailed")
            .Options;
        var category = new Category { Id = 1, Name = "Work", UserId = null, IsDefault = false };

        ApiResponseCode result;
        // Act
        using (var context = new TaskDbContext(options))
        {
            var repository = new CategoryRepository(context);
            result = await repository.AddCategoryAsync(category);
        }
        
        // Assert
        result.Should().Be(ApiResponseCode.CategoryCreateWithoutUserIdFail);
    }
    
    [Fact]
    public async Task AddCategoryAsync_AddCategoryFailed_ForDuplicateCategory()
    {
        // Arrange
        var options = new DbContextOptionsBuilder<TaskDbContext>()
            .UseInMemoryDatabase(databaseName: "Test_AddCategoryFailed1")
            .Options;
        var category = new Category { Id = 1, Name = "Work", UserId = "1", IsDefault = false };

        ApiResponseCode result;
        // Act
        using (var context = new TaskDbContext(options))
        {
            await context.Categories.AddAsync(category);
            await context.SaveChangesAsync();
            var repository = new CategoryRepository(context);
            result = await repository.AddCategoryAsync(category);
        }
        
        // Assert
        result.Should().Be(ApiResponseCode.CategoryCreateDuplicatedNameFail);
    }
    
    [Fact]
    public async Task AddCategoryAsync_AddCategorySuccess_ForNewCategory()
    {
        // Arrange
        var options = new DbContextOptionsBuilder<TaskDbContext>()
            .UseInMemoryDatabase(databaseName: "Test_AddCategorySuccess")
            .Options;
        var category = new Category { Id = 1, Name = "Work", UserId = "1", IsDefault = false };
        var category1 = new Category { Id = 2, Name = "Personal", UserId = "1", IsDefault = false };

        ApiResponseCode result;
        // Act
        using (var context = new TaskDbContext(options))
        {
            await context.Categories.AddAsync(category);
            await context.SaveChangesAsync();
            var repository = new CategoryRepository(context);
            result = await repository.AddCategoryAsync(category1);
        }
        
        // Assert
        result.Should().Be(ApiResponseCode.CategoryCreateSuccess);
    }

    [Fact]
    public async Task GetAllCategoriesWithTasksAsync_ReturnsAllCategories()
    {
        // Arrange
        var options = new DbContextOptionsBuilder<TaskDbContext>()
            .UseInMemoryDatabase(databaseName: "Test_GetAllCategoriesWithTasks")
            .Options;
        var category = new Category { Id = 1, Name = "Work", UserId = "1", IsDefault = false };
        var category1 = new Category { Id = 2, Name = "Personal", UserId = "1", IsDefault = false };
        var category2 = new Category { Id = 3, Name = "Hobby", UserId = "2", IsDefault = true };

        var taskItem = new TaskItem { Id = 1, Category = category, Title = "task1", Description = "task1" };
        var taskItem1 = new TaskItem { Id = 2, Category = category, Title = "task2", Description = "task2" };
        var taskItem2 = new TaskItem { Id = 3, Category = category1, Title = "task3", Description = "task3" };
        var taskItem3 = new TaskItem { Id = 4, Category = category1, Title = "task4", Description = "task4" };
        var taskItem4 = new TaskItem { Id = 5, Category = category2, Title = "task5", Description = "task5" };
        
        // Act
        IEnumerable<Category> result;
        using (var context = new TaskDbContext(options))
        {
            await context.Categories.AddAsync(category);
            await context.Categories.AddAsync(category1);
            await context.Categories.AddAsync(category2);
            await context.Tasks.AddAsync(taskItem);
            await context.Tasks.AddAsync(taskItem1);
            await context.Tasks.AddAsync(taskItem2);
            await context.Tasks.AddAsync(taskItem3);
            await context.Tasks.AddAsync(taskItem4);
            await context.SaveChangesAsync();
            var repository = new CategoryRepository(context);
            result = await repository.GetAllCategoriesWithTasksAsync("1");
        }
        
        // Assert
        List<Category> actualResult = result.ToList();
        actualResult.Should().NotBeNullOrEmpty();
        actualResult.Should().HaveCount(2);
        actualResult[0].Tasks.Should().HaveCount(2);
    }
    
       [Fact]
    public async Task GetAllCategoriesWithoutTasksAsync_ReturnsAllCategories()
    {
        // Arrange
        var options = new DbContextOptionsBuilder<TaskDbContext>()
            .UseInMemoryDatabase(databaseName: "Test_GetAllCategoriesWithoutTasks")
            .Options;
        var category = new Category { Id = 1, Name = "Work", UserId = "1", IsDefault = false };
        var category1 = new Category { Id = 2, Name = "Personal", UserId = "1", IsDefault = false };
        var category2 = new Category { Id = 3, Name = "Hobby", UserId = "2", IsDefault = true };

        var taskItem = new TaskItem { Id = 1, Category = category, Title = "task1", Description = "task1" };
        var taskItem1 = new TaskItem { Id = 2, Category = category, Title = "task2", Description = "task2" };
        var taskItem2 = new TaskItem { Id = 3, Category = category1, Title = "task3", Description = "task3" };
        var taskItem3 = new TaskItem { Id = 4, Category = category1, Title = "task4", Description = "task4" };
        var taskItem4 = new TaskItem { Id = 5, Category = category2, Title = "task5", Description = "task5" };
        
        // Act
        using (var context = new TaskDbContext(options))
        {
            await context.Categories.AddAsync(category);
            await context.Categories.AddAsync(category1);
            await context.Categories.AddAsync(category2);
            await context.Tasks.AddAsync(taskItem);
            await context.Tasks.AddAsync(taskItem1);
            await context.Tasks.AddAsync(taskItem2);
            await context.Tasks.AddAsync(taskItem3);
            await context.Tasks.AddAsync(taskItem4);
            await context.SaveChangesAsync();
        }
        
        IEnumerable<Category> result;
        using (var testContext = new TaskDbContext(options))
        {
            var repository = new CategoryRepository(testContext);
            result = await repository.GetAllCategoriesWithoutTasksAsync("1");
        }
        // Assert
        List<Category> actualResult = result.ToList();
        actualResult.Should().NotBeNullOrEmpty();
        actualResult.Should().HaveCount(2);
        actualResult[0].Tasks.Should().HaveCount(0);
    }

    [Fact]
    public async Task UpdateCategoryAsync_SuccessUpdatesCategory()
    {
        // Arrange
        var options = new DbContextOptionsBuilder<TaskDbContext>()
            .UseInMemoryDatabase(databaseName: "Test_UpdateCategoryAsync_SuccessUpdatesCategory")
            .Options;
        var category = new Category { Id = 1, Name = "Work", UserId = "1", IsDefault = false };
        var category1 = new Category { Id = 2, Name = "Personal", UserId = "1", IsDefault = false };
        var category2 = new Category { Id = 3, Name = "Hobby", UserId = "2", IsDefault = true };
        // Act
        using (var context = new TaskDbContext(options))
        {
            await context.Categories.AddAsync(category);
            await context.Categories.AddAsync(category1);
            await context.Categories.AddAsync(category2);
            await context.SaveChangesAsync();
        }
        
        Category? actualResult;
        using (var testContext = new TaskDbContext(options))
        {
            var repository = new CategoryRepository(testContext);
            var updatedCategory = new Category { Id = 1, Name = "Work", UserId = "1",
                IsDefault = false, Description = "Work Category"};
            await repository.UpdateCategoryAsync(updatedCategory);
            actualResult = testContext.Categories.Where(c => c.Id == 1).FirstOrDefault();
        }
        // Assert
        actualResult.Should().NotBeNull();
        category.Should().BeEquivalentTo(actualResult, opt 
            => opt.Excluding(c => c.Description)
                .Excluding(c => c.CreatedAt));
        actualResult.Description.Should().Be("Work Category");
    }
}