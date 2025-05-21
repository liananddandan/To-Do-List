using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;
using FluentAssertions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using To_Do_List.Identity.Interface;
using To_Do_List.Identity.Token;
using To_Do_List.Tasks.Controller;
using To_Do_List.Tasks.Entities;
using To_Do_List.Tests.Response;
using Xunit.Abstractions;

namespace To_Do_List.Tests.Tasks.Controller;

public class TaskControllerTests : IClassFixture<TestingWebApplicationFactory<Program>>
{
    private readonly HttpClient _client;
    private readonly ITokenHelper _tokenHelper;
    private readonly IConfiguration _configuration;
    private readonly ITestOutputHelper _output;

    public TaskControllerTests(TestingWebApplicationFactory<Program> factory, ITestOutputHelper output)
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
        var testUserId = "7";
        var entry = new
        {
            UserId = testUserId,
            JWTVersion = 1,
        };
        var token = _tokenHelper.CreateToken(entry, TokenType.AccessToken);
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token.TokenStr);
        string UUID = Guid.NewGuid().ToString();
        var request = new CreateTaskRequest("Test Task" + UUID, "Test Description" + UUID,
            DateTime.Today.AddDays(1), 1, "3");
        // Act: Call your API route (adjust route prefix if needed)
        var response = await _client.PostAsJsonAsync("/Task/CreateTask", request);

        // Assert: success status and response content
        response.EnsureSuccessStatusCode();

        var responseString = await response.Content.ReadAsStringAsync();
        var apiResponse = JsonHelper.Deserialize<ApiResponse<DataWrapper<string>>>(responseString);
        apiResponse.Should().NotBeNull();
        apiResponse.Data.Info.Should().BeEquivalentTo("Task created successfully");
        apiResponse.Data.Code.Should().Be(300000);
    }

    [Fact]
    public async Task GetAllTasks_ShouldReturnSuccess()
    {
        var testUserId = "7";
        var entry = new
        {
            UserId = testUserId,
            JWTVersion = 1,
        };
        var token = _tokenHelper.CreateToken(entry, TokenType.AccessToken);
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token.TokenStr);
        var response = await _client.GetAsync("/Task/GetAllTasks");
        var jsonString = await response.Content.ReadAsStringAsync();
        var apiResponse = JsonHelper.Deserialize<ApiResponse<DataWrapper<List<CategoryDto>>>>(jsonString);
        apiResponse.Should().NotBeNull();
        apiResponse.Data.Code.Should().Be(300001);
        var categories = apiResponse.Data.Info;
        categories.Should().NotBeNull();
    }

    [Fact]
    public async Task GetAllCategories_IfUserHaveCategories_ShouldReturnSuccess()
    {
        var testUserId = "7";
        var entry = new
        {
            UserId = testUserId,
            JWTVersion = 1,
        };
        var token = _tokenHelper.CreateToken(entry, TokenType.AccessToken);
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token.TokenStr);
        var response = await _client.GetAsync("/Task/GetAllCategories");
        var jsonString = await response.Content.ReadAsStringAsync();
        var apiResponse = JsonHelper.Deserialize<ApiResponse<DataWrapper<List<CategoryDto>>>>(jsonString);
        apiResponse.Should().NotBeNull();
        apiResponse.Data.Code.Should().Be(300005);
        var categories = apiResponse.Data.Info;
        categories.Should().NotBeNull();
        categories[0].Tasks.Should().HaveCount(0);
    }

    [Fact]
    public async Task CreateCategoryAsync_ShouldReturnSuccess()
    {
        var testUserId = "7";
        var entry = new
        {
            UserId = testUserId,
            JWTVersion = 1,
        };
        var token = _tokenHelper.CreateToken(entry, TokenType.AccessToken);
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token.TokenStr);
        string UUID = Guid.NewGuid().ToString();
        var request = new CreateCategoryRequest("Test" + UUID, "Test Description" + UUID);
        var response = await _client.PostAsJsonAsync("/Task/CreateCategory", request);
        var jsonString = await response.Content.ReadAsStringAsync();
        var apiResponse = JsonHelper.Deserialize<ApiResponse<DataWrapper<CategoryDto>>>(jsonString);
        apiResponse.Should().NotBeNull();
        apiResponse.Data.Code.Should().Be(300002);
        var categoryDto = apiResponse.Data.Info;
        categoryDto.Should().NotBeNull();
        categoryDto.Id.Should().BePositive();
    }
}