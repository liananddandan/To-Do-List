using Microsoft.AspNetCore.Mvc;
using To_Do_List.Application.DTOs;
using To_Do_List.Application.Services.Interface;
using To_Do_List.domain.Entities;

namespace To_Do_List.Api.Controllers;

[ApiController]
[Route("api/users")]
public class UsersController(IIdentityService identityService) : ProjectBaseController
{
    [HttpGet("me")]
    public async Task<ActionResult> GetCurrentUser()
    {
        var (code, user) = await identityService.GetUserByIdAsync(UserId);

        if (code != ApiResponseCode.UserFetchSuccess)
            return NotFound(new ResponseData(code, "User not found"));

        var dto = new UserDto(user!.UserName, user.Email);

        return Ok(new ResponseData(code, dto));
    }
}