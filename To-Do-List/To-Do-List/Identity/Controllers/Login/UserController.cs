using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using To_Do_List.Identity.Services;
using To_Do_List.Require;
using SignInResult = Microsoft.AspNetCore.Identity.SignInResult;

namespace To_Do_List.Identity.Controllers.Login;

[ApiController]
[Route("[controller]/[action]")]
public class UserController(IdentityService identityService, 
    IValidator<RegisterRequest> registerRequestValidator) : ControllerBase
{
    [HttpPost]
    public async Task<ActionResult> Register(RegisterRequest request)
    {
        var result = await registerRequestValidator.ValidateAsync(request);
        if (!result.IsValid)
        {
            return BadRequest(new {
                Code = ApiResponseCode.ParameterError,
                Errors = result.Errors.Select(e => e.ErrorMessage).ToList()
            });
        }
        var (signInResult, code) = await identityService.Register(request.userName, request.password, request.email);
        if (signInResult.Succeeded)
        {
            return Ok(code);
        }
        else
        {
            return BadRequest(new {
                Code = code,
                Errors = "Registration failed"
            });
        }
    }
}