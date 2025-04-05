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

var builder = WebApplication.CreateBuilder(args);

builder.Configuration.AddEFConfiguration(options =>
    {
        var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
        options.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString));
    }
);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

// configure Identity
builder.Services.AddDbContext<MyIdentityDbContext>(options =>
{
    var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
    options.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString));
});
builder.Services.AddDataProtection();
builder.Services.AddIdentityCore<MyUser>(options =>
{
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
            if (context.Exception.GetType() == typeof(SecurityTokenExpiredException))
            {
                context.Response.Headers.Add("Token-Expired", "true");
            }
            return Task.CompletedTask;
        }
    };
});

builder.Services.AddValidatorsFromAssemblyContaining<RegisterRequestValidator>();

builder.Services.AddScoped<IdentityService, IdentityService>();
builder.Services.AddScoped<IIdRepository, IdRepository>();

//Filter
builder.Services.AddControllers(options =>
{
    options.Filters.Add<ApiResponseWrapperFilter>();
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference();
}

app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();