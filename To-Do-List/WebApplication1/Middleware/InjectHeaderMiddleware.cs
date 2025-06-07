namespace WebApplication1.Middleware;

public class InjectHeaderMiddleware(RequestDelegate next)
{
    private readonly RequestDelegate _next = next;

    public async Task InvokeAsync(HttpContext context)
    {
        context.Request.Headers["X-Custom-Info"] = "InjectedByMiddleware";
        await _next(context);
    }
}