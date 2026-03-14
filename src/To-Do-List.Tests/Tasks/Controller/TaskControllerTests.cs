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
    
}
