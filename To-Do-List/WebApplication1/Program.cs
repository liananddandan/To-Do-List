using Scalar.AspNetCore;
using WebApplication1.Filter;
using WebApplication1.Middleware;

var builder = WebApplication.CreateBuilder(args);

builder.Logging.AddConsole();

builder.Services.AddControllers(options =>
{
    options.Filters.Add<LogActionFilter>();
    options.Filters.Add<GlobalExceptionFilter>();
});

// Add services to the container.
builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference();
}

app.UseHttpsRedirection();


// app.UseMiddleware<RequestLoggingMiddleware>();
// app.UseMiddleware<RequestTimeRecordMiddleware>();
// app.UseMiddleware<FakeAuthMiddleware>();
// app.UseMiddleware<InjectHeaderMiddleware>();

app.UseAuthorization();

app.MapControllers();

app.Run();