using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using To_Do_List.Attribute;
using To_Do_List.Controller;
using To_Do_List.Identity.Entities;
using To_Do_List.Identity.Interface;
using To_Do_List.Identity.Services;
using To_Do_List.Require;

namespace To_Do_List.Identity.Controllers.Login;

[ApiController]
[Route("[controller]/[action]")]
public class UserController(IdentityService identityService, ITokenHelper tokenHelper) : ProjectBaseController
{
    [HttpPost]
    [NotCheckJwtVersion]
    public async Task<ActionResult> Register(RegisterRequest request)
    {
        var (identityResult, code) = await identityService.RegisterAsync(request.UserName, request.Password, request.Email);
        if (identityResult.Succeeded)
        {
            return Ok(new ResponseData(code, null));
        }
        else
        {
            return BadRequest(new ResponseData(code, identityResult.Errors));
        }
    }

    [HttpPost]
    [NotCheckJwtVersion]
    public async Task<ActionResult> Login(LoginRequest request)
    {
        var (code, accessToken, refreshToken) = await identityService.LoginByEmailAndPasswordAsync(request.Email, request.Password);
        if (code != ApiResponseCode.UserLoginSuccess)
        {
            return BadRequest(new ResponseData(code, "Login failed"));
        }
        return Ok(new LoginResponseData(code, null, accessToken, refreshToken));
    }

    [HttpPut]
    public async Task<ActionResult> ChangePasswordAsync(ChangePasswordRequest request)
    {
        var (identityResult, code) = await identityService.ChangePasswordAsync(UserId, request.Password);
        if (identityResult.Succeeded)
        {
            return Ok(new ResponseData(code, null));
        }
        else
        {
            return BadRequest(new ResponseData(code, identityResult.Errors));
        }
    }

    [HttpGet]    
    public async Task<ActionResult> GetUserByIdAsync()
    {
        var (code, user) = await identityService.GetUserByIdAsync(UserId);
        if (code != ApiResponseCode.UserFetchSuccess)
        {
            return BadRequest(new ResponseData(code, "User not found"));
        }
        
        var dto = new UserDto(user?.UserName!, user?.Email!);
        return Ok(new ResponseData(code, dto));
    }

    [HttpPost]
    [NotCheckJwtVersion]
    public async Task<ActionResult> RefreshTokenAsync(RefreshTokenRequest request)
    {
        var (code, accessToken, refreshToken) = await identityService.RefreshTokenAsync(request.RefreshToken);
        if (code != ApiResponseCode.RefreshTokenSuccess)
        {
            return Unauthorized(new ResponseData(code, "RefreshToken failed"));
        }
        return Ok(new LoginResponseData(code, null, accessToken, refreshToken));
    }
}