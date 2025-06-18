using Microsoft.AspNetCore.Mvc;

namespace WebApplication1.Controllers;

[ApiController]
[ApiVersion("1.0")]
[ApiExplorerSettings(GroupName = "v1")]
[Route("api/v{version:apiVersion}/RoutingAttribute")]
public class RoutingAttributeController: ControllerBase
{
    [HttpGet("taxRate")]
    public IActionResult GetTaxRate()
    {
        return Ok("V1: Tax rate is 15%");
    }
    
    [HttpGet("user/{id}")]

    public IActionResult GetUserInfoById(int id)
    {
        return Ok("V1: User info is " + id);
    }

    [HttpGet("user")]
    public IActionResult GetUserInfoByQueryId([FromQuery] int id)
    {
        return Ok("V1: User info is " + id);
    }
    
    // Route Constraints
    [HttpGet("user/constraint/{id:int}")]
    public IActionResult GetUserByRouteConstraintId(int id)
    {
        return Ok("V1: User info is " + id);
    }
    
    // optional route parameter
    [HttpGet("user/optional/{id?}")]
    public IActionResult GetUserByRouteOptionalId(int id = 123)
    {
        return Ok("V1: User info is " + id);
    }
}