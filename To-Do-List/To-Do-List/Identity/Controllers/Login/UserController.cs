using System.Security.Claims;
using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using To_Do_List.Attribute;
using To_Do_List.Identity.Services;
using To_Do_List.Require;

namespace To_Do_List.Identity.Controllers.Login;

[ApiController]
[Route("[controller]/[action]")]
public class UserController(IdentityService identityService, 
    IValidator<RegisterRequest> registerRequestValidator, 
    IValidator<LoginRequest> loginRequestValidator,
    IValidator<ChangePasswordRequest> changePasswordValidator) : ControllerBase
{
    [HttpPost]
    [NotCheckJwtVersion]
    public async Task<ActionResult> Register(RegisterRequest request)
    {
        var (signInResult, code) = await identityService.RegisterAsync(request.UserName, request.Password, request.Email);
        if (signInResult.Succeeded)
        {
            return Ok(new ResponseData(code, null));
        }
        else
        {
            return BadRequest(new ResponseData(code, "Registration failed"));
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
        var userId = User.FindFirstValue("UserId");
        if (userId == null)
        {
            return BadRequest(new ResponseData(ApiResponseCode.TokenPhraseUserIdError, "User id is missing."));
        }
        var (identityResult, code) = await identityService.ChangePasswordAsync(userId, request.Password);
        if (identityResult.Succeeded)
        {
            return Ok(new ResponseData(code, null));
        }
        else
        {
            return BadRequest(new ResponseData(code, "Password change failed"));
        }
    }

    [HttpGet]
    public ActionResult TestToken()
    {
        return Ok("Token Test Ok");
    }
}