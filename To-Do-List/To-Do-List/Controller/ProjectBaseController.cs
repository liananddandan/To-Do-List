using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;

namespace To_Do_List.Controller;

public class ProjectBaseController : ControllerBase
{
    protected string UserId => 
        User.FindFirstValue("UserId") ?? throw new UnauthorizedAccessException("UserId is missing");
}