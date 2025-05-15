using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using To_Do_List.Tasks.DbContext;
using To_Do_List.Tasks.Entities;
using To_Do_List.Tasks.Implement;

namespace To_Do_List.Tests.Tasks.Implement;

public class TaskRepositoryTests
{
    [Fact]
    public async Task AddTaskAsync_ShouldAddAndSave_WhenCalled()
    {
        // Arrange
        var options = new DbContextOptionsBuilder<TaskDbContext>()
            .UseInMemoryDatabase(databaseName: "Test_AddTaskAsync")
            .Options;

        // Act
        using (var context = new TaskDbContext(options))
        {
            var task = new TaskItem{Title = "Test Task"};
            var taskRepository = new TaskRepository(context);
            await taskRepository.AddTaskAsync(task);
        }

        // Assert
        using (var context = new TaskDbContext(options))
        {
            context.Tasks.Count().Should().Be(1);
        }
    }
    
    [Fact]
    public async Task GetTasksByCategoryIdAsync_ShouldReturnTasksWithGivenCategoryId()
    {
        // Arrange: 创建InMemory数据库选项
        var options = new DbContextOptionsBuilder<TaskDbContext>()
            .UseInMemoryDatabase(databaseName: "Test_GetTasksByCategoryId")
            .Options;

        // 用using确保上下文Dispose
        using (var context = new TaskDbContext(options))
        {
            // 先创建Category对象
            var category = new Category { Id = 1, Name = "TestCategory", UserId = "1"};

            // 创建几个TaskItem，部分属于category.Id = 1
            var tasks = new List<TaskItem>
            {
                new TaskItem { Id = 1, Title = "Task 1", Category = category },
                new TaskItem { Id = 2, Title = "Task 2", Category = category },
                new TaskItem { Id = 3, Title = "Task 3", Category = new Category { Id = 2, Name = "OtherCategory", UserId = "1" } }
            };

            // 添加数据
            await context.Categories.AddAsync(category);
            await context.Tasks.AddRangeAsync(tasks);
            await context.SaveChangesAsync();
        }

        // Act & Assert: 新建上下文，调用Repository方法
        using (var context = new TaskDbContext(options))
        {
            var repo = new TaskRepository(context);
            var result = await repo.GetTasksByCategoryIdAsync("1");
            var resultList = result.ToList();

            resultList.Should().NotBeNull();
            resultList.Should().HaveCount(2);
            resultList.Should().Contain(t => t.Category == null!);
        }
    }
}