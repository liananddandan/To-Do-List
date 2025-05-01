using System.Security.Claims;
using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using To_Do_List.Attribute;
using To_Do_List.Controller;
using To_Do_List.Identity.Entities;
using To_Do_List.Identity.Services;
using To_Do_List.Require;

namespace To_Do_List.Identity.Controllers.Login;

[ApiController]
[Route("[controller]/[action]")]
public class UserController(IdentityService identityService) : ProjectBaseController
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
        var (signInResult, code, token) = await identityService.LoginByEmailAndPasswordAsync(request.Email, request.Password);
        if (!signInResult.Succeeded)
        {
            return BadRequest(new ResponseData(code, "Login failed"));
        }
        return Ok(new LoginResponseData(code, null, token));
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
}