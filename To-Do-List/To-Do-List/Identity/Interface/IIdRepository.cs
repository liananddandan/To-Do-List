using Microsoft.AspNetCore.Identity;
using To_Do_List.Identity.Entities;

namespace To_Do_List.Identity.Interface;

public interface IIdRepository
{ 
    Task<MyUser?> FindUserByUserNameAsync(string userName);
    Task<MyUser?> FindUserByEmailAsync(string email);
    Task<MyUser?> FindUserByIdAsync(string id);
    Task<IdentityResult> CreateUserAsync(MyUser user, string password);
    Task<IdentityResult> ChangePasswordAsync(string userId, string password);
    Task<MyUser?> GetUserByIdAsync(string id);
    Task<IdentityResult> IncrementTokenVersionAsync(MyUser user);
}