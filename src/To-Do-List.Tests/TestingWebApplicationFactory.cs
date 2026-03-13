using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using To_Do_List.Infrastructure.persistent.DbContext;

namespace To_Do_List.Tests;

public class TestingWebApplicationFactory<TProgram> : WebApplicationFactory<TProgram> where TProgram : class
{
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        Console.WriteLine("🧪 TestingWebApplicationFactory is invoked.");
        builder.UseEnvironment("Testing"); // 设置环境名，方便读取 appsettings.Testing.json
        
        builder.ConfigureAppConfiguration((context, configBuilder) =>
        {
            Console.WriteLine("🧪 TestingWebApplicationFactory(ConfigureAppConfiguration) is invoked.");
            configBuilder.AddJsonFile("appsettings.Testing.json", optional: false, reloadOnChange: true);
            configBuilder.AddEnvironmentVariables();
        });
        
        builder.ConfigureServices(services =>
        {
            Console.WriteLine("🧪 TestingWebApplicationFactory(ConfigureServices) is invoked.");

            // 构建临时 Provider
            var sp = services.BuildServiceProvider();
            using var scope = sp.CreateScope();
            var scopedServices = scope.ServiceProvider;

            var identityDb = scopedServices.GetRequiredService<MyIdentityDbContext>();
            identityDb.Database.EnsureDeleted();
            identityDb.Database.Migrate();
            TestDataSeeder.SeedIdentityData(identityDb);
            
            var taskDb = scopedServices.GetRequiredService<TaskDbContext>();
            taskDb.Database.Migrate();
            TestDataSeeder.SeedTaskData(taskDb);

        });

    }
}