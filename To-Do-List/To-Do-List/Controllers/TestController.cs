using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace To_Do_List.Controllers;

[ApiController]
[Route("[controller]/[action]")]
[Authorize]
public class TestController : ControllerBase
{
    [HttpGet]
    public ActionResult TestAuthentication()
    {
        return Ok("Test");
    }
}