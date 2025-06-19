using To_Do_List.Identity.DbContext;
using To_Do_List.Identity.Entities;
using To_Do_List.Tasks.DbContext;
using To_Do_List.Tasks.Entities;

namespace To_Do_List.Tests;

public static class TestDataSeeder
{
    public static void SeedIdentityData(MyIdentityDbContext context)
    {
        context.Users.AddRange(
            new MyUser
            {
                Id = 1001,
                UserName = "testuser1001",
                Email = "testuser1001@gmail.com",
                PasswordHash = "FAKE_PASSWORD_HASH_1001",
                UserGuid = Guid.NewGuid()
            },
            new MyUser
            {
                Id = 1002,
                UserName = "testuser1002",
                Email = "testuser1002@gmail.com",
                PasswordHash = "FAKE_PASSWORD_HASH_1002",
                UserGuid = Guid.NewGuid()
            }
        );
        context.SaveChanges();
    }

    public static void SeedTaskData(TaskDbContext context)
    {
        context.Categories.AddRange(
            new Category
            {
                Id = 1001,
                Name = "1001_test_category1001",
                Description = "1001_test_category1001_description",
                UserId = "1001",
                IsDefault = true
            },
            new Category
            {
                Id = 1002,
                Name = "1001_test_category1002",
                Description = "1001_test_category1002_description",
                UserId = "1001",
                IsDefault = false
            }
        );
        context.SaveChanges();
    }
}