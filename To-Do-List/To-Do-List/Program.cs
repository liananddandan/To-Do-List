using System.Text;
using System.Text.Json;
using FluentValidation;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Scalar.AspNetCore;
using To_Do_List.Configuration.Extensions;
using To_Do_List.Filter;
using To_Do_List.Identity.Controllers.Login;
using To_Do_List.Identity.DbContext;
using To_Do_List.Identity.Entities;
using To_Do_List.Identity.Implements;
using To_Do_List.Identity.Interface;
using To_Do_List.Identity.Options;
using To_Do_List.Identity.Services;
using To_Do_List.Identity.Token;
using To_Do_List.Tasks.DbContext;
using To_Do_List.Tasks.Implement;
using To_Do_List.Tasks.Interface;
using To_Do_List.Tasks.Service;

var builder = WebApplication.CreateBuilder(args);

// ✅ 手动添加 UserSecrets，而不是用 builder.Host.ConfigureAppConfiguration
if (!builder.Environment.IsProduction())
{
    builder.Configuration.AddUserSecrets(typeof(Program).Assembly, optional: true);
}

builder.Logging.AddConsole();

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

builder.Configuration.AddEFConfiguration(options =>
    {
        var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
        options.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString));
    }
);

// configure Identity
builder.Services.AddDbContext<MyIdentityDbContext>(options =>
{
    var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
    options.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString));
});
builder.Services.AddDbContext<TaskDbContext>(options =>
{
    var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
    options.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString));
});

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

// JWT
var jwtJson = builder.Configuration[JwtTokenOption.JwtKey];
JwtTokenOption jwtOptions = JsonSerializer.Deserialize<JwtTokenOption>(jwtJson);
if (jwtOptions != null)
{
    builder.Services.Configure<JwtTokenOption>(options =>
    {
        options.Issuer = jwtOptions.Issuer;
        options.Audience = jwtOptions.Audience;
        options.IssuerSigningKey = jwtOptions.IssuerSigningKey;
        options.AccessTokenExpiresMinutes = jwtOptions.AccessTokenExpiresMinutes;
        options.RefreshTokenExpiresDays = jwtOptions.RefreshTokenExpiresDays;
    });
}
else
{
    throw new ArgumentException("JWT token options are required.");
}

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(options =>
{
    options.RequireHttpsMetadata = false;
    options.SaveToken = true;
    options.TokenValidationParameters = new ()
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
    options.Events = new JwtBearerEvents()
    {
        OnAuthenticationFailed = context =>
        {
            Console.WriteLine($"Authentication failed: {context.Exception.Message}");
            return Task.CompletedTask;
        },
        OnTokenValidated = context =>
        {
            Console.WriteLine("Token validated successfully");
            return Task.CompletedTask;
        }
    };
});

builder.Services.AddValidatorsFromAssemblyContaining<RegisterRequestValidator>();

builder.Services.AddScoped<IdentityService, IdentityService>();
builder.Services.AddScoped<IIdRepository, IdRepository>();
builder.Services.AddScoped<ITokenHelper, TokenHelper>();
builder.Services.AddScoped<ITaskRepository, TaskRepository>();
builder.Services.AddScoped<ICategoryRepository, CategoryRepository>();
builder.Services.AddScoped<TaskCategoryService, TaskCategoryService>();

//Filter
builder.Services.AddControllers(options =>
{
    options.Filters.Add<ApiResponseWrapperFilter>(order: 1);
    options.Filters.Add<JwtVersionCheckFilter>(order: 2);
    options.Filters.Add<ValidationFilter>(order: 3);
});

builder.Services.AddCors(options =>
{
    options.AddPolicy("DynamicCORS", builder =>
    {
        builder.SetIsOriginAllowed(origin =>
            {
                return origin.Contains("localhost");
            })
            .AllowAnyHeader()
            .AllowAnyMethod()
            .AllowCredentials();
    });
});

var app = builder.Build();

// Configure the HTTP request pipeline.
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

app.Run();

public partial class Program { }