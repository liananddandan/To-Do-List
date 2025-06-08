using Microsoft.AspNetCore.Mvc;

namespace WebApplication1.Controllers;

[ApiController]
[Route("[controller]/[action]")]
public class FilterTestController: ControllerBase
{
    [HttpGet]
    public IActionResult Tasks()
    {
        return Ok();
    }

    [HttpGet]
    public IActionResult Fail()
    {
        throw new InvalidOperationException("test invalid operation exception");
    }

    [HttpGet]
    public IActionResult Error()
    {
        throw new Exception("test exception");
    }
}