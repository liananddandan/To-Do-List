using Microsoft.AspNetCore.Identity;
using To_Do_List.domain.Entities;

namespace To_Do_List.Application.Interfaces.Repositories;

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