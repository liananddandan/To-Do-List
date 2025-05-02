using System.Security.Claims;
using Microsoft.AspNetCore.Identity;
using To_Do_List.Identity.Entities;
using To_Do_List.Identity.Interface;
using To_Do_List.Identity.Token;
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

    public async Task<(ApiResponseCode code, string? accessToken, string? refreshToken)> 
        LoginByEmailAndPasswordAsync(string requestEmail, string requestPassword)
    {
        var user = await repository.FindUserByEmailAsync(requestEmail);
        if (user == null)
        {
            return (ApiResponseCode.UserNotFound, null, null);
        }

        var passwordVerityResult =
            new PasswordHasher<MyUser>().VerifyHashedPassword(null, user.PasswordHash, requestPassword);
        if (passwordVerityResult == PasswordVerificationResult.Failed)
        {
            // todo: lock user logic
            return (ApiResponseCode.UserPasswordError, null, null);
        }
        // Generate Token
        var entry = new
        {
            UserId = user.Id,
            JWTVersion = user.Version,
        };
        var accessToken = tokenHelper.CreateToken(entry, TokenType.AccessToken);
        var refreshToken = tokenHelper.CreateToken(entry, TokenType.RefreshToken);
        return (ApiResponseCode.UserLoginSuccess, accessToken.TokenStr, refreshToken.TokenStr);
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

    public async Task<(ApiResponseCode code, MyUser? user)> GetUserByIdAsync(string userId)
    {
        var user =  await repository.GetUserByIdAsync(userId);
        return (user == null ? ApiResponseCode.UserNotFound : ApiResponseCode.UserFetchSuccess,
                user);
    }

    public async Task<(ApiResponseCode code, string? accessToken, string? RefreshToken)> RefreshTokenAsync(string refreshToken)
    {
        var (code, principal) = tokenHelper.ValidateExpiredRefreshToken(refreshToken);
        if (code != ApiResponseCode.RefreshTokenInfoCheckSuccess || principal == null)
        {
            return (code, null, null);
        }
        
        var userId = principal.FindFirst("UserId")!.Value;
        var user = await repository.GetUserByIdAsync(userId);
        if (user == null)
        {
            return (ApiResponseCode.UserNotFound, null, null);
        }
        var jwtVersion = principal.FindFirst("JWTVersion")!.Value;
        if (user.Version > long.Parse(jwtVersion))
        {
            return (ApiResponseCode.RefreshTokenInvalidVersion, null, null);
        }
        
        await repository.IncrementTokenVersionAsync(user);
        var entry = new
        {
            UserId = user.Id,
            JWTVersion = user.Version,
        };
        return (ApiResponseCode.RefreshTokenSuccess, 
                tokenHelper.CreateToken(entry, TokenType.AccessToken).TokenStr,
                tokenHelper.CreateToken(entry, TokenType.RefreshToken).TokenStr);
    }
}