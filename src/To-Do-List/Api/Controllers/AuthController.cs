

using Microsoft.AspNetCore.Mvc;
using To_Do_List.Application.Attribute;
using To_Do_List.Application.DTOs;
using To_Do_List.Application.Services.Interface;

namespace To_Do_List.Api.Controllers;

[ApiController]
[Route("api/auth")]
public class AuthController(IIdentityService identityService) : ProjectBaseController
{
    [HttpPost("register")]
    [NotCheckJwtVersion]
    public async Task<ActionResult> Register(RegisterRequest request)
    {
        var (identityResult, code) = await identityService.RegisterAsync(
            request.UserName,
            request.Password,
            request.Email);

        if (!identityResult.Succeeded)
            return BadRequest(new ResponseData(code, identityResult.Errors));

        return Ok(new ResponseData(code, null));
    }

    [HttpPost("login")]
    [NotCheckJwtVersion]
    public async Task<ActionResult> Login(LoginRequest request)
    {
        var (code, accessToken, refreshToken) =
            await identityService.LoginByEmailAndPasswordAsync(request.Email, request.Password);

        if (code != ApiResponseCode.UserLoginSuccess)
            return Unauthorized(new ResponseData(code, "Login failed"));

        return Ok(new LoginResponseData(code, null, accessToken, refreshToken));
    }

    [HttpPost("refresh")]
    [NotCheckJwtVersion]
    public async Task<ActionResult> RefreshToken(RefreshTokenRequest request)
    {
        var (code, accessToken, refreshToken) =
            await identityService.RefreshTokenAsync(request.RefreshToken);

        if (code != ApiResponseCode.RefreshTokenSuccess)
            return Unauthorized(new ResponseData(code, "Refresh token failed"));

        return Ok(new LoginResponseData(code, null, accessToken, refreshToken));
    }

    [HttpPut("password")]
    public async Task<ActionResult> ChangePassword(ChangePasswordRequest request)
    {
        var (identityResult, code) =
            await identityService.ChangePasswordAsync(UserId, request.Password);

        if (!identityResult.Succeeded)
            return BadRequest(new ResponseData(code, identityResult.Errors));

        return Ok(new ResponseData(code, null));
    }
}