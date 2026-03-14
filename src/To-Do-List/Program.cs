using System.Text;
using FluentValidation;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Scalar.AspNetCore;
using To_Do_List.Application.Common.Options;
using To_Do_List.Application.DTOs;
using To_Do_List.Application.Filter;
using To_Do_List.Application.Interfaces.Repositories;
using To_Do_List.Application.Services;
using To_Do_List.Application.Services.Interface;
using To_Do_List.domain.Entities;
using To_Do_List.Infrastructure.persistent.DbContext;
using To_Do_List.Infrastructure.persistent.repositories;

var builder = WebApplication.CreateBuilder(args);

builder.Logging.AddConsole();

// DbContexts
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection")
    ?? throw new ArgumentException("DefaultConnection is required.");

builder.Services.AddDbContext<MyIdentityDbContext>(options =>
{
    options.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString));
});

builder.Services.AddDbContext<TaskDbContext>(options =>
{
    options.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString));
});

// Controllers + Filters
builder.Services.AddControllers(options =>
{
    options.Filters.Add<ApiResponseWrapperFilter>(order: 1);
    options.Filters.Add<JwtVersionCheckFilter>(order: 2);
    options.Filters.Add<ValidationFilter>(order: 3);
});

builder.Services.AddOpenApi();

// Identity
builder.Services.AddDataProtection();

builder.Services.AddIdentityCore<MyUser>(options =>
{
    options.Password.RequiredLength = 8;
    options.Password.RequireNonAlphanumeric = false;
    options.Password.RequireUppercase = false;
    options.Password.RequireLowercase = false;
    options.Tokens.PasswordResetTokenProvider = TokenOptions.DefaultEmailProvider;
    options.Tokens.EmailConfirmationTokenProvider = TokenOptions.DefaultEmailProvider;
});

var identityBuilder = new IdentityBuilder(typeof(MyUser), typeof(MyRole), builder.Services);
identityBuilder.AddEntityFrameworkStores<MyIdentityDbContext>()
    .AddDefaultTokenProviders()
    .AddRoleManager<RoleManager<MyRole>>()
    .AddUserManager<UserManager<MyUser>>();

// Jwt Options
builder.Services
    .AddOptions<JwtTokenOption>()
    .Bind(builder.Configuration.GetSection("Jwt"))
    .Validate(o =>
        !string.IsNullOrWhiteSpace(o.Issuer) &&
        !string.IsNullOrWhiteSpace(o.Audience) &&
        !string.IsNullOrWhiteSpace(o.IssuerSigningKey),
        "Jwt configuration is invalid.")
    .ValidateOnStart();

var jwtOptions = builder.Configuration.GetSection("Jwt").Get<JwtTokenOption>()
    ?? throw new ArgumentException("Jwt configuration is required.");

// Authentication
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.RequireHttpsMetadata = false;
    options.SaveToken = true;
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidIssuer = jwtOptions.Issuer,
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtOptions.IssuerSigningKey)),
        ValidateAudience = true,
        ValidAudience = jwtOptions.Audience,
        ValidateLifetime = true,
        ClockSkew = TimeSpan.FromMinutes(1)
    };

    options.Events = new JwtBearerEvents
    {
        OnAuthenticationFailed = context =>
        {
            Console.WriteLine($"Authentication failed: {context.Exception.Message}");
            return Task.CompletedTask;
        },
        OnTokenValidated = _ =>
        {
            Console.WriteLine("Token validated successfully");
            return Task.CompletedTask;
        }
    };
});

builder.Services.AddAuthorization();

builder.Services.AddValidatorsFromAssemblyContaining<RegisterRequestValidator>();

builder.Services.AddScoped<IIdentityService, IdentityService>();
builder.Services.AddScoped<IIdRepository, IdRepository>();
builder.Services.AddScoped<ITokenHelper, TokenHelper>();
builder.Services.AddScoped<ITaskRepository, TaskRepository>();
builder.Services.AddScoped<ICategoryRepository, CategoryRepository>();
builder.Services.AddScoped<ITaskCategoryService, TaskCategoryService>();

builder.Services.AddCors(options =>
{
    options.AddPolicy("DynamicCORS", policy =>
    {
        policy.SetIsOriginAllowed(origin => origin.Contains("localhost"))
            .AllowAnyHeader()
            .AllowAnyMethod()
            .AllowCredentials();
    });
});

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference();
}

app.UseCors("DynamicCORS");
app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;

    try
    {
        var identityDb = services.GetRequiredService<MyIdentityDbContext>();
        identityDb.Database.Migrate();

        var taskDb = services.GetRequiredService<TaskDbContext>();
        taskDb.Database.Migrate();
    }
    catch (Exception ex)
    {
        Console.WriteLine("Database migration failed:");
        Console.WriteLine(ex);
    }
}

app.Run();

public partial class Program { }