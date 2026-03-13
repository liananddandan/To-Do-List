using Microsoft.AspNetCore.Identity;
using To_Do_List.Application.DTOs;
using To_Do_List.domain.Entities;

namespace To_Do_List.Application.Services.Interface;

public interface IIdentityService
{
    Task<(IdentityResult identityResult, ApiResponseCode code)> RegisterAsync(
        string requestUserName,
        string requestPassword,
        string requestEmail);

    Task<(ApiResponseCode code, string? accessToken, string? refreshToken)> LoginByEmailAndPasswordAsync(
        string requestEmail,
        string requestPassword);

    Task<(IdentityResult identityResult, ApiResponseCode code)> ChangePasswordAsync(
        string userId,
        string password);

    Task<(ApiResponseCode code, MyUser? user)> GetUserByIdAsync(
        string userId);

    Task<(ApiResponseCode code, string? accessToken, string? refreshToken)> RefreshTokenAsync(
        string refreshToken);
}