using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace WebApplication1.Filter;

public class GlobalExceptionFilter : IAsyncExceptionFilter
{
    public Task OnExceptionAsync(ExceptionContext context)
    {
        if (context.Exception is InvalidOperationException ex)
        {
            context.Result = new JsonResult(new
            {
                code = 400,
                message = $"request failed. {ex.Message}"
            });
            context.HttpContext.Response.StatusCode = 400;
            context.ExceptionHandled = true;
            return Task.CompletedTask;
        }

        context.Result = new JsonResult(new
        {
            code = 500,
            message = $"Server internal failed."
        });
        context.HttpContext.Response.StatusCode = 500;
        context.ExceptionHandled = true;
        return Task.CompletedTask;
    }
}