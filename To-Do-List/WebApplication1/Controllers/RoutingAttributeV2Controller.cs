using Microsoft.AspNetCore.Mvc;

namespace WebApplication1.Controllers;

[ApiController]
[ApiVersion("2.0")]
[ApiExplorerSettings(GroupName = "v2")]
[Route("api/v{version:apiVersion}/RoutingAttribute")]
public class RoutingAttributeV2Controller: ControllerBase
{
    [HttpGet("TaxRate")]
    public IActionResult GetTaxRateV2()
    {
        return Ok("V2: Tax rate is 12.5%");
    }
}