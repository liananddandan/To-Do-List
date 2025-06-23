# Clean Architecture .NET Web API
## 1. **Introduction**
   * Project Title: Clean Architecture .NET Web API
   * An open-source Clean Architecture Web API template for .NET developers, featuring structured layers, test coverage, and modern API tooling.

## 2. **Tech Stack**

### Frameworks & Runtimes
- .NET 9 [![.NET 9](https://img.shields.io/badge/.NET-9.0-informational?logo=dotnet)](https://dotnet.microsoft.com/en-us/download/dotnet/9.0)(ASP.NET Core Web API)
- Entity Framework Core [![Microsoft.EntityFrameworkCore](https://img.shields.io/nuget/v/Microsoft.EntityFrameworkCore.svg)](https://www.nuget.org/packages/Microsoft.EntityFrameworkCore)(MySQL)

### Authentication & Validation
- JWT [![Microsoft.AspNetCore.Authentication.JwtBearer](https://img.shields.io/nuget/v/Microsoft.AspNetCore.Authentication.JwtBearer.svg)](https://www.nuget.org/packages/Microsoft.AspNetCore.Authentication.JwtBearer)(JSON Web Tokens)
- ASP.NET Core Identity [![Microsoft.AspNetCore.Identity](https://img.shields.io/nuget/v/Microsoft.AspNetCore.Identity.svg)](https://www.nuget.org/packages/Microsoft.AspNetCore.Identity)
- FluentValidation [![FluentValidation](https://img.shields.io/nuget/v/FluentValidation.svg)](https://www.nuget.org/packages/FluentValidation)(with custom validation filters)

### Testing
- xUnit [![xunit](https://img.shields.io/nuget/v/xunit.svg)](https://www.nuget.org/packages/xunit)
- Moq [![Moq](https://img.shields.io/nuget/v/Moq.svg)](https://www.nuget.org/packages/Moq)
- AutoFixture [![AutoFixture](https://img.shields.io/nuget/v/AutoFixture.svg)](https://www.nuget.org/packages/AutoFixture)
- FluentAssertions [![FluentAssertions](https://img.shields.io/nuget/v/FluentAssertions.svg)](https://www.nuget.org/packages/FluentAssertions)
- WireMock.Net [![WireMock.Net](https://img.shields.io/nuget/v/WireMock.Net.svg)](https://www.nuget.org/packages/WireMock.Net)
(mocking external HTTP APIs)
- In-memory database [![EFCore.InMemory](https://img.shields.io/nuget/v/Microsoft.EntityFrameworkCore.InMemory.svg)](https://www.nuget.org/packages/Microsoft.EntityFrameworkCore.InMemory)(for integration testing)
- MVC.Testing [![Mvc.Testing](https://img.shields.io/nuget/v/Microsoft.AspNetCore.Mvc.Testing.svg)](https://www.nuget.org/packages/Microsoft.AspNetCore.Mvc.Testing)(controller-level tests)

### Tooling & DevOps
- GitHub Actions [![CI](https://github.com/liananddandan/To-Do-List/actions/workflows/dotnet-mysql.yml/badge.svg)](https://github.com/liananddandan/To-Do-List/actions)(CI pipeline)
- Scalar [![Scalar](https://img.shields.io/badge/API%20Docs-Scalar-brightgreen)](https://your-scalar-link.com)(OpenAPI-based API documentation preview)
- dotnet CLI ![dotnet CLI](https://img.shields.io/badge/dotnet--cli-used-informational?logo=dotnet)
- JetBrains Rider ![Rider](https://img.shields.io/badge/IDE-Rider-orange?logo=jetbrains)/ Visual Studio Code ![VSCode](https://img.shields.io/badge/IDE-VSCode-blue?logo=visualstudiocode)


## 3. **Project Architecture**
This project follows modular and layered architecture principles, inspired by Clean Architecture. The overall structure is organized to separate concerns across different folders, and supports scalability and testing.

**Top-Level Structure**
```

To-Do-List/
├── To-Do-List/              Main Web API project
├── To-Do-List.Tests/        Unit and integration tests
├── RelatedTechnologies/     Exploratory code (e.g., dynamic proxy, AOP)
├── WebApplication1/         Middleware demos and Swagger configuration
├── To-Do-List.sln           Solution file

```

- `To-Do-List/` contains the primary application logic including controllers, filters, identity handling, and database configuration.
- `RelatedTechnologies/` includes experiments and prototype features explored during learning.
- `WebApplication1/` demonstrates middleware configuration, filter pipelines, and Swagger versioning strategies.

**Main Project Structure (`To-Do-List/`)**

```

To-Do-List/
├── Attribute/               Custom attributes (e.g., JWT version validation)
├── Configuration/           Strongly-typed configuration binding
├── Controller/              RESTful API controllers
├── Filter/                  Global filters (authorization, exception handling, validation)
├── Identity/                Token generation, claim validation, authentication logic
├── Require/                 Request constraints and interface contracts
├── Tasks/                   Task and category management logic
├── Program.cs               Application bootstrap and service registration
├── To-Do-List.http          HTTP request samples for testing

```

- **Attribute** includes reusable decorators for request validation or custom policies.
- **Filter** defines authorization filters, exception handling, and FluentValidation integration.
- **Identity** handles JWT-based authentication and security context.
- **Tasks** manages the core business logic around task and category entities.
- **Configuration** centralizes app settings, loaded via custom binding classes.

## 4. **Features**
   A checklist of completed features, for example:

   * [x] Layered clean architecture
   * [x] EF Core with MySQL
   * [x] FluentValidation + custom validation filter
   * [x] JWT authentication via custom filter
   * [x] Centralized configuration binding
   * [x] Unit & integration testing
   * [x] GitHub Actions CI
   * [x] Scalar-based interactive API docs
   * [ ] RESTful routing (planned)

## 5. **API Documentation (Scalar)**
This project uses [Scalar](https://github.com/RicoSuter/NSwag/wiki/Scalar) to provide modern, interactive API documentation based on OpenAPI.

### Features
- Auto-generated OpenAPI spec from annotated endpoints
- Interactive UI to try out APIs directly
- Support for JWT token testing
- Simple setup with minimal configuration

### Access the Docs

Once the project is running, visit: [https://localhost:{port}/scalar/](https://localhost:{port}/scalar/)
> Replace `{port}` with the port number shown in your terminal when starting the app.

### Setup

Scalar and OpenAPI are enabled via the following lines in `Program.cs`:

```csharp
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference();
}
````


## 6. **Testing**
This project includes both **unit tests** and **integration tests**, focusing on critical logic, filters, and controller behavior.

### Tools & Libraries
- **xUnit** – test framework
- **Moq** – mocking dependencies
- **AutoFixture** – auto-generating test data
- **FluentAssertions** – expressive assertion syntax
- **In-memory database** – used for EF Core integration testing
- **MVC.Testing** – testing controller pipelines
- **WireMock.Net** – simulating HTTP endpoints (for advanced mocking)

### Test Coverage
- **Repository layer** – tested with in-memory EF Core database
- **Service layer** – covers core logic, validation, and authorization
- **API controllers** – tested using controller-level integration tests

### Running the Tests

Run all tests using:

```

dotnet test

````

### Test Project Structure

Tests are located in the `To-Do-List.Tests/` folder

### Sample Test Cases

#### 1. Repository Integration Test with In-Memory Database
```csharp
[Fact]
public async Task AddTaskAsync_ShouldAddAndSave_WhenCalled()
{
    var options = new DbContextOptionsBuilder<TaskDbContext>()
        .UseInMemoryDatabase("Test_AddTaskAsync").Options;

    using (var context = new TaskDbContext(options))
    {
        var task = new TaskItem { Title = "Test Task" };
        var repo = new TaskRepository(context);
        await repo.AddTaskAsync(task);
    }

    using (var context = new TaskDbContext(options))
    {
        context.Tasks.Count().Should().Be(1);
    }
}
````

#### 2. Service Unit Test with Moq + AutoFixture

```csharp
[Theory, AutoMoqData]
public async Task CreateTaskAsync_ShouldReturnSuccess_WhenCategoryExists(
    [Frozen] Mock<ICategoryRepository> mockCategoryRepo,
    [Frozen] Mock<ITaskRepository> mockTaskRepo,
    TaskCategoryService sut,
    string title, string description, DateTime dueDate,
    string userId, string categoryId, int priority, Category category)
{
    mockCategoryRepo.Setup(r => 
    r.GetCategoryByIdAsync(categoryId, userId))
     .ReturnsAsync(category);

    var result = await sut.CreateTaskAsync(title, description, dueDate, priority, userId, categoryId);

    result.Should().Be(ApiResponseCode.TaskCreateSuccess);
    mockTaskRepo.Verify(r => r.AddTaskAsync(It.IsAny<TaskItem>()), Times.Once);
}
```

#### 3. API Endpoint Test with JWT Authorization

```csharp
[Fact]
public async Task CreateTaskAsync_ShouldReturnSuccess()
{
    var testUserId = "1001";
    var token = _tokenHelper.CreateToken(new { UserId = testUserId, JWTVersion = 1 }, TokenType.AccessToken);
    _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token.TokenStr);

    var request = new CreateTaskRequest("id", "desc", DateTime.Today.AddDays(1), 1, testUserId);
    var response = await _client.PostAsJsonAsync("/Task/CreateTask", request);

    response.EnsureSuccessStatusCode();
    var responseStr = await response.Content.ReadAsStringAsync();
    var result = JsonHelper.Deserialize<ApiResponse<DataWrapper<string>>>(responseStr);

    result.Data.Code.Should().Be(300000);
    result.Data.Info.Should().BeEquivalentTo("Task created successfully");
}
```
## 7. **CI/CD (GitHub Actions)**

This project uses GitHub Actions to automate the continuous integration process.

### Current CI Setup
- Runs on every `push` and `pull request` to the `main` branch
- Restores dependencies using `dotnet restore`
- Builds the solution with `dotnet build`
- Executes all unit and integration tests using `dotnet test`
- Uses detailed test logs and warnings display for better debugging

## 8. **Getting Started**

### Prerequisites
- [.NET 9 SDK](https://dotnet.microsoft.com/en-us/download/dotnet/9.0)
- MySQL (if using real database instead of in-memory)
- IDE: JetBrains Rider or Visual Studio Code

### Database Setup (EF Core Migrations)
This project uses multiple `DbContext` classes. Run the following commands to apply all necessary migrations:
```bash
# Apply migration for Identity
dotnet ef database update --context To_Do_List.Identity.DbContext.MyIdentityDbContext \
  --project To-Do-List/To-Do-List.csproj \
  --startup-project To-Do-List/To-Do-List.csproj

# Apply migration for Task module
dotnet ef database update --context To_Do_List.Tasks.DbContext.TaskDbContext \
  --project To-Do-List/To-Do-List.csproj \
  --startup-project To-Do-List/To-Do-List.csproj

# Apply migration for System Configuration
dotnet ef database update --context To_Do_List.Configuration.DbContext.SystemConfigurationContext \
  --project To-Do-List/To-Do-List.csproj \
  --startup-project To-Do-List/To-Do-List.csproj
````

> ⚠️ Make sure configuration contains the correct connection strings for all contexts.

### Run the Project

```bash
dotnet run --project To-Do-List
```

Visit your API at:
`https://localhost:{port}`

### API Documentation

* Scalar UI: `https://localhost:{port}/scalar/`

### Run Tests

```bash
dotnet test
```

> All tests run using in-memory DB and mock dependencies, controller run using test MySQL database

## 9. **Developer Resources / Blog**

For in-depth explanations of design choices, technical challenges, and clean architecture practices used in this project, check out my blog series on [DEV Community](https://dev.to/alexleeeeeeeeee):

🔗 https://dev.to/alexleeeeeeeeee

> Articles cover topics like layered architecture, custom middleware, test automation, and CI/CD integration.