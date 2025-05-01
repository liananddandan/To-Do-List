using System.Security.Claims;
using Microsoft.AspNetCore.Identity;
using To_Do_List.Identity.Entities;
using To_Do_List.Identity.Interface;
using To_Do_List.Require;

namespace To_Do_List.Identity.Services;

public class IdentityService(IIdRepository repository, ITokenHelper tokenHelper)
{
    public async Task<(IdentityResult, ApiResponseCode)> RegisterAsync(string requestUserName, string requestPassword,
        string requestEmail)
    {
        var user = await repository.FindUserByUserNameAsync(requestUserName);
        if (user != null)
        {
            return (IdentityResult.Failed(), ApiResponseCode.UserExisted);
        }
        else
        {
            var myUser = new MyUser()
            {
                UserName = requestUserName,
                Email = requestEmail,
                UserGuid = Guid.NewGuid()
            };
            var identityResult = await repository.CreateUserAsync(myUser, requestPassword);
            return (identityResult, 
                identityResult.Succeeded ? ApiResponseCode.UserCreateSuccess : ApiResponseCode.UserCreatFailed);
        }
    }

    public async Task<(SignInResult signInResult, ApiResponseCode code, string? token)> LoginByEmailAndPasswordAsync(
        string requestEmail, string requestPassword)
    {
        var user = await repository.FindUserByEmailAsync(requestEmail);
        if (user == null)
        {
            return (SignInResult.Failed, ApiResponseCode.UserNotFound, null);
        }

        var passwordVerityResult =
            new PasswordHasher<MyUser>().VerifyHashedPassword(null, user.PasswordHash, requestPassword);
        if (passwordVerityResult == PasswordVerificationResult.Failed)
        {
            // todo: lock user logic
            return (SignInResult.Failed, ApiResponseCode.UserPasswordError, null);
        }
        // Generate Token
        var jwtToken = tokenHelper.CreateToken(new
        {
            UserId = user.Id,
            JWTVersion = user.Version,
        });
        return (SignInResult.Success, ApiResponseCode.UserLoginSuccess, jwtToken.TokenStr);
    }

    public async Task<(IdentityResult identityResult, ApiResponseCode code)> ChangePasswordAsync(string userId, string password)
    {
        var identityResult = await repository.ChangePasswordAsync(userId, password);
        if (identityResult.Succeeded)
        {
            return (identityResult, ApiResponseCode.UserChangePasswordSuccess);
        }
        else
        {
            return (identityResult, ApiResponseCode.UserChangePasswordFailed);
        }
    }

    private string GetHashPassword(string password)
    {
        return new PasswordHasher<object>().HashPassword(null, password);
    }

    public async Task<(ApiResponseCode code, MyUser? user)> GetUserByIdAsync(string userId)
    {
        var user =  await repository.GetUserByIdAsync(userId);
        if (user == null)
        {
            return (ApiResponseCode.UserNotFound, null);
        }
        else
        {
            return (ApiResponseCode.UserFetchSuccess ,user);
        }
    }
}