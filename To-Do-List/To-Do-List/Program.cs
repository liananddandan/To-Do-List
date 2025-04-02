using System.Text.Json;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Scalar.AspNetCore;
using To_Do_List.Configuration.Extensions;
using To_Do_List.DbContext;
using To_Do_List.Entities;
using To_Do_List.JWT.Options;

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
if (!string.IsNullOrEmpty(jwtJson))
{
    var jwtOptions = JsonSerializer.Deserialize<JwtTokenOption>(jwtJson);
    builder.Services.Configure<JwtTokenOption>(options =>
    {
        options.Issuer = jwtOptions.Issuer;
        options.Audience = jwtOptions.Audience;
        options.IssuerSigningKey = jwtOptions.IssuerSigningKey;
        options.AccessTokenExpiresMinutes = jwtOptions.AccessTokenExpiresMinutes;
    });
}

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();