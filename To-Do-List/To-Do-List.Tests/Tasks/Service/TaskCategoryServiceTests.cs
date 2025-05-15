using AutoFixture.Xunit2;
using FluentAssertions;
using Moq;
using To_Do_List.Require;
using To_Do_List.Tasks.Entities;
using To_Do_List.Tasks.Interface;
using To_Do_List.Tasks.Service;

namespace To_Do_List.Tests.Tasks.Service;

public class TaskCategoryServiceTests
{

    [Theory, AutoMoqData]
    public async Task CreateTaskAsync_ShouldReturnSuccess_WhenCategoryExists(
        [Frozen] Mock<ICategoryRepository> mockCategoryRepository,
        [Frozen] Mock<ITaskRepository> mockTaskRepository,
        TaskCategoryService sut,
        string title,
        string description,
        DateTime dueDate,
        string userId,
        string categoryId,
        int priority,
        Category category)
    {
        // Arrange
        mockCategoryRepository
            .Setup(repo => repo.GetCategoryByIdAsync(categoryId, userId))
            .ReturnsAsync(category);
        
        // Act
        var actualResult = await sut
            .CreateTaskAsync(title, description, dueDate, priority, userId, categoryId);
        
        // Assert
        actualResult.Should().Be(ApiResponseCode.TaskCreateSuccess);
        mockTaskRepository.Verify(r => r.AddTaskAsync(It.IsAny<TaskItem>()), Times.Once);
    }

    [Theory, AutoMoqData]
    public async Task CreateTaskAsync_ShouldReturnFail_WhenCategoryNotExist(
        [Frozen] Mock<ICategoryRepository> mockCategoryRepository,
        [Frozen] Mock<ITaskRepository> mockTaskRepository,
        TaskCategoryService sut,
        string title,
        string description,
        DateTime dueDate,
        string userId,
        string categoryId,
        int priority
        )
    {
        // Arrange
        mockCategoryRepository
            .Setup(rep => rep.GetCategoryByIdAsync(categoryId, userId))
            .ReturnsAsync(null as Category);
        
        // Act
        var actualResult = await sut.CreateTaskAsync(title, description, dueDate, priority, userId, categoryId);
        
        // Assert
        actualResult.Should().Be(ApiResponseCode.CategoryIdNotFoundForCurrentUser);
        mockTaskRepository.Verify(r => r.AddTaskAsync(It.IsAny<TaskItem>()), Times.Never);
    }

    [Theory, AutoMoqData]
    public async Task CreateTaskAsync_ShouldReturnFail_WhenAddTaskThrowException(
        [Frozen] Mock<ICategoryRepository> mockCategoryRepository,
        [Frozen] Mock<ITaskRepository> mockTaskRepository,
        TaskCategoryService sut,
        string title,
        string description,
        DateTime dueDate,
        string userId,
        string categoryId,
        int priority,
        Category category)
    {
        // Arrange
        mockCategoryRepository
            .Setup(rep => rep.GetCategoryByIdAsync(categoryId, userId))
            .ReturnsAsync(category);
        mockTaskRepository.Setup(rep => rep.AddTaskAsync(It.IsAny<TaskItem>()))
            .Throws(new Exception("Add task exception"));
        
        // Act
        var actualResult = await sut.CreateTaskAsync(title, description, dueDate, priority, userId, categoryId);
        
        // Assert
        actualResult.Should().Be(ApiResponseCode.TaskAddExceptionInDB);
        mockTaskRepository.Verify(r => r.AddTaskAsync(It.IsAny<TaskItem>()), Times.Once);
    }

    [Theory, AutoMoqData]
    public async Task GetAllCategoryWithTasksAsync_ShouldReturnAllTask_WhenUserHaveTask(
        [Frozen] Mock<ICategoryRepository> mockCategoryRepository,
        TaskCategoryService sut,
        string userId,
        IEnumerable<Category> items)
    {
        IEnumerable<Category> itemsList = items.ToList();
        // Arrange
        mockCategoryRepository
            .Setup(rep 
                => rep.GetAllCategoriesWithTasksAsync(userId))
            .ReturnsAsync(itemsList);
        
        // Act
        var actualResult = await sut.GetAllCategoryWithTasksAsync(userId);
        actualResult.Should().BeEquivalentTo(itemsList);
        mockCategoryRepository.Verify(rep => rep.GetAllCategoriesWithTasksAsync(userId), Times.Once);
    }

    [Theory, AutoMoqData]
    public async Task GetAllCategoryWithTasksAsync_ShouldReturnNullTask_WhenUserHaveTask(
        [Frozen] Mock<ICategoryRepository> mockCategoryRepository,
        TaskCategoryService sut,
        string userId)
    {
        // Arrange
        mockCategoryRepository
            .Setup(rep => rep.GetAllCategoriesWithTasksAsync(userId))
            .ReturnsAsync(new List<Category>());
        
        // Act
        var actualResult = await sut.GetAllCategoryWithTasksAsync(userId);
        
        // Assert
        actualResult.Should().BeEmpty();
        mockCategoryRepository.Verify(rep => rep.GetAllCategoriesWithTasksAsync(userId), Times.Once);
    }

    [Theory, AutoMoqData]
    public async Task CreateDefaultCategoryAsync_ShouldReturnSuccess_WhenAddSuccess(
        [Frozen] Mock<ICategoryRepository> mockCategoryRepository,
        TaskCategoryService sut,
        long userId
        )
    {
        // Arrange
        Category category = new Category()
        {
            Name = "Default",
            Description = "Default category.",
            UserId = userId.ToString(),
            IsDefault = true
        };
        mockCategoryRepository.Setup(rep =>
                rep.AddCategoryAsync(It.Is<Category>(c =>
                    c.Name == category.Name
                    && c.Description == category.Description
                    && c.IsDefault
                    && c.UserId == userId.ToString())))
            .ReturnsAsync(ApiResponseCode.CategoryCreateSuccess);
        
        // Act
        var actualResult = await sut.CreateDefaultCategoryAsync(userId);
        
        // Assert
        actualResult.Should().Be(ApiResponseCode.DefaultCategoryCreateSuccess);
        mockCategoryRepository.Verify(rep => rep.AddCategoryAsync(It.IsAny<Category>()), Times.Once);
    }

    [Theory, AutoMoqData]
    public async Task CreateDefaultCategoryAsync_ShouldReturnFail_WhenAddFail(
        [Frozen] Mock<ICategoryRepository> mockCategoryRepository,
        TaskCategoryService sut,
        long userId)
    {
        // Arrange
        Category category = new Category()
        {
            Name = "Default",
            Description = "Default category.",
            UserId = userId.ToString(),
            IsDefault = true
        };
        mockCategoryRepository.Setup(rep =>
                rep.AddCategoryAsync(It.Is<Category>(c =>
                    c.Name == category.Name
                    && c.Description == category.Description
                    && c.IsDefault
                    && c.UserId == userId.ToString())))
            .ReturnsAsync(ApiResponseCode.CategoryCreateFailed);
        
        // Act
        var actualResult = await sut.CreateDefaultCategoryAsync(userId);
        
        // Assert
        actualResult.Should().Be(ApiResponseCode.DefaultCategoryCreateFailed);
        mockCategoryRepository.Verify(rep => rep.AddCategoryAsync(It.IsAny<Category>()), Times.Once);
    }

    [Theory, AutoMoqData]
    public async Task CreateDefaultCategoryAsync_ShouldReturnFail_WhenAddException(
        [Frozen] Mock<ICategoryRepository> mockCategoryRepository,
        TaskCategoryService sut,
        long userId)
    {
        // Arrange
        Category category = new Category()
        {
            Name = "Default",
            Description = "Default category.",
            UserId = userId.ToString(),
            IsDefault = true
        };
        mockCategoryRepository.Setup(rep =>
                rep.AddCategoryAsync(It.Is<Category>(c =>
                    c.Name == category.Name
                    && c.Description == category.Description
                    && c.IsDefault
                    && c.UserId == userId.ToString())))
            .Throws<Exception>();
        
        // Act
        var actualResult = await sut.CreateDefaultCategoryAsync(userId);
        
        // Assert
        actualResult.Should().Be(ApiResponseCode.DefaultCategoryCreateFailed);
        mockCategoryRepository.Verify(rep => rep.AddCategoryAsync(It.IsAny<Category>()), Times.Once);
    }

    [Theory, AutoMoqData]
    public async Task CreateCategoryAsync_ShouldReturnSuccess_WhenAddCategorySuccess(
        [Frozen] Mock<ICategoryRepository> mockCategoryRepository,
        TaskCategoryService sut,
        string name,
        string description,
        string userId,
        long categoryId)
    {
        // Arrange
        Category category = new Category
        {
            Name = name,
            Description = description,
            UserId = userId,
            IsDeleted = false
        };
        mockCategoryRepository.Setup(rep 
            => rep.AddCategoryAsync(It.Is<Category>(c =>
                c.Name == name 
                && c.Description == description 
                && c.UserId == userId)))
            .Callback<Category>(c => c.Id = categoryId)
            .ReturnsAsync(ApiResponseCode.CategoryCreateSuccess);

        
        // Act
        var (actualCode, actualCategory) = await sut.CreateCategoryAsync(name, description, userId);
        
        // Assert
        actualCode.Should().Be(ApiResponseCode.CategoryCreateSuccess);
        actualCategory!.Id.Should().Be(categoryId);
        actualCategory.Should().BeEquivalentTo(category,
            opt => opt.Excluding(c => c.Id).Excluding(c => c.CreatedAt));
        mockCategoryRepository.Verify(rep => rep.AddCategoryAsync(It.IsAny<Category>()), Times.Once);
    }
    
    [Theory, AutoMoqData]
    public async Task CreateCategoryAsync_ShouldReturnFailed_WhenAddCategoryFailed(
        [Frozen] Mock<ICategoryRepository> mockCategoryRepository,
        TaskCategoryService sut,
        string name,
        string description,
        string userId)
    {
        // Arrange
        mockCategoryRepository.Setup(rep 
                => rep.AddCategoryAsync(It.Is<Category>(c =>
                    c.Name == name 
                    && c.Description == description 
                    && c.UserId == userId)))
            .ReturnsAsync(ApiResponseCode.CategoryCreateFailed);

        
        // Act
        var (actualCode, actualCategory) = await sut.CreateCategoryAsync(name, description, userId);
        
        // Assert
        actualCode.Should().Be(ApiResponseCode.CategoryCreateFailed);
        actualCategory.Should().BeNull();
        mockCategoryRepository.Verify(rep => rep.AddCategoryAsync(It.IsAny<Category>()), Times.Once);
    }
    
    [Theory, AutoMoqData]
    public async Task CreateCategoryAsync_ShouldReturnFailed_WhenAddCategoryThrowException(
        [Frozen] Mock<ICategoryRepository> mockCategoryRepository,
        TaskCategoryService sut,
        string name,
        string description,
        string userId)
    {
        // Arrange
        mockCategoryRepository.Setup(rep
                => rep.AddCategoryAsync(It.Is<Category>(c =>
                    c.Name == name
                    && c.Description == description
                    && c.UserId == userId)))
            .Throws<Exception>();

        
        // Act
        var (actualCode, actualCategory) = await sut.CreateCategoryAsync(name, description, userId);
        
        // Assert
        actualCode.Should().Be(ApiResponseCode.CategoryCreateFailed);
        actualCategory.Should().BeNull();
        mockCategoryRepository.Verify(rep => rep.AddCategoryAsync(It.IsAny<Category>()), Times.Once);
    }

    [Theory, AutoMoqData]
    public async Task UpdateCategoryAsync_ShouldReturnSuccess_WhenUpdateSuccessAndAllValueAreNotNull(
        [Frozen] Mock<ICategoryRepository> mockCategoryRepository,
        TaskCategoryService sut,
        string categoryId,
        string name,
        string description,
        string userId,
        Category category
        )
    {
        // Arrange
        mockCategoryRepository
            .Setup(rep => rep.GetCategoryByIdAsync(categoryId, userId))
            .ReturnsAsync(category);
        category.Name = name;
        category.Description = description;
        mockCategoryRepository
            .Setup(rep => rep.UpdateCategoryAsync(category))
            .Returns(Task.CompletedTask);
        
        // Act
        var actualResult = await sut
            .UpdateCategoryAsync(categoryId, name, description, userId);
        
        // Assert
        actualResult.Should().Be(ApiResponseCode.CategoryUpdateSuccess);
        mockCategoryRepository.Verify(rep => rep.GetCategoryByIdAsync(categoryId, userId), Times.Once);
        mockCategoryRepository.Verify(rep => rep.UpdateCategoryAsync(It.IsAny<Category>()), Times.Once);
        category.Name.Should().Be(name);
        category.Description.Should().Be(description);
    }
    
    [Theory, AutoMoqData]
    public async Task UpdateCategoryAsync_ShouldReturnSuccess_WhenUpdateSuccessAndNameIsNull(
        [Frozen] Mock<ICategoryRepository> mockCategoryRepository,
        TaskCategoryService sut,
        string categoryId,
        string description,
        string userId,
        Category category
    )
    {
        var originalName = category.Name;

        // Arrange
        mockCategoryRepository
            .Setup(rep => rep.GetCategoryByIdAsync(categoryId, userId))
            .ReturnsAsync(category);
        category.Description = description;
        mockCategoryRepository
            .Setup(rep => rep.UpdateCategoryAsync(category))
            .Returns(Task.CompletedTask);
        
        // Act
        var actualResult = await sut
            .UpdateCategoryAsync(categoryId, null, description, userId);
        
        // Assert
        actualResult.Should().Be(ApiResponseCode.CategoryUpdateSuccess);
        mockCategoryRepository.Verify(rep => rep.GetCategoryByIdAsync(categoryId, userId), Times.Once);
        mockCategoryRepository.Verify(rep => rep.UpdateCategoryAsync(It.IsAny<Category>()), Times.Once);
        category.Name.Should().Be(originalName);
        category.Description.Should().Be(description);
    }
    
    [Theory, AutoMoqData]
    public async Task UpdateCategoryAsync_ShouldReturnSuccess_WhenUpdateSuccessAndDescriptionIsNull(
        [Frozen] Mock<ICategoryRepository> mockCategoryRepository,
        TaskCategoryService sut,
        string categoryId,
        string name,
        string userId,
        Category category
    )
    {
        var originalDes = category.Description;

        // Arrange
        mockCategoryRepository
            .Setup(rep => rep.GetCategoryByIdAsync(categoryId, userId))
            .ReturnsAsync(category);
        category.Name = name;
        mockCategoryRepository
            .Setup(rep => rep.UpdateCategoryAsync(category))
            .Returns(Task.CompletedTask);
        
        // Act
        var actualResult = await sut
            .UpdateCategoryAsync(categoryId, name, null, userId);
        
        // Assert
        actualResult.Should().Be(ApiResponseCode.CategoryUpdateSuccess);
        mockCategoryRepository.Verify(rep => rep.GetCategoryByIdAsync(categoryId, userId), Times.Once);
        mockCategoryRepository.Verify(rep => rep.UpdateCategoryAsync(It.IsAny<Category>()), Times.Once);
        category.Name.Should().Be(name);
        category.Description.Should().Be(originalDes);
    }
    
    [Theory, AutoMoqData]
    public async Task UpdateCategoryAsync_ShouldReturnFailed_WhenCategoryIsNull(
        [Frozen] Mock<ICategoryRepository> mockCategoryRepository,
        TaskCategoryService sut,
        string categoryId,
        string name,
        string description,
        string userId
    )
    {
        // Arrange
        mockCategoryRepository
            .Setup(rep => rep.GetCategoryByIdAsync(categoryId, userId))
            .ReturnsAsync(null as Category);
        
        // Act
        var actualResult = await sut
            .UpdateCategoryAsync(categoryId, name, description, userId);
        
        // Assert
        actualResult.Should().Be(ApiResponseCode.CategoryIdNotFoundForCurrentUser);
        mockCategoryRepository.Verify(rep => rep.GetCategoryByIdAsync(categoryId, userId), Times.Once);
        mockCategoryRepository.Verify(rep => rep.UpdateCategoryAsync(It.IsAny<Category>()), Times.Never);
    }
    
    [Theory, AutoMoqData]
    public async Task UpdateCategoryAsync_ShouldReturnFailed_WhenUpdateExceptionIsThrown(
        [Frozen] Mock<ICategoryRepository> mockCategoryRepository,
        TaskCategoryService sut,
        string categoryId,
        string name,
        string userId,
        Category category
    )
    {
        // Arrange
        mockCategoryRepository
            .Setup(rep => rep.GetCategoryByIdAsync(categoryId, userId))
            .ReturnsAsync(category);
        category.Name = name;
        mockCategoryRepository
            .Setup(rep => rep.UpdateCategoryAsync(category))
            .Throws<Exception>();
        
        // Act
        var actualResult = await sut
            .UpdateCategoryAsync(categoryId, name, null, userId);
        
        // Assert
        actualResult.Should().Be(ApiResponseCode.CategoryUpdateFailedInDB);
        mockCategoryRepository.Verify(rep => rep.GetCategoryByIdAsync(categoryId, userId), Times.Once);
        mockCategoryRepository.Verify(rep => rep.UpdateCategoryAsync(It.IsAny<Category>()), Times.Once);
    }
    
    [Theory, AutoMoqData]
    public async Task DeleteCategoryById_ShouldReturnNotFound_WhenCategoryDoesNotExist(
        [Frozen] Mock<ICategoryRepository> mockCategoryRepository,
        TaskCategoryService sut,
        string categoryId,
        string userId)
    {
        // Arrange
        mockCategoryRepository.Setup(r => r.GetCategoryByIdAsync(categoryId, userId))
            .ReturnsAsync((Category?)null);

        // Act
        var result = await sut.DeleteCategoryById(categoryId, userId);

        // Assert
        result.Should().Be(ApiResponseCode.CategoryIdNotFoundForCurrentUser);
    }
    
    [Theory, AutoMoqData]
    public async Task DeleteCategoryById_ShouldReturnError_WhenDefaultCategoryIsMissing(
        [Frozen] Mock<ICategoryRepository> mockCategoryRepository,
        long categoryId,
        string userId,
        Category existingCategory,
        TaskCategoryService sut)
    {
        // Arrange
        existingCategory.Id = categoryId;
        existingCategory.UserId = userId;

        mockCategoryRepository.Setup(r => r.GetCategoryByIdAsync(categoryId.ToString(), userId))
            .ReturnsAsync(existingCategory);
        mockCategoryRepository.Setup(r => r.GetDefaultCategoryAsync(userId))
            .ReturnsAsync((Category?)null);

        // Act
        var result = await sut.DeleteCategoryById(categoryId.ToString(), userId);

        // Assert
        result.Should().Be(ApiResponseCode.CategoryDefaultIsMissing);
    }

    [Theory, AutoMoqData]
    public async Task DeleteCategoryById_ShouldReturnSuccess_WhenCategoryDeleted(
        [Frozen] Mock<ICategoryRepository> mockCategoryRepository,
        [Frozen] Mock<ITaskRepository> mockTaskRepository,
        long categoryId,
        string userId,
        Category categoryToDelete,
        Category defaultCategory,
        List<TaskItem> tasks,
        TaskCategoryService sut)
    {
        // Arrange
        categoryToDelete.Id = categoryId;
        categoryToDelete.UserId = userId;
        defaultCategory.IsDefault = true;
        defaultCategory.UserId = userId;

        mockCategoryRepository.Setup(r => r.GetCategoryByIdAsync(categoryId.ToString(), userId))
            .ReturnsAsync(categoryToDelete);
        mockCategoryRepository.Setup(r => r.GetDefaultCategoryAsync(userId))
            .ReturnsAsync(defaultCategory);
        mockTaskRepository.Setup(r => r.GetTasksByCategoryIdAsync(categoryId.ToString()))
            .ReturnsAsync(tasks);

        // Act
        var result = await sut.DeleteCategoryById(categoryId.ToString(), userId);

        // Assert
        result.Should().Be(ApiResponseCode.DeleteCategorySuccess);
        categoryToDelete.IsDeleted.Should().BeTrue();

        foreach (var task in tasks)
        {
            task.Category.Should().Be(defaultCategory);
        }

        mockCategoryRepository.Verify(r => r.UpdateCategoryAsync(categoryToDelete), Times.Once);
    }
    [Theory, AutoMoqData]
    public async Task DeleteCategoryById_ShouldSucceed_WhenNoTasksInCategory(
        [Frozen] Mock<ICategoryRepository> mockCategoryRepository,
        [Frozen] Mock<ITaskRepository> mockTaskRepository,
        long categoryId,
        string userId,
        Category categoryToDelete,
        Category defaultCategory,
        TaskCategoryService sut)
    {
        // Arrange
        categoryToDelete.Id = categoryId;
        categoryToDelete.UserId = userId;
        defaultCategory.IsDefault = true;
        defaultCategory.UserId = userId;

        mockCategoryRepository.Setup(r => r.GetCategoryByIdAsync(categoryId.ToString(), userId))
            .ReturnsAsync(categoryToDelete);
        mockCategoryRepository.Setup(r => r.GetDefaultCategoryAsync(userId))
            .ReturnsAsync(defaultCategory);
        mockTaskRepository.Setup(r => r.GetTasksByCategoryIdAsync(categoryId.ToString()))
            .ReturnsAsync(new List<TaskItem>());

        // Act
        var result = await sut.DeleteCategoryById(categoryId.ToString(), userId);

        // Assert
        result.Should().Be(ApiResponseCode.DeleteCategorySuccess);
        mockCategoryRepository.Verify(r => r.UpdateCategoryAsync(categoryToDelete), Times.Once);
    }
    
    [Theory, AutoMoqData]
    public async Task DeleteCategoryById_ShouldMarkCategoryAsDeleted(
        [Frozen] Mock<ICategoryRepository> mockCategoryRepository,
        [Frozen] Mock<ITaskRepository> mockTaskRepository,
        long categoryId,
        string userId,
        Category categoryToDelete,
        Category defaultCategory,
        TaskCategoryService sut)
    {
        // Arrange
        categoryToDelete.Id = categoryId;
        categoryToDelete.UserId = userId;

        mockCategoryRepository.Setup(r => r.GetCategoryByIdAsync(categoryId.ToString(), userId))
            .ReturnsAsync(categoryToDelete);
        mockCategoryRepository.Setup(r => r.GetDefaultCategoryAsync(userId))
            .ReturnsAsync(defaultCategory);
        mockTaskRepository.Setup(r => r.GetTasksByCategoryIdAsync(categoryId.ToString()))
            .ReturnsAsync(new List<TaskItem>());

        // Act
        await sut.DeleteCategoryById(categoryId.ToString(), userId);

        // Assert
        categoryToDelete.IsDeleted.Should().BeTrue();
    }


}