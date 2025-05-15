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
}