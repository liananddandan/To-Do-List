using Microsoft.AspNetCore.Identity;
using To_Do_List.Identity.Entities;

namespace To_Do_List.Identity.Interface;

public interface IIdRepository
{ 
    Task<MyUser?> FindUserByUserName(string userName);
    Task<MyUser?> FindUserByEmail(string email);
    Task<IdentityResult> CreateUser(MyUser user);
}