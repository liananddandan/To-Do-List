using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.AspNetCore.Mvc.Versioning;
using Scalar.AspNetCore;
using WebApplication1;
using WebApplication1.Filter;
using WebApplication1.Middleware;

var builder = WebApplication.CreateBuilder(args);

builder.Logging.AddConsole();
// Add services to the container.
builder.Services.AddControllers();

// builder.Services.AddControllers(options =>
// {
//     options.Filters.Add<LogActionFilter>();
//     options.Filters.Add<GlobalExceptionFilter>();
// });
// Controller version control
builder.Services.AddApiVersioning(options =>
{
    options.AssumeDefaultVersionWhenUnspecified = true;
    options.DefaultApiVersion = new ApiVersion(1, 0);
    options.ReportApiVersions = true;
    options.ApiVersionReader = new UrlSegmentApiVersionReader();
});

builder.Services.AddVersionedApiExplorer(options =>
{
    options.GroupNameFormat = "'v'VVV"; // v1, v1.0, v2 这样的分组名
    options.SubstituteApiVersionInUrl = true; // 把 {version} 替换成实际版本号
});

builder.Services.AddSwaggerGen();
builder.Services.ConfigureOptions<ConfigureSwaggerOptions>();


// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
// builder.Services.AddOpenApi();

var app = builder.Build();


// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    var provider = app.Services.GetRequiredService<IApiVersionDescriptionProvider>();
    foreach (var d in provider.ApiVersionDescriptions)
    {
        Console.WriteLine($"Registered API version: {d.GroupName} = {d.ApiVersion}");
    }
    app.UseSwagger();
    app.UseSwaggerUI(options =>
    {
        foreach (var desc in provider.ApiVersionDescriptions)
        {
            options.SwaggerEndpoint($"/swagger/{desc.GroupName}/swagger.json", 
                $"API {desc.GroupName}");
        }
    });
}

app.UseHttpsRedirection();


// app.UseMiddleware<RequestLoggingMiddleware>();
// app.UseMiddleware<RequestTimeRecordMiddleware>();
// app.UseMiddleware<FakeAuthMiddleware>();
// app.UseMiddleware<InjectHeaderMiddleware>();
app.UseRouting();
app.UseAuthorization();

app.MapControllers();

app.Run();