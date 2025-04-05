using Microsoft.AspNetCore.Identity;
using To_Do_List.Identity.Entities;
using To_Do_List.Identity.Interface;

namespace To_Do_List.Identity.Implements;

public class IdRepository(UserManager<MyUser> userManager) : IIdRepository
{
    public Task<MyUser?> FindUserByUserName(string userName)
    {
        return userManager.FindByNameAsync(userName);
    }

    public Task<IdentityResult> CreateUser(MyUser user)
    {
        return userManager.CreateAsync(user);
    }
}