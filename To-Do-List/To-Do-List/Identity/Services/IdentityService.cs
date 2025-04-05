using Microsoft.AspNetCore.Identity;
using To_Do_List.Identity.Entities;
using To_Do_List.Identity.Interface;
using To_Do_List.Require;

namespace To_Do_List.Identity.Services;

public class IdentityService(IIdRepository repository)
{
    public async Task<(SignInResult, ApiResponseCode)> Register(string requestUserName, string requestPassword,
        string requestEmail)
    {
        var user = await repository.FindUserByUserName(requestUserName);
        if (user != null)
        {
            return (SignInResult.Failed, ApiResponseCode.UserExisted);
        }
        else
        {
            MyUser myUser = new MyUser();
            myUser.UserName = requestUserName;
            myUser.Email = requestEmail;
            myUser.PasswordHash = new PasswordHasher<object>().HashPassword(null, requestPassword);
            myUser.UserGuid = Guid.NewGuid();
            var identityResult = await repository.CreateUser(myUser);
            return identityResult.Succeeded
                ? (SignInResult.Success, ApiResponseCode.UserCreateSuccess)
                : (SignInResult.Failed, ApiResponseCode.UserCreatFailed);
        }
    }
}