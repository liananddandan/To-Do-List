using Microsoft.AspNetCore.Mvc.Filters;

namespace WebApplication1.Filter;

public class LogActionFilter(ILogger<LogActionFilter> logger) : IActionFilter
{
    
    public void OnActionExecuting(ActionExecutingContext context)
    {
        var name = context.ActionDescriptor.DisplayName;
        logger.LogInformation($"[START] {name}");
    }

    public void OnActionExecuted(ActionExecutedContext context)
    {
        var name = context.ActionDescriptor.DisplayName;
        logger.LogInformation($"[END] {name}");
    }
}