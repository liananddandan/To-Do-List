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

    public Task<IdentityResult> CreateUserAsync(MyUser user)
    {
        return userManager.CreateAsync(user);
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
        await userManager.ResetPasswordAsync(user, token, password);
        user.Version++;
        return await UpdateUserAsync(user);
    }
}