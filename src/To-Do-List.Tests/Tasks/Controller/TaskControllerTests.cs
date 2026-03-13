using System.Net.Http.Headers;
using System.Net.Http.Json;
using FluentAssertions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using To_Do_List.Application.Common.Token;
using To_Do_List.Application.DTOs;
using To_Do_List.Application.Services.Interface;
using To_Do_List.domain.Entities;
using To_Do_List.Tests.Response;
using Xunit.Abstractions;

namespace To_Do_List.Tests.Tasks.Controller;

public class TaskControllerTests : IClassFixture<TestingWebApplicationFactory<Program>>
{
     private readonly HttpClient _client;
     private readonly ITokenHelper _tokenHelper;
     private readonly IConfiguration _configuration;
     private readonly ITestOutputHelper _output;

     public TaskControllerTests(
          TestingWebApplicationFactory<Program> factory,
          ITestOutputHelper output)
     {
          _client = factory.CreateClient();
          _tokenHelper = factory.Services.GetRequiredService<ITokenHelper>();
          _configuration = factory.Services.GetRequiredService<IConfiguration>();
          _output = output;
     }

     [Fact]
     public void ShouldReadUserSecrets()
     {
         var cs = _configuration.GetConnectionString("DefaultConnection");
         cs.Should().NotBeEmpty();
     }

     [Fact]
     public async Task CreateTaskAsync_ShouldReturnSuccess()
     {
         // Arrange: JSON request matching CreateTaskRequest shape
         var testUserId = "1001";
         var entry = new
         {
             UserId = testUserId,
             JWTVersion = 1,
         };
         var token = _tokenHelper.CreateToken(entry, TokenType.AccessToken);
         _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token.TokenStr);
         var uuid = Guid.NewGuid().ToString();
         var request = new CreateTaskRequest("1001_1001_task1", "1001_1001_task1_description",
             DateTime.Today.AddDays(1), 1, "1001");
         // Act: Call your API route (adjust route prefix if needed)
         var response = await _client.PostAsJsonAsync("/Task/CreateTask", request);

         // Assert: success status and response content
         response.EnsureSuccessStatusCode();

         var responseString = await response.Content.ReadAsStringAsync();
         var apiResponse = JsonHelper.Deserialize<Response.ApiResponse<DataWrapper<string>>>(responseString);
         apiResponse.Should().NotBeNull();
         apiResponse.Data.Info.Should().BeEquivalentTo("Task created successfully");
         apiResponse.Data.Code.Should().Be(300000);
     }

     [Fact]
     public async Task CreateTaskAsync_ResourceRoute_ShouldReturnSuccess()
     {
         var testUserId = "1001";
         var entry = new
         {
             UserId = testUserId,
             JWTVersion = 1,
         };
         var token = _tokenHelper.CreateToken(entry, TokenType.AccessToken);
         _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token.TokenStr);
         var request = new CreateTaskRequest(
             $"resource_task_{Guid.NewGuid():N}",
             "resource task description",
             DateTime.Today.AddDays(2),
             500,
             "1001");

         var response = await _client.PostAsJsonAsync("/api/tasks", request);

         response.EnsureSuccessStatusCode();
         var responseString = await response.Content.ReadAsStringAsync();
         var apiResponse = JsonHelper.Deserialize<Response.ApiResponse<DataWrapper<string>>>(responseString);
         apiResponse.Should().NotBeNull();
         apiResponse.Data.Code.Should().Be(300000);
     }
//
     [Fact]
     public async Task GetAllTasks_ShouldReturnSuccess()
     {
         // Arrange: JSON request matching CreateTaskRequest shape
         var testUserId = "1001";
         var entry = new
         {
             UserId = testUserId,
             JWTVersion = 1,
         };
         var token = _tokenHelper.CreateToken(entry, TokenType.AccessToken);
         _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token.TokenStr);
         
         // Act: Call your API route (adjust route prefix if needed)
         var response = await _client.GetAsync("/Task/GetAllTasks");
         
         // Assert: success status and response content
         response.EnsureSuccessStatusCode();
         var jsonString = await response.Content.ReadAsStringAsync();
         var apiResponse = JsonHelper.Deserialize<Response.ApiResponse<DataWrapper<List<CategoryDto>>>>(jsonString);
         apiResponse.Should().NotBeNull();
         apiResponse.Data.Code.Should().Be(300001);
         var categories = apiResponse.Data.Info;
         categories.Should().NotBeNull();
     }

     [Fact]
     public async Task UpdateTaskCompletionAndDelete_ResourceRoutes_ShouldReturnSuccess()
     {
         var testUserId = "1001";
         var entry = new
         {
             UserId = testUserId,
             JWTVersion = 1,
         };
         var token = _tokenHelper.CreateToken(entry, TokenType.AccessToken);
         _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token.TokenStr);

         var title = $"task_{Guid.NewGuid():N}";
         var createRequest = new CreateTaskRequest(title, "integration task", DateTime.Today.AddDays(3), 1000, "1001");
         var createResponse = await _client.PostAsJsonAsync("/api/tasks", createRequest);
         createResponse.EnsureSuccessStatusCode();

         var allResponse = await _client.GetAsync("/api/tasks");
         allResponse.EnsureSuccessStatusCode();
         var allJson = await allResponse.Content.ReadAsStringAsync();
         var allTasks = JsonHelper.Deserialize<Response.ApiResponse<DataWrapper<List<CategoryDto>>>>(allJson);
         var createdTask = allTasks!.Data.Info
             .SelectMany(category => category.Tasks)
             .First(task => task.Title == title);

         var updateResponse = await _client.PutAsJsonAsync($"/api/tasks/{createdTask.Id}", new UpdateTaskRequest(
             $"{title}_updated",
             "updated description",
             DateTime.Today.AddDays(5),
             500,
             "1001",
             false));
         updateResponse.EnsureSuccessStatusCode();

         var completionResponse = await _client.PatchAsJsonAsync($"/api/tasks/{createdTask.Id}/completion",
             new UpdateTaskCompletionRequest(true));
         completionResponse.EnsureSuccessStatusCode();

         var deleteResponse = await _client.DeleteAsync($"/api/tasks/{createdTask.Id}");
         deleteResponse.EnsureSuccessStatusCode();
     }

     [Fact]
     public async Task GetAllCategories_IfUserHaveCategories_ShouldReturnSuccess()
     {
         // Arrage
         var testUserId = "1001";
         var entry = new
         {
             UserId = testUserId,
             JWTVersion = 1,
         };
         var token = _tokenHelper.CreateToken(entry, TokenType.AccessToken);
         _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token.TokenStr);
        
         // Action
         var response = await _client.GetAsync("/Task/GetAllCategories");
         
         // Assert
         response.EnsureSuccessStatusCode();
         var jsonString = await response.Content.ReadAsStringAsync();
         var apiResponse = JsonHelper.Deserialize<Response.ApiResponse<DataWrapper<List<CategoryDto>>>>(jsonString);
         apiResponse.Should().NotBeNull();
         apiResponse.Data.Code.Should().Be(300005);
         var categories = apiResponse.Data.Info;
         categories.Should().NotBeNull();
         categories[0].Tasks.Should().HaveCount(0);
     }

     [Fact]
     public async Task CreateCategoryAsync_ShouldReturnSuccess()
     {
         // Arrage
         var testUserId = "1001";
         var entry = new
         {
             UserId = testUserId,
             JWTVersion = 1,
         };
         var token = _tokenHelper.CreateToken(entry, TokenType.AccessToken);
         _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token.TokenStr);
         string uuid = Guid.NewGuid().ToString();
         var request = new CreateCategoryRequest("1001_test_category1003", "1001_test_category1003_description");
         
         // Action
         var response = await _client.PostAsJsonAsync("/Task/CreateCategory", request);
         
         // Assert
         response.EnsureSuccessStatusCode();
         var jsonString = await response.Content.ReadAsStringAsync();
         var apiResponse = JsonHelper.Deserialize<Response.ApiResponse<DataWrapper<CategoryDto>>>(jsonString);
         apiResponse.Should().NotBeNull();
         apiResponse.Data.Code.Should().Be(300002);
         var categoryDto = apiResponse.Data.Info;
         categoryDto.Should().NotBeNull();
         categoryDto.Id.Should().BePositive();
     }

     [Fact]
     public async Task CreateUpdateDeleteCategory_ResourceRoutes_ShouldReturnSuccess()
     {
         var testUserId = "1001";
         var entry = new
         {
             UserId = testUserId,
             JWTVersion = 1,
         };
         var token = _tokenHelper.CreateToken(entry, TokenType.AccessToken);
         _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token.TokenStr);
         var categoryName = $"category_{Guid.NewGuid():N}";

         var createResponse = await _client.PostAsJsonAsync("/api/categories", new CreateCategoryRequest(categoryName, "resource category"));
         createResponse.EnsureSuccessStatusCode();
         var createJson = await createResponse.Content.ReadAsStringAsync();
         var createApiResponse = JsonHelper.Deserialize<Response.ApiResponse<DataWrapper<CategoryDto>>>(createJson);
         var category = createApiResponse!.Data.Info;

         var updateResponse = await _client.PutAsJsonAsync($"/api/categories/{category.Id}",
             new UpdateCategoryBodyRequest($"{categoryName}_updated", "updated description"));
         updateResponse.EnsureSuccessStatusCode();

         var deleteResponse = await _client.DeleteAsync($"/api/categories/{category.Id}");
         deleteResponse.EnsureSuccessStatusCode();
     }

     [Fact]
     public async Task UpdateCategoryAsync_ShouldReturnSuccess()
     {
         // Arrange
         var testUserId = "1001";
         var entry = new
         {
             UserId = testUserId,
             JWTVersion = 1,
         };
         var token = _tokenHelper.CreateToken(entry, TokenType.AccessToken);
         _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token.TokenStr);
         var uuid = Guid.NewGuid().ToString();
         var request = new UpdateCategoryRequest("1001", "", "1001_test_category1001_description_update", null);
         
         // Action
         var response = await _client.PutAsJsonAsync("/Task/UpdateCategory", request);
         
         // Assert
         response.EnsureSuccessStatusCode();
         var jsonString = await response.Content.ReadAsStringAsync();
         var apiResponse = JsonHelper.Deserialize<Response.ApiResponse<DataWrapper<string>>>(jsonString);
         apiResponse.Should().NotBeNull();
         apiResponse.Data.Code.Should().Be(300003);
         apiResponse.Data.Info.Should().BeEquivalentTo("Category updated successfully");
     }

     [Fact]
     public async Task DeleteCategoryAsync_ShouldReturnSuccess()
     {
         // Arrange
         var testUserId = "1001";
         var entry = new
         {
             UserId = testUserId,
             JWTVersion = 1,
         };
         var token = _tokenHelper.CreateToken(entry, TokenType.AccessToken);
         _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token.TokenStr);
         var request = new DeleteCategoryRequest("1002");
         
         // Action
         var response = await _client.PostAsJsonAsync("/Task/DeleteCategory", request);
         
         // Assert
         response.EnsureSuccessStatusCode();
         var jsonString = await response.Content.ReadAsStringAsync();
         var apiResponse = JsonHelper.Deserialize<Response.ApiResponse<DataWrapper<string>>>(jsonString);
         apiResponse.Should().NotBeNull();
         apiResponse.Data.Code.Should().Be(300004);
         apiResponse.Data.Info.Should().BeEquivalentTo("Category deleted successfully");
         
         // restore delete status
         var resetRequest = new UpdateCategoryRequest("1002", null, null, false);
         await _client.PutAsJsonAsync("/Task/UpdateCategory", resetRequest);
     }
}
