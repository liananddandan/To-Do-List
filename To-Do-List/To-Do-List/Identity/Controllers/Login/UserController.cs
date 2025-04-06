using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using To_Do_List.Attribute;
using To_Do_List.Identity.Entities;
using To_Do_List.Identity.Services;
using To_Do_List.Require;

namespace To_Do_List.Identity.Controllers.Login;

[ApiController]
[Route("[controller]/[action]")]
public class UserController(IdentityService identityService, 
    IValidator<RegisterRequest> registerRequestValidator, 
    IValidator<LoginRequest> loginRequestValidator) : ControllerBase
{
    [HttpPost]
    [NotCheckJwtVersion]
    public async Task<ActionResult> Register(RegisterRequest request)
    {
        var result = await registerRequestValidator.ValidateAsync(request);
        if (!result.IsValid)
        {
            return BadRequest(new ResponseData(ApiResponseCode.ParameterError, 
                result.Errors.Select(e => e.ErrorMessage).ToList()));
        }
        var (signInResult, code) = await identityService.Register(request.UserName, request.Password, request.Email);
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
        var validationResult = await loginRequestValidator.ValidateAsync(request);
        if (!validationResult.IsValid)
        {
            return BadRequest(new ResponseData(ApiResponseCode.ParameterError,
                validationResult.Errors.Select(e => e.ErrorMessage).ToList()));
        }
        var (signInResult, code, token) = await identityService.LoginByEmailAndPassword(request.Email, request.Password);
        if (!signInResult.Succeeded)
        {
            return BadRequest(new ResponseData(code, "Login failed"));
        }
        return Ok(new LoginResponseData(code, null, token));
    }

    [HttpGet]
    public ActionResult TestToken()
    {
        return Ok("Token Test Ok");
    }
}