using Microsoft.AspNetCore.Identity;
using To_Do_List.Identity.Entities;
using To_Do_List.Identity.Interface;

namespace To_Do_List.Identity.Implements;

public class IdRepository(UserManager<MyUser> userManager) : IIdRepository
{
    public Task<MyUser?> FindUserByUserNameAsync(string userName)
    {
        return userManager.FindByNameAsync(userName);
    }

    public Task<MyUser?> FindUserByEmailAsync(string email)
    {
        return userManager.FindByEmailAsync(email);
    }

    public Task<MyUser?> FindUserByIdAsync(string id)
    {
        return userManager.FindByIdAsync(id);
    }

    public Task<IdentityResult> CreateUserAsync(MyUser user, string password)
    {
        // (user, password)会检查密码强度，（user）这个方法不回检查密码强度
        return userManager.CreateAsync(user, password);
    }

    public Task<IdentityResult> UpdateUserAsync(MyUser user)
    {
        return userManager.UpdateAsync(user);
    }

    public async Task<IdentityResult> ChangePasswordAsync(string userId, string password)
    {
        var user = await FindUserByIdAsync(userId);
        if (user == null)
        {
            return IdentityResult.Failed();
        }
        var token = await userManager.GeneratePasswordResetTokenAsync(user);
        var resetResult = await userManager.ResetPasswordAsync(user, token, password);
        if (!resetResult.Succeeded)
        {
            return resetResult;
        }
        user.Version++;
        return await UpdateUserAsync(user);
    }

    public Task<MyUser?> GetUserByIdAsync(string id)
    {
        return userManager.FindByIdAsync(id);
    }
}