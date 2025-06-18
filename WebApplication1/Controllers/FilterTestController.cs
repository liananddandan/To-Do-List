using Microsoft.AspNetCore.Mvc;

namespace WebApplication1.Controllers;

[ApiController]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/FilterTest")]
public class FilterTestController: ControllerBase
{
    [HttpGet("tasks")]
    public IActionResult Tasks()
    {
        return Ok();
    }

    [HttpGet("fail")]
    public IActionResult Fail()
    {
        throw new InvalidOperationException("test invalid operation exception");
    }

    [HttpGet("error")]
    public IActionResult Error()
    {
        throw new Exception("test exception");
    }
}