using Moq;
using To_Do_List.Require;
using To_Do_List.Tasks.Entities;
using To_Do_List.Tasks.Interface;
using To_Do_List.Tasks.Service;

namespace To_Do_List.Tests.Tasks;

public class TaskCategoryServiceTests
{
    private readonly TaskCategoryService _service;
    private readonly Mock<ICategoryRepository> _mockCategoryRepository;
    private readonly Mock<ITaskRepository> _mockTaskRepository;

    public TaskCategoryServiceTests()
    {
        _mockCategoryRepository = new Mock<ICategoryRepository>();
        _mockTaskRepository = new Mock<ITaskRepository>();
        _service = new TaskCategoryService(_mockCategoryRepository.Object, _mockTaskRepository.Object);
    }

    [Fact]
    public async Task CreateCategoryAsync_ReturenSuccess_WhenValidInput()
    {
        var name = "Dev";
        var description = "Dev";
        var userId = "1";

        _mockCategoryRepository
            .Setup(r => r.AddCategoryAsync(It.IsAny<Category>()))
            .Returns(Task.FromResult(ApiResponseCode.CategoryCreateSuccess));
        
        var result = await _service.CreateCategoryAsync(name, description, userId);
        Assert.Equal(ApiResponseCode.CategoryCreateSuccess, result);
        _mockCategoryRepository.Verify(r => r.AddCategoryAsync(It.IsAny<Category>()), Times.Once);
    }

    [Fact]
    public async Task CreateCategoryAsync_ReturenServerError_WhenRepositoryThrowsException()
    {
        var name = "Dev";
        var description = "Dev";
        var userId = "1";
        _mockCategoryRepository
            .Setup(r => r.AddCategoryAsync(It.IsAny<Category>()))
            .Returns(Task.FromResult(ApiResponseCode.CategoryCreateFailed));
        
        var result = await _service.CreateCategoryAsync(name, description, userId);
        Assert.Equal(ApiResponseCode.CategoryCreateFailed, result);
        _mockCategoryRepository.Verify(r => r.AddCategoryAsync(It.IsAny<Category>()), Times.Once);
    }
}