using System.Net.Http.Headers;
using System.Net.Http.Json;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using To_Do_List.Identity.Interface;
using To_Do_List.Identity.Token;
using To_Do_List.Tasks.Controller;

namespace To_Do_List.Tests.Tasks.Controller;

public class TaskControllerTests : IClassFixture<TestingWebApplicationFactory<Program>>
{
    private readonly HttpClient _client;
    private readonly ITokenHelper _tokenHelper;
    private readonly IConfiguration _configuration;

    public TaskControllerTests(TestingWebApplicationFactory<Program> factory)
    {
        _client = factory.CreateClient();
        _tokenHelper = factory.Services.GetRequiredService<ITokenHelper>();
        _configuration = factory.Services.GetRequiredService<IConfiguration>();
    }
    
    [Fact]
    public void ShouldReadUserSecrets()
    {
        var cs = _configuration.GetConnectionString("DefaultConnection");
        Assert.False(string.IsNullOrEmpty(cs), "Connection string is missing!");
    }

    [Fact]
    public async Task CreateTaskAsync_ShouldReturnSuccess()
    {
        // Arrange: JSON request matching CreateTaskRequest shape
        var testUserId = "7";
        var entry = new
        {
            UserId = testUserId,
            JWTVersion = 1,
        };
        var token = _tokenHelper.CreateToken(entry, TokenType.AccessToken);
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token.TokenStr);
        var request = new CreateTaskRequest("Test Task1", "Test Description1", 
            DateTime.Today.AddDays(1), 1, "3");
        // Act: Call your API route (adjust route prefix if needed)
        var response = await _client.PostAsJsonAsync("/Task/CreateTask", request);

        // Assert: success status and response content
        response.EnsureSuccessStatusCode();

        var responseString = await response.Content.ReadAsStringAsync();
        Assert.Contains("Task created successfully", responseString);
    }
}